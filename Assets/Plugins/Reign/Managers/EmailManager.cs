// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	/// <summary>
	/// Used to manage email requests.
	/// </summary>
	public static class EmailManager
	{
		private static IEmailPlugin plugin;

		static EmailManager()
		{
			ReignServices.CheckStatus();
			
			#if !DISABLE_REIGN
			#if UNITY_EDITOR
			plugin = new EmailPlugin();
			#elif UNITY_WINRT
			plugin = new EmailPlugin_WinRT();
			#elif UNITY_ANDROID
			plugin = new EmailPlugin_Android();
			#elif UNITY_IOS
			plugin = new EmailPlugin_iOS();
			#elif UNITY_BLACKBERRY
			plugin = new EmailPlugin_BB10();
			#elif UNITY_STANDALONE_WIN
			plugin = new EmailPlugin_Win32();
			#else
			plugin = new EmailPlugin_Dumy();
			#endif
			#endif
		}

		/// <summary>
		/// Use to open a native email request.
		/// </summary>
		/// <param name="to">The email the message is to.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="body">The body of the email.</param>
		public static void Send(string to, string subject, string body)
		{
			plugin.Send(to, subject, body);
		}
	}

	namespace Plugin
	{
		public class EmailPlugin_Dumy : IEmailPlugin
		{
			public void Send(string to, string subject, string body)
			{
				// do nothing...
			}
		}
	}
}