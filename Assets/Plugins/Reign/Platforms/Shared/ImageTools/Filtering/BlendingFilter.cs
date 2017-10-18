#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// BlendingFilter.cs
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
    /// An <see cref="IImageFilter"/> for adding an overlay image to an <see cref="ExtendedImage"/>. The transperency
    /// of the overlay is respected, so an alpha value of 255 in the overlay image pixel means that the original image pixel
    /// is fully replaced. A value of 0 means that the original image pixel is not changed at all.
    /// </summary>
    public sealed class BlendingFilter : IImageFilter
    {
        #region Fields

        private readonly ImageBase _blendedImage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global alpha factor.
        /// </summary>
        /// <value>The global alpha factor.</value>
        public double? GlobalAlphaFactor { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendingFilter"/> class.
        /// </summary>
        /// <param name="blendedImage">The image that should be added to another image when apply is called. Is not allowed to be null.</param>
        /// <exception cref="ArgumentException"><paramref name="blendedImage"/> is null.</exception>
        public BlendingFilter(ImageBase blendedImage)
        {
            Contract.Requires<ArgumentException>(blendedImage != null, "Pased image is not allowed to be null!");

            _blendedImage = blendedImage;
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
        /// 	<para><paramref name="source"/> is null.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException"><paramref name="rectangle"/> doesnt fits
        /// to the image.</exception>
        [ContractVerification(false)]
        public void Apply(ImageBase target, ImageBase source, Rectangle rectangle)
        {
            // Make sure we stop combining when the whole image that should be combined has been processed.
            if (rectangle.Right > _blendedImage.PixelWidth)
            {
                rectangle.Width = _blendedImage.PixelWidth - rectangle.Left;
            }

            if (rectangle.Bottom > _blendedImage.PixelHeight)
            {
                rectangle.Height = _blendedImage.PixelHeight - rectangle.Top;
            }

            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    Color color = source[x, y], blendedColor = _blendedImage[x, y];

                    // combining colors is dependent o the alpha of the blended color
                    double alphaFactor = GlobalAlphaFactor != null ? GlobalAlphaFactor.Value : blendedColor.A / 255.0;

                    double invertedAlphaFactor = 1 - alphaFactor;

                    int r = (int) (color.R * invertedAlphaFactor) + (int) (blendedColor.R * alphaFactor);
                    int g = (int) (color.G * invertedAlphaFactor) + (int) (blendedColor.G * alphaFactor);
                    int b = (int) (color.B * invertedAlphaFactor) + (int) (blendedColor.B * alphaFactor);

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
