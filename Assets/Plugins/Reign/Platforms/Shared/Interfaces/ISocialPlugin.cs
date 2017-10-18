using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Reign
{
	/// <summary>
	/// Social Share Types
	/// </summary>
	public enum SocialShareDataTypes
	{
		/// <summary>
		/// PNG data type
		/// </summary>
		Image_PNG,

		/// <summary>
		/// JPG data type
		/// </summary>
		Image_JPG
	}

	/// <summary>
	/// Object passed to Social init method
	/// </summary>
	public class SocialDesc
	{
		/// <summary>
		/// UnityUI share Canvas object for letting the user select between: (BBM, Facebook or Twitter)
		/// </summary>
		public GameObject BB10_ShareSelectorUI;

		/// <summary>
		/// UnityUI share title text
		/// </summary>
		public Text BB10_ShareSelectorTitle;

		/// <summary>
		/// UnityUI share selection button
		/// </summary>
		public Button BB10_CloseButton, BB10_ShareSelectorBBM, BB10_ShareSelectorFacebook, BB10_ShareSelectorTwitter;
	}
}

namespace Reign.Plugin
{
	public interface ISocialPlugin
	{
		void Init(SocialDesc desc);
		void Share(byte[] data, string dataFilename, string text, string title, string desc, SocialShareDataTypes type);
		void Share(byte[] data, string dataFilename, string text, string title, string desc, int x, int y, int width, int height, SocialShareDataTypes type);
	}
}