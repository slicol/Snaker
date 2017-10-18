using System;

namespace Reign.Plugin
{
	/// <summary>
	/// Base Email interface object
	/// </summary>
	public interface IEmailPlugin
	{
		/// <summary>
		/// Used to send email
		/// </summary>
		/// <param name="to">To email</param>
		/// <param name="subject">Email subject</param>
		/// <param name="body">Email body</param>
		void Send(string to, string subject, string body);
	}
}
