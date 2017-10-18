// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Reign;

public class MarketingDemo : MonoBehaviour
{
	public Button ReviewButton, BackButton;

	void Start ()
	{
		// bind button events
		ReviewButton.Select();
		ReviewButton.onClick.AddListener(reviewClicked);
		BackButton.onClick.AddListener(backClicked);
	}

	private void reviewClicked()
	{
		var desc = new MarketingDesc();

		desc.Editor_URL = "http://reign-studios.net/";// Any full URL
		desc.Win8_PackageFamilyName = "";// This is the "Package family name" that can be found in your "Package.appxmanifest".
		desc.WP8_AppID = "";// This is the "App ID" that can be found in your "Package.appxmanifest" under "Package Name".
		// NOTE: For Windows Phone 8.0 you don't need to set anything...

		desc.iOS_AppID = "";// Pass in your AppID "xxxxxxxxx"
		desc.BB10_AppID = "";// You pass in your AppID "xxxxxxxx".

		desc.Android_MarketingStore = MarketingStores.GooglePlay;
		desc.Android_GooglePlay_BundleID = "";// Pass in your bundle ID "com.Company.AppName"
		desc.Android_Amazon_BundleID = "";// Pass in your bundle ID "com.Company.AppName"
		desc.Android_Samsung_BundleID = "";// Pass in your bundle ID "com.Company.AppName"

		MarketingManager.OpenStoreForReview(desc);
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
