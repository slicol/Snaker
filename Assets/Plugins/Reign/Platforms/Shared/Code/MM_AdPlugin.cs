using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

#if UNITY_EDITOR || UNITY_STANDALONE
using System.Security.Cryptography;
#endif

namespace Reign.MM_AdXML
{
	public class TextContent
	{
		[XmlText] public string Content;
	}

	public class Image
	{
		[XmlElement("url")] public TextContent url;
		[XmlElement("mime_type")] public TextContent mime_type;
		[XmlElement("height")] public TextContent height;
		[XmlElement("width")] public TextContent width;
		[XmlElement("altText")] public TextContent altText;
	}

	[XmlRoot("ad")]
	public class Ad
	{
		[XmlElement("bodyType")] public TextContent bodyType;
		[XmlElement("clickUrl")] public TextContent clickUrl;
		[XmlElement("image")] public Image image;
		[XmlElement("text")] public TextContent text;
	}
}

namespace Reign.Plugin
{
    public class MM_AdPlugin : IAdPlugin
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
		private Texture2D guiTexture;
		private Rect guiRect;
		private float uiScale;
		private bool testing;
		private AdEventCallbackMethod adEvent;
		private int refreshRate;
		private float refreshRateTic;
		private string deviceID, externalIP, apid, userAgent;
		private TextureGIF gifImage;
		private MonoBehaviour service;
		private Reign.MM_AdXML.Ad adMeta;
		private AdGravity gravity;
		private GUIStyle guiSytle;

