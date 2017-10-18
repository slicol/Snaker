// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

//#define TEST_ASYNC
#if (UNITY_WINRT && !UNITY_EDITOR) || TEST_ASYNC
#define ASYNC
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using Reign.Plugin;
using System.Collections;

namespace Reign
{
	/// <summary>
	/// Used to manage a single Ad instance.
	/// </summary>
	public class Ad
	{
		internal IAdPlugin plugin;
		
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="plugin">Plugin interface reference.</param>
		public Ad(IAdPlugin plugin)
		{
			this.plugin = plugin;
		}
		
		/// <summary>
		/// Use to Show or Hide the Ad.
		/// </summary>
		public bool Visible
		{
			get {return plugin.Visible;}
			set {plugin.Visible = value;}
		}
		
		/// <summary>
		/// Use to set the gravity of the Ad.
		/// </summary>
		/// <param name="gravity">Gravity type.</param>
		public void SetGravity(AdGravity gravity)
		{
			plugin.SetGravity(gravity);
		}
			
		/// <summary>
		/// Use to refresh the Ad.
		/// NOTE: Not supported on (iOS iAd) or (BB10 Native Ads).
		/// </summary>
		public void Refresh()
		{
			plugin.Refresh();
		}

		/// <summary>
		/// Use if you are using classic GUI mode and manually rendering
		/// </summary>
		public void Draw()
		{
			plugin.OverrideOnGUI();
		}
	}

	/// <summary>
	/// Used to manage all Ads.
	/// </summary>
    public static class AdManager
    {
		private static List<IAdPlugin> plugins;
		private static bool creatingAds;
		private static AdCreatedCallbackMethod createdCallback;

		static AdManager()
		{
			ReignServices.CheckStatus();
			plugins = new List<IAdPlugin>();

			#if !DISABLE_REIGN
			ReignServices.AddService(update, onGui, null);
			#endif
		}

		private static void update()
		{
			foreach (var plugin in plugins)
			{
				plugin.Update();
			}
		}

		private static void onGui()
		{
			foreach (var plugin in plugins)
			{
				plugin.OnGUI();
			}
		}
		
		private static void async_CreatedCallback(bool succeeded)
		{
			#if ASYNC
			ReignServices.InvokeOnUnityThread(delegate
			{
				creatingAds = false;
				ReignServices.Singleton.StartCoroutine(createdCallbackDelay(succeeded));
			});
			#else
			creatingAds = false;
			ReignServices.Singleton.StartCoroutine(createdCallbackDelay(succeeded));
			#endif
		}

		private static IEnumerator createdCallbackDelay(bool succeeded)
		{
			// delay object callback so .NET instance is guaranteed to be created
			yield return null;
			if (createdCallback != null) createdCallback(succeeded);
		}

		/// <summary>
		/// Use to create a single Ad.
		/// </summary>
		/// <param name="desc">Your AdDesc settings.</param>
		/// <param name="createdCallback">The callback that fires when done.</param>
		/// <returns>Returns Ad object</returns>
		public static Ad CreateAd(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last ad to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return null;
			}
			creatingAds = true;
			AdManager.createdCallback = createdCallback;
			plugins.Add(AdPluginAPI.New(desc, async_CreatedCallback));
			
			return new Ad(plugins[plugins.Count-1]);
		}

		/// <summary>
		/// Use to create multiple Ads.
		/// </summary>
		/// <param name="descs">Your AdDesc settings.</param>
		/// <param name="createdCallback">The callback that fires when done.</param>
		/// <returns>Returns array of Ad objects</returns>
		public static Ad[] CreateAd(AdDesc[] descs, AdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last ads to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return null;
			}
			creatingAds = true;
			AdManager.createdCallback = createdCallback;

			int startLength = plugins.Count;
			for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI.New(descs[i], async_CreatedCallback));
			
			var ads = new Ad[descs.Length];
			for (int i = 0, i2 = startLength; i != descs.Length; ++i, ++i2) ads[i] = new Ad(plugins[i2]);
			return ads;
		}

		/// <summary>
		/// Use to dispose of an Ad.
		/// NOTE: Use "ad.Visible = true/false;" instead in most cases.
		/// </summary>
		/// <param name="ad"></param>
		public static void DisposeAd(Ad ad)
		{
			int index = getAdIndex(ad);
			if (index == -1)
			{
				Debug.LogError("DisposeAd Failed: Ad not found in AdManager.");
				return;
			}
			
			var plugin = plugins[index];
			plugin.Dispose();
			plugins.Remove(plugin);
		}
		
		private static int getAdIndex(Ad ad)
		{
			for (int i = 0; i != plugins.Count; ++i)
			{
				if (ad.plugin == plugins[i]) return i;
			}
			
			return -1;
		}
    }
}

