#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// IFilter.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System.Diagnostics.Contracts;

namespace ImageTools
{    
    /// <summary>
    /// Image processing filter interface.
    /// </summary>
    /// <remarks>The interface defines the set of methods, which should be
    /// provided by all image processing filters. Methods of this interface
    /// manipulate the original image.</remarks>
#if !WINDOWS_PHONE
    [ContractClass(typeof(IImageFilterContract))]
#endif
    public interface IImageFilter
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
        void Apply(ImageBase target, ImageBase source, Rectangle rectangle);
    }
}
