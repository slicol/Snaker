#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// BmpInfoHeader.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Bmp
{
    /// <summary>
    /// This block of bytes tells the application detailed information 
    /// about the image, which will be used to display the image on 
    /// the screen.
    /// </summary>
    class BmpInfoHeader
    {        
        /// <summary>
        /// Defines of the data structure in the bitmap file.
        /// </summary>
        public const int Size = 40;

        /// <summary>
        /// The size of this header (40 bytes)
        /// </summary>
        public int HeaderSize;
        /// <summary>
        /// The bitmap width in pixels (signed integer).
        /// </summary>
        public int Width;
        /// <summary>
        /// The bitmap height in pixels (signed integer).
        /// </summary>
        public int Height;
        /// <summary>
        /// The number of color planes being used. Must be set to 1.
        /// </summary>
        public short Planes;
        /// <summary>
        /// The number of bits per pixel, which is the color depth of the image. 
        /// Typical values are 1, 4, 8, 16, 24 and 32.
        /// </summary>
        public short BitsPerPixel;
        /// <summary>
        /// The compression method being used. 
        /// See the next table for a list of possible values.
        /// </summary>
        public BmpCompression Compression;
        /// <summary>
        /// The image size. This is the size of the raw bitmap data (see below), 
        /// and should not be confused with the file size.
        /// </summary>
        public int ImageSize;
        /// <summary>
        /// The horizontal resolution of the image. 
        /// (pixel per meter, signed integer)
        /// </summary>
        public int XPelsPerMeter;
        /// <summary>
        /// The vertical resolution of the image. 
        /// (pixel per meter, signed integer)
        /// </summary>
        public int YPelsPerMeter;
        /// <summary>
        /// The number of colors in the color palette, 
        /// or 0 to default to 2^n.
        /// </summary>
        public int ClrUsed;
        /// <summary>
        /// The number of important colors used, 
        /// or 0 when every color is important; generally ignored.
        /// </summary>
        public int ClrImportant;
    }
}
