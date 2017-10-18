using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class SocialDemo : MonoBehaviour
{
	public Button ShareButton, BackButton;
	public Sprite ReignLogo;

	public GameObject BB10_ShareSelectorUI;
	public Text BB10_ShareSelectorTitle;
	public Button BB10_CloseButton, BB10_ShareSelectorBBM, BB10_ShareSelectorFacebook, BB10_ShareSelectorTwitter;

	void Start ()
	{
		// bind button events
		ShareButton.Select();
		ShareButton.onClick.AddListener(shareClicked);
		BackButton.onClick.AddListener(backClicked);

		// Init the share plugin
		var desc = new SocialDesc()
		{
			BB10_ShareSelectorUI = BB10_ShareSelectorUI,
			BB10_ShareSelectorTitle = BB10_ShareSelectorTitle,
			BB10_CloseButton = BB10_CloseButton,
			BB10_ShareSelectorBBM = BB10_ShareSelectorBBM,
			BB10_ShareSelectorFacebook = BB10_ShareSelectorFacebook,
			BB10_ShareSelectorTwitter = BB10_ShareSelectorTwitter
		};
		SocialManager.Init(desc);
	}

	private void shareClicked()
	{
		// NOTE: If the platform doesn't support multiple share types at once, then data will take priority over text.
		var data = ReignLogo.texture.EncodeToPNG();
		SocialManager.Share(data, "ReignSocialImage", "Demo Text", "Reign Demo", "Reign Demo Desc", SocialShareDataTypes.Image_PNG);

		// NOTE: If you want to share a screen shot you can use the helper method below
		//ReignServices.CaptureScreenShot(captureScreenShotCallback);
	}

	/*private void captureScreenShotCallback(byte[] data)
	{
		SocialManager.Share(data, "ReignSocialImage", "Demo Text", "Reign Demo", "Reign Demo Desc", SocialShareDataTypes.Image_PNG);
	}*/

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
