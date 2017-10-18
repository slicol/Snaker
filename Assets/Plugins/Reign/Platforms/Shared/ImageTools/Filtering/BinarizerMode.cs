#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// ZXingBarcodeReaderBinarizerMode.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.Filtering
{
    /// <summary>
    /// Defines which binarizer is used when reading a barcode using zxing.
    /// </summary>
    public enum BinarizerMode
    {
        /// <summary>
        /// The hybrid binarizer is used.
        /// This class implements a local thresholding algorithm, which while slower than the
        /// GlobalHistogramBinarizer, is fairly efficient for what it does. It is designed for
        /// high frequency images of barcodes with black data on white backgrounds. For this application,
        /// it does a much better job than a global blackpoint with severe shadows and gradients.
        /// </summary>
        Hybrid,
        /// <summary>
        /// The global historgram binarizer is used.
        /// This Binarizer implementation uses the old ZXing global histogram approach. It is suitable
        /// for low-end mobile devices which don't have enough CPU or memory to use a local thresholding
        /// algorithm. However, because it picks a global black point, it cannot handle difficult shadows
        /// and gradients.
        /// </summary>
        Histogram
    }
}
