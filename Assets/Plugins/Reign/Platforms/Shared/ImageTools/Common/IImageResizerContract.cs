#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IImageResizerContract.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools
{
    [ContractClassFor(typeof(IImageResizer))]
    abstract class IImageResizerContract : IImageResizer
    {
        public void Resize(ImageBase source, ImageBase target, int width, int height)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            Contract.Requires<ArgumentNullException>(target != null, "Target image cannot be null.");
            Contract.Requires<ArgumentException>(width >= 0, "Width must be greater or equals than zero.");
            Contract.Requires<ArgumentException>(height >= 0, "Height must be greater or equals than zero.");

            throw new NotImplementedException();
        }
    }
}
