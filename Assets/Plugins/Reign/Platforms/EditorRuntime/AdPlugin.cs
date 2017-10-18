#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Reign.Plugin
{
    public class AdPlugin : IAdPlugin
    {
		private bool visible;
		public bool Visible
		{
			get {return visible;}
			set
			{
				visible = value;
				if (!desc.UseClassicGUI) adCanvas.SetActive(value);
			}
		}

		private AdDesc desc;
		private GameObject adCanvas;
		private RectTransform adRect;
		private Image adImage;
		private Rect guiRect;
		
		public AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			bool pass = true;
			try
			{
				this.desc = desc;

				if (!desc.UseClassicGUI)
				{
					// Create Ad Canvas
					adCanvas = new GameObject("Editor Ad");
					GameObject.DontDestroyOnLoad(adCanvas);
					adCanvas.AddComponent<RectTransform>();
					var canvas = adCanvas.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.sortingOrder = desc.UnityUI_SortIndex;
					adCanvas.AddComponent<CanvasScaler>();
					adCanvas.AddComponent<GraphicRaycaster>();

					// Create ad
					var ad = new GameObject("AdButtonImage");
					ad.transform.parent = adCanvas.transform;
					adRect = ad.AddComponent<RectTransform>();
					adImage = ad.AddComponent<Image>();
					adImage.sprite = Resources.Load<Sprite>("Reign/Ads/DemoAd");
					adImage.preserveAspect = true;
					var button = ad.AddComponent<Button>();
					button.onClick.AddListener(adClicked);
				}

				// set default visible state and gravity
				Visible = desc.Visible;
				SetGravity(desc.Editor_AdGravity);

				// set screen size changed callback
				ReignServices.ScreenSizeChangedCallback += ReignServices_ScreenSizeChangedCallback;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
				
			if (createdCallback != null) createdCallback(pass);
		}

		private void ReignServices_ScreenSizeChangedCallback(int oldWidth, int oldHeight, int newWidth, int newHeight)
		{
			SetGravity(desc.Editor_AdGravity);
		}

		private void adClicked()
		{
			Debug.Log("Ad Clicked!");
			if (desc.EventCallback != null) desc.EventCallback(AdEvents.Clicked, null);
		}

		public void Dispose()
		{
			ReignServices.ScreenSizeChangedCallback -= ReignServices_ScreenSizeChangedCallback;
			if (!desc.UseClassicGUI && adCanvas != null)
			{
				GameObject.Destroy(adCanvas);
				adCanvas = null;
			}
		}

		public void SetGravity(AdGravity gravity)
		{
			if (desc.UseClassicGUI)
			{
				float screenWidth = Screen.width, screenHeight = Screen.height;
				float scale = new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude;
				float adWidth = 320 * desc.Editor_AdScale * scale, adHeight = 53 * desc.Editor_AdScale * scale;
				switch (gravity)
				{
					case AdGravity.CenterScreen:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), (screenHeight/2)-(adHeight/2), adWidth, adHeight);
						break;

					case AdGravity.BottomCenter:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.BottomLeft:
						guiRect = new Rect(0, screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.BottomRight:
						guiRect = new Rect(screenWidth-adWidth, screenHeight-adHeight, adWidth, adHeight);
						break;

					case AdGravity.TopCenter:
						guiRect = new Rect((screenWidth/2)-(adWidth/2), 0, adWidth, adHeight);
						break;

					case AdGravity.TopLeft:
						guiRect = new Rect(0, 0, adWidth, adHeight);
						break;

					case AdGravity.TopRight:
						guiRect = new Rect(screenWidth-adWidth, 0, adWidth, adHeight);
						break;

					default:
						Debug.LogError("Unsuported Gravity: " + gravity);
						break;
				}
			}
			else
			{
				if (adImage.sprite == null) return;

				float screenWidth = Screen.width, screenHeight = Screen.height;
				float scale = (new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude) * desc.Editor_AdScale;
				var texture = adImage.sprite.texture;
				float adWidth = (texture.width / screenWidth) * scale, adHeight = (texture.height / screenHeight) * scale;
				switch (gravity)
				{
					case AdGravity.CenterScreen:
						adRect.anchorMin = new Vector2(-(adWidth*.5f) + .5f, -(adHeight*.5f) + .5f);
						adRect.anchorMax = new Vector2(-(adWidth*.5f) + .5f + adWidth, -(adHeight*.5f) + .5f + adHeight);
						break;

					case AdGravity.BottomCenter:
						adRect.anchorMin = new Vector2(-(adWidth*.5f) + .5f, 0);
						adRect.anchorMax = new Vector2(-(adWidth*.5f) + .5f + adWidth, adHeight);
						break;

					case AdGravity.BottomLeft:
						adRect.anchorMin = new Vector2(0, 0);
						adRect.anchorMax = new Vector2(adWidth, adHeight);
						break;

					case AdGravity.BottomRight:
						adRect.anchorMin = new Vector2(1 - adWidth, 0);
						adRect.anchorMax = new Vector2(1, adHeight);
						break;

					case AdGravity.TopCenter:
						adRect.anchorMin = new Vector2(-(adWidth*.5f) + .5f, 1 - adHeight);
						adRect.anchorMax = new Vector2(-(adWidth*.5f) + .5f + adWidth, 1);
						break;

					case AdGravity.TopLeft:
						adRect.anchorMin = new Vector2(0, 1 - adHeight);
						adRect.anchorMax = new Vector2(adWidth, 1);
						break;

					case AdGravity.TopRight:
						adRect.anchorMin = new Vector2(1 - adWidth, 1 - adHeight);
						adRect.anchorMax = new Vector2(1, 1);
						break;

					default:
						Debug.LogError("Unsuported Gravity: " + gravity);
						break;
				}

				adRect.offsetMin = Vector2.zero;
				adRect.offsetMax = Vector2.zero;
			}
		}
		
		public void Refresh()
		{
			Debug.Log("Editor Ad Refreshed");
			if (desc.EventCallback != null) desc.EventCallback(AdEvents.Refreshed, null);
		}
		
		public void Update()
		{
			// do nothing...
		}

		public void OnGUI()
		{
			if (!desc.GUIOverrideEnabled) onGUI();
		}

		public void OverrideOnGUI()
		{
			if (desc.GUIOverrideEnabled) onGUI();
		}

		private void onGUI()
		{
			if (desc.UseClassicGUI && visible && GUI.Button(guiRect, "Reign Test Ad")) adClicked();
		}
    }
}
#endif