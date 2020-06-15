/*
 * Copyright 2009 ZXing authors
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
    /// This object renders an EAN8 code as a <see cref="BitMatrix"/>.
    /// <author>aripollak@gmail.com (Ari Pollak)</author>
    /// </summary>
    public sealed class EAN8Writer : UPCEANWriter
    {
        private const int CODE_WIDTH = 3 + // start guard
            (7 * 4) + // left bars
            5 + // middle guard
            (7 * 4) + // right bars
            3; // end guard

        private static readonly IList<BarcodeFormat> supportedWriteFormats = new List<BarcodeFormat> { BarcodeFormat.EAN_8 };

        /// <summary>
        /// returns supported formats
        /// </summary>
        protected override IList<BarcodeFormat> SupportedWriteFormats
        {
            get { return supportedWriteFormats; }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// a byte array of horizontal pixels (false = white, true = black)
        /// </returns>
        public override bool[] encode(String contents)
        {
            int length = contents.Length;
            switch (length)
            {
                case 7:
                    // No check digit present, calculate it and add it
                    var check = UPCEANReader.getStandardUPCEANChecksum(contents);
                    if (check == null)
                    {
                        throw new ArgumentException("Checksum can't be calculated");
                    }
                    contents += check.Value;
                    break;
                case 8:
                    try
                    {
                        if (!UPCEANReader.checkStandardUPCEANChecksum(contents))
                        {
                            throw new ArgumentException("Contents do not pass checksum");
                        }
                    }
                    catch (FormatException ignored)
                    {
                        throw new ArgumentException("Illegal contents", ignored);
                    }
                    break;
                default:
                    throw new ArgumentException("Requested contents should be 7 (without checksum digit) or 8 digits long, but got " + contents.Length);
            }

            checkNumeric(contents);

            var result = new bool[CODE_WIDTH];
            int pos = 0;

            pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

            for (int i = 0; i <= 3; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, 1));
                pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], false);
            }

            pos += appendPattern(result, pos, UPCEANReader.MIDDLE_PATTERN, false);

            for (int i = 4; i <= 7; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, 1));
                pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], true);
            }
            appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

            return result;
        }
    }
}