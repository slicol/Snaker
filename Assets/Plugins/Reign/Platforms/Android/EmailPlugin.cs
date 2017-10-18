#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class EmailPlugin_Android : IEmailPlugin
	{
		private AndroidJavaClass native;
		
		public EmailPlugin_Android()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.EmailNative");
		}
		
		~EmailPlugin_Android()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}
	
		public void Send(string to, string subject, string body)
		{
			native.CallStatic("Send", to, subject, body);
		}
	}
}
#endif