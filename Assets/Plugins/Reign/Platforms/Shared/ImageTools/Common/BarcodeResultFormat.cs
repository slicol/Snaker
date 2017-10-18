#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// BarcodeResultFormat.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools
{
    /// <summary>
    /// Defines the format of the read barcode.
    /// </summary>
    public enum BarcodeResultFormat
    {
        /// <summary>
        /// QR Code 2D barcode format. 
        /// </summary>
        QrCode,
        /// <summary>
        /// DataMatrix 2D barcode format.
        /// </summary>
        DateMatrix,
        /// <summary>
        /// UPC-E 1D format. 
        /// </summary>
        UpcE,
        /// <summary>
        /// UPC-A 1D format. 
        /// </summary>
        UpcA,
        /// <summary>
        /// EAN-8 1D format.
        /// </summary>
        Ean8,
        /// <summary>
        /// EAN-13 1D format.
        /// </summary>
        Ean13,
        /// <summary>
        /// Code 128 1D format.
        /// </summary>
        Code128,
        /// <summary>
        /// Code 39 1D format.
        /// </summary>
        Code39,
        /// <summary>
        /// ITF (Interleaved Two of Five) 1D format.
        /// </summary>
        Itf,
        /// <summary>
        /// PDF417 format.
        /// </summary>
        Pdf417,
        /// <summary>
        /// The format of the barcode is not known.
        /// </summary>
        Unknown
    }
}
