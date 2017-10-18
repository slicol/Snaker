using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	/// <summary>
	/// Used to manage social features
	/// </summary>
	public static class SocialManager
	{
		private static ISocialPlugin plugin;

		static SocialManager()
		{
			#if !DISABLE_REIGN
			#if UNITY_EDITOR
			plugin = new SocialPlugin_Dumy();
			#elif UNITY_WINRT
			plugin = new SocialPlugin_WinRT();
			#elif UNITY_ANDROID
			plugin = new SocialPlugin_Android();
			#elif UNITY_IOS
			plugin = new SocialPlugin_iOS();
			#elif UNITY_BLACKBERRY
			plugin = new SocialPlugin_BB10();
			#else
			plugin = new SocialPlugin_Dumy();
			#endif
			#endif
		}

		/// <summary>
		/// Used to init the Social API
		/// </summary>
		/// <param name="desc">Social Desc</param>
		public static void Init(SocialDesc desc)
		{
			plugin.Init(desc);
		}

		/// <summary>
		/// Invokes the native share view (BlackBerry uses Unity UI)
		/// </summary>
		/// <param name="data">Data you wish to share (taks priority over Text if multiple share types not supported)</param>
		/// <param name="dataFilename">Name of file without ext</param>
		/// <param name="text">Text you wish to share</param>
		/// <param name="title">Title for native view</param>
		/// <param name="desc">Description for native view</param>
		/// <param name="type">Type of data sharing</param>
		public static void Share(byte[] data, string dataFilename, string text, string title, string desc, SocialShareDataTypes type)
		{
			plugin.Share(data, dataFilename, text, title, desc, type);
		}

		/// <summary>
		/// Invokes the native share view (BlackBerry uses Unity UI)
		/// </summary>
		/// <param name="data">Data you wish to share (taks priority over Text if multiple share types not supported)</param>
		/// <param name="dataFilename">Name of file without ext</param>
		/// <param name="text">Text you wish to share</param>
		/// <param name="title">Title for native view</param>
		/// <param name="desc">Description for native view</param>
		/// <param name="x">iOS view position X</param>
		/// <param name="y">iOS view position Y</param>
		/// <param name="width">iOS view Width</param>
		/// <param name="height">iOS view Height</param>
		/// <param name="type">Type of data sharing</param>
		public static void Share(byte[] data, string dataFilename, string text, string title, string desc, int x, int y, int width, int height, SocialShareDataTypes type)
		{
			plugin.Share(data, dataFilename, text, title, desc, x, y, width, height, type);
		}
	}

	public class SocialPlugin_Dumy : ISocialPlugin
	{
		public void Init(SocialDesc desc)
		{
			Debug.Log("Share not supported in this environment!");
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, SocialShareDataTypes type)
		{
			Debug.Log("Share not supported in this environment!");
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, int x, int y, int width, int height, SocialShareDataTypes type)
		{
			Debug.Log("Share not supported in this environment!");
		}
	}
}