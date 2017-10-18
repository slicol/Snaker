#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Reign.Plugin
{
	public class DFP_AdPlugin_iOS : IAdPlugin
	{
		private bool visible;
		public bool Visible
		{
			get {return visible;}
			set
			{
				visible = value;
				DFP_SetAdVisible(native, value);
			}
		}

		private IntPtr native;
		private AdEventCallbackMethod eventCallback;

		[DllImport("__Internal", EntryPoint="DFP_InitAd")]
		private static extern IntPtr DFP_InitAd(bool testing);

		[DllImport("__Internal", EntryPoint="DFP_DisposeAd")]
		private static extern void DFP_DisposeAd(IntPtr native);

		[DllImport("__Internal", EntryPoint="DFP_CreateAd")]
		private static extern void DFP_CreateAd(IntPtr native, int gravity, int adSizeIndex, string unitID);

		[DllImport("__Internal", EntryPoint="DFP_SetAdGravity")]
		private static extern void DFP_SetAdGravity(IntPtr native, int gravity);

		[DllImport("__Internal", EntryPoint="DFP_SetAdVisible")]
		private static extern void DFP_SetAdVisible(IntPtr native, bool visible);
		[DllImport("__Internal", EntryPoint="DFP_Refresh")]
		private static extern void DFP_Refresh(IntPtr native);

		[DllImport("__Internal", EntryPoint="DFP_AdHasEvents")]
		private static extern bool DFP_AdHasEvents(IntPtr native);

		[DllImport("__Internal", EntryPoint="DFP_GetNextAdEvent")]
		private static extern IntPtr DFP_GetNextAdEvent(IntPtr native);

		public DFP_AdPlugin_iOS(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			bool pass = true;
			try
			{
				eventCallback = desc.EventCallback;
				native = DFP_InitAd(desc.Testing);
				int gravity = convertGravity(desc.iOS_DFP_AdGravity);

				DFP_CreateAd(native, gravity, convertAdSize(desc.iOS_DFP_AdSize), desc.iOS_DFP_UnitID);
				Visible = desc.Visible;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}

			if (createdCallback != null) createdCallback(pass);
		}

		~DFP_AdPlugin_iOS()
		{
			Dispose();
		}

		public void Dispose()
		{
			DFP_DisposeAd(native);
			native = IntPtr.Zero;
		}

		private int convertAdSize(iOS_DFP_AdSize adSize)
		{
			switch (adSize)
			{
				case iOS_DFP_AdSize.Banner_320x50: return 0;
				case iOS_DFP_AdSize.FullBanner_468x60: return 1;
				case iOS_DFP_AdSize.Leaderboard_728x90: return 2;
				case iOS_DFP_AdSize.MediumRectangle_300x250: return 3;
				case iOS_DFP_AdSize.SmartBannerLandscape: return 4;
				case iOS_DFP_AdSize.SmartBannerPortrait: return 5;
			}

			return 0;
		}

		private int convertGravity(AdGravity gravity)
		{
			int gravityIndex = 0;
			switch (gravity)
			{
				case AdGravity.BottomLeft: gravityIndex = 0; break;
				case AdGravity.BottomRight: gravityIndex = 1; break;
				case AdGravity.BottomCenter: gravityIndex = 2; break;
				case AdGravity.TopLeft: gravityIndex = 3; break;
				case AdGravity.TopRight: gravityIndex = 4; break;
				case AdGravity.TopCenter: gravityIndex = 5; break;
				case AdGravity.CenterScreen: gravityIndex = 6; break;
			}

			return gravityIndex;
		}

		public void SetGravity(AdGravity gravity)
		{
			int gravityIndex = convertGravity(gravity);
			DFP_SetAdGravity(native, gravityIndex);
		}

		public void Refresh()
		{
			DFP_Refresh(native);
		}

		public void Update()
		{
			if (eventCallback != null && DFP_AdHasEvents(native))
			{
				IntPtr ptr = DFP_GetNextAdEvent(native);
				string message = Marshal.PtrToStringAnsi(ptr);
				var values = message.Split(':');
				switch (values[0])
				{
					case "Refreshed": eventCallback(AdEvents.Refreshed, null); break;
					case "Clicked": eventCallback(AdEvents.Clicked, null); break;
					case "Error": eventCallback(AdEvents.Error, values[1]); break;
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