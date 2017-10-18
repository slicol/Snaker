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
	public class InterstitialAd
	{
		internal IInterstitialAdPlugin plugin;
		
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="plugin">Plugin interface reference.</param>
		public InterstitialAd(IInterstitialAdPlugin plugin)
		{
			this.plugin = plugin;
		}
		
		/// <summary>
		/// Use to cache an Ad.
		/// </summary>
		public void Cache()
		{
			plugin.Cache();
		}

		/// <summary>
		/// Use to show an Ad.
		/// NOTE: Must cache first.
		/// </summary>
		public void Show()
		{
			plugin.Show();
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
    public static class InterstitialAdManager
    {
		private static List<IInterstitialAdPlugin> plugins;
		private static bool creatingAds;
		private static InterstitialAdCreatedCallbackMethod createdCallback;

		static InterstitialAdManager()
		{
			ReignServices.CheckStatus();
			plugins = new List<IInterstitialAdPlugin>();

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
		public static InterstitialAd CreateAd(InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last interstitial ad to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return null;
			}
			creatingAds = true;
			InterstitialAdManager.createdCallback = createdCallback;
			plugins.Add(InterstitialAdPluginAPI.New(desc, async_CreatedCallback));
			
			return new InterstitialAd(plugins[plugins.Count-1]);
		}

		/// <summary>
		/// Use to create multiple Ads.
		/// </summary>
		/// <param name="descs">Your AdDesc settings.</param>
		/// <param name="createdCallback">The callback that fires when done.</param>
		/// <returns>Returns array of Ad objects</returns>
		public static InterstitialAd[] CreateAd(InterstitialAdDesc[] descs, InterstitialAdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last interstitial ads to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return null;
			}
			creatingAds = true;
			InterstitialAdManager.createdCallback = createdCallback;

			int startLength = plugins.Count;
			for (int i = 0; i != descs.Length; ++i) plugins.Add(InterstitialAdPluginAPI.New(descs[i], async_CreatedCallback));
			
			var ads = new InterstitialAd[descs.Length];
			for (int i = 0, i2 = startLength; i != descs.Length; ++i, ++i2) ads[i] = new InterstitialAd(plugins[i2]);
			return ads;
		}

		/// <summary>
		/// Use to dispose of an Ad.
		/// NOTE: Use "ad.Cache();" to get a new Ad to show.
		/// </summary>
		/// <param name="ad"></param>
		public static void DisposeAd(InterstitialAd ad)
		{
			int index = getAdIndex(ad);
			if (index == -1)
			{
				Debug.LogError("DisposeAd Failed: InterstitialAd not found in InterstitialAdManager.");
				return;
			}
			
			var plugin = plugins[index];
			plugin.Dispose();
			plugins.Remove(plugin);
		}
		
		private static int getAdIndex(InterstitialAd ad)
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
	public class Dumy_InterstitialAdPlugin : IInterstitialAdPlugin
	{
		private InterstitialAdEventCallbackMethod eventCallback;

		/// <summary>
		/// Dumy constructor.
		/// </summary>
		/// <param name="desc"></param>
		/// <param name="createdCallback"></param>
		public Dumy_InterstitialAdPlugin(InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod createdCallback)
		{
			eventCallback = desc.EventCallback;
			if (createdCallback != null) createdCallback(true);
		}
		
		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Cache()
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Show()
		{
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Canceled, null);
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Dispose()
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
	static class InterstitialAdPluginAPI
	{
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="desc">The Ad desc.</param>
		/// <param name="callback">The callback fired when done.</param>
		/// <returns>Returns Ad plugin interface</returns>
		public static IInterstitialAdPlugin New(InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod callback)
		{
			#if DISABLE_REIGN
			return new Dumy_InterstitialAdPlugin(desc, callback);
			#elif UNITY_EDITOR
			return new InterstitialAdPlugin(desc, callback);
			#elif UNITY_WP8
			if (desc.WP8_AdAPI == InterstitialAdAPIs.None) return new Dumy_InterstitialAdPlugin(desc, callback);
			else if (desc.WP8_AdAPI == InterstitialAdAPIs.AdMob) return new AdMob_InterstitialAdPlugin_WP8(desc, callback);
			else throw new Exception("Unsuported WP8_AdAPI: " + desc.WP8_AdAPI);
			#elif UNITY_IOS
			if (desc.iOS_AdAPI == InterstitialAdAPIs.None) return new Dumy_InterstitialAdPlugin(desc, callback);
			else if (desc.iOS_AdAPI == InterstitialAdAPIs.AdMob) return new AdMob_InterstitialAdPlugin_iOS(desc, callback);
			else if (desc.iOS_AdAPI == InterstitialAdAPIs.DFP) return new DFP_InterstitialAdPlugin_iOS(desc, callback);
			else throw new Exception("Unsuported iOS_AdAPI: " + desc.iOS_AdAPI);
			#elif UNITY_ANDROID
			if (desc.Android_AdAPI == InterstitialAdAPIs.None) return new Dumy_InterstitialAdPlugin(desc, callback);
			else if (desc.Android_AdAPI == InterstitialAdAPIs.AdMob) return new AdMob_InterstitialAdPlugin_Android(desc, callback);
			else if (desc.Android_AdAPI == InterstitialAdAPIs.DFP) return new DFP_InterstitialAdPlugin_Android(desc, callback);
			else if (desc.Android_AdAPI == InterstitialAdAPIs.Amazon) return new Amazon_InterstitialAdPlugin_Android(desc, callback);
			else throw new Exception("Unsuported Android_AdAPI: " + desc.Android_AdAPI);
			#else
			return new Dumy_InterstitialAdPlugin(desc, callback);
			#endif
		}
	}
}