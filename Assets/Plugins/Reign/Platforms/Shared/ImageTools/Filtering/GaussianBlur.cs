#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// GaussianBlur.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;
using System.Diagnostics.CodeAnalysis;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Implements a gaussian blur filter using a filter matrix.
    /// </summary>
    public sealed class GaussianBlur : MatrixFilter
    {
        private double _oldVariance;

        /// <summary>
        /// Gets or sets the variance that is used to calculate the filter matrix.
        /// </summary>
        /// <value>The variance to calculate the filter matrix.</value>
        public double Variance { get; set; }

        /// <summary>
        /// This method is called before the filter is applied to prepare the filter
        /// matrix. Only calculate a new matrix, when the properties has been changed.
        /// </summary>
        [ContractVerification(false)]
        protected override void PrepareFilter()
        {
            if (_oldVariance != Variance && Variance > 0)
            {
                int filterSize = (int)(2 * Variance) * 2 + 1;

                double[,] filter = new double[filterSize, filterSize];

                for (int y = 0; y < filterSize; y++)
                {
                    for (int x = 0; x < filterSize; x++)
                    {
                        int filterX = x - (filterSize / 2);
                        int filterY = y - (filterSize / 2);

                        double v2 = Variance * Variance;

                        int x2 = filterX * filterX;
                        int y2 = filterY * filterY;

                        double factor = (1.0 / (2 * Math.PI * v2));

                        double exponent = -(x2 + y2) / (2 * v2);

                        filter[x, y] = factor * Math.Pow(Math.E, exponent);
                    }
                }

                Initialize(filter);

                _oldVariance = Variance;
            }
        }
    }
}
