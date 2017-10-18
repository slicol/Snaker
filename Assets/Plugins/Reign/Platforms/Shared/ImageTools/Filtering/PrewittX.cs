#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// PrewittX.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Implements the prewitt filter for detection of horizontal edges.
    /// </summary>
    public sealed class PrewittX : MatrixFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrewittX"/> class.
        /// </summary>
        public PrewittX()
        {
            double[,] filter = new double[3, 3] 
                                {
                                    { -1, 0, 1 },
                                    { -1, 0, 1 },
                                    { -1, 0, 1 }
                                };

            Initialize(filter);
        }
    }
}
