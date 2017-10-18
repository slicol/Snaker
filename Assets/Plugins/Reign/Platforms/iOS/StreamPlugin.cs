#if UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace Reign.Plugin
{
	public class StreamPlugin_iOS : StreamPluginBase
	{
		[DllImport("__Internal", EntryPoint="InitStream")]
		private static extern void InitStream();
		
		[DllImport("__Internal", EntryPoint="DisposeStream")]
		private static extern void DisposeStream();
		
		[DllImport("__Internal", EntryPoint="CheckImageSavedDoneStatus")]
		private static extern bool CheckImageSavedDoneStatus();
		
		[DllImport("__Internal", EntryPoint="CheckImageSavedSucceededStatus")]
		private static extern bool CheckImageSavedSucceededStatus();
		
		[DllImport("__Internal", EntryPoint="SaveImageStream")]
		private unsafe static extern void SaveImageStream(byte* data, int dataSize);
		
		[DllImport("__Internal", EntryPoint="CheckImageLoadStatus")]
		private static extern bool CheckImageLoadStatus();
		
		[DllImport("__Internal", EntryPoint="CheckImageLoadSucceededStatus")]
		private unsafe static extern bool CheckImageLoadSucceededStatus(ref IntPtr data, int* dataSize);
		
		[DllImport("__Internal", EntryPoint="LoadImagePicker")]
		private static extern void LoadImagePicker(int maxWidth, int maxHeight, int x, int y, int width, int height);

		[DllImport("__Internal", EntryPoint="LoadCameraPicker")]
		private static extern void LoadCameraPicker(int maxWidth, int maxHeight);
		
		public StreamPlugin_iOS()
		{
			InitStream();
		}
		
		public void Dispose()
		{
			DisposeStream();
		}
		
		StreamSavedCallbackMethod streamFileSavedCallback;
		StreamLoadedCallbackMethod streamFileLoadedCallback;
		public override void Update()
		{
			if (CheckImageSavedDoneStatus())
			{
				if (streamFileSavedCallback != null)
				{
					streamFileSavedCallback(CheckImageSavedSucceededStatus());
					streamFileSavedCallback = null;
				}
			}
			
			if (CheckImageLoadStatus())
			{
				if (streamFileLoadedCallback != null)
				{
					bool succeeded;
					byte[] data = null;
					unsafe
					{
						IntPtr dataPtr = IntPtr.Zero;
						int dataSize;
						succeeded = CheckImageLoadSucceededStatus(ref dataPtr, &dataSize);
						if (succeeded)
						{
							data = new byte[dataSize];
							Marshal.Copy(dataPtr, data, 0, data.Length);
							Marshal.FreeHGlobal(dataPtr);
						}
					}
					
					streamFileLoadedCallback(succeeded ? new MemoryStream(data) : null, succeeded);
					streamFileLoadedCallback = null;
				}
			}
		}

		public override void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback)
		{
			if (folderLocation == FolderLocations.Pictures)
			{
				streamFileSavedCallback = steamSavedCallback;
				unsafe
				{
					fixed (byte* dataPtr = data)
					{
						SaveImageStream(dataPtr, data.Length);
					}
				}
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
				LoadImagePicker(maxWidth, maxHeight, x, y, width, height);
			}
		}

		public override void LoadCameraPicker (CameraQuality quality, int maxWidth, int maxHeight, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			streamFileLoadedCallback = streamLoadedCallback;
			LoadCameraPicker(maxWidth, maxHeight);
		}
	}
}
#endif