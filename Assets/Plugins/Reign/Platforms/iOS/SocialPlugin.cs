#if UNITY_IOS
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class SocialPlugin_iOS : ISocialPlugin
	{
		[DllImport("__Internal", EntryPoint="InitSocial")]
		private static extern void InitSocial();

		[DllImport("__Internal", EntryPoint="DisposeSocial")]
		private static extern void DisposeSocial();

		[DllImport("__Internal", EntryPoint="Social_ShareImage")]
		private static extern void Social_ShareImage(byte[] data, string text, int dataLength, bool isPNG, int x, int y, int width, int height);

		public SocialPlugin_iOS()
		{
			InitSocial();
		}

		~SocialPlugin_iOS()
		{
			DisposeSocial();
		}

		public void Init(SocialDesc desc)
		{
			// do nothing...
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, SocialShareDataTypes type)
		{
			Share(data, dataFilename, text, title, desc, 0, 0, 10, 10, type);
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, int x, int y, int width, int height, SocialShareDataTypes type)
		{
			// check data type is valid
			if (data != null && type != SocialShareDataTypes.Image_PNG && type != SocialShareDataTypes.Image_JPG)
			{
				Debug.LogError("Unusported Share type: " + type);
				return;
			}

			Social_ShareImage(data, text, data.Length, type == SocialShareDataTypes.Image_PNG, x, y, width, height);
		}
	}
}
#endif