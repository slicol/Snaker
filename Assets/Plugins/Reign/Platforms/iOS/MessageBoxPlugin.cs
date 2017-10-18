#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class MessageBoxPlugin_iOS : IMessageBoxPlugin
	{
		private MessageBoxCallback callback;
		
		[DllImport("__Internal", EntryPoint="InitMessageBox")]
		private static extern void InitMessageBox();
		
		[DllImport("__Internal", EntryPoint="DisposeMessageBox")]
		private static extern void DisposeMessageBox();
		
		[DllImport("__Internal", EntryPoint="ShowMessageBox")]
		private static extern void ShowMessageBox(string title, string message, string okButtonText, string cancelButtonText, int type);
		
		[DllImport("__Internal", EntryPoint="MessageBoxOkClicked")]
		private static extern bool MessageBoxOkClicked();
		
		[DllImport("__Internal", EntryPoint="MessageBoxCancelClicked")]
		private static extern bool MessageBoxCancelClicked();
	
		public MessageBoxPlugin_iOS()
		{
			InitMessageBox();
		}
		
		~MessageBoxPlugin_iOS()
		{
			DisposeMessageBox();
		}
	
		public void Show(string title, string message, MessageBoxTypes type, MessageBoxOptions options, MessageBoxCallback callback)
		{
			this.callback = callback;
			ShowMessageBox(title, message, options.OkButtonName, options.CancelButtonText, type == MessageBoxTypes.Ok ? 0 : 1);
		}
		
		public void Update()
		{
			if (MessageBoxOkClicked() && callback != null) callback(MessageBoxResult.Ok);
			if (MessageBoxCancelClicked() && callback != null) callback(MessageBoxResult.Cancel);
		}
	}
}
#endif