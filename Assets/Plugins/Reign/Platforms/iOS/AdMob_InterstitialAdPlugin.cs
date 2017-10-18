#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Reign.Plugin
{
	public class AdMob_InterstitialAdPlugin_iOS : IInterstitialAdPlugin
	{
		private IntPtr native;
		private InterstitialAdEventCallbackMethod eventCallback;
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_InitAd")]
		private static extern IntPtr AdMob_Interstitial_InitAd(bool testing);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_DisposeAd")]
		private static extern void AdMob_Interstitial_DisposeAd(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_CreateAd")]
		private static extern void AdMob_Interstitial_CreateAd(IntPtr native, string unitID);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_AdHasEvents")]
		private static extern bool AdMob_Interstitial_AdHasEvents(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_GetNextAdEvent")]
		private static extern IntPtr AdMob_Interstitial_GetNextAdEvent(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_Show")]
		private static extern void AdMob_Interstitial_Show(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="AdMob_Interstitial_Cache")]
		private static extern void AdMob_Interstitial_Cache(IntPtr native);
		
		public AdMob_InterstitialAdPlugin_iOS (InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod callback)
		{
			bool pass = true;
			try
			{
				eventCallback = desc.EventCallback;
				native = AdMob_Interstitial_InitAd(desc.Testing);
				
				AdMob_Interstitial_CreateAd(native, desc.iOS_AdMob_UnitID);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
			
			if (callback != null) callback(pass);
		}
		
		~AdMob_InterstitialAdPlugin_iOS()
		{
			Dispose();
		}

		public void Dispose()
		{
			AdMob_Interstitial_DisposeAd(native);
			native = IntPtr.Zero;
		}
		
		public void Cache()
		{
			AdMob_Interstitial_Cache(native);
		}
		
		public void Show()
		{
			AdMob_Interstitial_Show(native);
		}
		
		public void Update()
		{
			if (eventCallback != null && AdMob_Interstitial_AdHasEvents(native))
			{
				IntPtr ptr = AdMob_Interstitial_GetNextAdEvent(native);
				string message = Marshal.PtrToStringAnsi(ptr);
				var values = message.Split(':');
				switch (values[0])
				{
					case "Cached": eventCallback(InterstitialAdEvents.Cached, null); break;
					case "Shown": eventCallback(InterstitialAdEvents.Shown, null); break;
					case "Canceled": eventCallback(InterstitialAdEvents.Canceled, null); break;
					case "Clicked": eventCallback(InterstitialAdEvents.Clicked, null); break;
					case "Error": eventCallback(InterstitialAdEvents.Error, values[1]); break;
				}
			}
		}

		public void OnGUI()
		{
			// do nothing...
		}

		public void OverrideOnGUI()
		{
			// do nothing...
		}
	}
}
#endif