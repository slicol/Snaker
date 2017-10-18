// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Helper to force Per-Pixel Ortho camera settings.
/// </summary>
public class PerPixelOrthoCamera : MonoBehaviour
{
	/// <summary>
	/// Use to force the camera to use Screen size values instead of the cameras viewport sizes.
	/// </summary>
	public bool ScreenScaleValues;

	/// <summary>
	/// Use to offset the camera XY to the bottom left.
	/// </summary>
	public bool BottomLeftMode;

	private new Camera camera;

	void Start()
	{
		this.camera = GetComponent<Camera>();
		LateUpdate();
	}

	void LateUpdate()
	{
		if (BottomLeftMode)
		{
			var pos = camera.transform.position;
			if (ScreenScaleValues)
			{
				pos.x = Screen.width / 2f;
				pos.y = Screen.height / 2f;
			}
			else
			{
				pos.x = camera.pixelWidth / 2f;
				pos.y = camera.pixelHeight / 2f;
			}
			camera.transform.position = pos;
		}

		if (ScreenScaleValues)
		{
			camera.orthographicSize = Screen.height / 2f;
		}
		else
		{
			camera.orthographicSize = camera.pixelHeight / 2f;
			camera.aspect = camera.pixelWidth / camera.pixelHeight;
		}
	}
}
