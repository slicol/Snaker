// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using Reign;

public class StreamsDemo : MonoBehaviour
{
	public Sprite ReignLogo, ReignLogo2;
	public Image ReignImage;
	public Button BackButton, SaveFileButton, LoadFileButton, SaveToPicturesButton, LoadFromPicturesButton, CameraPickerButton, ImagePickerButton, SaveImageDlgButton;

	void Start()
	{
		// NOTE: Other usfull methods...
		//StreamManager.SaveScreenShotToPictures(...);
		//StreamManager.MakeFourCC(...);

		// NOTE: With ImageTools you can do stuff like >>>
		// <<< Decode images and resize them: (Example)
		/*
		var decoder = new PngDecoder();
		var image = new ExtendedImage();
		decoder.Decode(image, stream);
		var newImage = ExtendedImage.Resize(image, 32, 32, new NearestNeighborResizer());
		currentImage = new Texture2D(newImage.PixelWidth, newImage.PixelHeight);
		currentImage.SetPixels(newImage.Colors);
		currentImage.Apply();
		*/

		// <<< Encode images to other formats Unity doesn't support: (Example)
		/*
		var encoder = new PngEncoder();
		encoder.Encode(image, stream);
		*/

		// bind button events
		SaveFileButton.Select();
		BackButton.onClick.AddListener(backClicked);
		SaveFileButton.onClick.AddListener(saveFileClicked);
		LoadFileButton.onClick.AddListener(loadFileClicked);
		SaveToPicturesButton.onClick.AddListener(saveToPicturesClicked);
		LoadFromPicturesButton.onClick.AddListener(loadFromPicturesClicked);
		CameraPickerButton.onClick.AddListener(cameraPickerClicked);
		ImagePickerButton.onClick.AddListener(imagePickerClicked);
		SaveImageDlgButton.onClick.AddListener(saveImageDlgClicked);
	}

	private void saveImageDlgClicked()
	{
		// NOTE: SaveFileDialog are only supported on WinRT
		var data = ReignLogo.texture.EncodeToPNG();
		StreamManager.SaveFileDialog(data, FolderLocations.Pictures, new string[]{".png"}, imageSavedCallback);
	}

	private void imagePickerClicked()
	{
		// NOTE: Unity only supports loading png and jpg data
		StreamManager.LoadFileDialog(FolderLocations.Pictures, 128, 128, new string[]{".png", ".jpg", ".jpeg"}, imageLoadedCallback);
	}

	private void cameraPickerClicked()
	{
		StreamManager.LoadCameraPicker(CameraQuality.Med, 128, 128, imageLoadedCallback);
	}

	private void loadFromPicturesClicked()
	{
		// NOTE: LoadFile doesn't support the Pictures folder on iOS.
		disableButtons();
		StreamManager.LoadFile("TEST.png", FolderLocations.Pictures, imageLoadedCallback);
	}

	private void saveToPicturesClicked()
	{
		disableButtons();
		var data = ReignLogo.texture.EncodeToPNG();
		StreamManager.SaveFile("TEST.png", data, FolderLocations.Pictures, imageSavedCallback);
	}

	private void loadFileClicked()
	{
		disableButtons();
		StreamManager.LoadFile("MyFile.data", FolderLocations.Storage, dataFileLoadedCallback);
	}

	private void saveFileClicked()
	{
		disableButtons();
		var data = new byte[1] {(byte)UnityEngine.Random.Range(0, 255)};
		StreamManager.SaveFile("MyFile.data", data, FolderLocations.Storage, dataFileSavedCallback);
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	private void disableButtons()
	{
		BackButton.enabled = false;
		SaveFileButton.enabled = false;
		LoadFileButton.enabled = false;
		SaveToPicturesButton.enabled = false;
		LoadFromPicturesButton.enabled = false;
		CameraPickerButton.enabled = false;
		ImagePickerButton.enabled = false;
		SaveImageDlgButton.enabled = false;
	}

	private void enableButtons()
	{
		BackButton.enabled = true;
		SaveFileButton.enabled = true;
		LoadFileButton.enabled = true;
		SaveToPicturesButton.enabled = true;
		LoadFromPicturesButton.enabled = true;
		CameraPickerButton.enabled = true;
		ImagePickerButton.enabled = true;
		SaveImageDlgButton.enabled = true;
	}

	private void dataFileSavedCallback(bool succeeded)
	{
		enableButtons();
		MessageBoxManager.Show("Data Status", "Data Saved: " + succeeded);
	}

	private void dataFileLoadedCallback(Stream stream, bool succeeded)
	{
		try
		{
			enableButtons();
			MessageBoxManager.Show("Image Status", "Data Loaded: " + succeeded);
			if (succeeded) Debug.Log("Data Value: " + stream.ReadByte());
		}
		catch (Exception e)
		{
			MessageBoxManager.Show("Error", e.Message);
		}
		finally
		{
			// NOTE: Make sure you dispose of this stream !!!
			if (stream != null) stream.Dispose();
		}
	}

	private void imageSavedCallback(bool succeeded)
	{
		enableButtons();
		MessageBoxManager.Show("Image Status", "Image Saved: " + succeeded);
		if (succeeded) ReignImage.sprite = ReignLogo2;
	}

	private void imageLoadedCallback(Stream stream, bool succeeded)
	{
		enableButtons();
		MessageBoxManager.Show("Image Status", "Image Loaded: " + succeeded);
		if (!succeeded)
		{
			if (stream != null) stream.Dispose();
			return;
		}
		
		try
		{
			var data = new byte[stream.Length];
			stream.Read(data, 0, data.Length);
			var newImage = new Texture2D(4, 4);
			newImage.LoadImage(data);
			newImage.Apply();
			ReignImage.sprite = Sprite.Create(newImage, new Rect(0, 0, newImage.width, newImage.height), new Vector2(.5f, .5f));
		}
		catch (Exception e)
		{
			MessageBoxManager.Show("Error", e.Message);
		}
		finally
		{
			// NOTE: Make sure you dispose of this stream !!!
			if (stream != null) stream.Dispose();
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
