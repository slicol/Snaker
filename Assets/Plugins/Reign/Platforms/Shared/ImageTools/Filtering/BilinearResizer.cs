#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// BilinearResizer.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Bilinear image resizer, which resizes the image with the bilinear interpolation.
    /// </summary>
    public sealed class BilinearResizer : IImageResizer
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

            byte[] sourcePixels = source.Pixels;

            var GetColor = new Func<double, double, int, byte>((x, y, offset) => sourcePixels[(int)((y * source.PixelWidth + x) * 4 + offset)]);

            double factorX = (double)source.PixelWidth  / width;
            double factorY = (double)source.PixelHeight / height;

            double fractionX, oneMinusX, l, r;
            double fractionY, oneMinusY, t, b;

            byte c1, c2, c3, c4, b1, b2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int dstOffset = (y * width + x) * 4;

                    l = (int)Math.Floor(x * factorX);
                    t = (int)Math.Floor(y * factorY);

                    r = l + 1;
                    b = t + 1;

                    if (r >= source.PixelWidth)
                    {
                        r = l;
                    }

                    if (b >= source.PixelHeight)
                    {
                        b = t;
                    }

                    fractionX = x * factorX - l;
                    fractionY = y * factorY - t;

                    oneMinusX = 1.0 - fractionX;
                    oneMinusY = 1.0 - fractionY;

                    var function = new Func<int, byte>(offset => 
                        {
                            c1 = GetColor(l, t, offset);
                            c2 = GetColor(r, t, offset);
                            c3 = GetColor(l, b, offset);
                            c4 = GetColor(r, b, offset);

                            b1 = (byte)(oneMinusX * c1 + fractionX * c2);
                            b2 = (byte)(oneMinusX * c3 + fractionX * c4);

                            return (byte)(oneMinusY * b1 + fractionY * b2);
                        });

                    newPixels[dstOffset + 0] = function(0);
                    newPixels[dstOffset + 1] = function(1);
                    newPixels[dstOffset + 2] = function(2);
                    newPixels[dstOffset + 3] = 255;
                }
            }

            target.SetPixels(width, height, newPixels);
        }

        #endregion
    }
}
