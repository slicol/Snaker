using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Reign.Tools
{
	public class InputMapper : MonoBehaviour
	{
		public Text InputText;

		void Update()
		{
			// map axises
			for (int i = 0; i != 20; ++i)
			{
				float value = Input.GetAxis("Axis" + (i+1));
				if (Mathf.Abs(value) >= .5f)
				{
					InputText.text = string.Format("Axis {0} of value {1}", i+1, value);
				}
			}

			// map keys and buttons
			for (int i = 0; i != 430; ++i)
			{
				if (Input.GetKeyDown((KeyCode)i))
				{
					InputText.text = string.Format("Key/Button pressed {0}", (KeyCode)i);
				}
			}
		}
	}
}