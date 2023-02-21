/*
 * Copyright 2011 ZXing authors
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
    ///   <p>Encapsulates functionality and implementation that is common to one-dimensional barcodes.</p>
    ///   <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
    /// </summary>
    public abstract class OneDimensionalCodeWriter : Writer
    {
        private static readonly System.Text.RegularExpressions.Regex NUMERIC = new System.Text.RegularExpressions.Regex("[0-9]+");

        /// <summary>
        /// returns supported formats
        /// </summary>
        protected abstract IList<BarcodeFormat> SupportedWriteFormats { get; }

        /// <summary>
        /// Encode the contents to boolean array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// </summary>
        /// <param name="contents">barcode contents to encode</param>
        /// <returns>a <c>bool[]</c> of horizontal pixels (false = white, true = black)</returns>
        public abstract bool[] encode(String contents);

        /// <summary>
        /// Can be overwritten if the encode requires to read the hints map. Otherwise it defaults to {@code encode}.
        /// </summary>
        /// <param name="contents">barcode contents to encode</param>
        /// <param name="hints">encoding hints</param>
        /// <returns>a <c>bool[]</c> of horizontal pixels (false = white, true = black)</returns>
        protected virtual bool[] encode(String contents, IDictionary<EncodeHintType, object> hints)
        {
            return encode(contents);
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
        /// Encode the contents following specified format.
        /// {@code width} and {@code height} are required size. This method may return bigger size
        /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
        /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
        /// or {@code height}, {@code IllegalArgumentException} is thrown.
        /// </summary>
        public virtual BitMatrix encode(String contents,
            BarcodeFormat format,
            int width,
            int height,
            IDictionary<EncodeHintType, object> hints)
        {
            if (String.IsNullOrEmpty(contents))
            {
                throw new ArgumentException("Found empty contents");
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Negative size is not allowed. Input: "
                                            + width + 'x' + height);
            }
            var supportedFormats = SupportedWriteFormats;
            if (supportedFormats != null && !supportedFormats.Contains(format))
            {
#if WindowsCE || WINDOWS_PHONE70 || NET20 || NET35 || UNITY || PORTABLE
                var supportedFormatsArray = new string[supportedFormats.Count];
                for (var i = 0; i < supportedFormats.Count; i++)
                    supportedFormatsArray[i] = supportedFormats[i].ToString();
                throw new ArgumentException("Can only encode " + string.Join(", ", supportedFormatsArray) + ", but got " + format);
#else
                throw new ArgumentException("Can only encode " + string.Join(", ", supportedFormats) + ", but got " + format);
#endif
            }

            int sidesMargin = DefaultMargin;
            var noPadding = false;
            if (hints != null)
            {
                var sidesMarginInt = hints.ContainsKey(EncodeHintType.MARGIN) ? hints[EncodeHintType.MARGIN] : null;
                if (sidesMarginInt != null)
                {
                    sidesMargin = Convert.ToInt32(sidesMarginInt);
                }
                var noPaddingObj = hints.ContainsKey(EncodeHintType.NO_PADDING) ? hints[EncodeHintType.NO_PADDING] : null;
                if (noPaddingObj != null)
                {
                    bool.TryParse(noPaddingObj.ToString(), out noPadding);
                }
            }

            var code = encode(contents, hints);
            return renderResult(code, width, height, sidesMargin, noPadding);
        }

        /// <summary>
        /// </summary>
        /// <returns>a byte array of horizontal pixels (0 = white, 1 = black)</returns>
        private static BitMatrix renderResult(bool[] code, int width, int height, int sidesMargin, bool noPadding)
        {
            int inputWidth = code.Length;
            // Add quiet zone on both sides.
            int fullWidth = inputWidth + sidesMargin * 2;
            int outputWidth = Math.Max(width, fullWidth);
            int outputHeight = Math.Max(1, height);

            int multiple = outputWidth / fullWidth;
            int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;

            if (noPadding)
            {
                outputWidth = fullWidth * multiple;
                leftPadding = sidesMargin * multiple;
            }

            BitMatrix output = new BitMatrix(outputWidth, outputHeight);
            for (int inputX = 0, outputX = leftPadding; inputX < inputWidth; inputX++, outputX += multiple)
            {
                if (code[inputX])
                {
                    output.setRegion(outputX, 0, multiple, outputHeight);
                }
            }
            return output;
        }

        /// <summary>
        /// Throw ArgumentException if input contains characters other than digits 0-9.
        /// </summary>
        /// <param name="contents">string to check for numeric characters</param>
        /// <exception cref="ArgumentException">if input contains characters other than digits 0-9.</exception>
        protected static void checkNumeric(String contents)
        {
            if (!NUMERIC.Match(contents).Success)
            {
                throw new ArgumentException("Input should only contain digits 0-9");
            }
        }

        /// <summary>
        /// Appends the given pattern to the target array starting at pos.
        /// </summary>
        /// <param name="target">encode black/white pattern into this array</param>
        /// <param name="pos">position to start encoding at in <c>target</c></param>
        /// <param name="pattern">lengths of black/white runs to encode</param>
        /// <param name="startColor">starting color - false for white, true for black</param>
        /// <returns>the number of elements added to target.</returns>
        protected static int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
        {
            bool color = startColor;
            int numAdded = 0;
            foreach (int len in pattern)
            {
                for (int j = 0; j < len; j++)
                {
                    target[pos++] = color;
                }
                numAdded += len;
                color = !color; // flip color after each segment
            }
            return numAdded;
        }

        /// <summary>
        /// Gets the default margin.
        /// </summary>
        virtual public int DefaultMargin
        {
            get
            {
                // CodaBar spec requires a side margin to be more than ten times wider than narrow space.
                // This seems like a decent idea for a default for all formats.
                return 10;
            }
        }

        /// <summary>
        /// Calculates the checksum digit modulo10.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public static String CalculateChecksumDigitModulo10(String contents)
        {
            var oddsum = 0;
            var evensum = 0;

            for (var index = contents.Length - 1; index >= 0; index -= 2)
            {
                oddsum += (contents[index] - '0');
            }
            for (var index = contents.Length - 2; index >= 0; index -= 2)
            {
                evensum += (contents[index] - '0');
            }

            return contents + ((10 - ((oddsum * 3 + evensum) % 10)) % 10);
        }
    }
}
