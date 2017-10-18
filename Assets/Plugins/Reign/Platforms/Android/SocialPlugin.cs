#if UNITY_ANDROID
using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
	public class SocialPlugin_Android : ISocialPlugin
	{
		private AndroidJavaClass native;

		public SocialPlugin_Android()
		{
			native = new AndroidJavaClass("com.reignstudios.reignnative.SocialNative");
		}

		public void Init(SocialDesc desc)
		{
			// do nothing...
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, SocialShareDataTypes type)
		{
			// check data type is valid
			if (data != null && type != SocialShareDataTypes.Image_PNG && type != SocialShareDataTypes.Image_JPG)
			{
				Debug.LogError("Unusported Share type: " + type);
				return;
			}

			native.CallStatic("ShareImage", data, dataFilename, text, title, type == SocialShareDataTypes.Image_PNG);
		}

		public void Share(byte[] data, string dataFilename, string text, string title, string desc, int x, int y, int width, int height, SocialShareDataTypes type)
		{
			Share(data, dataFilename, text, title, desc, type);
		}
	}
}
#endif