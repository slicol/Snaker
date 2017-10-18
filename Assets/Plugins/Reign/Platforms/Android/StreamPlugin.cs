#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.IO;

namespace Reign.Plugin
{
	public class StreamPlugin_Android : StreamPluginBase
	{
		private UnityEngine.AndroidJavaClass native;
		StreamSavedCallbackMethod streamFileSavedCallback;
		StreamLoadedCallbackMethod streamFileLoadedCallback;
		
		public StreamPlugin_Android()
		{
			native = new UnityEngine.AndroidJavaClass("com.reignstudios.reignnative.StreamNative");
			native.CallStatic("Init");
		}
		
		~StreamPlugin_Android()
		{
			Dispose();
		}
		
		public void Dispose()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}
		}

		public override void Update()
		{
			if (native.CallStatic<bool>("CheckSaveImageDone"))
			{
				if (streamFileSavedCallback != null)
				{
					streamFileSavedCallback(native.CallStatic<bool>("CheckSaveImageSucceeded"));
					streamFileSavedCallback = null;
				}
			}
			
			if (native.CallStatic<bool>("CheckLoadImageDone"))
			{
				if (streamFileLoadedCallback != null)
				{
					bool succeeded = native.CallStatic<bool>("CheckLoadImageSucceeded");
					if (succeeded) streamFileLoadedCallback(new MemoryStream(native.CallStatic<byte[]>("GetLoadedImageData")), succeeded);
					else streamFileLoadedCallback(null, succeeded);
					
					streamFileLoadedCallback = null;
				}
			}
		}

		public override void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback)
		{
			if (folderLocation == FolderLocations.Pictures)
			{
				streamFileSavedCallback = steamSavedCallback;
				native.CallStatic("SaveImage", data, Path.GetFileNameWithoutExtension(fileName), "");
			}
			else if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("Save file in folder location: " + folderLocation + " is not supported.");
				if (steamSavedCallback != null) steamSavedCallback(false);
			}
			else
			{
				base.SaveFile(fileName, data, folderLocation, steamSavedCallback);
			}
		}

		public override void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("Load file in folder location: " + folderLocation + " is not supported.");
				streamLoadedCallback(null, false);
			}
			else
			{
				base.LoadFile(fileName, folderLocation, streamLoadedCallback);
			}
		}

		public override void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (folderLocation != FolderLocations.Pictures)
			{
				Debug.LogError("LoadFileDialog not supported for folder location: " + folderLocation + " on this Platform yet.");
				streamLoadedCallback(null, false);
			}
			else
			{
				streamFileLoadedCallback = streamLoadedCallback;
				native.CallStatic("LoadImage", maxWidth, maxHeight);
			}
		}

		public override void LoadCameraPicker (CameraQuality quality, int maxWidth, int maxHeight, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			streamFileLoadedCallback = streamLoadedCallback;
			native.CallStatic("LoadCameraPicker", maxWidth, maxHeight);
		}
	}
}
#endif