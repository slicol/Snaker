using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using ImageTools.IO.Gif;
using ImageTools;

namespace Reign
{
	/// <summary>
	/// Used when the gif image needs to be updated
	/// </summary>
	/// <param name="frame">Next frame</param>
	public delegate void GIFFrameUpdatedCallbackMethod(TextureGIFFrame frame);

	/// <summary>
	/// Used to manage gif frame animation
	/// </summary>
	public class TextureGIFFrame
	{
		private static float time;

		/// <summary>
		/// The frame texture
		/// </summary>
		public Texture2D Texture {get; private set;}

		/// <summary>
		/// The frame sprite
		/// </summary>
		public Sprite Sprite {get; private set;}

		/// <summary>
		/// The next frame after this one
		/// </summary>
		public TextureGIFFrame NextFrame {get{return nextFrame;}}
		internal TextureGIFFrame nextFrame;

		/// <summary>
		/// The length of time this frame is active
		/// </summary>
		public TimeSpan FrameTime {get; private set;}

		/// <summary>
		/// Used to contruct a gif frame
		/// </summary>
		/// <param name="texture">Frame texture</param>
		/// <param name="timeSpan">Frame time length</param>
		public TextureGIFFrame(Texture2D texture, TimeSpan timeSpan)
		{
			this.Texture = texture;
			this.Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			this.FrameTime = timeSpan;
		}

		/// <summary>
		/// Used to update frame
		/// </summary>
		/// <returns>Next or Current frame</returns>
		public TextureGIFFrame Update()
		{
			if (FrameTime.TotalSeconds == 0) return this;

			TextureGIFFrame currentFrame = this;
			time += Time.deltaTime;
			if (time >= FrameTime.TotalSeconds)
			{
				time = 0;
				currentFrame = NextFrame;
			}

			return currentFrame;
		}
	}

	/// <summary>
	/// Use to manage gif images
	/// </summary>
	public class TextureGIF
	{
		private List<TextureGIFFrame> frames;

		/// <summary>
		/// Get the current frame
		/// </summary>
		public TextureGIFFrame CurrentFrame {get; private set;}

		/// <summary>
		/// Called when gif frame texture has updated
		/// </summary>
		public GIFFrameUpdatedCallbackMethod FrameUpdatedCallback;

		private Texture2D createTexture(int width, int height, byte[] pixels)
		{
			var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			var colors = new Color32[pixels.Length / 4];
			for (int y = 0; y != texture.height; ++y)
			{
				for (int x = 0; x != texture.width; ++x)
				{
					int i = x + (y * texture.width);
					int i2 = (x + ((texture.height-1-y) * texture.width)) * 4;
					colors[i] = new Color32(pixels[i2], pixels[i2+1], pixels[i2+2], pixels[i2+3]);
				}
			}

			texture.SetPixels32(colors);
			texture.Apply();
			return texture;
		}

		private void init(Stream stream, GIFFrameUpdatedCallbackMethod frameUpdatedCallback)
		{
			// set gif frame update callback
			this.FrameUpdatedCallback = frameUpdatedCallback;

			// decode gif image
			var gifDecoder = new GifDecoder();
			var image = new ExtendedImage();
			gifDecoder.Decode(image, stream);

			// add frames and load unity textures
			frames = new List<TextureGIFFrame>();
			var firstFrame = new TextureGIFFrame(createTexture(image.PixelWidth, image.PixelHeight, image.Pixels), TimeSpan.FromSeconds(image.DelayTime / 100d));
			frames.Add(firstFrame);
			var lastFrame = firstFrame;
			foreach (var frame in image.Frames)
			{
				var newFrame = new TextureGIFFrame(createTexture(frame.PixelWidth, frame.PixelHeight, frame.Pixels), TimeSpan.FromSeconds(frame.DelayTime / 100d));
				frames.Add(newFrame);
				if (lastFrame != null) lastFrame.nextFrame = newFrame;
				lastFrame = newFrame;
			}

			// set starting image
			CurrentFrame = frames[0];
			lastFrame.nextFrame = firstFrame;
		}

		/// <summary>
		/// Construct gif from stream
		/// </summary>
		/// <param name="stream">Image Stream</param>
		/// <param name="frameUpdatedCallback">Called when gif frame texture has updated</param>
		public TextureGIF(Stream stream, GIFFrameUpdatedCallbackMethod frameUpdatedCallback)
		{
			init(stream, frameUpdatedCallback);
		}

		/// <summary>
		/// Construct gif from byte array
		/// </summary>
		/// <param name="data">Image data</param>
		/// <param name="frameUpdatedCallback">Called when gif frame texture has updated</param>
		public TextureGIF(byte[] data, GIFFrameUpdatedCallbackMethod frameUpdatedCallback)
		{
			using (var stream = new MemoryStream(data))
			{
				stream.Position = 0;
				init(stream, frameUpdatedCallback);
			}
		}

		/// <summary>
		/// Call to dispose object
		/// </summary>
		public void Dispose()
		{
			if (frames != null)
			{
				foreach (var frame in frames)
				{
					if (frame != null && frame.Texture != null) GameObject.DestroyImmediate(frame.Texture);
				}

				frames = null;
			}
		}

		/// <summary>
		/// Call to update gif animation
		/// </summary>
		public void Update()
		{
			var currentFrame = CurrentFrame.Update();
			if (currentFrame != CurrentFrame && FrameUpdatedCallback != null)
			{
				CurrentFrame = currentFrame;
				FrameUpdatedCallback(currentFrame);
			}
		}
	}
}