namespace Reign.Plugin
{
	/// <summary>
	/// Dumy object.
	/// </summary>
	public class Dumy_AdPlugin : IAdPlugin
    {
		/// <summary>
		/// Dumy property.
		/// </summary>
		public bool Visible {get; set;}

		/// <summary>
		/// Dumy constructor.
		/// </summary>
		/// <param name="desc"></param>
		/// <param name="createdCallback"></param>
		public Dumy_AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			Visible = desc.Visible;
			if (createdCallback != null) createdCallback(true);
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Dispose()
		{
			// do nothing...
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gravity"></param>
		public void SetGravity(AdGravity gravity)
		{
			// do nothing...
		}
		
		/// <summary>
		/// Dumy method.
		/// </summary>	
		public void Refresh()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Update()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void OnGUI()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void OverrideOnGUI()
		{
			// do nothing...
		}
    }

	/// <summary>
	/// Use to create a platform specific interface.
	/// </summary>
	static class AdPluginAPI
	{
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="desc">The Ad desc.</param>
		/// <param name="callback">The callback fired when done.</param>
		/// <returns>Returns Ad interface</returns>
		public static IAdPlugin New(AdDesc desc, AdCreatedCallbackMethod callback)
		{
			#if DISABLE_REIGN
			return new Dumy_AdPlugin(desc, callback);
			#elif UNITY_EDITOR
			if (desc.Editor_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.Editor_AdAPI == AdAPIs.EditorTestAd) return new AdPlugin(desc, callback);
			else if (desc.Editor_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported Editor_AdAPI: " + desc.Editor_AdAPI);
			#elif UNITY_WP8
			if (desc.WP8_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.WP8_AdAPI == AdAPIs.MicrosoftAdvertising) return new MicrosoftAdvertising_AdPlugin_WinRT(desc, callback);
			else if (desc.WP8_AdAPI == AdAPIs.AdMob) return new AdMob_AdPlugin_WP8(desc, callback);
			else if (desc.WP8_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported WP8_AdAPI: " + desc.WP8_AdAPI);
			#elif UNITY_METRO
			if (desc.WinRT_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.WinRT_AdAPI == AdAPIs.MicrosoftAdvertising) return new MicrosoftAdvertising_AdPlugin_WinRT(desc, callback);
			else if (desc.WinRT_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported WinRT_AdAPI: " + desc.WinRT_AdAPI);
			#elif UNITY_BLACKBERRY
			if (desc.BB10_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.BB10_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported BB10_AdAPI: " + desc.BB10_AdAPI);
			#elif UNITY_IOS
			if (desc.iOS_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.iOS_AdAPI == AdAPIs.iAd) return new iAd_AdPlugin_iOS(desc, callback);
			else if (desc.iOS_AdAPI == AdAPIs.AdMob) return new AdMob_AdPlugin_iOS(desc, callback);
			else if (desc.iOS_AdAPI == AdAPIs.DFP) return new DFP_AdPlugin_iOS(desc, callback);
			else if (desc.iOS_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported iOS_AdAPI: " + desc.iOS_AdAPI);
			#elif UNITY_ANDROID
			if (desc.Android_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.Android_AdAPI == AdAPIs.AdMob) return new AdMob_AdPlugin_Android(desc, callback);
			else if (desc.Android_AdAPI == AdAPIs.DFP) return new DFP_AdPlugin_Android(desc, callback);
			else if (desc.Android_AdAPI == AdAPIs.Amazon) return new Amazon_AdPlugin_Android(desc, callback);
			else if (desc.Android_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported Android_AdAPI: " + desc.Android_AdAPI);
			#elif UNITY_STANDALONE_WIN
			if (desc.Win32_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.Win32_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported Win32_AdAPI: " + desc.Win32_AdAPI);
			#elif UNITY_STANDALONE_OSX
			if (desc.OSX_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.OSX_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported OSX_AdAPI: " + desc.OSX_AdAPI);
			#elif UNITY_STANDALONE_LINUX
			if (desc.Linux_AdAPI == AdAPIs.None) return new Dumy_AdPlugin(desc, callback);
			else if (desc.Linux_AdAPI == AdAPIs.MillennialMediaAdvertising) return new MM_AdPlugin(desc, callback, ReignServices.Singleton);
			else throw new Exception("Unsuported Linux_AdAPI: " + desc.Linux_AdAPI);
			#else
			return new Dumy_AdPlugin(desc, callback);
			#endif
		}
	}
}