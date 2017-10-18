#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Sepia.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Windows.Media.Reign;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Simple sepia filter, which makes the image look like an old brown photo.
    /// </summary>
    /// <remarks>The filter makes an image look like an old brown photo.</remarks>
    public sealed class Sepia : IImageFilter
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
            byte temp = 0;

            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    Color color = source[x, y];

                    temp = (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);

                    color.R = (byte)((temp > 206) ? 255 : temp + 49);
                    color.G = (byte)((temp < 14) ? 0 : temp - 14);
                    color.B = (byte)((temp < 56) ? 0 : temp - 56);

                    target[x, y] = color;
                }
            }
        }
    }
}
