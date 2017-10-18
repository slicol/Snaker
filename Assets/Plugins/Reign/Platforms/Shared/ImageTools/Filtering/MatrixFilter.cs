#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// MatrixFilter.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;
using System.Windows.Media.Reign;
using ImageTools.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Defines an abstract base filter that uses a matrix a factor and a bias value to 
    /// change the color of a matrix.
    /// </summary>
    public abstract class MatrixFilter : IImageFilter
    {
        #region Fields

        private double[,] _filter;
        private double _factor;
        private double _bias;

        #endregion

        #region Properties

        /// <summary>
        /// Initializes this filter with the filter matrix.
        /// </summary>
        /// <param name="filter">The filter matrix. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filter"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// 	<para>The row length of the <paramref name="filter"/> matrix is less or equals than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para>The row length of the <paramref name="filter"/> matrix is an even number.</para>
        /// 	<para>- or -</para>
        /// 	<para>The column length of the <paramref name="filter"/> matrix is less or equals than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para>The column length of the <paramref name="filter"/> matrix is an even number.</para>
        /// </exception>
        protected void Initialize(double[,] filter)
        {
            Contract.Requires<ArgumentNullException>(filter != null, "Filter cannot be null.");

            Initialize(filter, 1f, 0);
        }

        /// <summary>
        /// Initializes this filter with the filter matrix a factor and a bias.
        /// </summary>
        /// <param name="filter">The filter matrix. Cannot be null.</param>
        /// <param name="factor">The factor of the filter. Must be greater than zero.</param>
        /// <param name="bias">The bias that is added to the final color.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filter"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="factor"/> is equals or less than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para>The row length of the <paramref name="filter"/> matrix is less or equals than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para>The row length of the <paramref name="filter"/> matrix is an even number.</para>
        /// 	<para>- or -</para>
        /// 	<para>The column length of the <paramref name="filter"/> matrix is less or equals than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para>The column length of the <paramref name="filter"/> matrix is an even number.</para>
        /// </exception>
        protected void Initialize(double[,] filter, double factor, double bias)
        {
            Contract.Requires<ArgumentNullException>(filter != null, "Filter cannot be null.");

            Guard.GreaterThan(filter.GetLength(0), 0, "filter.GetLength(0)");
            Guard.GreaterThan(filter.GetLength(1), 0, "filter.GetLength(1)");

            if (filter.GetLength(0) % 2 == 0)
            {
                throw new ArgumentException("The number of rows cannot be an even number.", "filter");
            }

            if (filter.GetLength(1) % 2 == 0)
            {
                throw new ArgumentException("The number of columns cannot be an even number.", "filter");
            }

            _filter = filter;

            _factor = factor;

            _bias = bias;
        }

        /// <summary>
        /// This method is called before the filter is applied to prepare the filter 
        /// matrix. Only calculate a new matrix, when the properties has been changed.
        /// </summary>
        protected virtual void PrepareFilter()
        {
        }

        /// <summary>
        /// Apply filter to an image at the area of the specified rectangle.
        /// </summary>
        /// <param name="target">Target image to apply filter to.</param>
        /// <param name="source">The source image. Cannot be null.</param>
        /// <param name="rectangle">The rectangle, which defines the area of the
        /// image where the filter should be applied to.</param>
        /// <remarks>The method keeps the source image unchanged and returns the
        /// the result of image processing filter as new image.</remarks>
        /// <exception cref="System.ArgumentNullException">
        /// 	<para><paramref name="target"/> is null.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="target"/> is null.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException"><paramref name="rectangle"/> doesnt fits
        /// to the image.</exception>
        [ContractVerification(false)]
        public void Apply(ImageBase target, ImageBase source, Rectangle rectangle)
        {
            PrepareFilter();

            if (_filter != null)
            {
                int filterSize = _filter.GetLength(0);

                for (int y = rectangle.Y; y < rectangle.Bottom; y++)
                {
                    for (int x = rectangle.X; x < rectangle.Right; x++)
                    {
                        double r = 0, g = 0, b = 0;

                        Color color = source[x, y];

                        for (int filterY = 0; filterY < filterSize; filterY++)
                        {
                            for (int filterX = 0; filterX < filterSize; filterX++)
                            {
                                int imageX = (x - filterSize / 2 + filterX + rectangle.Width) % rectangle.Width;
                                int imageY = (y - filterSize / 2 + filterY + rectangle.Height) % rectangle.Height;

                                Color tempColor = source[imageX, imageY];

                                r += tempColor.R * _filter[filterX, filterY];
                                g += tempColor.G * _filter[filterX, filterY];
                                b += tempColor.B * _filter[filterX, filterY];
                            }
                        }

                        color.R = (byte)(_factor * r + _bias).RemainBetween(0, 255);
                        color.G = (byte)(_factor * g + _bias).RemainBetween(0, 255);
                        color.B = (byte)(_factor * b + _bias).RemainBetween(0, 255);

                        target[x, y] = color;
                    }
                }
            }
        }

        #endregion
    }
}
