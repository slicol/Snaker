// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class InterstitialAdDemo : MonoBehaviour
{
	private static bool created;
	private static InterstitialAd ad;
	public Button ShowAdButton, BackButton;

	void Start()
	{
		// bind button events
		ShowAdButton.Select();
		ShowAdButton.onClick.AddListener(showAdClicked);
		BackButton.onClick.AddListener(backClicked);

		// make sure we don't init the same Ad twice
		if (created) return;
		created = true;

		// create add
		var desc = new InterstitialAdDesc();

		// Global
		desc.Testing = true;
		desc.EventCallback = eventCallback;
		desc.UseClassicGUI = false;
		desc.GUIOverrideEnabled = false;
		desc.UnityUI_SortIndex = 1001;

		// WP8
		desc.WP8_AdAPI = InterstitialAdAPIs.AdMob;
		desc.WP8_AdMob_UnitID = "";// NOTE: Must set event for testing
			
		// iOS
		desc.iOS_AdAPI = InterstitialAdAPIs.AdMob;
		desc.iOS_AdMob_UnitID = "";// NOTE: Must set event for testing
		
		// Android
		#if AMAZON
		desc.Android_AdAPI = InterstitialAdAPIs.Amazon;
		#else
		desc.Android_AdAPI = InterstitialAdAPIs.AdMob;
		#endif
		desc.Android_AdMob_UnitID = "";// NOTE: Must set event for testing
		desc.Android_Amazon_ApplicationKey = "";// NOTE: Must set event for testing

		// create ad
		ad = InterstitialAdManager.CreateAd(desc, createdCallback);
	}

	private void showAdClicked()
	{
		// its a good idea to cache ads when the level starts, then show it when the level ends
		ad.Cache();
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	private void createdCallback(bool success)
	{
		Debug.Log(success);
		if (!success) Debug.LogError("Failed to create InterstitialAd!");
	}

	private static void eventCallback(InterstitialAdEvents adEvent, string eventMessage)
	{
		Debug.Log(adEvent);
		if (adEvent == InterstitialAdEvents.Error) Debug.LogError(eventMessage);
		if (adEvent == InterstitialAdEvents.Cached) ad.Show();
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
