// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class AdsDemo : MonoBehaviour
{
	private static AdsDemo singleton;
	private static bool created;
	private static Ad ad;

	public Text AdStatusText;
	public Button RefreshButton, VisibilityButton, BackButton;

	// -----------------------------------------------
	// NOTE: You can set up multiple platforms at once and the API will use the correct desc data for each
	// -----------------------------------------------
	void Start()
	{
		singleton = this;

		// bind button events
		RefreshButton.Select();
		RefreshButton.onClick.AddListener(refreshClicked);
		VisibilityButton.onClick.AddListener(visibilityClicked);
		BackButton.onClick.AddListener(backClicked);

		// make sure we don't init the same Ad twice
		if (created)
		{
			if (ad != null) ad.Visible = true;
			return;
		}
		created = true;

		// Ads - NOTE: You can pass in multiple "AdDesc" objects if you want more then one ad.
		var desc = new AdDesc();

		// global settings
		desc.Testing = true;// NOTE: To test ads on iOS, you must enable them in iTunes Connect.
		desc.Visible = true;
		desc.EventCallback = eventCallback;
		desc.UnityUI_AdMaxWidth = 0.3f;
		desc.UnityUI_AdMaxHeight = 0.15f;
		desc.UseClassicGUI = false;
		desc.GUIOverrideEnabled = false;
		desc.UnityUI_SortIndex = 1000;

		// Editor
		desc.Editor_AdAPI = AdAPIs.EditorTestAd;
		desc.Editor_AdGravity = AdGravity.BottomCenter;
		desc.Editor_AdScale = 2;

		desc.Editor_MillennialMediaAdvertising_APID = "";
		desc.Editor_MillennialMediaAdvertising_AdGravity = AdGravity.BottomCenter;
		//desc.Editor_MillennialMediaAdvertising_RefreshRate = 120,

		// WinRT settings (Windows 8.0 & 8.1)
		desc.WinRT_AdAPI = AdAPIs.MicrosoftAdvertising;
		desc.WinRT_MicrosoftAdvertising_ApplicationID = "";
		desc.WinRT_MicrosoftAdvertising_UnitID = "";
		desc.WinRT_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.WinRT_MicrosoftAdvertising_AdSize = WinRT_MicrosoftAdvertising_AdSize.Wide_728x90;
		//desc.WinRT_MicrosoftAdvertising_UseBuiltInRefresh = false;
		//desc.WinRT_MicrosoftAdvertising_RefreshRate = 120;
			
		// WP8 settings (Windows Phone 8.0 & 8.1)
		desc.WP8_AdAPI = AdAPIs.MicrosoftAdvertising;
		desc.WP8_MicrosoftAdvertising_ApplicationID = "";
		desc.WP8_MicrosoftAdvertising_UnitID = "";
		desc.WP8_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.WP8_MicrosoftAdvertising_AdSize = WP8_MicrosoftAdvertising_AdSize.Wide_480x80;
		//desc.WP8_MicrosoftAdvertising_UseBuiltInRefresh = false;
		//desc.WP8_MicrosoftAdvertising_RefreshRate = 120;
			
		desc.WP8_AdMob_UnitID = "";// NOTE: You MUST have this even for Testing!
		desc.WP8_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.WP8_AdMob_AdSize = Reign.WP8_AdMob_AdSize.Banner;
			
		// BB10 settings
		desc.BB10_AdAPI = AdAPIs.MillennialMediaAdvertising;
		desc.BB10_BlackBerryAdvertising_ZoneID = "";
		desc.BB10_BlackBerryAdvertising_AdGravity = AdGravity.BottomCenter;
		desc.BB10_BlackBerryAdvertising_AdSize = BB10_BlackBerryAdvertising_AdSize.Wide_320x53;

		desc.BB10_MillennialMediaAdvertising_APID = "";
		desc.BB10_MillennialMediaAdvertising_AdGravity = AdGravity.BottomCenter;
		//desc.BB10_MillennialMediaAdvertising_RefreshRate = 120;
		desc.BB10_AdScale = 2;
			
		// iOS settings
		desc.iOS_AdAPI = AdAPIs.AdMob;
		desc.iOS_iAd_AdGravity = AdGravity.BottomCenter;
			
		desc.iOS_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.iOS_AdMob_UnitID = "";// NOTE: You can use legacy (PublisherID) too, You MUST have this even for Testing!
		desc.iOS_AdMob_AdSize = iOS_AdMob_AdSize.Banner_320x50;
			
		// Android settings
		#if AMAZON
		desc.Android_AdAPI = AdAPIs.Amazon;// Choose between AdMob or Amazon
		#else
		desc.Android_AdAPI = AdAPIs.AdMob;// Choose between AdMob or Amazon
		#endif
			
		desc.Android_AdMob_UnitID = "";// NOTE: You MUST have this even for Testing!
		desc.Android_AdMob_AdGravity = AdGravity.BottomCenter;
		desc.Android_AdMob_AdSize = Android_AdMob_AdSize.Banner_320x50;
			
		desc.Android_AmazonAds_ApplicationKey = "";
		desc.Android_AmazonAds_AdSize = Android_AmazonAds_AdSize.Wide_320x50;
		desc.Android_AmazonAds_AdGravity = AdGravity.BottomCenter;
		//desc.Android_AmazonAds_RefreshRate = 120;

		// create ad
		ad = AdManager.CreateAd(desc, adCreatedCallback);
	}

	private void refreshClicked()
	{
		ad.Refresh();
	}

	private void visibilityClicked()
	{
		ad.Visible = !ad.Visible;
	}

	private void backClicked()
	{
		ad.Visible = false;
		Application.LoadLevel("MainDemo");
	}

	private void adCreatedCallback(bool succeeded)
	{
		AdStatusText.text = succeeded ? "Ads Succeded" : "Ads Failed";
	}

	private static void eventCallback(AdEvents adEvent, string eventMessage)
	{
		// NOTE: On BB10 these events never get called!
		switch (adEvent)
		{
			case AdEvents.Refreshed: singleton.AdStatusText.text = "Refreshed"; break;
			case AdEvents.Clicked: singleton.AdStatusText.text = "Clicked"; break;
			case AdEvents.Error: singleton.AdStatusText.text = "Error: " + eventMessage; break;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
