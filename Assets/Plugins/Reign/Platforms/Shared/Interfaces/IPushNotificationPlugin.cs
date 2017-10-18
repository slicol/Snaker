using UnityEngine;
using System.Collections;

namespace Reign
{
	public class PushNotificationsDesc
	{
		public string WinRT_ServicesURL;
	}
}

namespace Reign.Plugin
{
	public interface IPushNotificationPlugin
	{
		void Init(PushNotificationsDesc desc);
	}
}