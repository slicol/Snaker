// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using Reign.Plugin;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Reign
{
	/// <summary>
	/// Used to manage message boxes.
	/// </summary>
	public static class MessageBoxManager
	{
		private static IMessageBoxPlugin plugin;

		static MessageBoxManager()
		{
			ReignServices.CheckStatus();
			
			#if !DISABLE_REIGN
			#if UNITY_EDITOR
			plugin = new MessageBoxPlugin();
			#elif UNITY_WINRT
			plugin = new MessageBoxPlugin_WinRT();
			#elif UNITY_ANDROID
			plugin = new MessageBoxPlugin_Android();
			#elif UNITY_IOS
			plugin = new MessageBoxPlugin_iOS();
			#elif UNITY_BLACKBERRY
			plugin = new MessageBoxPlugin_BB10();
			#elif UNITY_STANDALONE_WIN
			plugin = new MessageBoxPlugin_Win32();
			#elif UNITY_STANDALONE_OSX
			plugin = new MessageBoxPlugin_OSX();
			#else
			plugin = new MessageBoxPlugin_Dumy();
			#endif

			ReignServices.AddService(update, null, null);
			#endif
		}

		/// <summary>
		/// Use to show a simple OK message box.
		/// </summary>
		/// <param name="title">MessageBox Title.</param>
		/// <param name="message">MessageBox Message.</param>
		public static void Show(string title, string message)
		{
			plugin.Show(title, message, MessageBoxTypes.Ok, new MessageBoxOptions(), null);
		}

		/// <summary>
		/// Use to show an OK or OK/Cancel message box.
		/// </summary>
		/// <param name="title">MessageBox Title.</param>
		/// <param name="message">MessageBox Message.</param>
		/// <param name="type">MessageBox Type.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void Show(string title, string message, MessageBoxTypes type, MessageBoxCallback callback)
		{
			plugin.Show(title, message, type, new MessageBoxOptions(), callback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="title">MessageBox Title.</param>
		/// <param name="message">MessageBox Message.</param>
		/// <param name="type">MessageBox Type.</param>
		/// <param name="options">MessageBox Options.</param>
		/// <param name="callback">The callback that fires when done.</param>
		public static void Show(string title, string message, MessageBoxTypes type, MessageBoxOptions options, MessageBoxCallback callback)
		{
			if (options == null) options = new MessageBoxOptions();
			plugin.Show(title, message, type, options, callback);
		}
		
		private static void update()
		{
			plugin.Update();
		}
	}

	namespace Plugin
	{
		public class MessageBoxPlugin_Dumy : IMessageBoxPlugin
		{
			public void Show(string title, string message, MessageBoxTypes type, MessageBoxOptions options, MessageBoxCallback callback)
			{
				if (callback != null) callback(MessageBoxResult.Cancel);
			}

			public void Update()
			{
				// do nothing...
			}
		}
	}
}