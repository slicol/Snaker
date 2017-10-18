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
	/// Used to handle IAP.
	/// </summary>
	public class InAppAPI
	{
		/// <summary>
		/// Use to check if the app is in trial mode.
		/// </summary>
		public bool IsTrial {get{return plugin.IsTrial;}}

		private IInAppPurchasePlugin plugin;
		private InAppPurchaseAPIs pluginAPI;
		private bool restoringProducts, buyingProduct;
		private InAppPurchaseCreatedCallbackMethod createdCallback;
		private InAppPurchaseRestoreCallbackMethod restoreCallback;
		private InAppPurchaseBuyCallbackMethod buyCallback;

		internal void init(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			#if DISABLE_REIGN
			pluginAPI = InAppPurchaseAPIs.None;
			#elif UNITY_WP8
			pluginAPI = desc.WP8_InAppPurchaseAPI;
			#elif UNITY_METRO
			pluginAPI = desc.WinRT_InAppPurchaseAPI;
			#elif UNITY_IOS
			pluginAPI = desc.iOS_InAppPurchaseAPI;
			#elif UNITY_ANDROID
			pluginAPI = desc.Android_InAppPurchaseAPI;
			#elif UNITY_BLACKBERRY
			pluginAPI = desc.BB10_InAppPurchaseAPI;
			#else
			pluginAPI = InAppPurchaseAPIs.None;
			#endif

			this.createdCallback = createdCallback;
			plugin = InAppPurchaseAPI.New(desc, async_CreatedCallback);
		}

		internal void update()
		{
			plugin.Update();
		}

		private void async_CreatedCallback(bool succeeded)
		{
			#if ASYNC
			ReignServices.InvokeOnUnityThread(delegate
			{
				ReignServices.Singleton.StartCoroutine(createdCallbackDelay(succeeded));
			});
			#else
			ReignServices.Singleton.StartCoroutine(createdCallbackDelay(succeeded));
			#endif
		}

		private IEnumerator createdCallbackDelay(bool succeeded)
		{
			// delay object callback so .NET instance is guaranteed to be created
			yield return null;
			if (createdCallback != null) createdCallback(succeeded);
		}
		
		private void async_RestoreCallback(string inAppID, bool succeeded)
		{
			#if ASYNC
			ReignServices.InvokeOnUnityThread(delegate
			{
				restoringProducts = false;
				saveBuyToPrefs(inAppID, succeeded);
				if (restoreCallback != null) restoreCallback(inAppID, succeeded);
			});
			#else
			restoringProducts = false;
			saveBuyToPrefs(inAppID, succeeded);
			if (restoreCallback != null) restoreCallback(inAppID, succeeded);
			#endif
		}
		
		private void async_BuyCallback(string inAppID, string receipt, bool succeeded)
		{
			#if ASYNC
			ReignServices.InvokeOnUnityThread(delegate
			{
				buyingProduct = false;
				saveBuyToPrefs(inAppID, succeeded);
				if (buyCallback != null) buyCallback(inAppID, receipt, succeeded);
			});
			#else
			buyingProduct = false;
			saveBuyToPrefs(inAppID, succeeded);
			if (buyCallback != null) buyCallback(inAppID, receipt, succeeded);
			#endif
		}

		private void saveBuyToPrefs(string inAppID, bool succeeded)
		{
			if (succeeded)
			{
				PlayerPrefs.SetInt("ReignIAP_PurchasedAwarded_" + inAppID, 1);
				PlayerPrefs.SetInt(pluginAPI + "_" + inAppID, 1);
				PlayerPrefs.Save();
			}
		}

		/// <summary>
		/// Use to clear ONLY IAP PlayerPref cache data.
		/// </summary>
		public void ClearPlayerPrefData()
		{
			foreach (var inAppID in plugin.InAppIDs)
			{
				string key = pluginAPI + "_" + inAppID.ID;
				if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
			}
		}

		/// <summary>
		/// Use to get the IAP index array value.
		/// </summary>
		/// <param name="inAppID">IAP ID</param>
		/// <returns>Returns index</returns>
		public int GetAppIndexForAppID(string inAppID)
		{
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				if (plugin.InAppIDs[i].ID == inAppID) return i;
			}

			return -1;
		}
		
		/// <summary>
		/// Use to get the IAP item.
		/// </summary>
		/// <param name="inAppID">IAP ID</param>
		/// <returns>Returns IAP object</returns>
		public InAppPurchaseID GetAppID(string inAppID)
		{
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				if (plugin.InAppIDs[i].ID == inAppID) return plugin.InAppIDs[i];
			}

			return null;
		}

		/// <summary>
		/// Use to check if the IAP is purchases in the local cache.
		/// NOTE: This does not check againsed the server. Use Restore for that.
		/// </summary>
		/// <param name="inAppIndex">IAP Index</param>
		/// <returns>Returns true or false</returns>
		public bool IsPurchased(int inAppIndex)
		{
			if (plugin.InAppIDs[inAppIndex].Type == InAppPurchaseTypes.Consumable) return false;
			return PlayerPrefs.GetInt(pluginAPI + "_" + plugin.InAppIDs[inAppIndex].ID) == 1;
		}

		/// <summary>
		/// Use to check if the IAP is purchases in the local cache.
		/// NOTE: This does not check againsed the server. Use Restore for that.
		/// </summary>
		/// <param name="inAppIndex">IAP ID</param>
		/// <returns>Returns true or false</returns>
		public bool IsPurchased(string inAppID)
		{
			int i = GetAppIndexForAppID(inAppID);
			if (i == -1) return false;
			
			if (plugin.InAppIDs[i].Type == InAppPurchaseTypes.Consumable) return false;
			return PlayerPrefs.GetInt(pluginAPI + "_" + plugin.InAppIDs[i].ID) == 1;
		}

		/// <summary>
		/// Use to get product price information
		/// </summary>
		/// <param name="callback"></param>
		public void GetProductInfo(InAppPurchaseGetProductInfoCallbackMethod callback)
		{
			if (restoringProducts || buyingProduct)
			{
				Debug.LogError("You must wait for the last restore, buy or consume to finish!");
				if (callback != null) callback(null, false);
				return;
			}

			plugin.GetProductInfo(callback);
		}

		/// <summary>
		/// Use to restore purchased items.
		/// NOTE: This will check the IAP server for existing items owned by the user.
		/// </summary>
		/// <param name="restoreCallback">The callback that fires when done.</param>
		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			if (restoringProducts || buyingProduct)
			{
				Debug.LogError("You must wait for the last restore, buy or consume to finish!");
				if (restoreCallback != null) restoreCallback(null, false);
				return;
			}
			restoringProducts = true;
			this.restoreCallback = restoreCallback;
			plugin.Restore(async_RestoreCallback);
		}
		
		/// <summary>
		/// Use to when the user wants to buy a IAP.
		/// </summary>
		/// <param name="inAppID">IAP Object</param>
		/// <param name="buyCallback">The callback that fires when done.</param>
		public void Buy(InAppPurchaseID inAppID, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			Buy(inAppID.ID, buyCallback);
		}

		/// <summary>
		/// Use to when the user wants to buy a IAP.
		/// </summary>
		/// <param name="inAppID">IAP Index</param>
		/// <param name="buyCallback">The callback that fires when done.</param>
		public void Buy(int inAppIndex, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			Buy(plugin.InAppIDs[inAppIndex].ID, buyCallback);
		}

		/// <summary>
		/// Use to when the user wants to buy a IAP.
		/// </summary>
		/// <param name="inAppID">IAP ID</param>
		/// <param name="buyCallback">The callback that fires when done.</param>
		public void Buy(string inAppID, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			if (buyingProduct || restoringProducts)
			{
				Debug.LogError("You must wait for the last buy, restore or consume to finish!");
				if (buyCallback != null) buyCallback(inAppID, null, false);
				return;
			}
			buyingProduct = true;
			this.buyCallback = buyCallback;

			// skip if item is already purchased locally
			if (IsPurchased(inAppID))
			{
				Debug.Log("InApp already puchased: " + inAppID);
				buyingProduct = false;
				if (buyCallback != null) buyCallback(inAppID, null, true);
				return;
			}

			plugin.BuyInApp(inAppID, async_BuyCallback);
		}

		/// <summary>
		/// Use to award interrupted IAP
		/// </summary>
		/// <param name="callback"></param>
		public void AwardInterruptedPurchases(InAppPurchaseAwardCallbackMethod callback)
		{
			if (callback == null) return;
			foreach (var inAppID in plugin.InAppIDs)
			{
				if (!PlayerPrefs.HasKey("ReignIAP_PurchasedAwarded_" + inAppID.ID)) continue;
				int value = PlayerPrefs.GetInt("ReignIAP_PurchasedAwarded_" + inAppID.ID);
				if (value == 0)
				{
					PlayerPrefs.SetInt("ReignIAP_PurchasedAwarded_" + inAppID.ID, 1);
					callback(inAppID.ID, true);
				}
			}
		}
	}

	/// <summary>
	/// Used to manage IAP.
	/// </summary>
	public static class InAppPurchaseManager
	{
		/// <summary>
		/// Use to access your main IAP API.
		/// NOTE: Normaly you will only have one.
		/// </summary>
		public static InAppAPI MainInAppAPI {get{return InAppAPIs[0];}}

		/// <summary>
		/// Use to get the array of IAP API objects.
		/// </summary>
		public static InAppAPI[] InAppAPIs {get; private set;}

		static InAppPurchaseManager()
		{
			ReignServices.CheckStatus();
		
			#if !DISABLE_REIGN
			ReignServices.AddService(update, null, null);
			#endif
		}

		private static void update()
		{
			if (InAppAPIs == null) return;

			foreach (var app in InAppAPIs)
			{
				app.update();
			}
		}

		/// <summary>
		/// Use to init the IAP system.
		/// NOTE: This method is normaly used when you don't have any IAP items and only want to check if the app is in trial mode.
		/// </summary>
		/// <param name="testTrialMode">Set true to test trial mode.</param>
		/// <param name="createdCallback">The callback that gets fired when done.</param>
		/// <returns>Returns IAP API object</returns>
		public static InAppAPI Init(bool testTrialMode, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			var desc = new InAppPurchaseDesc()
			{
				Testing = testTrialMode,
				TestTrialMode = testTrialMode
			};
			return Init(desc, createdCallback);
		}

		/// <summary>
		/// Use to init a single IAP system.
		/// </summary>
		/// <param name="desc">IAP Desc.</param>
		/// <param name="createdCallback">The callback that fires when done.</param>
		/// <returns>Returns IAP API object</returns>
		public static InAppAPI Init(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			InAppAPIs = new InAppAPI[1];
			InAppAPIs[0] = new InAppAPI();
			InAppAPIs[0].init(desc, createdCallback);
			return InAppAPIs[0];
		}

		/// <summary>
		/// Use to init a multiple IAP systems.
		/// </summary>
		/// <param name="desc">IAP Desc.</param>
		/// <param name="createdCallback">The callback that fires when done.</param>
		/// <returns>Returns array of IAP API objects</returns>
		public static InAppAPI[] Init(InAppPurchaseDesc[] descs, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			var inAppAPIs = new InAppAPI[descs.Length];
			for (int i = 0; i != descs.Length; ++i)
			{
				inAppAPIs[i] = new InAppAPI();
				inAppAPIs[i].init(descs[i], createdCallback);
			}

			return inAppAPIs;
		}
	}
}

