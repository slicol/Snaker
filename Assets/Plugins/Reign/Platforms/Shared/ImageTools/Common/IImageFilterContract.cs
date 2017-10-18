#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IImageFilterContract.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools
{
    [ContractClassFor(typeof(IImageFilter))]
    abstract class IImageFilterContract : IImageFilter
    {
        void IImageFilter.Apply(ImageBase target, ImageBase source, Rectangle rectangle)
        {
            Contract.Requires<ArgumentNullException>(target != null, "Target image cannot be null.");
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");

            throw new NotImplementedException();
        }
    }
}
