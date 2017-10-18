#if UNITY_IPHONE
using System;
using UnityEngine;

namespace Reign.Plugin
{
    public class MarketingPlugin_iOS : IIMarketingPlugin
    {
		private int osMajorVersion()
		{
			Debug.Log("Market for: " + SystemInfo.operatingSystem);
			string version = System.Text.RegularExpressions.Regex.Match(SystemInfo.operatingSystem, @"\d*\.").Value.Replace(".", "");
			int value;
			if (int.TryParse(version, out value)) return value;
			else return 0;
		}
		
    	public void OpenStore(MarketingDesc desc)
		{
			if (osMajorVersion() >= 7) Application.OpenURL("itms-apps://itunes.apple.com/app/id" + desc.iOS_AppID);
			else Application.OpenURL("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=" + desc.iOS_AppID);
		}
		
		public void OpenStoreForReview(MarketingDesc desc)
		{
			if (osMajorVersion() >= 7) Application.OpenURL("itms-apps://itunes.apple.com/app/id" + desc.iOS_AppID);
			else Application.OpenURL("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=" + desc.iOS_AppID);
		}
    }
}
#endif