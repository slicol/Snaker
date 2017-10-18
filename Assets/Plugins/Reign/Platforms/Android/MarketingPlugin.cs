#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Reign.Plugin
{
    public class MarketingPlugin_Android : IIMarketingPlugin
    {
		private AndroidJavaClass nativeGooglePlay, nativeAmazon, nativeSamsung;
		
		public MarketingPlugin_Android()
		{
			nativeGooglePlay = new AndroidJavaClass("com.reignstudios.reignnative.GooglePlay_MarketingNative");
			nativeAmazon = new AndroidJavaClass("com.reignstudios.reignnative.Amazon_MarketingNative");
			nativeSamsung = new AndroidJavaClass("com.reignstudios.reignnative.Samsung_MarketingNative");
		}
    
    	public void OpenStore(MarketingDesc desc)
		{
			switch (desc.Android_MarketingStore)
			{
				case MarketingStores.GooglePlay: nativeGooglePlay.CallStatic("OpenStore", desc.Android_GooglePlay_BundleID); break;
				case MarketingStores.Amazon: nativeAmazon.CallStatic("OpenStore", desc.Android_Amazon_BundleID); break;
				case MarketingStores.Samsung: nativeSamsung.CallStatic("OpenStore", desc.Android_Samsung_BundleID); break;
				default: throw new Exception("Unknown Android market: " + desc.Android_MarketingStore);
			}
		}
		
		public void OpenStoreForReview(MarketingDesc desc)
		{
			switch (desc.Android_MarketingStore)
			{
				case MarketingStores.GooglePlay: nativeGooglePlay.CallStatic("OpenStore", desc.Android_GooglePlay_BundleID); break;
				case MarketingStores.Amazon: nativeAmazon.CallStatic("OpenStore", desc.Android_Amazon_BundleID); break;
				case MarketingStores.Samsung: nativeSamsung.CallStatic("OpenStore", desc.Android_Samsung_BundleID); break;
				default: throw new Exception("Unknown Android market: " + desc.Android_MarketingStore);
			}
		}
    }
}
#endif