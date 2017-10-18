#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Png;
using ImageTools.Filtering;
using ImageTools;
using ImageTools.IO;
using System;

namespace Reign.Plugin
{
	public class StreamPlugin : StreamPluginBase
	{
		private static string generateFilterValue(string[] fileTypes)
		{
			string filterValue = "File Types;";
			foreach (var type in fileTypes)
			{
				filterValue += "*" + type;
				if (type != fileTypes[fileTypes.Length - 1]) filterValue += ";";
			}

			return filterValue;
		}

		public override void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (streamSavedCallback == null) return;

			var data = new byte[stream.Length];
			stream.Position = 0;
			stream.Read(data, 0, data.Length);
			SaveFileDialog(data, folderLocation, fileTypes, streamSavedCallback);
		}

		public override void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", generateFilterValue(fileTypes));
			if (!string.IsNullOrEmpty(fileName))
			{
				using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					stream.Write(data, 0, data.Length);
				}

				if (streamSavedCallback != null) streamSavedCallback(true);
			}
			else
			{
				if (streamSavedCallback != null) streamSavedCallback(false);
			}
		}

		public override void LoadFileDialog(FolderLocations folderLocation, int maxWidth, int maxHeight, int x, int y, int width, int height, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (streamLoadedCallback == null) return;
			string filename = EditorUtility.OpenFilePanel("Load file", "", generateFilterValue(fileTypes));
			if (!string.IsNullOrEmpty(filename))
			{
				if (maxWidth == 0 || maxHeight == 0 || folderLocation != FolderLocations.Pictures)
				{
					streamLoadedCallback(new FileStream(filename, FileMode.Open, FileAccess.Read), true);
				}
				else
				{
					var newStream = new MemoryStream();
					try
					{
						using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
						{
							IImageDecoder decoder = null;
							switch (Path.GetExtension(filename).ToLower())
							{
								case ".jpg": decoder = new JpegDecoder(); break;
								case ".jpeg": decoder = new JpegDecoder(); break;
								case ".png": decoder = new PngDecoder(); break;
								default:
									Debug.LogError("Unsuported file ext type: " + Path.GetExtension(filename));
									streamLoadedCallback(null, false);
									return;
							}
							var image = new ExtendedImage();
							decoder.Decode(image, stream);
							var newSize = MathUtilities.FitInViewIfLarger(image.PixelWidth, image.PixelHeight, maxWidth, maxHeight);
							var newImage = ExtendedImage.Resize(image, (int)newSize.x, (int)newSize.y, new NearestNeighborResizer());

							var encoder = new PngEncoder();
							encoder.Encode(newImage, newStream);
							newStream.Position = 0;
						}
					}
					catch (Exception e)
					{
						newStream.Dispose();
						newStream = null;
						Debug.LogError(e.Message);
					}
					finally
					{
						streamLoadedCallback(newStream, true);
					}
				}
			}
			else
			{
				streamLoadedCallback(null, false);
			}
		}
	}
}
#endif