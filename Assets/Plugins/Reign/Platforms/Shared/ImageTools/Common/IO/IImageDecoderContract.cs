#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IImageDecoderContract.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;
using System.IO;

namespace ImageTools.IO
{
    [ContractClassFor(typeof(IImageDecoder))]
    abstract class IImageDecoderContract : IImageDecoder
    {
        int IImageDecoder.HeaderSize
        {
            get { throw new NotImplementedException(); }
        }

        bool IImageDecoder.IsSupportedFileExtension(string extension)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(extension), "Extension cannot be null or empty.");

            throw new NotImplementedException();
        }

        bool IImageDecoder.IsSupportedFileFormat(byte[] header)
        {
            Contract.Requires<ArgumentNullException>(header != null, "Header cannot be null.");

            throw new NotImplementedException();
        }

        void IImageDecoder.Decode(ExtendedImage image, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(image != null, "Image cannot be null.");
            Contract.Requires<ArgumentNullException>(stream != null, "Stream cannot be null.");

            throw new NotImplementedException();
        }
    }
}
