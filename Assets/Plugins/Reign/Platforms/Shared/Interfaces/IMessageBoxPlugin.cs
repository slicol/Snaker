using System;

namespace Reign
{
	/// <summary>
	/// MessageBox result
	/// </summary>
	public enum MessageBoxResult
	{
		/// <summary>
		/// OK
		/// </summary>
		Ok,

		/// <summary>
		/// Cancel
		/// </summary>
		Cancel
	}

	/// <summary>
	/// MessageBox types
	/// </summary>
	public enum MessageBoxTypes
	{
		/// <summary>
		/// Ok button
		/// </summary>
		Ok,

		/// <summary>
		/// OK/Cancel button
		/// </summary>
		OkCancel
	}

	public class MessageBoxOptions
	{
		public string OkButtonName = "OK", CancelButtonText = "Cancel";
	}

	/// <summary>
	/// Used to fire back message box results
	/// </summary>
	/// <param name="result">Result type</param>
	public delegate void MessageBoxCallback(MessageBoxResult result);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base MessageBox interface object
	/// </summary>
	public interface IMessageBoxPlugin
	{
		/// <summary>
		/// Use to show message box
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="message">Message</param>
		/// <param name="type">MessageBox type</param>
		/// <param name="callback">Callback fired when done.</param>
		void Show(string title, string message, MessageBoxTypes type, MessageBoxOptions options, MessageBoxCallback callback);

		/// <summary>
		/// Used to handle events
		/// </summary>
		void Update();
	}
}
