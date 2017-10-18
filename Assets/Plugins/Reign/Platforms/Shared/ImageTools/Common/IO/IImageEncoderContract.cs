#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IImageEncoderContract.cs
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
    [ContractClassFor(typeof(IImageEncoder))]
    abstract class IImageEncoderContract : IImageEncoder
    {
        string IImageEncoder.Extension
        {
            get { throw new NotImplementedException(); }
        }

        bool IImageEncoder.IsSupportedFileExtension(string extension)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(extension), "Extension cannot be null or empty.");

            throw new NotImplementedException();
        }

        void IImageEncoder.Encode(ExtendedImage image, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(image != null, "Image cannot be null.");
            Contract.Requires<ArgumentNullException>(stream != null, "Stream cannot be null.");

            throw new NotImplementedException();
        }
    }
}
