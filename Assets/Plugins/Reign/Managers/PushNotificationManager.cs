using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	public static class PushNotificationManager
	{
		private static IPushNotificationPlugin plugin;

		static PushNotificationManager()
		{
			#if UNITY_WINRT
			plugin = new PushNotificationPlugin_WinRT();
			#else
			plugin = new PushNotification_Dumy();
			#endif
		}

		public static void Init(PushNotificationsDesc desc)
		{
			plugin.Init(desc);
		}
	}
}

namespace Reign.Plugin
{
	public class PushNotification_Dumy : IPushNotificationPlugin
	{
		public void Init(PushNotificationsDesc desc)
		{
			// do nothing...
		}
	}
}