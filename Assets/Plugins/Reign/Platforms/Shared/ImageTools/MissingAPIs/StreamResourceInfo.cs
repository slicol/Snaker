#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

using System;
using System.IO;

namespace System.Windows.Resources.Reign
{
	// Summary:
	//     Provides resource stream information for application resources or other packages
	//     obtained through the System.Windows.Application.GetResourceStream(System.Windows.Resources.StreamResourceInfo,System.Uri)
	//     method.
	public class StreamResourceInfo
	{
		private Stream stream;
		private string contentType;

		// Summary:
		//     Initializes a new instance of the System.Windows.Resources.StreamResourceInfo
		//     class.
		//
		// Parameters:
		//   stream:
		//     The stream to use to create the resource.
		//
		//   contentType:
		//     The MIME type of the content.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     stream is null.
		public StreamResourceInfo(Stream stream, string contentType)
		{
			this.stream = stream;
			this.contentType = contentType;
		}

		// Summary:
		//     Gets the MIME type of the content in the stream.
		//
		// Returns:
		//     The MIME type of the content in the stream, as a string.
		public string ContentType { get{return contentType;} }
		//
		// Summary:
		//     Gets the stream that is contained by the resource.
		//
		// Returns:
		//     The stream that is contained by the resource.
		public Stream Stream { get{return stream;} }
	}
}