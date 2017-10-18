//#define WINDOWS_PHONE
//#define SILVERLIGHT

//// ===============================================================================
//// ZXingBarcodeReader.cs
//// .NET Image Tools
//// ===============================================================================
//// Copyright (c) .NET Image Tools Development Group. 
//// All rights reserved.
//// ===============================================================================

//using System;
//using com.google.zxing;
//using com.google.zxing.common;
//using System.Windows;
//using System.Diagnostics.Contracts;
//using System.Collections;

//#pragma warning disable 1685

//namespace ImageTools.Filtering
//{
//	/// <summary>
//	/// Reads a barcode from an image using the zxing barcode reader.
//	/// </summary>
//	/// <remarks>This class wrapps the most important options of zxing. However it does not provide all the flexibility 
//	/// of zxing and you should not use this wrapper when you want to do some finetuning of the detection process.</remarks>
//	public sealed class ZXingBarcodeReader : IBarcodeReader
//	{
//		#region Fields

//		private Hashtable _hints;
//		private MultiFormatReader _reader;
//		private BinarizerMode _binarizerMode;

//		#endregion

//		#region Constructors

//		/// <summary>
//		/// Initializes a new instance of the <see cref="ZXingBarcodeReader"/> class.
//		/// </summary>
//		/// <remarks>The reader tries to detect the barcode in the fastest mode. 
//		/// The accurate mode is off and the global histogram binarizer is used.</remarks>
//		public ZXingBarcodeReader()
//			: this(false, BinarizerMode.Histogram)
//		{

//		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="ZXingBarcodeReader"/> class with
//		/// a flag indicating if the barcode reader should work accurate and the mode of the binarizer.
//		/// </summary>
//		/// <param name="accurate">True, when the barcode reader must work accurate.</param>
//		/// <param name="binarizerMode">The binarizer mode.</param>
//		public ZXingBarcodeReader(bool accurate, BinarizerMode binarizerMode)
//		{
//			_reader = new MultiFormatReader();

//			if (accurate)
//			{
//				_hints = new Hashtable();
//				_hints.Add(DecodeHintType.TRY_HARDER, DecodeHintType.TRY_HARDER);
//			}

//			_binarizerMode = binarizerMode;
//		}

//		#endregion

//		#region Methods

//		/// <summary>
//		/// Reads an barcode from the specified image.
//		/// </summary>
//		/// <param name="image">The image to read the barcode from.</param>
//		/// <returns>
//		/// The result of the reading operation when a barcode could be read or null otherwise.
//		/// </returns>
//		/// <exception cref="ArgumentNullException"><paramref name="image"/> is null.</exception>
//		/// <exception cref="ArgumentException"><paramref name="image"/> is empty.</exception>
//		[ContractVerification(false)]
//		public BarcodeResult ReadBarcode(ExtendedImage image)
//		{
//			byte[] array = new byte[image.PixelWidth * image.PixelHeight * 3];

//			byte[] pixels = image.Pixels;

//			for (int y = 0; y < image.PixelHeight; y++)
//			{
//				for (int x = 0; x < image.PixelWidth; x++)
//				{
//					int offset = y * image.PixelWidth + x;

//					array[offset * 3 + 0] = pixels[offset * 4 + 0];
//					array[offset * 3 + 1] = pixels[offset * 4 + 1];
//					array[offset * 3 + 2] = pixels[offset * 4 + 2];

//				}
//			}

//			BarcodeResult barcodeResult = null;
//			try
//			{
//				RGBLuminanceSource source = new RGBLuminanceSource(array, image.PixelWidth, image.PixelHeight);

//				BinaryBitmap bitmap = null;
//				switch (_binarizerMode)
//				{
//					case BinarizerMode.Hybrid:
//						bitmap = new BinaryBitmap(new HybridBinarizer(source));
//						break;
//					case BinarizerMode.Histogram:
//						bitmap = new BinaryBitmap(new GlobalHistogramBinarizer(source));
//						break;
//					default:
//						break;
//				}

//				Result result = _reader.decode(bitmap, _hints);

//				barcodeResult = new BarcodeResult();
//				barcodeResult.Text = result.Text;

//				if (result.RawBytes != null)
//				{
//					foreach (sbyte item in result.RawBytes)
//					{
//						barcodeResult.RawBytes.Add(item);
//					}
//				}

//				if (result.ResultPoints != null)
//				{
//					foreach (ResultPoint point in result.ResultPoints)
//					{
//						barcodeResult.Points.Add(new Point(point.X, point.Y));
//					}
//				}

//				if (result.BarcodeFormat == BarcodeFormat.CODE_128)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Code128;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.CODE_39)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Code39;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.DATAMATRIX)
//				{
//					barcodeResult.Format = BarcodeResultFormat.DateMatrix;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.EAN_13)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Ean13;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.EAN_8)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Ean8;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.ITF)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Itf;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.PDF417)
//				{
//					barcodeResult.Format = BarcodeResultFormat.Pdf417;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.QR_CODE)
//				{
//					barcodeResult.Format = BarcodeResultFormat.QrCode;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.UPC_A)
//				{
//					barcodeResult.Format = BarcodeResultFormat.UpcA;
//				}
//				else if (result.BarcodeFormat == BarcodeFormat.UPC_E)
//				{
//					barcodeResult.Format = BarcodeResultFormat.UpcE;
//				}
//				else
//				{
//					barcodeResult.Format = BarcodeResultFormat.Unknown;
//				}
//			}
//			catch (ReaderException)
//			{
//			}

//			return barcodeResult;
//		}

//		#endregion
//	}
//}
