#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class EmailPlugin_iOS : IEmailPlugin
	{
		[DllImport("__Internal", EntryPoint="InitEmail")]
		private static extern void InitEmail();
		
		[DllImport("__Internal", EntryPoint="DisposeEmail")]
		private static extern void DisposeEmail();
		
		[DllImport("__Internal", EntryPoint="SendEmail")]
		private static extern void SendEmail(string to, string subject, string body);
	
		public EmailPlugin_iOS()
		{
			InitEmail();
		}
		
		~EmailPlugin_iOS()
		{
			DisposeEmail();
		}
	
		public void Send(string to, string subject, string body)
		{
			SendEmail(to, subject, body);
		}
	}
}
#endif