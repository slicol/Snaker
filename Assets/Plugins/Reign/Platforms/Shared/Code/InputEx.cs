using UnityEngine;
using System.Collections.Generic;

namespace Reign
{
	/// <summary>
	/// InputEx Button types.
	/// </summary>
	public enum ButtonTypes
	{
		/// <summary>
		/// 'X' on Xbox360, Button 'Square' on PS3, ect
		/// </summary>
		CrossButtonLeft,

		/// <summary>
		/// 'B' on Xbox360, Button 'O' on PS3, ect
		/// </summary>
		CrossButtonRight,

		/// <summary>
		/// 'A' on Xbox360, Button 'X' on PS3, ect
		/// </summary>
		CrossButtonBottom,

		/// <summary>
		/// 'Y' on Xbox360, Button 'Triangle' on PS3, ect
		/// </summary>
		CrossButtonTop,

		/// <summary>
		/// DPad left
		/// </summary>
		DPadLeft,

		/// <summary>
		/// DPad right
		/// </summary>
		DPadRight,

		/// <summary>
		/// DPad down
		/// </summary>
		DPadDown,

		/// <summary>
		/// DPad up
		/// </summary>
		DPadUp,

		/// <summary>
		/// Bumper left
		/// </summary>
		BumperLeft,

		/// <summary>
		/// Bumper right
		/// </summary>
		BumperRight,

		/// <summary>
		/// Analog Stick button left
		/// </summary>
		AnalogLeft,

		/// <summary>
		/// Analog Stick button right
		/// </summary>
		AnalogRight,

		/// <summary>
		/// Trigger button left
		/// </summary>
		TriggerLeft,

		/// <summary>
		/// Trigger button right
		/// </summary>
		TriggerRight,

		/// <summary>
		/// Start
		/// </summary>
		Start,

		/// <summary>
		/// 'Back' on Xbox360, 'Select' on PS3, ect
		/// </summary>
		Back
	}

	/// <summary>
	/// InputEx Analog types
	/// </summary>
	public enum AnalogTypes
	{
		/// <summary>
		/// Left Joystick of Axis X
		/// </summary>
		AxisLeftX,

		/// <summary>
		/// Left Joystick of Axis Y
		/// </summary>
		AxisLeftY,

		/// <summary>
		/// Right Joystick of Axis X
		/// </summary>
		AxisRightX,

		/// <summary>
		/// Right Joystick of Axis Y
		/// </summary>
		AxisRightY,

		/// <summary>
		/// Trigger left
		/// </summary>
		TriggerLeft,

		/// <summary>
		/// Trigger right
		/// </summary>
		TriggerRight
	}

	/// <summary>
	/// InputEx Controller players.
	/// </summary>
	public enum ControllerPlayers
	{
		/// <summary>
		/// Any Player input
		/// </summary>
		Any,

		/// <summary>
		/// Player 1 input
		/// </summary>
		Player1,

		/// <summary>
		/// Player 2 input
		/// </summary>
		Player2,

		/// <summary>
		/// Player 3 input
		/// </summary>
		Player3,

		/// <summary>
		/// Player 4 input
		/// </summary>
		Player4,

		/// <summary>
		/// Player 5 input
		/// </summary>
		Player5,

		/// <summary>
		/// Player 6 input
		/// </summary>
		Player6,

		/// <summary>
		/// Player 7 input
		/// </summary>
		Player7,

		/// <summary>
		/// Player 8 input
		/// </summary>
		Player8
	}

	/// <summary>
	/// InputEx Buttom Mapping
	/// </summary>
	public class InputExButtonMap
	{
		/// <summary>
		/// Type of button
		/// </summary>
		public ButtonTypes Type;

		/// <summary>
		/// Player input accepted
		/// </summary>
		public ControllerPlayers Player;

		/// <summary>
		/// Button name defined in Unitys settings
		/// </summary>
		public string Name;

		/// <summary>
		/// Analog Button name defined in Unitys settings
		/// </summary>
		public string AnalogName;

		/// <summary>
		/// Is analog button valid when value is positive
		/// </summary>
		public bool AnalogPositive;

		/// <summary>
		/// Analog button information (internal use only)
		/// </summary>
		internal bool analogOn, analogDown, analogUp;

		/// <summary>
		/// Initializes a new instance of the <see cref="Reign.InputExButtonMap"/> class.
		/// </summary>
		/// <param name="type">Button Type.</param>
		/// <param name="player">Player input accepted.</param>
		/// <param name="name">Name from Unitys settings.</param>
		public InputExButtonMap(ButtonTypes type, ControllerPlayers player, string name)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Reign.InputExButtonMap"/> class.
		/// </summary>
		/// <param name="type">Button Type.</param>
		/// <param name="player">Player input accepted.</param>
		/// <param name="name">Name from Unitys settings.</param>
		/// <param name="analogName">Analog name from Unitys settings.</param>
		/// <param name="analogPositive">Is analog button valid when value is positive.</param>
		public InputExButtonMap(ButtonTypes type, ControllerPlayers player, string name, string analogName, bool analogPositive)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
			this.AnalogName = analogName;
			this.AnalogPositive = analogPositive;
		}

