#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Reign.Plugin
{
	public class InterstitialAdPlugin : IInterstitialAdPlugin
	{
		private InterstitialAdEventCallbackMethod eventCallback;
		private GameObject adCanvas;
		private RectTransform adRect;
		private Rect guiRect;
		private bool isCached, isShowing;
		private InterstitialAdDesc desc;

		public InterstitialAdPlugin(InterstitialAdDesc desc, InterstitialAdCreatedCallbackMethod callback)
		{
			bool pass = true;
			try
			{
				eventCallback = desc.EventCallback;
				this.desc = desc;

				if (!desc.UseClassicGUI)
				{
					// Create Ad Canvas
					adCanvas = new GameObject("Editor Interstitial Ad");
					GameObject.DontDestroyOnLoad(adCanvas);
					adCanvas.AddComponent<RectTransform>();
					var canvas = adCanvas.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.sortingOrder = desc.UnityUI_SortIndex;
					adCanvas.AddComponent<CanvasScaler>();
					adCanvas.AddComponent<GraphicRaycaster>();
					adCanvas.SetActive(false);

					// Create ad
					var ad = new GameObject("AdButtonImage");
					ad.transform.parent = adCanvas.transform;
					adRect = ad.AddComponent<RectTransform>();
					var image = ad.AddComponent<Image>();
					image.sprite = Resources.Load<Sprite>("Reign/Ads/DemoInterstitialAd");
					image.preserveAspect = false;
					var button = ad.AddComponent<Button>();
					button.onClick.AddListener(adClicked);

					adRect.anchorMin = new Vector2(.1f, .1f);
					adRect.anchorMax = new Vector2(.9f, .9f);
					adRect.offsetMin = Vector2.zero;
					adRect.offsetMax = Vector2.zero;

					// close box
					var closeBox = new GameObject("AdButtonImage");
					closeBox.transform.parent = ad.transform;
					adRect = closeBox.AddComponent<RectTransform>();
					image = closeBox.AddComponent<Image>();
					image.sprite = Resources.Load<Sprite>("Reign/Ads/CloseBox");
					image.preserveAspect = false;
					button = closeBox.AddComponent<Button>();
					button.onClick.AddListener(adClosed);

					adRect.anchorMin = new Vector2(.9f, .9f);
					adRect.anchorMax = new Vector2(1, 1);
					adRect.offsetMin = Vector2.zero;
					adRect.offsetMax = Vector2.zero;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
			
			if (callback != null) callback(pass);
		}

		private void adClicked()
		{
			if (!desc.UseClassicGUI) adCanvas.SetActive(false);
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Clicked, null);
		}

		private void adClosed()
		{
			if (!desc.UseClassicGUI) adCanvas.SetActive(false);
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Canceled, null);
		}

		public void Dispose()
		{
			// do nothing...
		}
		
		public void Cache()
		{
			isCached = true;
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Cached, null);
		}

		public void Show()
		{
			if (!isCached)
			{
				if (eventCallback != null) eventCallback(InterstitialAdEvents.Error, "Ad must first be cached!");
				return;
			}

			isCached = false;
			if (!desc.UseClassicGUI) adCanvas.SetActive(true);
			else isShowing = true;
			if (eventCallback != null) eventCallback(InterstitialAdEvents.Shown, null);
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
			if (desc.UseClassicGUI && isShowing)
			{
				int screenWidth = Screen.width, screenHeight = Screen.height;
				float edgeX = screenWidth * .2f, edgeY = screenHeight * .2f;
				var rect = new Rect(edgeX*.5f, edgeY*.5f, screenWidth*.8f, screenHeight*.8f);
				if (GUI.Button(rect, "Reign Test Ad"))
				{
					isShowing = false;
					adClicked();
				}

				rect = new Rect(rect.x + rect.width, rect.y, rect.width * .1f, rect.height * .1f);
				if (GUI.Button(rect, "X"))
				{
					isShowing = false;
					adClosed();
					return;
				}
			}
		}
	}
}
#endif