#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Inverter.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts.Reign;
using System.Windows.Media.Reign;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Simple filter for inverting the colors of an image.
    /// </summary>
    /// <remarks>The filter inverts colored and grayscale images.</remarks> 
    public sealed class Inverter : IImageFilter
    {
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

                    color.R = (byte)(255 - color.R);
                    color.G = (byte)(255 - color.G);
                    color.B = (byte)(255 - color.B);

                    target[x, y] = color;
                }
            }
        }
    }
}
