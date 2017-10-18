using System;

namespace Reign
{
	/// <summary>
	/// Marketing stores.
	/// </summary>
	public enum MarketingStores
	{
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
	/// Marketing desc object
	/// </summary>
	public class MarketingDesc
	{
		/// <summary>
		/// Any full URL
		/// </summary>
		public string Editor_URL;

		/// <summary>
		/// This is the "Package family name" that can be found in your "Package.appxmanifest".
		/// </summary>
		public string Win8_PackageFamilyName;

		/// <summary>
		/// This is the "App ID" that can be found in your "Package.appxmanifest" under "Package Name".
		/// </summary>
		public string WP8_AppID;

		/// <summary>
		/// Pass in your AppID "xxxxxxxxx"
		/// </summary>
		public string iOS_AppID;

		/// <summary>
		/// Set to your target Android device store
		/// </summary>
		public MarketingStores Android_MarketingStore = MarketingStores.GooglePlay;

		/// <summary>
		/// Pass in your bundle ID "com.Company.AppName"
		/// </summary>
		public string Android_GooglePlay_BundleID;

		/// <summary>
		/// Pass in your bundle ID "com.Company.AppName"
		/// </summary>
		public string Android_Amazon_BundleID;

		/// <summary>
		/// Pass in your bundle ID "com.Company.AppName"
		/// </summary>
		public string Android_Samsung_BundleID;

		/// <summary>
		/// Pass in your AppID "xxxxxxxx".
		/// </summary>
		public string BB10_AppID;
	}
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base Marketing interface object
	/// </summary>
	public interface IIMarketingPlugin
	{
		/// <summary>
		/// Use to open store
		/// </summary>
		/// <param name="desc">Market desc</param>
		void OpenStore(MarketingDesc desc);

		/// <summary>
		/// Use to open store for app review
		/// </summary>
		/// <param name="desc">Market desc</param>
		void OpenStoreForReview(MarketingDesc desc);
	}
}
