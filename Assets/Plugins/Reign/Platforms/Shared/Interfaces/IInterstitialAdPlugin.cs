using System;

namespace Reign
{
	/// <summary>
	/// Interstitial Ad API types
	/// </summary>
	public enum InterstitialAdAPIs
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// AdMob
		/// </summary>
		AdMob,

		/// <summary>
		/// DFP
		/// </summary>
		DFP,

		/// <summary>
		/// Amazon
		/// </summary>
		Amazon,
	}

	/// <summary>
	/// Interstitial Ad Event types
	/// </summary>
	public enum InterstitialAdEvents
	{
		/// <summary>
		/// Cached
		/// </summary>
		Cached,

		/// <summary>
		/// Shown
		/// </summary>
		Shown,

		/// <summary>
		/// Error
		/// </summary>
		Error,

		/// <summary>
		/// Clicked
		/// </summary>
		Clicked,

		/// <summary>
		/// Canceled
		/// </summary>
		Canceled
	}

	/// <summary>
	/// Interstitial Ad Desc object
	/// </summary>
	public class InterstitialAdDesc
	{
		// global
		/// <summary>
		/// Set to true, to test Ad system.
		/// NOTE: Set to false before publishing your app.
		/// </summary>
		public bool Testing;

		/// <summary>
		/// Ad callbacks will get fires through this delegate.
		/// </summary>
		public InterstitialAdEventCallbackMethod EventCallback;

		/// <summary>
		/// Set to enable the use of classic GUI Ads
		/// </summary>
		public bool UseClassicGUI;

		/// <summary>
		/// Set true to enable GUIOverride draw calls
		/// </summary>
		public bool GUIOverrideEnabled;

		/// <summary>
		/// Used to set the UI layer order of the Ad
		/// </summary>
		public int UnityUI_SortIndex = 1001;

		// WinRT
		// TODO...

		// WP8
		/// <summary>
		/// Ad API type
		/// </summary>
		public InterstitialAdAPIs WP8_AdAPI = InterstitialAdAPIs.None;

		/// <summary>
		/// AdMob UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string WP8_AdMob_UnitID;
		
		// BB10
		// TODO...
		
		// iOS
		/// <summary>
		/// Ad API type
		/// </summary>
		public InterstitialAdAPIs iOS_AdAPI = InterstitialAdAPIs.None;

		/// <summary>
		/// AdMob UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string iOS_AdMob_UnitID;

		/// <summary>
		/// DFP UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string iOS_DFP_UnitID;
		
		// Android
		/// <summary>
		/// Ad API type
		/// </summary>
		public InterstitialAdAPIs Android_AdAPI = InterstitialAdAPIs.None;

		/// <summary>
		/// AdMob UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string Android_AdMob_UnitID;

		/// <summary>
		/// DFP UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string Android_DFP_UnitID;

		/// <summary>
		/// Amazon UnitID
		/// NOTE: Must set event for testing
		/// </summary>
		public string Android_Amazon_ApplicationKey;
	}

	/// <summary>
	/// Used for creating Interstitial Ads
	/// </summary>
	/// <param name="succeeded">Tells if the Ad was successful or not.</param>
	public delegate void InterstitialAdCreatedCallbackMethod(bool succeeded);

	/// <summary>
	/// Used to fire Ad events.
	/// </summary>
	/// <param name="adEvent">Event type</param>
	/// <param name="eventMessage">Event, error message or null.</param>
	public delegate void InterstitialAdEventCallbackMethod(InterstitialAdEvents adEvent, string eventMessage);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base Interstitial Ad interface object
	/// </summary>
	public interface IInterstitialAdPlugin
    {
		/// <summary>
		/// Use to cache an Ad
		/// </summary>
		void Cache();

		/// <summary>
		/// Use to show an Ad
		/// </summary>
		void Show();

		/// <summary>
		/// Use to dispose an Ad
		/// </summary>
		void Dispose();

		/// <summary>
		/// Used to handle Ad events
		/// </summary>
		void Update();

		/// <summary>
		/// Used to render classic gui
		/// </summary>
		void OnGUI();

		/// <summary>
		/// Used to manually render gui operations
		/// </summary>
		void OverrideOnGUI();
    }
}