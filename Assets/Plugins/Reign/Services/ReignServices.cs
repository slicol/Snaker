// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Main Reign delegate class.
/// </summary>
public class ReignServices : MonoBehaviour
{
	/// <summary>
	/// Current instance of the ReignServices object.
	/// </summary>
	public static ReignServices Singleton {get; private set;}
	private bool canDestroy;

	public delegate void FrameDoneCallbackMethod();
	private static FrameDoneCallbackMethod frameDoneCallback;
	private static bool requestingFrame;

	public delegate void CaptureScreenShotCallbackMethod(byte[] data);
	private static CaptureScreenShotCallbackMethod captureScreenShotCallback;

	private static int lastScreenWidth, lastScreenHeight;
	public delegate void ScreenSizeChangedCallbackMethod(int oldWidth, int oldHeight, int newWidth, int newHeight);
	public static event ScreenSizeChangedCallbackMethod ScreenSizeChangedCallback;

	/// <summary>
	/// Sevice delegate method.
	/// </summary>
	public delegate void ServiceMethod();
	private static event ServiceMethod updateService, onguiService, destroyService;
	private static List<ServiceMethod>[] invokeOnUnityThreadCallbacks;
	private static int invokeSwap;

	/// <summary>
	/// Used to add service callbacks.
	/// </summary>
	/// <param name="update">Update callback.</param>
	/// <param name="onGUI">OnGUI callback.</param>
	/// <param name="destroy">OnDestroy callback.</param>
	public static void AddService(ServiceMethod update, ServiceMethod onGUI, ServiceMethod destroy)
	{
		if (update != null)
		{
			updateService -= update;
			updateService += update;
		}

		if (onGUI != null)
		{
			onguiService -= onGUI;
			onguiService += onGUI;
		}

		if (destroy != null)
		{
			destroyService -= destroy;
			destroyService += destroy;
		}
	}

	/// <summary>
	/// Used to remove service callbacks.
	/// </summary>
	/// <param name="update">Update callback.</param>
	/// <param name="onGUI">OnGUI callback.</param>
	/// <param name="destroy">OnDestroy callback.</param>
	public static void RemoveService(ServiceMethod update, ServiceMethod onGUI, ServiceMethod destroy)
	{
		if (update != null) updateService -= update;
		if (onGUI != null) onguiService -= onGUI;
		if (destroy != null) destroyService -= destroy;
	}

	public static void InvokeOnUnityThread(ServiceMethod callback)
	{
		lock (Singleton)
		{
			invokeOnUnityThreadCallbacks[invokeSwap].Add(callback);
		}
	}

	public static void RequestEndOfFrame(FrameDoneCallbackMethod frameDoneCallback)
	{
		if (ReignServices.frameDoneCallback != null)
		{
			Debug.LogError("You must wait until RequestEndOfFrame has finished!");
			return;
		}

		ReignServices.frameDoneCallback = frameDoneCallback;
		requestingFrame = true;
	}

	public static void CaptureScreenShot(CaptureScreenShotCallbackMethod captureScreenShotCallback)
	{
		if (ReignServices.captureScreenShotCallback != null)
		{
			Debug.LogError("You must wait until CaptureScreenShot has finished!");
			return;
		}

		ReignServices.captureScreenShotCallback = captureScreenShotCallback;
		RequestEndOfFrame(captureScreenShotEndOfFrame);
	}

	private static void captureScreenShotEndOfFrame()
	{
		var width = Screen.width;
		var height = Screen.height;
		var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
		texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		texture.Apply();
		var data = texture.EncodeToPNG();
		GameObject.Destroy(texture);

		if (captureScreenShotCallback != null) captureScreenShotCallback(data);
		captureScreenShotCallback = null;
	}

	static ReignServices()
	{
		#if DISABLE_REIGN
		Debug.LogWarning("NOTE: Reign is disabled for this platform. Check Player Settings for 'DISABLE_REIGN'");
		#endif

		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		invokeOnUnityThreadCallbacks = new List<ServiceMethod>[2];
		invokeOnUnityThreadCallbacks[0] = new List<ServiceMethod>();
		invokeOnUnityThreadCallbacks[1] = new List<ServiceMethod>();
	}
	
	/// <summary>
	/// Used to check if the ReignServices obj exists in the scene.
	/// </summary>
	public static void CheckStatus()
	{
		if (Singleton == null) Debug.LogError("ReignServices Prefab or Script does NOT exist in your scene!");
	}

	void Awake()
	{
		if (Singleton != null)
		{
			canDestroy = false;
			Destroy(gameObject);
			return;
		}

		canDestroy = true;
		Singleton = this;
		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		if (!canDestroy) return;

		PlayerPrefs.Save ();
		if (destroyService != null) destroyService();
		
		Singleton = null;
		updateService = null;
		onguiService = null;
		destroyService = null;
	}

	void Update()
	{
		// invoke update server delegates
		if (updateService != null) updateService();

		// if end of frame wait requested
		if (requestingFrame)
		{
			requestingFrame = false;
			StartCoroutine(frameSync());
		}

		// screen size changed callback helper
		int newScreenWidth = Screen.width;
		int newScreenHeight = Screen.height;
		if (newScreenWidth != lastScreenWidth || newScreenHeight != lastScreenHeight)
		{
			if (ScreenSizeChangedCallback != null) ScreenSizeChangedCallback(lastScreenWidth, lastScreenHeight, newScreenWidth, newScreenHeight);
		}
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		// unity thread delegates
		if (invokeOnUnityThreadCallbacks != null && invokeOnUnityThreadCallbacks[invokeSwap].Count != 0)
		{
			invokeSwap = 1 - invokeSwap;
			foreach (var callback in invokeOnUnityThreadCallbacks[1 - invokeSwap])
			{
				callback();
			}

			invokeOnUnityThreadCallbacks[1 - invokeSwap].Clear();
		}
	}

	private IEnumerator frameSync()
	{
		yield return new WaitForEndOfFrame();
		if (frameDoneCallback != null) frameDoneCallback();
		frameDoneCallback = null;
	}

	void OnGUI()
	{
		if (onguiService != null) onguiService(); 
	}
}