		public MM_AdPlugin(AdDesc desc, AdCreatedCallbackMethod createdCallback, MonoBehaviour service)
		{
			this.service = service;

			try
			{
				this.desc = desc;
				testing = desc.Testing;
				adEvent = desc.EventCallback;

				gravity = AdGravity.CenterScreen;
				#if !DISABLE_REIGN
				#if UNITY_EDITOR
				refreshRate = desc.Editor_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Editor_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.Editor_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.Editor_AdScale;
				#elif UNITY_BLACKBERRY
				refreshRate = desc.BB10_MillennialMediaAdvertising_RefreshRate;
				apid = desc.BB10_MillennialMediaAdvertising_APID;

				IntPtr handle = IntPtr.Zero;
				if (Common.deviceinfo_get_details(ref handle) != 0) throw new Exception("Failed: deviceinfo_get_details");
				string deviceType = Common.deviceinfo_details_get_keyboard(handle) == 0 ? "Touch" : "Kbd";
				Common.deviceinfo_free_details(ref handle);
				string osVersion = System.Text.RegularExpressions.Regex.Match(SystemInfo.operatingSystem, @"\d*\.\d*\.\d*\.\d*").Groups[0].Value;
				userAgent = WWW.EscapeURL("Mozilla/5.0 (BB10; " + deviceType + ") AppleWebKit/537.10+ (KHTML, like Gecko) Version/" + osVersion + " Mobile Safari/537.10+");

				gravity = desc.BB10_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.BB10_AdScale;
				#elif UNITY_WP8
				refreshRate = desc.WP8_MillennialMediaAdvertising_RefreshRate;
				apid = desc.WP8_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.WP8_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.WP8_AdScale;
				#elif UNITY_METRO
				refreshRate = desc.WinRT_MillennialMediaAdvertising_RefreshRate;
				apid = desc.WinRT_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.WinRT_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.WinRT_AdScale;
				#elif UNITY_IOS
				refreshRate = desc.iOS_MillennialMediaAdvertising_RefreshRate;
				apid = desc.iOS_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.iOS_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.iOS_AdScale;
				#elif UNITY_ANDROID
				refreshRate = desc.Android_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Android_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.Android_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.Android_AdScale;
				#elif UNITY_STANDALONE_WIN
				refreshRate = desc.Win32_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Win32_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.Win32_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.Win32_AdScale;
				#elif UNITY_STANDALONE_OSX
				refreshRate = desc.OSX_MillennialMediaAdvertising_RefreshRate;
				apid = desc.OSX_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.OSX_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.OSX_AdScale;
				#elif UNITY_STANDALONE_LINUX
				refreshRate = desc.Linux_MillennialMediaAdvertising_RefreshRate;
				apid = desc.Linux_MillennialMediaAdvertising_APID;
				userAgent = "";
				gravity = desc.Linux_MillennialMediaAdvertising_AdGravity;
				uiScale = desc.Linux_AdScale;
				#endif
				#endif

				// make sure ad refresh rate doesn't go under 1 min
				if (refreshRate < 60) refreshRate = 60;

				// create or get device ID
				if (PlayerPrefs.HasKey("Reign_MMWebAds_DeviceID"))
				{
					deviceID = PlayerPrefs.GetString("Reign_MMWebAds_DeviceID");
				}
				else
				{
					#if UNITY_EDITOR || UNITY_STANDALONE
					var hash = new SHA1CryptoServiceProvider().ComputeHash(Guid.NewGuid().ToByteArray());
					deviceID = BitConverter.ToString(hash).ToLower();
					#else
					deviceID = Guid.NewGuid().ToString().Replace("-", "0").ToLower() + "0000";
					#endif

					PlayerPrefs.SetString("Reign_MMWebAds_DeviceID", deviceID);
				}

				if (desc.UseClassicGUI)
				{
					guiSytle = new GUIStyle()
					{
						stretchWidth = true,
						stretchHeight = true
					};
				}
				else
				{
					// Create Ad Canvas
					adCanvas = new GameObject("MM Ad");
					GameObject.DontDestroyOnLoad(adCanvas);
					adCanvas.AddComponent<RectTransform>();
					var canvas = adCanvas.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.sortingOrder = 1000;
					adCanvas.AddComponent<CanvasScaler>();
					adCanvas.AddComponent<GraphicRaycaster>();

					// Create ad
					var ad = new GameObject("AdButtonImage");
					ad.transform.parent = adCanvas.transform;
					adRect = ad.AddComponent<RectTransform>();
					adImage = ad.AddComponent<Image>();
					adImage.sprite = Resources.Load<Sprite>("Reign/Ads/AdLoading");
					adImage.preserveAspect = true;
					var button = ad.AddComponent<Button>();
					button.onClick.AddListener(adClicked);
				}

				// set default visible state
				Visible = desc.Visible;
				SetGravity(gravity);

				// load ad
				service.StartCoroutine(init(createdCallback));

				// set screen size changed callback
				ReignServices.ScreenSizeChangedCallback += ReignServices_ScreenSizeChangedCallback;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false);
			}
		}

		private void ReignServices_ScreenSizeChangedCallback(int oldWidth, int oldHeight, int newWidth, int newHeight)
		{
			SetGravity(gravity);
		}

		private void adClicked()
		{
			if (testing)
			{
				Debug.Log("Ad Clicked!");
				if (gifImage != null) Application.OpenURL("http://www.millennialmedia.com/");
			}
			else
			{
				Debug.Log("Opening Ad at URL: " + adMeta.clickUrl.Content);
				if (adMeta != null && adMeta.clickUrl != null && !string.IsNullOrEmpty(adMeta.clickUrl.Content)) Application.OpenURL(adMeta.clickUrl.Content);
				Debug.Log("Ad Clicked!");
			}

			if (adEvent != null) adEvent(AdEvents.Clicked, null);
		}

		private IEnumerator init(AdCreatedCallbackMethod createdCallback)
		{
			// request Ad
			if (testing)
			{
				var www = new WWW("http://media.mydas.mobi/images/rich/T/test_mm/collapsed.gif");
				yield return www;

				var data = www.bytes;
				if (data == null || data.Length == 0)
				{
					Debug.LogError("Test Ad failed to loadb");
					if (createdCallback != null) createdCallback(false);
					yield break;
				}

				gifImage = new TextureGIF(data, frameUpdatedCallback);
				if (desc.UseClassicGUI) guiTexture = gifImage.CurrentFrame.Texture;
				else adImage.sprite = gifImage.CurrentFrame.Sprite;
				SetGravity(gravity);
				var texture = gifImage.CurrentFrame.Texture;
				Debug.Log(string.Format("Ad Image Size: {0}x{1}", texture.width, texture.height));
				if (createdCallback != null) createdCallback(true);
				if (adEvent != null) adEvent(AdEvents.Refreshed, null);
			}
			else
			{
				// get external IP address
				var ipWWW = new WWW("http://checkip.dyndns.org/");
				yield return ipWWW;
				var match = Regex.Match(ipWWW.text, @"Current IP Address\: (\d*\.\d*\.\d*\.\d*)");
				if (!match.Success)
				{
					if (createdCallback != null) createdCallback(false);
					yield break;
				}
				externalIP = match.Groups[1].Value;
				Debug.Log("External IP: " + externalIP);

				// load ad
				service.StartCoroutine(asyncRefresh(createdCallback));
			}
		}

		private void frameUpdatedCallback(TextureGIFFrame frame)
		{
			if (desc.UseClassicGUI) guiTexture = frame.Texture;
			else adImage.sprite = frame.Sprite;
		}

		public void Dispose()
		{
			ReignServices.ScreenSizeChangedCallback -= ReignServices_ScreenSizeChangedCallback;
			if (gifImage != null)
			{
				gifImage.Dispose();
				gifImage = null;
			}
		}

		public void SetGravity(AdGravity gravity)
		{
			if (desc.UseClassicGUI)
			{
				if (guiTexture == null) return;
				
				float screenWidth = Screen.width, screenHeight = Screen.height;
				float scale = (new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude) * uiScale;
				float adWidth = guiTexture.width * scale, adHeight = guiTexture.height * scale;
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
				float scale = (new Vector2(screenWidth, screenHeight).magnitude / new Vector2(1280, 720).magnitude) * uiScale;
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
			Debug.Log("Refreshing Ad");
			if (!testing) service.StartCoroutine(asyncRefresh(null));
			else if (adEvent != null) adEvent(AdEvents.Refreshed, null);
		}

		private IEnumerator asyncRefresh(AdCreatedCallbackMethod createdCallback)
		{
			if (!Visible) yield break;

			string url = "http://ads.mp.mydas.mobi/getAd?";
			url += "&apid=" + apid;// ID
			url += "&auid=" + deviceID;// Device UID Hash HEX value
			url += "&ua=" + userAgent;
			url += "&uip=" + externalIP;
			Debug.Log("Ad Request URL: " + url);
			var www = new WWW(url);
			yield return www;

			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, www.error);
				yield break;
			}

			if (www.text.Contains(@"""error"""))
			{
				Debug.LogError(www.text);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, www.text);
				yield break;
			}

			//Debug.Log("Request Text: " + www.text);
			if (string.IsNullOrEmpty(www.text))
			{
				string error = "Invalid server responce! No data!";
				Debug.LogError(error);
				if (createdCallback != null) createdCallback(false);
				else if (adEvent != null) adEvent(AdEvents.Error, error);
				yield break;
			}

			var xml = new XmlSerializer(typeof(Reign.MM_AdXML.Ad));
			using (var data = new MemoryStream(www.bytes))
			{
				adMeta = (Reign.MM_AdXML.Ad)xml.Deserialize(data);
			}

			string imageURL = adMeta.image.url.Content;
			Debug.Log("MMWeb Ad ImageURL: " + imageURL);
			www = new WWW(imageURL);
			yield return www;

			if (gifImage != null)
			{
				gifImage.Dispose();
				gifImage = null;
			}
			gifImage = new TextureGIF(www.bytes, frameUpdatedCallback);
			if (desc.UseClassicGUI) guiTexture = gifImage.CurrentFrame.Texture;
			else adImage.sprite = gifImage.CurrentFrame.Sprite;
			var texture = gifImage.CurrentFrame.Texture;
			Debug.Log(string.Format("Ad Image Size: {0}x{1}", texture.width, texture.height));

			SetGravity(gravity);
			if (adEvent != null) adEvent(AdEvents.Refreshed, null);
		}
		
		public void Update()
		{
			if (gifImage != null) gifImage.Update();

			refreshRateTic += Time.deltaTime;
			if (refreshRateTic >= refreshRate)
			{
				refreshRateTic = 0;
				Refresh();
			}
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
			if (desc.UseClassicGUI && visible && guiTexture != null)
			{
				GUI.DrawTexture(guiRect, guiTexture, ScaleMode.StretchToFill);
				if (GUI.Button(guiRect, "", guiSytle)) adClicked();
			}
		}

    }
}