namespace Reign.Plugin
{
	/// <summary>
	/// Dumy object.
	/// </summary>
	public class Dumy_InAppPurchasePlugin : IInAppPurchasePlugin
	{
		/// <summary>
		/// Dumy property.
		/// </summary>
		public bool IsTrial {get; private set;}

		/// <summary>
		///  Dumy property.
		/// </summary>
		public InAppPurchaseID[] InAppIDs {get; private set;}

		/// <summary>
		/// Dumy Constructor.
		/// </summary>
		/// <param name="desc"></param>
		/// <param name="callback"></param>
		public Dumy_InAppPurchasePlugin(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod callback)
		{
			IsTrial = desc.TestTrialMode;
			InAppIDs = new InAppPurchaseID[0];
			if (callback != null) callback(true);
		}

		/// <summary>
		/// Dumy method
		/// </summary>
		/// <param name="callback">Callback</param>
		public void GetProductInfo(InAppPurchaseGetProductInfoCallbackMethod callback)
		{
			// do nothing...
		}
		
		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="restoreCallback"></param>
		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			// do nothing...
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		/// <param name="inAppID"></param>
		/// <param name="purchasedCallback"></param>
		public void BuyInApp(string inAppID, InAppPurchaseBuyCallbackMethod purchasedCallback)
		{
			if (purchasedCallback != null) purchasedCallback(inAppID, null, false); 
		}

