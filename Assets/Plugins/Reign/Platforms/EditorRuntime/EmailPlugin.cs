#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.Text;

namespace Reign.Plugin
{
	public class EmailPlugin : IEmailPlugin
	{
		public void Send(string to, string subject, string body)
		{
			UnityEngine.Debug.Log(string.Format("Send mail: To={0} Subject={1} Body={2}", to, subject, body));

			var theStringBuilder = new StringBuilder();
			theStringBuilder.Append("mailto:" + to + "?");
			theStringBuilder.Append("&subject=" + subject);
			theStringBuilder.Append("&body=" + body);
			//&attachment="/files/audio/attachment.mp3"
			#if UNITY_WEBPLAYER
			UnityEngine.Application.OpenURL(theStringBuilder.ToString());
			#else
			Process.Start(theStringBuilder.ToString());
			#endif
		}
	}
}
#endif