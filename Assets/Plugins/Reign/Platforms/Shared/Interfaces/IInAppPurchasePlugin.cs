using System;
using System.Collections.Generic;

namespace Reign
{
	/// <summary>
	/// IAP API types
	/// </summary>
	public enum InAppPurchaseAPIs
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		
		/// <summary>
		/// Microsoft Store
		/// </summary>
		MicrosoftStore,

		/// <summary>
		/// BlackBerry World
		/// </summary>
		BlackBerryWorld,

		/// <summary>
		/// Apple Store
		/// </summary>
		AppleStore,

		/// <summary>
		/// GooglePlay
		/// </summary>
		GooglePlay,

		/// <summary>
		/// Amazon
		/// </summary>
		Amazon,

		/// <summary>
		/// Samsung
		/// </summary>
		Samsung
	}
	
	/// <summary>
	/// IAP types
	/// </summary>
	public enum InAppPurchaseTypes
	{
		/// <summary>
		/// Non Consumable
		/// </summary>
		NonConsumable,

		/// <summary>
		/// Consumable
		/// </summary>
		Consumable
	}
	
	/// <summary>
	/// IAP object
	/// </summary>
	public class InAppPurchaseID
	{
		/// <summary>
		/// IAP ID
		/// </summary>
		public string ID;

		/// <summary>
		/// IAP type
		/// </summary>
		public InAppPurchaseTypes Type;

		/// <summary>
		/// Used only on platforms that dont support getting price information from a remote server.
		/// </summary>
		public decimal Price;

		/// <summary>
		/// Used only on platforms that dont support getting price information from a remote server.
		/// </summary>
		public string CurrencySymbol = "$";
		
		/// <summary>
		/// Used to construct a new item.
		/// </summary>
		/// <param name="id">IAP ID</param>
		/// <param name="type">IAP type</param>
		public InAppPurchaseID(string id, InAppPurchaseTypes type)
		{
			ID = id;
			Type = type;
		}

		/// <summary>
		/// Used to construct a new item.
		/// </summary>
		/// <param name="id">IAP ID</param>
		/// <param name="price">IAP Price. Used only on platforms that dont support getting price information from a remote server.</param>
		/// <param name="currencySymbol">IAP Currency Symbol. Used only on platforms that dont support getting price information from a remote server.</param>
		/// <param name="type">IAP type</param>
		public InAppPurchaseID(string id, decimal price, string currencySymbol, InAppPurchaseTypes type)
		{
			ID = id;
			Price = price;
			CurrencySymbol = currencySymbol;
			Type = type;
		}
	}

	/// <summary>
	/// IAP desc object.
	/// </summary>
	public class InAppPurchaseDesc
	{
		// Global
		/// <summary>
		/// Set to true, to test IAP system.
		/// NOTE: Set to false before publishing your app.
		/// </summary>
		public bool Testing;
		
		/// <summary>
		/// Set to true to test trial mode.
		/// NOTE: Only applies to WinRT or WP8.
		/// </summary>
		public bool TestTrialMode;

		/// <summary>
		/// Clears native IAP purchase cache if possible for testing IAP
		/// </summary>
		public bool ClearNativeCache;

		// Editor
		/// <summary>
		/// IAP IDs
		/// </summary>
		public InAppPurchaseID[] Editor_InAppIDs;

		// WinRT
		/// <summary>
		/// IAP API type
		/// </summary>
		public InAppPurchaseAPIs WinRT_InAppPurchaseAPI = InAppPurchaseAPIs.None;

		/// <summary>
		/// IAP IDs for Win8 and WP8.1
		/// </summary>
		public InAppPurchaseID[] WinRT_MicrosoftStore_InAppIDs;

		// WP8
		/// <summary>
		/// IAP API type
		/// </summary>
		public InAppPurchaseAPIs WP8_InAppPurchaseAPI = InAppPurchaseAPIs.None;

		/// <summary>
		/// IAP IDs for WP8.0
		/// </summary>
		public InAppPurchaseID[] WP8_MicrosoftStore_InAppIDs;
		
		// BB10
		/// <summary>
		/// IAP API type
		/// </summary>
		public InAppPurchaseAPIs BB10_InAppPurchaseAPI = InAppPurchaseAPIs.None;

		/// <summary>
		/// IAP IDs
		/// </summary>
		public InAppPurchaseID[] BB10_BlackBerryWorld_InAppIDs;
		
		// iOS
		/// <summary>
		/// IAP API type
		/// </summary>
		public InAppPurchaseAPIs iOS_InAppPurchaseAPI = InAppPurchaseAPIs.None;

		/// <summary>
		/// IAP IDs
		/// </summary>
		public InAppPurchaseID[] iOS_AppleStore_InAppIDs;

		/// <summary>
		/// iOS Shared Secret Key (NOTE: must set even for testing)
		/// </summary>
		public string iOS_AppleStore_SharedSecretKey;
		
		// Android
		/// <summary>
		/// IAP API type
		/// </summary>
		public InAppPurchaseAPIs Android_InAppPurchaseAPI = InAppPurchaseAPIs.None;

		/// <summary>
		/// IAP IDs
		/// </summary>
		public InAppPurchaseID[] Android_GooglePlay_InAppIDs, Android_Amazon_InAppIDs, Android_Samsung_InAppIDs;

		/// <summary>
		/// GooglePlay Base64 Key (NOTE: must set even for testing)
		/// </summary>
		public string Android_GooglePlay_Base64Key;

		/// <summary>
		/// Samsung Item Group ID (NOTE: must set even for testing)
		/// </summary>
		public string Android_Samsung_ItemGroupID;
	}

	/// <summary>
	/// Use for IAP price information
	/// </summary>
	public class InAppPurchaseInfo
	{
		/// <summary>
		/// IAP ID
		/// </summary>
		public string ID;
		
		/// <summary>
		/// Formated price value
		/// </summary>
		public string FormattedPrice;
	}

	/// <summary>
	/// Used for creating IAP systems
	/// </summary>
	/// <param name="succeeded">Tells if the IAP system was created successful or not.</param>
	public delegate void InAppPurchaseCreatedCallbackMethod(bool succeeded);

	/// <summary>
	/// Used for awarding interrupted purchased products
	/// </summary>
	/// <param name="inAppID">IAP ID</param>
	/// <param name="succeeded">Tells if the IAP was successful or not.</param>
	public delegate void InAppPurchaseAwardCallbackMethod(string inAppID, bool succeeded);

	/// <summary>
	/// Used for buying a IAP
	/// </summary>
	/// <param name="inAppID">IAP ID</param>
	/// <param name="succeeded">Tells if the IAP was successful or not.</param>
	public delegate void InAppPurchaseBuyCallbackMethod(string inAppID, string receipt, bool succeeded);

	/// <summary>
	/// Used for restoring all IAPs
	/// </summary>
	/// <param name="inAppID">IAP ID</param>
	/// <param name="succeeded">Tells if the IAP was successful or not.</param>
	public delegate void InAppPurchaseRestoreCallbackMethod(string inAppID, bool succeeded);

	/// <summary>
	/// Used for getting IAP price information
	/// </summary>
	/// <param name="priceInfos">Price Information list</param>
	/// <param name="succeeded">Tells if the IAP was successful or not.</param>
	public delegate void InAppPurchaseGetProductInfoCallbackMethod(InAppPurchaseInfo[] priceInfos, bool succeeded);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base IAP interface object
	/// </summary>
	public interface IInAppPurchasePlugin
	{
		/// <summary>
		/// Get if the app is in trial mode or not.
		/// </summary>
		bool IsTrial {get;}

		/// <summary>
		/// Get IAP IDs
		/// </summary>
		InAppPurchaseID[] InAppIDs {get;}

		/// <summary>
		/// Gets a list of IAP price information
		/// </summary>
		/// <param name="callback">Called when done</param>
		void GetProductInfo(InAppPurchaseGetProductInfoCallbackMethod callback);

		/// <summary>
		/// Restore all IAP items
		/// </summary>
		/// <param name="restoreCallback">Called when done</param>
		void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback);

		/// <summary>
		/// Buy IAP item
		/// </summary>
		/// <param name="inAppID">IAP ID</param>
		/// <param name="purchasedCallback">Called when done</param>
		void BuyInApp(string inAppID, InAppPurchaseBuyCallbackMethod purchasedCallback);

		/// <summary>
		/// Update IAP events
		/// </summary>
		void Update();
	}
}