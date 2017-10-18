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
    /// An <see cref="IImageFilter"/> for changing the brightness of an <see cref="ExtendedImage"/>.
    /// </summary>
    public sealed class Brightness : IImageFilter
    {
        #region Fields

        private int _brightness;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Brightness"/> class.
        /// </summary>
        /// <param name="brightness">The brightness value. Must be between -255 and 255.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="brightness"/> is less than -255.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="brightness"/> is greater than 255.</para>
        /// </exception>
        public Brightness(int brightness)
        {
            Contract.Requires<ArgumentException>(brightness >=-255, "Brightness must be greater than -255.");
            Contract.Requires<ArgumentException>(brightness <= 255, "Brightness must be less than 255.");

            _brightness = brightness;
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
            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    Color color = source[x, y];

                    int r = color.R + _brightness;
                    int g = color.G + _brightness;
                    int b = color.B + _brightness;

                    r = r.RemainBetween(0, 255);
                    g = g.RemainBetween(0, 255);
                    b = b.RemainBetween(0, 255);

                    color.R = (byte)r;
                    color.G = (byte)g;
                    color.B = (byte)b;

                    target[x, y] = color;
                }
            }
        }

        #endregion
    }
}
