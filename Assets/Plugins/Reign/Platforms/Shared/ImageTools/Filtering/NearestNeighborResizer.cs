#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// NearestNeighborResizer.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using ImageTools.Helpers;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Default image resizer, which resizes the image with the fast known method,
    /// without optimizing the quality of the image. Uses the nearest neighbor interpolation.
    /// </summary>
    public sealed class NearestNeighborResizer : IImageResizer
    {
        #region IImageResizer Members

        /// <summary>
        /// Resizes the specified source image by creating a new image with
        /// the spezified size which is a resized version of the passed image..
        /// </summary>
        /// <param name="source">The source image, where the pixel data should be get from.</param>
        /// <param name="target">The resized image.</param>
        /// <param name="width">The width of the new image. Must be greater than zero.</param>
        /// <param name="height">The height of the new image. Must be greater than zero..</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="target"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is negative.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is negative.</para>
        /// </exception>
        [ContractVerification(false)]
        public void Resize(ImageBase source, ImageBase target, int width, int height)
        {
            byte[] newPixels = new byte[width * height * 4];

            double xFactor = (double)source.PixelWidth / width;
            double yFactor = (double)source.PixelHeight / height;

            int dstOffsetLine = 0;
            int dstOffset = 0;

            int srcOffsetLine = 0;
            int srcOffset = 0;

            byte[] sourcePixels = source.Pixels;

            for (int y = 0; y < height; y++)
            {
                dstOffsetLine = 4 * width * y;

                // Calculate the line offset at the source image, where the pixels should be get from.
                srcOffsetLine = 4 * source.PixelWidth * (int)(y * yFactor);

                for (int x = 0; x < width; x++)
                {
                    dstOffset = dstOffsetLine + 4 * x;
                    srcOffset = srcOffsetLine + 4 * (int)(x * xFactor);

                    newPixels[dstOffset + 0] = sourcePixels[srcOffset + 0];
                    newPixels[dstOffset + 1] = sourcePixels[srcOffset + 1];
                    newPixels[dstOffset + 2] = sourcePixels[srcOffset + 2];
                    newPixels[dstOffset + 3] = sourcePixels[srcOffset + 3];
                }
            }

            target.SetPixels(width, height, newPixels);
        }

        #endregion
    }
}
