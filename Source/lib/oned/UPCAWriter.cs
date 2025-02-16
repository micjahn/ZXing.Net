/*
 * Copyright 2010 ZXing authors
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
using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// This object renders a UPC-A code as a <see cref="BitMatrix"/>.
    /// <author>qwandor@google.com (Andrew Walbran)</author>
    /// </summary>
    public class UPCAWriter : Writer
    {
        private readonly EAN13Writer subWriter = new EAN13Writer();

        /// <summary>
        /// Gets the default margin.
        /// </summary>
        public int DefaultMargin
        {
            get
            {
                // CodaBar spec requires a side margin to be more than ten times wider than narrow space.
                // This seems like a decent idea for a default for all formats.
                return subWriter.DefaultMargin;
            }
            internal set
            {
                // mainly for test cases
                subWriter.DefaultMargin = value;
            }
        }

        /// <summary>
        /// Encode a barcode using the default settings.
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        public BitMatrix encode(String contents, BarcodeFormat format, int width, int height)
        {
            return encode(contents, format, width, height, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="hints">Additional parameters to supply to the encoder</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        public BitMatrix encode(String contents,
                                BarcodeFormat format,
                                int width,
                                int height,
                                IDictionary<EncodeHintType, object> hints)
        {
            if (format != BarcodeFormat.UPC_A)
            {
                throw new ArgumentException("Can only encode UPC-A, but got " + format);
            }
            int length = contents.Length;
            if (length != 11 && length != 12)
            {
                throw new ArgumentException("Requested contents should be 11 (without checksum digit) or 12 digits long, but got " + length);
            }
            // Transform a UPC-A code into the equivalent EAN-13 code and write it that way
            return subWriter.encode('0' + contents, BarcodeFormat.EAN_13, width, height, hints);
        }
    }
}