		/// <summary>
		/// internal use only
		/// </summary>
		internal void update()
		{
			if (AnalogName == null) return;

			bool lastOn = analogOn;
			if (AnalogPositive) analogOn = Input.GetAxisRaw(AnalogName) >= .5;
			else analogOn = Input.GetAxisRaw(AnalogName) <= -.5;

			if (analogOn && !lastOn) analogDown = true;
			else analogDown = false;

			if (!analogOn && lastOn) analogUp = true;
			else analogUp = false;
		}
	}

	/// <summary>
	/// InputExanalog mapping.
	/// </summary>
	public class InputExAnalogMap
	{
		/// <summary>
		/// Analog type
		/// </summary>
		public AnalogTypes Type;

		/// <summary>
		/// Player input accepted.
		/// </summary>
		public ControllerPlayers Player;

		/// <summary>
		/// Button name defined in Unitys settings
		/// </summary>
		public string Name;

		/// <summary>
		/// Initializes a new instance of the <see cref="Reign.InputExAnalogMap"/> class.
		/// </summary>
		/// <param name="type">Analog Type.</param>
		/// <param name="player">Player input accepted.</param>
		/// <param name="name">Button name defined in Unitys settings.</param>
		public InputExAnalogMap(AnalogTypes type, ControllerPlayers player, string name)
		{
			this.Type = type;
			this.Player = player;
			this.Name = name;
		}
	}

	/// <summary>
	/// InputEx extending Unitys Input API
	/// </summary>
	public static class InputEx
	{
		/// <summary>
		/// Analog tolerance value (Default .25, OSX Default .35)
		/// </summary>
		#if UNITY_STANDALONE_OSX
		public static float AnalogTolerance = .35f;
		#else
		public static float AnalogTolerance = .25f;
		#endif

		/// <summary>
		/// Analog Triggle tolerance value (Default .1)
		/// </summary>
		public static float AnalogTriggerTolerance = .1f;

		/// <summary>
		/// Button mappings
		/// </summary>
		public static InputExButtonMap[] ButtonMappings;

		/// <summary>
		/// Analog mappings
		/// </summary>
		public static InputExAnalogMap[] AnalogMappings;

		static InputEx()
		{
			ButtonMappings = new InputExButtonMap[]
			{
				// Cross buttons
				new InputExButtonMap(ButtonTypes.CrossButtonLeft, ControllerPlayers.Any, "Left CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonRight, ControllerPlayers.Any, "Right CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonBottom, ControllerPlayers.Any, "Bottom CrossButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.CrossButtonTop, ControllerPlayers.Any, "Top CrossButton - PlayerAny"),

				// DPad buttons
				new InputExButtonMap(ButtonTypes.DPadLeft, ControllerPlayers.Any, "Left DPadButton - PlayerAny", "Horizontal DPadAnalog - PlayerAny", false),
				new InputExButtonMap(ButtonTypes.DPadRight, ControllerPlayers.Any, "Right DPadButton - PlayerAny", "Horizontal DPadAnalog - PlayerAny", true),
				new InputExButtonMap(ButtonTypes.DPadDown, ControllerPlayers.Any, "Down DPadButton - PlayerAny", "Vertical DPadAnalog - PlayerAny", false),
				new InputExButtonMap(ButtonTypes.DPadUp, ControllerPlayers.Any, "Up DPadButton - PlayerAny", "Vertical DPadAnalog - PlayerAny", true),

				// Bumper buttons
				new InputExButtonMap(ButtonTypes.BumperLeft, ControllerPlayers.Any, "Left Bumper - PlayerAny"),
				new InputExButtonMap(ButtonTypes.BumperRight, ControllerPlayers.Any, "Right Bumper - PlayerAny"),

				// Analog buttons
				new InputExButtonMap(ButtonTypes.AnalogLeft, ControllerPlayers.Any, "Left AnalogButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.AnalogRight, ControllerPlayers.Any, "Right AnalogButton - PlayerAny"),

				// Trigger buttons
				new InputExButtonMap(ButtonTypes.TriggerLeft, ControllerPlayers.Any, "Left TriggerButton - PlayerAny"),
				new InputExButtonMap(ButtonTypes.TriggerRight, ControllerPlayers.Any, "Right TriggerButton - PlayerAny"),

				// System buttons
				new InputExButtonMap(ButtonTypes.Start, ControllerPlayers.Any, "Start Button - PlayerAny"),
				new InputExButtonMap(ButtonTypes.Back, ControllerPlayers.Any, "Back Button - PlayerAny")
			};

			AnalogMappings = new InputExAnalogMap[]
			{
				// Sticks
				new InputExAnalogMap(AnalogTypes.AxisLeftX, ControllerPlayers.Any, "Left AnalogX - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisLeftY, ControllerPlayers.Any, "Left AnalogY - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisRightX, ControllerPlayers.Any, "Right AnalogX - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.AxisRightY, ControllerPlayers.Any, "Right AnalogY - PlayerAny"),

				// Triggers
				new InputExAnalogMap(AnalogTypes.TriggerLeft, ControllerPlayers.Any, "Left Trigger - PlayerAny"),
				new InputExAnalogMap(AnalogTypes.TriggerRight, ControllerPlayers.Any, "Right Trigger - PlayerAny")
			};
		}

		/// <summary>
		/// Logs all Keys pressed
		/// </summary>
		/// <returns>The last key pressed.</returns>
		public static string LogKeys()
		{
			string label = null;
			for (int i = 0; i != 430; ++i)
			{
				var key = (KeyCode)i;
				if (Input.GetKeyDown(key))
				{
					label = "KeyPressed: " + key;
					Debug.Log(label);
				}
			}

			return label;
		}

		/// <summary>
		/// Logs all Buttons pressed
		/// </summary>
		/// <returns>The last button pressed.</returns>
		public static string LogButtons()
		{
			string label = null;
			for (int i = 0; i != 16; ++i)
			{
				var button = (ButtonTypes)i;
				if (InputEx.GetButtonDown(button, ControllerPlayers.Any))
				{
					label = "ButtonPressed: " + button;
					Debug.Log(label);
				}
			}

			return label;
		}

		/// <summary>
		/// Logs all Analogs moved
		/// </summary>
		/// <returns>The last analog moved.</returns>
		public static string LogAnalogs()
		{
			string label = null;
			for (int i = 0; i != 6; ++i)
			{
				var analog = (AnalogTypes)i;
				float value = GetAxis(analog, ControllerPlayers.Any);
				if (value != 0)
				{
					label = string.Format("AnalogType {0} value: {1}", analog, value);
					Debug.Log(label);
				}
			}

			return label;
		}

		private static string findButtonName(ButtonTypes type, ControllerPlayers player, out InputExButtonMap mapping)
		{
			foreach (var map in ButtonMappings)
			{
				if (map.Type == type && map.Player == player)
				{
					mapping = map;
					return map.Name;
				}
			}

			Debug.LogError(string.Format("Failed to find Button {0} for Player {1}", type, player));
			mapping = null;
			return "Unknown";
		}

		/// <summary>
		/// Gets if the button is being held down
		/// </summary>
		/// <returns>Returns if valid or not</returns>
		/// <param name="type">Button Type.</param>
		/// <param name="player">Player input accepted.</param>
		public static bool GetButton(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.analogOn) return true;

			return Input.GetButton(name);
		}

		/// <summary>
		/// Gets if the button was pressed down
		/// </summary>
		/// <returns>Returns if valid or not</returns>
		/// <param name="type">Button Type.</param>
		/// <param name="player">Player input accepted.</param>
		public static bool GetButtonDown(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.analogDown) return true;

			return Input.GetButtonDown(name);
		}

		/// <summary>
		/// Gets if the button was let up
		/// </summary>
		/// <returns>Returns if valid or not</returns>
		/// <param name="type">Button Type.</param>
		/// <param name="player">Player input accepted.</param>
		public static bool GetButtonUp(ButtonTypes type, ControllerPlayers player)
		{
			InputExButtonMap mapping;
			string name = findButtonName(type, player, out mapping);
			if (mapping != null && mapping.analogUp) return true;

			return Input.GetButtonUp(name);
		}

		private static string findAnalogName(AnalogTypes type, ControllerPlayers player)
		{
			foreach (var map in AnalogMappings)
			{
				if (map.Type == type && map.Player == player)
				{
					return map.Name;
				}
			}

			Debug.LogError(string.Format("Failed to find Analog {0} for Player {1}", type, player));
			return "Unknown";
		}

		private static float processAnalogValue(AnalogTypes type, float value)
		{
			float tolerance = AnalogTolerance;
			switch (type)
			{
				case AnalogTypes.TriggerLeft:
				case AnalogTypes.TriggerRight:
					tolerance = AnalogTriggerTolerance;
					break;
			}

			return (Mathf.Abs(value) <= tolerance) ? 0 : value;
		}

		/// <summary>
		/// Gets the Analog Axis value
		/// </summary>
		/// <returns>The axis.</returns>
		/// <param name="type">Analog Type.</param>
		/// <param name="player">Player input accepted.</param>
		public static float GetAxis(AnalogTypes type, ControllerPlayers player)
		{
			return processAnalogValue(type, Input.GetAxisRaw(findAnalogName(type, player)));
		}

		/// <summary>
		/// Gets the Analog Axis value with no tolerance
		/// </summary>
		/// <returns>The axis.</returns>
		/// <param name="type">Analog Type.</param>
		/// <param name="player">Player input accepted.</param>
		public static float GetAxisRaw(AnalogTypes type, ControllerPlayers player)
		{
			return Input.GetAxisRaw(findAnalogName(type, player));
		}
	}
}