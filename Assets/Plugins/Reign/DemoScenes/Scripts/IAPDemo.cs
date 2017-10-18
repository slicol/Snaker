// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Reign;

public class IAPDemo : MonoBehaviour
{
	private static bool created;

	#if SAMSUNG
	private string item1 = "xxxxxxxxxxx1";
	private string item2 = "xxxxxxxxxxx2";
	private string item3 = "xxxxxxxxxxx3";
	#else
	private string item1 = "com.reignstudios.test_app1";
	private string item2 = "com.reignstudios.test_app2";
	private string item3 = "com.reignstudios.test_app3";
	#endif

	public Text StatusText;
	public Button BuyDurableButton, BuyConsumableButton, RestoreButton, GetPriceInfoButton, BackButton;

	void Start()
	{
		// bind button events
		BuyDurableButton.Select();
		BuyDurableButton.onClick.AddListener(buyDurableClicked);
		BuyConsumableButton.onClick.AddListener(buyConsumableClicked);
		RestoreButton.onClick.AddListener(restoreClicked);
		GetPriceInfoButton.onClick.AddListener(getPriceInfoClicked);
		BackButton.onClick.AddListener(backClicked);

		// make sure we don't init the same IAP items twice
		if (created) return;
		created = true;

		// InApp-Purchases - NOTE: you can set different "In App IDs" for each platform.
		var inAppIDs = new InAppPurchaseID[3];
		inAppIDs[0] = new InAppPurchaseID(item1, 1.99m, "$", InAppPurchaseTypes.NonConsumable);
		inAppIDs[1] = new InAppPurchaseID(item2, 0.99m, "$", InAppPurchaseTypes.NonConsumable);
		inAppIDs[2] = new InAppPurchaseID(item3, 2.49m, "$", InAppPurchaseTypes.Consumable);
		
		// create desc object
		var desc = new InAppPurchaseDesc();

		// Global
		desc.Testing = true;
		desc.ClearNativeCache = false;
			
		// Editor
		desc.Editor_InAppIDs = inAppIDs;
			
		// WinRT
		desc.WinRT_InAppPurchaseAPI = InAppPurchaseAPIs.MicrosoftStore;
		desc.WinRT_MicrosoftStore_InAppIDs = inAppIDs;
			
		// WP8
		desc.WP8_InAppPurchaseAPI = InAppPurchaseAPIs.MicrosoftStore;
		desc.WP8_MicrosoftStore_InAppIDs = inAppIDs;
			
		// BB10
		desc.BB10_InAppPurchaseAPI = InAppPurchaseAPIs.BlackBerryWorld;
		desc.BB10_BlackBerryWorld_InAppIDs = inAppIDs;
			
		// iOS
		desc.iOS_InAppPurchaseAPI = InAppPurchaseAPIs.AppleStore;
		desc.iOS_AppleStore_InAppIDs = inAppIDs;
		desc.iOS_AppleStore_SharedSecretKey = "";// NOTE: Must set SharedSecretKey, even for Testing!
			
		// Android
		// Choose for either GooglePlay or Amazon.
		// NOTE: Use "player settings" to define compiler directives.
		#if AMAZON
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.Amazon;
		#elif SAMSUNG
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.Samsung;
		#else
		desc.Android_InAppPurchaseAPI = InAppPurchaseAPIs.GooglePlay;
		#endif

		desc.Android_GooglePlay_InAppIDs = inAppIDs;
		desc.Android_GooglePlay_Base64Key = "";// NOTE: Must set Base64Key for GooglePlay in Apps to work, even for Testing!
		desc.Android_Amazon_InAppIDs = inAppIDs;
		desc.Android_Samsung_InAppIDs = inAppIDs;
		desc.Android_Samsung_ItemGroupID = "";

		// init
		InAppPurchaseManager.Init(desc, createdCallback);
	}

	private void buyDurableClicked()
	{
		StatusText.text = "";
		InAppPurchaseManager.MainInAppAPI.Buy(item1, buyAppCallback);
	}

	private void buyConsumableClicked()
	{
		StatusText.text = "";
		InAppPurchaseManager.MainInAppAPI.Buy(item3, buyAppCallback);
	}

	private void restoreClicked()
	{
		StatusText.text = "";
		InAppPurchaseManager.MainInAppAPI.Restore(restoreAppsCallback);
	}

	private void getPriceInfoClicked()
	{
		StatusText.text = "";
		InAppPurchaseManager.MainInAppAPI.GetProductInfo(productInfoCallback);
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}
	
	private void createdCallback(bool succeeded)
	{
		StatusText.text = "Init: " + succeeded + System.Environment.NewLine + System.Environment.NewLine;
		InAppPurchaseManager.MainInAppAPI.AwardInterruptedPurchases(awardInterruptedPurchases);
	}

	private void awardInterruptedPurchases(string inAppID, bool succeeded)
	{
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			StatusText.text += "Interrupted Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			StatusText.text += System.Environment.NewLine + System.Environment.NewLine;
		}
	}

	private void productInfoCallback(InAppPurchaseInfo[] priceInfos, bool succeeded)
	{
		if (succeeded)
		{
			StatusText.text = "";
			foreach (var info in priceInfos)
			{
				if (info.ID == item1) StatusText.text += string.Format("ID: {0} Price: {1}", info.ID, info.FormattedPrice);
			}
		}
		else
		{
			StatusText.text += "Get Price Info Failed!";
		}
	}

	void buyAppCallback(string inAppID, string receipt, bool succeeded)
	{
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			StatusText.text += "Buy Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			if (!string.IsNullOrEmpty(receipt))
			{
				StatusText.text += System.Environment.NewLine + System.Environment.NewLine;
				StatusText.text += receipt;
			}
		}
		else
		{
			StatusText.text += "Failed: " + inAppID + System.Environment.NewLine;
		}
	}

	void restoreAppsCallback(string inAppID, bool succeeded)
	{
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			StatusText.text += "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			StatusText.text += System.Environment.NewLine + System.Environment.NewLine;
		}
		else
		{
			StatusText.text += "Failed: " + inAppID + System.Environment.NewLine;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
