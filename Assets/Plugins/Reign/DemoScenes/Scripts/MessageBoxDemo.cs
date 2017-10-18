// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class MessageBoxDemo : MonoBehaviour
{
	public Button ShowOkCancelButton, ShowOkButton, BackButton;

	void Start ()
	{
		// bind button events
		ShowOkCancelButton.Select();
		ShowOkCancelButton.onClick.AddListener(showOkCancelClicked);
		ShowOkButton.onClick.AddListener(showOkClicked);
		BackButton.onClick.AddListener(backClicked);
	}

	private void showOkCancelClicked()
	{
		// NOTE: You can pass in options and override the default button names
		/*var options = new MessageBoxOptions()
		{
			OkButtonName = "Ok",
			CancelButtonText = "Cancel"
		};*/
		MessageBoxManager.Show("Yahoo", "Are you Awesome!?", MessageBoxTypes.OkCancel, callback);
	}

	private void showOkClicked()
	{
		MessageBoxManager.Show("Yahoo", "Hello World!");
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}

	private void callback(MessageBoxResult result)
	{
		Debug.Log(result);
		if (result == MessageBoxResult.Ok) Debug.Log("+1 for you!");
		else if (result == MessageBoxResult.Cancel) Debug.Log("How sad...");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
