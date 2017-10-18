#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Reign.Plugin
{
	public class MessageBoxPlugin_Android : IMessageBoxPlugin
	{
		private AndroidJavaClass native;
		private MessageBoxCallback callback;
	
		public MessageBoxPlugin_Android()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.MessageBoxNative");
		}
		
		~MessageBoxPlugin_Android()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}
	
		public void Show(string title, string message, MessageBoxTypes type, MessageBoxOptions options, MessageBoxCallback callback)
		{
			this.callback = callback;
			native.CallStatic("Show", title, message, options.OkButtonName, options.CancelButtonText, convertType(type));
		}
		
		private int convertType(MessageBoxTypes type)
		{
			switch (type)
			{
				case MessageBoxTypes.Ok: return 0;
				case MessageBoxTypes.OkCancel: return 1;
				default: throw new Exception("Unsuported MessageBoxType: " + type);
			}
		}
		
		public void Update()
		{
			if (native.CallStatic<bool>("GetOkStatus") && callback != null) callback(MessageBoxResult.Ok);
			if (native.CallStatic<bool>("GetCancelStatus") && callback != null) callback(MessageBoxResult.Cancel);
		}
	}
}
#endif