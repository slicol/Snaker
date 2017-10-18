#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// BmpEncoder.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Bmp
{
    using System;
    using System.IO;
    using ImageTools.Helpers;
    using System.Globalization;

    /// <summary>
    /// Image encoder for writing an image to a stream
    /// as windows bitmap.
    /// </summary>
    /// <remarks>The encoder can only write 24-bit rpg images
    /// to streams. All other formats does not make much sense today.</remarks>
    public class BmpEncoder : IImageEncoder
    {
        #region IImageEncoder Members

        /// <summary>
        /// Gets the default file extension for this encoder ("bmp");
        /// </summary>
        /// <value>The default file extension for this encoder.</value>
        /// <remarks>Please recognize, that "dmp" is also a valid file
        /// extension for Windows Bitmap files but should not be used.</remarks>
        public string Extension
        {
            get { return "bmp"; }
        }

        /// <summary>
        /// Indicates if the image encoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>
        /// <c>true</c>, if the encoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>For example, the <see cref="ImageTools.IO.Bmp.BmpDecoder"/>
        /// supports BMP or DIP as file extension.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        public bool IsSupportedFileExtension(string extension)
        {
            Guard.NotNullOrEmpty(extension, "extension");

			#if UNITY_METRO
			string extensionAsUpper = extension.ToUpper();
			#else
            string extensionAsUpper = extension.ToUpper(CultureInfo.CurrentCulture);
			#endif
            return extensionAsUpper == "BMP" || extensionAsUpper == "DIP";
        }

        /// <summary>
        /// Encodes the data of the specified image and writes the result to
        /// the specified stream.
        /// </summary>
        /// <param name="image">The image, where the data should be get from.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image data should be written to. 
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public void Encode(ExtendedImage image, Stream stream)
        {
            Guard.NotNull(image, "image");
            Guard.NotNull(stream, "stream");

            int rowWidth = image.PixelWidth;

            int amount = (image.PixelWidth * 3) % 4; 
            if (amount != 0)
            {
                rowWidth += 4 - amount;
            }

            BinaryWriter writer = new BinaryWriter(stream);

            BmpFileHeader fileHeader = new BmpFileHeader();
            fileHeader.Type = 19778;
            fileHeader.Offset = 54;
            fileHeader.FileSize = 54 + image.PixelHeight * rowWidth * 3;
            Write(writer, fileHeader);

            BmpInfoHeader infoHeader = new BmpInfoHeader();
            infoHeader.HeaderSize = 40;
            infoHeader.Height = image.PixelHeight;
            infoHeader.Width = image.PixelWidth;
            infoHeader.BitsPerPixel = 24;
            infoHeader.Planes = 1;
            infoHeader.Compression = BmpCompression.RGB;
            infoHeader.ImageSize = image.PixelHeight * rowWidth * 3;
            infoHeader.ClrUsed = 0;
            infoHeader.ClrImportant = 0;
            Write(writer, infoHeader);

            WriteImage(writer, image);

            writer.Flush();
        }

        private static void WriteImage(BinaryWriter writer, ExtendedImage image)
        {
            int amount = (image.PixelWidth * 3) % 4, offset = 0;
            if (amount != 0)
            {
                amount = 4 - amount;
            }

            byte[] data = image.Pixels;

            for (int y = image.PixelHeight - 1; y >= 0; y--)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    offset = (y * image.PixelWidth + x) * 4;

                    writer.Write(data[offset + 2]);
                    writer.Write(data[offset + 1]);
                    writer.Write(data[offset + 0]);
                }

                for (int i = 0; i < amount; i++)
                {
                    writer.Write((byte)0);
                }
            }
        }

        private static void Write(BinaryWriter writer, BmpFileHeader fileHeader)
        {
            writer.Write(fileHeader.Type);
            writer.Write(fileHeader.FileSize);
            writer.Write(fileHeader.Reserved);
            writer.Write(fileHeader.Offset);
        }

        private static void Write(BinaryWriter writer, BmpInfoHeader infoHeader)
        {
            writer.Write(infoHeader.HeaderSize);
            writer.Write(infoHeader.Width);
            writer.Write(infoHeader.Height);
            writer.Write(infoHeader.Planes);
            writer.Write(infoHeader.BitsPerPixel);
            writer.Write((int)infoHeader.Compression);
            writer.Write(infoHeader.ImageSize);
            writer.Write(infoHeader.XPelsPerMeter);
            writer.Write(infoHeader.YPelsPerMeter);
            writer.Write(infoHeader.ClrUsed);
            writer.Write(infoHeader.ClrImportant);
        }

        #endregion
    }
}
