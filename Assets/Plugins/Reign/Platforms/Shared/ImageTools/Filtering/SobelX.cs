#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// SobelX.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Implements the sobel filter for detection of horizontal edges.
    /// </summary>
    public sealed class SobelX : MatrixFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SobelX"/> class.
        /// </summary>
        public SobelX()
        {
            double[,] filter = new double[3, 3] 
                                {
                                    { -1, 0, 1 },
                                    { -2, 0, 2 },
                                    { -1, 0, 1 }
                                };

            Initialize(filter);
        }
    }
}
