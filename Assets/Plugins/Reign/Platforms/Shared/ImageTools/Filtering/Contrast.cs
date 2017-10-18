#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Contrast.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;
using System.Windows.Media.Reign;
using ImageTools.Helpers;

namespace ImageTools.Filtering
{
    /// <summary>
    /// An <see cref="IImageFilter"/> to change the contrast of an <see cref="ExtendedImage"/>.
    /// </summary>
    public sealed class Contrast : IImageFilter
    {
        #region Fields

        private int _contrast;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Contrast"/> class and sets 
        /// the new contrast of the image.
        /// </summary>
        /// <param name="contrast">The new contrast of the image. Must be between -100 and 100.</param>      
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="contrast"/> is less than -100.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="contrast"/> is greater than 100.</para>
        /// </exception>
        public Contrast(int contrast)
        {
            Contract.Requires<ArgumentException>(contrast >= -255, "Brightness must be greater than -255.");
            Contract.Requires<ArgumentException>(contrast <= 255, "Brightness must be less than 255.");

            _contrast = contrast;
        }

        #endregion

        #region Methods

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
            double pixel = 0, contrast = (100.0 + _contrast) / 100.0;

            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    Color color = source[x, y];

                    pixel = color.R / 255.0;
                    pixel -= 0.5;
                    pixel *= contrast;
                    pixel += 0.5;
                    pixel *= 255;
                    pixel = pixel.RemainBetween(0, 255);

                    color.R = (byte)pixel;

                    pixel = color.G / 255.0;
                    pixel -= 0.5;
                    pixel *= contrast;
                    pixel += 0.5;
                    pixel *= 255;
                    pixel = pixel.RemainBetween(0, 255);

                    color.G = (byte)pixel;

                    pixel = color.B / 255.0;
                    pixel -= 0.5;
                    pixel *= contrast;
                    pixel += 0.5;
                    pixel *= 255;
                    pixel = pixel.RemainBetween(0, 255);

                    color.B = (byte)pixel;

                    target[x, y] = color;
                }
            }
        }

        #endregion
    }
}