		/// <summary>
		/// Dumy method.
		/// </summary>
		public void Update()
		{
			// do nothing...
		}
	}

	/// <summary>
	/// Use to create a platform specific interface.
	/// </summary>
	static class InAppPurchaseAPI
	{
		/// <summary>
		/// Used by the Reign plugin.
		/// </summary>
		/// <param name="desc">IAP Desc.</param>
		/// <param name="callback">The callback fired when done.</param>
		/// <returns>Returns IAP plugin interface</returns>
		public static IInAppPurchasePlugin New(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod callback)
		{
			#if DISABLE_REIGN
			return new Dumy_InAppPurchasePlugin(desc, callback);
			#elif UNITY_EDITOR
			return new InAppPurchasePlugin(desc, callback);
			#elif UNITY_WP8
			if (desc.WP8_InAppPurchaseAPI == InAppPurchaseAPIs.None) return new Dumy_InAppPurchasePlugin(desc, callback);
			else if (desc.WP8_InAppPurchaseAPI == InAppPurchaseAPIs.MicrosoftStore) return new MicrosoftStore_InAppPurchasePlugin_WinRT(desc, callback);
			else throw new Exception("Unsuported WP8_InAppPurchaseAPI " + desc.WP8_InAppPurchaseAPI);
			#elif UNITY_METRO
			if (desc.WinRT_InAppPurchaseAPI == InAppPurchaseAPIs.None) return new Dumy_InAppPurchasePlugin(desc, callback);
			else if (desc.WinRT_InAppPurchaseAPI == InAppPurchaseAPIs.MicrosoftStore) return new MicrosoftStore_InAppPurchasePlugin_WinRT(desc, callback);
			else throw new Exception("Unsuported WinRT_InAppPurchaseAPI: " + desc.WinRT_InAppPurchaseAPI);
			#elif UNITY_BLACKBERRY
			if (desc.BB10_InAppPurchaseAPI == InAppPurchaseAPIs.None) return new Dumy_InAppPurchasePlugin(desc, callback);
			else if (desc.BB10_InAppPurchaseAPI == InAppPurchaseAPIs.BlackBerryWorld) return new InAppPurchasePlugin_BB10(desc, callback);
			else throw new Exception("Unsuported BB10_InAppPurchaseAPI: " + desc.BB10_InAppPurchaseAPI);
			#elif UNITY_IOS
			if (desc.iOS_InAppPurchaseAPI == InAppPurchaseAPIs.None) return new Dumy_InAppPurchasePlugin(desc, callback);
			else if (desc.iOS_InAppPurchaseAPI == InAppPurchaseAPIs.AppleStore) return new AppleStore_InAppPurchasePlugin_iOS(desc, callback);
			else throw new Exception("Unsuported iOS_InAppPurchaseAPI: " + desc.iOS_InAppPurchaseAPI);
			#elif UNITY_ANDROID
			if (desc.Android_InAppPurchaseAPI == InAppPurchaseAPIs.None) return new Dumy_InAppPurchasePlugin(desc, callback);
			else if (desc.Android_InAppPurchaseAPI == InAppPurchaseAPIs.GooglePlay) return new GooglePlay_InAppPurchasePlugin_Android(desc, callback);
			else if (desc.Android_InAppPurchaseAPI == InAppPurchaseAPIs.Amazon) return new Amazon_InAppPurchasePlugin_Android(desc, callback);
			else if (desc.Android_InAppPurchaseAPI == InAppPurchaseAPIs.Samsung) return new Samsung_InAppPurchasePlugin_Android(desc, callback);
			else throw new Exception("Unsuported Android_InAppPurchaseAPI: " + desc.Android_InAppPurchaseAPI);
			#else
			return new Dumy_InAppPurchasePlugin(desc, callback);
			#endif
		}
	}
}