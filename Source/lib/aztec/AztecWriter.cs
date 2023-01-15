/*
 * Copyright 2013 ZXing authors
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
using System.Text;

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec
{
    /// <summary>
    /// Renders an Aztec code as a <see cref="BitMatrix" />
    /// </summary>
    public sealed class AztecWriter : Writer
    {
        /// <summary>
        /// default character set
        /// </summary>
        public static readonly Encoding DEFAULT_CHARSET;

        static AztecWriter()
        {
#if !(WindowsCE || SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE)
            DEFAULT_CHARSET = Encoding.GetEncoding(StringUtils.ISO88591);
#elif WindowsCE
         try
         {
            DEFAULT_CHARSET = Encoding.GetEncoding(StringUtils.ISO88591);
         }
         catch (PlatformNotSupportedException)
         {
            DEFAULT_CHARSET = Encoding.GetEncoding(1252);
         }
#else
         // not fully correct but what else
         DEFAULT_CHARSET = Encoding.GetEncoding(StringUtils.UTF8);
#endif
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
        public BitMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
        {
            var charset = IDictionaryExtensions.GetEncoding(hints, DEFAULT_CHARSET);

            var byteContent = charset.GetBytes(contents);
            return encode(byteContent, format, width, height, hints);
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
        public BitMatrix encode(byte[] contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
        {
            var charset = IDictionaryExtensions.GetEncoding(hints);

            return encode(contents, format, width, height, charset, hints);
        }

        /// <summary>
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="charset">Encoding of the byte array</param>
        /// <param name="hints">Additional parameters to supply to the encoder</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        private BitMatrix encode(byte[] contents, BarcodeFormat format, int width, int height, Encoding charset, IDictionary<EncodeHintType, object> hints)
        {
            var eccPercent = Internal.Encoder.DEFAULT_EC_PERCENT;
            var layers = Internal.Encoder.DEFAULT_AZTEC_LAYERS;
            var disableEci = false;
            var margin = 0;
            if (hints != null)
            {
                if (hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
                {
                    object eccPercentObject = hints[EncodeHintType.ERROR_CORRECTION];
                    if (eccPercentObject != null)
                    {
                        eccPercent = Convert.ToInt32(eccPercentObject);
                    }
                }
                if (hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
                {
                    object layersObject = hints[EncodeHintType.AZTEC_LAYERS];
                    if (layersObject != null)
                    {
                        layers = Convert.ToInt32(layersObject);
                    }
                }
                if (hints.ContainsKey(EncodeHintType.DISABLE_ECI))
                {
                    disableEci = (hints[EncodeHintType.DISABLE_ECI] != null && Convert.ToBoolean(hints[EncodeHintType.DISABLE_ECI].ToString()));
                }
                if (hints.ContainsKey(EncodeHintType.MARGIN))
                {
                    var marginInt = hints[EncodeHintType.MARGIN];
                    if (marginInt != null)
                    {
                        margin = Convert.ToInt32(marginInt.ToString());
                    }
                }
            }

            return encode(contents,
                          format,
                          width,
                          height,
                          charset,
                          eccPercent,
                          layers,
                          disableEci,
                          margin);
        }

        private static BitMatrix encode(byte[] contents, BarcodeFormat format, int width, int height, Encoding charset, int eccPercent, int layers, bool disableEci, int margin)
        {
            // charset stays here for later use in ECI segment

            if (format != BarcodeFormat.AZTEC)
            {
                throw new ArgumentException("Can only encode AZTEC code, but got " + format);
            }

            var aztec = Internal.Encoder.encode(contents, eccPercent, layers, charset, disableEci);
            return renderResult(aztec, width, height, margin);
        }

        private static BitMatrix renderResult(AztecCode code, int width, int height, int margin)
        {
            var input = code.Matrix;
            if (input == null)
            {
                throw new InvalidOperationException("No input code matrix");
            }

            int inputWidth = input.Width;
            int inputHeight = input.Height;
            int aztecWidth = inputWidth + (margin << 1);
            int aztecHeight = inputHeight + (margin << 1);
            int outputWidth = Math.Max(width, aztecWidth);
            int outputHeight = Math.Max(height, aztecHeight);

            int multiple = Math.Min(outputWidth / aztecWidth, outputHeight / aztecHeight);
            int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;
            int topPadding = (outputHeight - (inputHeight * multiple)) / 2;

            var output = new BitMatrix(outputWidth, outputHeight);

            for (int inputY = 0, outputY = topPadding; inputY < inputHeight; inputY++, outputY += multiple)
            {
                // Write the contents of this row of the barcode
                for (int inputX = 0, outputX = leftPadding; inputX < inputWidth; inputX++, outputX += multiple)
                {
                    if (input[inputX, inputY])
                    {
                        output.setRegion(outputX, outputY, multiple, multiple);
                    }
                }
            }

            return output;
        }
    }
}