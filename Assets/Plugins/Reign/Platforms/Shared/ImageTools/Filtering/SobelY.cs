#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// SobelY.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Implements the sobel filter for detection of vertical edges.
    /// </summary>
    public sealed class SobelY : MatrixFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SobelY"/> class.
        /// </summary>
        public SobelY()
        {
            double[,] filter = new double[3, 3] 
                                {
                                    { -1, -2, -1 },
                                    {  0,  0,  0 },
                                    {  1,  2,  1 }
                                };

            Initialize(filter);
        }
    }
}
