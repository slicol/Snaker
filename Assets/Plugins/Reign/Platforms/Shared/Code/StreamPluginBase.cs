#if !UNITY_WINRT || UNITY_EDITOR
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Reign.Plugin
{
	public class StreamPluginBase : IStreamPlugin
	{
		public virtual void Update()
		{
			// do nothing...
		}

		public void FileExists(string fileName, FolderLocations folderLocation, StreamExistsCallbackMethod callback)
		{
			if (callback == null) return;
			if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("FileExists Error: Only Storage folder location is currently supported.");
				callback(false);
				return;
			}

			callback(File.Exists(fileName));
		}

		public void DeleteFile(string fileName, FolderLocations folderLocation, StreamDeleteCallbackMethod callback)
		{
			if (folderLocation != FolderLocations.Storage)
			{
				Debug.LogError("DeleteFile Error: Only Storage folder location is currently supported.");
				callback(false);
				return;
			}

			try
			{
				File.Delete(fileName);
			}
			catch (Exception e)
			{
				Debug.LogError("DeleteFile Error: " + e.Message);
				callback(false);
				return;
			}

			callback(true);
		}
	
		public void SaveFile(string fileName, Stream stream, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback)
		{
			var data = new byte[stream.Length];
			stream.Read(data, 0, data.Length);
			SaveFile(fileName, data, folderLocation, steamSavedCallback);
		}

		public virtual void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod steamSavedCallback)
		{
			try
			{
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if(steamSavedCallback != null) steamSavedCallback(false);
				return;
			}

			if(steamSavedCallback != null) steamSavedCallback(true);
		}

		public virtual void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (streamLoadedCallback == null) return;

			MemoryStream stream = null;
			try
			{
				using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					var data = new byte[file.Length];
					file.Read(data, 0, data.Length);
					stream = new MemoryStream(data);
				}
			}
			catch (Exception e)
			{
				if (stream != null) stream.Dispose();
				Debug.LogError(e.Message);
				streamLoadedCallback(null, false);
				return;
			}

			streamLoadedCallback(stream, true);
		}

		public virtual void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			Debug.LogError("SaveFileDialog not supported on this Platform!");
			if (streamSavedCallback != null) streamSavedCallback(false);
		}

		public virtual void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			Debug.LogError("SaveFileDialog not supported on this Platform!");
			if (streamSavedCallback != null) streamSavedCallback(false);
		}
		
		public virtual void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			Debug.LogError("LoadFileDialog not supported on this Platform!");
			if (streamLoadedCallback != null) streamLoadedCallback(null, false);
		}

		public virtual void LoadCameraPicker(CameraQuality quality, int maxWidth, int maxHeight, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			Debug.LogError("LoadCameraPicker not supported on this Platform!");
			if (streamLoadedCallback != null) streamLoadedCallback(null, false);
		}
	}
}
#endif