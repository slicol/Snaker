using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Reign;

public class InputExDemo : MonoBehaviour
{
	public Button BackButton;
	public Text InputText;

	void Update()
	{
		// bind button events
		BackButton.onClick.AddListener(backClicked);

		// ====================================
		// You can log input here for debuging...
		// ====================================

		// All button and key input
		//string keyLabel = InputEx.LogKeys();
		//if (keyLabel != null)
		//{
		//	InputText.text = keyLabel;
		//	return;
		//}

		// All GamePad input
		string buttonLabel = InputEx.LogButtons();
		string analogLabel = InputEx.LogAnalogs();

		if (buttonLabel != null) InputText.text = buttonLabel;
		else if (analogLabel != null) InputText.text = analogLabel;

		// Example Input use case examples
		//if (InputEx.GetButton(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetButtonDown(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetButtonUp(ButtonTypes.Start, ControllerPlayers.Any));// do soething...
		//if (InputEx.GetAxis(AnalogTypes.AxisLeftX, ControllerPlayers.Any) >= .1f);// do soething...
	}

	private void backClicked()
	{
		Application.LoadLevel("MainDemo");
	}
}
