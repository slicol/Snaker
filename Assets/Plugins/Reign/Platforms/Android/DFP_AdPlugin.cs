#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class DFP_AdPlugin_Android : IAdPlugin
	{
		private bool visible;
		public bool Visible
		{
			get {return visible;}
			set
			{
				visible = value;
				native.CallStatic("SetVisible", id, value);
			}
		}

		private string id;
		private AndroidJavaClass native;
		private AdEventCallbackMethod eventCallback;
		private AdCreatedCallbackMethod createdCallback;

		public DFP_AdPlugin_Android(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			this.createdCallback = createdCallback;
			try
			{
				eventCallback = desc.EventCallback;
				native = new AndroidJavaClass("com.reignstudios.reignnative.DFP_AdsNative");
				visible = desc.Visible;
				id = Guid.NewGuid().ToString();
				native.CallStatic("CreateAd", desc.Android_DFP_UnitID, desc.Testing, desc.Visible, convertGravity(desc.Android_DFP_AdGravity), convertAdSize(desc.Android_DFP_AdSize), id);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false);
			}
		}

		~DFP_AdPlugin_Android()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}

		public void Dispose()
		{
			if (native != null)
			{
				native.CallStatic("Dispose", id);
				native.Dispose();
				native = null;
			}
		}

		public void Refresh()
		{
			native.CallStatic("Refresh", id);
		}

		public void SetGravity(AdGravity gravity)
		{
			native.CallStatic("SetGravity", id, convertGravity(gravity));
		}

		private int convertAdSize(Android_DFP_AdSize adSize)
		{
			switch (adSize)
			{
				case Android_DFP_AdSize.Banner_320x50: return 0;
				case Android_DFP_AdSize.SmartBanner: return 1;
				case Android_DFP_AdSize.FullBanner_468x60: return 2;
				case Android_DFP_AdSize.Leaderboard_728x90: return 3;
				case Android_DFP_AdSize.MediumRectangle_300x250: return 4;
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
			}

			return gravityIndex;
		}

		public void Update()
		{
			if (eventCallback != null && native.CallStatic<bool>("HasEvents"))
			{
				string eventName = native.CallStatic<string>("GetNextEvent");
				var eventValues = eventName.Split(':');
				switch (eventValues[0])
				{
					case "Created": if (createdCallback != null) createdCallback(eventValues[1] != "Failed"); break;
					case "Refreshed": eventCallback(AdEvents.Refreshed, null); break;
					case "Clicked": eventCallback(AdEvents.Clicked, null); break;
					case "Error": eventCallback(AdEvents.Error, eventValues[1]); break;
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