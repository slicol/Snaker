#define WINDOWS_PHONE
#define SILVERLIGHT

 // ===============================================================================
// ImageBase.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts.Reign;
using System.Windows.Media.Reign;
using ImageTools.Helpers;

namespace ImageTools
{
    /// <summary>
    /// Base classes for all Images.
    /// </summary>
    [ContractVerification(false)]
    public partial class ImageBase
    {
        #region Constants

        /// <summary>
        /// The default animation speed, when the image is animated.
        /// </summary>
        public const int DefaultDelayTime = 10;

        #endregion

        #region Invariant

#if !WINDOWS_PHONE
        [ContractInvariantMethod]
        private void ImageBaseInvariantMethod()
        {
            Contract.Invariant(!_isFilled || _pixels != null);
            Contract.Invariant(!_isFilled || _pixelWidth >= 0);
            Contract.Invariant(!_isFilled || _pixelHeight >= 0);
        }
#endif

        #endregion

        #region Properties

        private int _delayTime;
        /// <summary>
		/// If not 0, this field specifies the number of hundredths (1/100) of a second to 
		/// wait before continuing with the processing of the Data Stream. 
		/// The clock starts ticking immediately after the graphic is rendered. 
		/// This field may be used in conjunction with the User Input Flag field. 
		/// </summary>
        [Pure]
        public int DelayTime
        {
            get
            {
                int delayTime = _delayTime;

                if (delayTime <= 0)
                {
                    delayTime = DefaultDelayTime;
                }

                return delayTime;
            }
            set { _delayTime = value; }
        }

        private bool _isFilled;
        /// <summary>
        /// Gets or sets a value indicating whether this image has been loaded.
        /// </summary>
        /// <value><c>true</c> if this image has been loaded; otherwise, <c>false</c>.</value>
        [Pure]
        public bool IsFilled
        {
            get
            {
                return _isFilled;
            }
        }

        private byte[] _pixels;
        /// <summary>
        /// Returns all pixels of the image as simple byte array.
        /// </summary>
        /// <value>All image pixels as byte array.</value>
        /// <remarks>The returned array has a length of Width * Length * 4 bytes
        /// and stores the red, the green, the blue and the alpha value for
        /// each pixel in this order.</remarks>
        [Pure]
        public byte[] Pixels
        {
            get
            {
                Contract.Ensures(!IsFilled || Contract.Result<byte[]>() != null);
                return _pixels;
            }
        }

		/// <summary>
		/// Use to get Unity3D Pixel colors
		/// </summary>
		public UnityEngine.Color[] Colors
        {
            get
            {
                var colors = new UnityEngine.Color[_pixels.Length / 4];
				for (int i = 0, i2 = 0; i != colors.Length; ++i, i2 += 4)
				{
					colors[i] = new UnityEngine.Color
					(
						_pixels[i2] / 255f,
						_pixels[i2+1] / 255f,
						_pixels[i2+2] / 255f,
						_pixels[i2+3] / 255f
					);
				}

                return colors;
            }
        }

        private int _pixelHeight;
        /// <summary>
        /// Gets the height of this <see cref="ExtendedImage"/> in pixels.
        /// </summary>
        /// <value>The height of this image.</value>
        /// <remarks>The height will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelHeight
        {
            get 
            {
                Contract.Ensures(!IsFilled || Contract.Result<int>() > 0);
                return _pixelHeight; 
            }
        }

        private int _pixelWidth;
        /// <summary>
        /// Gets the width of this <see cref="ExtendedImage"/> in pixels.
        /// </summary>
        /// <value>The width of this image.</value>
        /// <remarks>The width will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelWidth
        {
            get
            {
                Contract.Ensures(!IsFilled || Contract.Result<int>() > 0);
                return _pixelWidth;
            }
        }

