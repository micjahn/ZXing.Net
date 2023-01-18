/*
 * Copyright 2017 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

using OpenCvSharp;

namespace ZXing.OpenCV
{
	/// <summary>
	/// a barcode reader class which can be used with the Mat type from OpenCVSharp
	/// </summary>
	public class BarcodeReader : BarcodeReader<Mat>
	{
		/// <summary>
		/// define a custom function for creation of a luminance source with our specialized Mat-supporting class
		/// </summary>
		private static readonly Func<Mat, LuminanceSource> defaultCreateLuminanceSource =
			(image) => new MatLuminanceSource(image);

		/// <summary>
		/// constructor which uses a custom luminance source with Mat support
		/// </summary>
		public BarcodeReader()
			: base(null, defaultCreateLuminanceSource, null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
        /// </summary>
        /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
        /// If null then MultiFormatReader is used</param>
        /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
        /// If null, an exception is thrown when Decode is called</param>
        /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
        /// If null then HybridBinarizer is used</param>
        public BarcodeReader(Reader reader,
           Func<Mat, LuminanceSource> createLuminanceSource,
           Func<LuminanceSource, Binarizer> createBinarizer
        )
           : base(reader, createLuminanceSource ?? defaultCreateLuminanceSource, createBinarizer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
        /// </summary>
        /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
        /// If null then MultiFormatReader is used</param>
        /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
        /// If null, an exception is thrown when Decode is called</param>
        /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
        /// If null then HybridBinarizer is used</param>
        /// <param name="createRGBLuminanceSource">Sets the function to create a luminance source object for a rgb raw byte array.</param>
        public BarcodeReader(Reader reader,
           Func<Mat, LuminanceSource> createLuminanceSource,
           Func<LuminanceSource, Binarizer> createBinarizer,
           Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> createRGBLuminanceSource
        )
           : base(reader, createLuminanceSource ?? defaultCreateLuminanceSource, createBinarizer, createRGBLuminanceSource)
        {
        }
    }
}
