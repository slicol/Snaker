#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Image_Operations.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools
{
    partial class ExtendedImage
    {
        /// <summary>
        /// Applies the specified filter to the image.
        /// </summary>
        /// <param name="source">The image, where the filter should be applied to.</param>
        /// <param name="filters">The filter, which should be applied to.</param>
        /// <returns>
        /// A copy of the image with the applied filter.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="filters"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public static ExtendedImage ApplyFilters(ExtendedImage source, params IImageFilter[] filters)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            Contract.Requires<ArgumentNullException>(filters != null, "Filters cannot be null.");

            Rectangle bounds = source.Bounds;

            foreach (IImageFilter filter in filters)
            {
                source = PerformAction(source, true, (sourceImage, targetImage) => filter.Apply(targetImage, sourceImage, bounds));
            }

            return source;
        }

        /// <summary>
        /// Applies the specified filter to the image ath the specified rectangle.
        /// </summary>
        /// <param name="source">The image, where the filter should be applied to.</param>
        /// <param name="filters">The filter, which should be applied to.</param>
        /// <param name="rectangle">The rectangle, where the filter should be applied to.</param>
        /// <returns>
        /// A copy of the image with the applied filter.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="filters"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public static ExtendedImage ApplyFilters(ExtendedImage source, Rectangle rectangle, params IImageFilter[] filters)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            Contract.Requires<ArgumentNullException>(filters != null, "Filters cannot be null.");

            foreach (IImageFilter filter in filters)
            {
                source = PerformAction(source, true, (sourceImage, targetImage) => filter.Apply(targetImage, sourceImage, rectangle));
            }

            return source;
        }

        /// <summary>
        /// Cuts the image with the specified rectangle and returns a new image.
        /// </summary>
        /// <param name="source">The image, where a cutted copy should be made from.</param>
        /// <param name="bounds">The bounds of the new image.</param>
        /// <returns>The new cutted image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null
        /// (Nothing in Visual Basic).</exception>
        public static ExtendedImage Crop(ExtendedImage source, Rectangle bounds)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");

            return PerformAction(source, false, (sourceImage, targetImage) => ImageBase.Crop(sourceImage, targetImage, bounds));
        }

        /// <summary>
        /// Transforms the specified image by flipping and rotating it. First the image
        /// will be rotated, then the image will be flipped. A new image will be returned. The original image
        /// will not be changed.
        /// </summary>
        /// <param name="source">The image, which should be transformed.</param>
        /// <param name="rotationType">Type of the rotation.</param>
        /// <param name="flippingType">Type of the flipping.</param>
        /// <returns>The new and transformed image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null
        /// (Nothing in Visual Basic).</exception>
        public static ExtendedImage Transform(ExtendedImage source, RotationType rotationType, FlippingType flippingType)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");

            return PerformAction(source, false, (sourceImage, targetImage) => ImageBase.Transform(sourceImage, targetImage, rotationType, flippingType));
        }

        /// <summary>
        /// Resizes the specified image by using the specified <see cref="IImageResizer"/> and
        /// return a new image with
        /// the spezified size which is a resized version of the passed image.
        /// </summary>
        /// <param name="source">The width of the new image. Must be greater than zero.</param>
        /// <param name="width">The width of the new image. Must be greater than zero.</param>
        /// <param name="height">The height of the new image. Must be greater than zero.</param>
        /// <param name="resizer">The resizer, which should resize the image.</param>
        /// <returns>The new image.</returns>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="resizer"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is negative.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is negative.</para>
        /// </exception>
        public static ExtendedImage Resize(ExtendedImage source, int width, int height, IImageResizer resizer)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            Contract.Requires<ArgumentNullException>(resizer != null, "Image Resizer cannot be null.");

            return PerformAction(source, false, (sourceImage, targetImage) => resizer.Resize(sourceImage, targetImage, width, height));
        }

        /// <summary>
        /// Resizes the specified image by using the specified <see cref="IImageResizer"/> and
        /// returns new image which has the specified maximum
        /// extension in x and y direction.
        /// </summary>
        /// <param name="source">The source image to resize.</param>
        /// <param name="size">The maximum size of the image in x and y direction.</param>
        /// <param name="resizer">The resizer, which should resize the image.</param>
        /// <returns>The resized image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="size"/> is negative.</exception>
        public static ExtendedImage Resize(ExtendedImage source, int size, IImageResizer resizer)
        {
            Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            Contract.Requires<ArgumentNullException>(resizer != null, "Image Resizer cannot be null.");

            int width = 0;
            int height = 0;

            float ratio = (float)source.PixelWidth / source.PixelHeight;

            if (source.PixelWidth > source.PixelHeight && ratio > 0)
            {
                width = size;
                height = (int)(width / ratio);
            }
            else
            {
                height = size;
                width = (int)(height * ratio);
            }

            return PerformAction(source, false, (sourceImage, targetImage) => resizer.Resize(sourceImage, targetImage, width, height));
        }

        [ContractVerification(false)]
        private static ExtendedImage PerformAction(ExtendedImage source, bool clone, Action<ImageBase, ImageBase> action)
        {
            VerifyHasLoaded(source);

            ExtendedImage transformedImage = clone ? new ExtendedImage(source) : new ExtendedImage();
            
            action(source, transformedImage);

            foreach (ImageFrame frame in source.Frames)
            {
                ImageFrame temp = new ImageFrame();

                action(frame, temp);

                if (!clone)
                {
                    transformedImage.Frames.Add(temp);
                }
            }

            return transformedImage;
        }        
        
        private static void VerifyHasLoaded(ExtendedImage image)
        {
            Contract.Requires(image != null);

            if (!image.IsFilled)
            {
                throw new InvalidOperationException("Image has not been loaded");
            }

            foreach (ImageFrame frame in image.Frames)
            {
                if (frame != null && frame.IsFilled)
                {
                    throw new InvalidOperationException("Not all frames has been loaded yet.");
                }
            }
        }
    }
}