        /// <summary>
        /// Gets the ratio between the width and the height of this <see cref="ImageBase"/> instance.
        /// </summary>
        /// <value>The ratio between the width and the height.</value>
        public double PixelRatio
        {
            get
            {
                Contract.Ensures(!IsFilled || Contract.Result<double>() > 0);

                if (IsFilled)
                {
                    return (double)PixelWidth / PixelHeight;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of a pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <value>The color of the pixel.</value>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="x"/> is smaller than zero or greater than
        ///     the width of the image.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="y"/> is smaller than zero or greater than
        ///     the height of the image.</para>
        /// </exception>
        [Pure]
        public Color this[int x, int y]
        {
            get
            {
                Contract.Requires<InvalidOperationException>(IsFilled, "Image is not loaded.");
                Contract.Requires<ArgumentException>(x >= 0 && x < PixelWidth, "X must be in the range of the image.");
                Contract.Requires<ArgumentException>(y >= 0 && y < PixelHeight, "Y must be in the range of the image.");
                Contract.Ensures(IsFilled);

                int start = (y * PixelWidth + x) * 4;

                Color result = new Color();

                result.R = _pixels[start + 0];
                result.G = _pixels[start + 1];
                result.B = _pixels[start + 2];
                result.A = _pixels[start + 3];

                return result;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(IsFilled, "Image is not loaded.");
                Contract.Requires<ArgumentException>(x >= 0 && x < PixelWidth, "X must be in the range of the image.");
                Contract.Requires<ArgumentException>(y >= 0 && y < PixelHeight, "Y must be in the range of the image.");
                Contract.Ensures(IsFilled);

                int start = (y * PixelWidth + x) * 4;

                _pixels[start + 0] = value.R;
                _pixels[start + 1] = value.G;
                _pixels[start + 2] = value.B;
                _pixels[start + 3] = value.A;
            }
        }

        /// <summary>
        /// Calculates a new rectangle which represents 
        /// the dimensions of this image.
        /// </summary>
        /// <value>The <see cref="Rectangle"/> object, which
        /// represents the image dimension.</value>
        [Pure]
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, PixelWidth, PixelHeight);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="width"/> is equals or less than zero.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="height"/> is equals or less than zero.</para>
        /// </exception>
        public ImageBase(int width, int height)
        {
            Contract.Requires<ArgumentException>(width >= 0, "Width must be greater or equals than zero.");
            Contract.Requires<ArgumentException>(height >= 0, "Height must be greater or equals than zero.");
            Contract.Ensures(IsFilled);

            _pixelWidth  = width;
            _pixelHeight = height;

            _pixels = new byte[PixelWidth * PixelHeight * 4];

            _isFilled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> is not loaded.</exception>
        public ImageBase(ImageBase other)
        {
            Contract.Requires<ArgumentNullException>(other != null, "Other image cannot be null.");
            Contract.Requires<ArgumentException>(other.IsFilled, "Other image has not been loaded.");
            Contract.Ensures(IsFilled);

            byte[] pixels = other.Pixels;

            _pixelWidth  = other.PixelWidth;
            _pixelHeight = other.PixelHeight;
            _pixels = new byte[pixels.Length];

            Array.Copy(pixels, _pixels, pixels.Length);

            _isFilled = other.IsFilled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class.
        /// </summary>
        public ImageBase()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the pixel array of the image.
        /// </summary>
        /// <param name="width">The new width of the image.
        /// Must be greater than zero.</param>
        /// <param name="height">The new height of the image.
        /// Must be greater than zero.</param>
        /// <param name="pixels">The array with colors. Must be a multiple
        /// of four, width and height.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="pixels"/> is not a multiple of four, 
        /// 	width and height.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is null.</exception>
        public void SetPixels(int width, int height, byte[] pixels)
        {
            Contract.Requires<ArgumentException>(width >= 0, "Width must be greater than zero.");
            Contract.Requires<ArgumentException>(height >= 0, "Height must be greater than zero.");
            Contract.Requires<ArgumentNullException>(pixels != null, "Pixels cannot be null.");
            Contract.Ensures(IsFilled);

            if (pixels.Length != width * height * 4)
            {
                throw new ArgumentException(
                    "Pixel array must have the length of width * height * 4.",
                    "pixels");
            }

            _pixelWidth  = width;
            _pixelHeight = height;
            _pixels = pixels;

            _isFilled = true;
        }

        #endregion
    }
}
