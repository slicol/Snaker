#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IImageResizer.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts;

namespace ImageTools
{
    /// <summary>
    /// Interface for all image resizing algorithms.
    /// </summary>
#if !WINDOWS_PHONE
    [ContractClass(typeof(IImageResizerContract))]
#endif
    public interface IImageResizer
    {
        /// <summary>
        /// Resizes the specified source image by creating a new image with
        /// the spezified size which is a resized version of the passed image..
        /// </summary>
        /// <param name="source">The source image, where the pixel data should be get from.</param>
        /// <param name="target">The resized image.</param>
        /// <param name="width">The width of the new image. Must be greater than zero.</param>
        /// <param name="height">The height of the new image. Must be greater than zero..</param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="target"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// 	<para><paramref name="width"/> is negative.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is negative.</para>
        /// </exception>
        void Resize(ImageBase source, ImageBase target, int width, int height);
    }
}
