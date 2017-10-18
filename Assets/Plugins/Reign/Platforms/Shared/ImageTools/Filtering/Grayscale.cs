#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Grayscale.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Windows.Media.Reign;
using System.Diagnostics.Contracts.Reign;

namespace ImageTools.Filtering
{
    /// <summary>
    /// Base class for image grayscaling.
    /// </summary>
    /// <remarks>
    /// This class is the base class for image grayscaling. Other classes should inherit from 
    /// this class and specify coefficients used for image conversion to grayscale.
    /// </remarks>
    public abstract class Grayscale : IImageFilter
    {
        #region Fields

        private double _cr;
        private double _cg;
        private double _cb;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        /// <param name="redCoefficient">Red coefficient.</param>
        /// <param name="greenCoefficient">Green coefficient.</param>
        /// <param name="blueCoefficient">Blue coefficient.</param>
        protected Grayscale(double redCoefficient, double greenCoefficient, double blueCoefficient)
        {
            _cr = redCoefficient;
            _cg = greenCoefficient;
            _cb = blueCoefficient;
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
            byte temp = 0;

            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    Color color = source[x, y];

                    temp = (byte)(color.R * _cr + color.G * _cg + color.B * _cb);

                    color.R = temp;
                    color.G = temp;
                    color.B = temp;

                    target[x, y] = color;
                }
            }
        }

        #endregion
    }
}
