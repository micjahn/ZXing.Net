/*
 * Copyright 2008 ZXing authors
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

using System.Text;
using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// <p>Implements decoding of the EAN-13 format.</p>
    ///
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    /// <author>Sean Owen</author>
    /// <author>alasdair@google.com (Alasdair Mackintosh)</author>
    /// </summary>
    public sealed class EAN13Reader : UPCEANReader
    {
        // For an EAN-13 barcode, the first digit is represented by the parities used
        // to encode the next six digits, according to the table below. For example,
        // if the barcode is 5 123456 789012 then the value of the first digit is
        // signified by using odd for '1', even for '2', even for '3', odd for '4',
        // odd for '5', and even for '6'. See http://en.wikipedia.org/wiki/EAN-13
        //
        //                Parity of next 6 digits
        //    Digit   0     1     2     3     4     5
        //       0    Odd   Odd   Odd   Odd   Odd   Odd
        //       1    Odd   Odd   Even  Odd   Even  Even
        //       2    Odd   Odd   Even  Even  Odd   Even
        //       3    Odd   Odd   Even  Even  Even  Odd
        //       4    Odd   Even  Odd   Odd   Even  Even
        //       5    Odd   Even  Even  Odd   Odd   Even
        //       6    Odd   Even  Even  Even  Odd   Odd
        //       7    Odd   Even  Odd   Even  Odd   Even
        //       8    Odd   Even  Odd   Even  Even  Odd
        //       9    Odd   Even  Even  Odd   Even  Odd
        //
        // Note that the encoding for '0' uses the same parity as a UPC barcode. Hence
        // a UPC barcode can be converted to an EAN-13 barcode by prepending a 0.
        //
        // The encoding is represented by the following array, which is a bit pattern
        // using Odd = 0 and Even = 1. For example, 5 is represented by:
        //
        //              Odd Even Even Odd Odd Even
        // in binary:
        //                0    1    1   0   0    1   == 0x19
        //
        internal static int[] FIRST_DIGIT_ENCODINGS = {
                                                      0x00, 0x0B, 0x0D, 0xE, 0x13, 0x19, 0x1C, 0x15, 0x16, 0x1A
                                                   };

        private readonly int[] decodeMiddleCounters;

        /// <summary>
        /// Initializes a new instance of the <see cref="EAN13Reader"/> class.
        /// </summary>
        public EAN13Reader()
        {
            decodeMiddleCounters = new int[4];
        }

        /// <summary>
        /// Subclasses override this to decode the portion of a barcode between the start
        /// and end guard patterns.
        /// </summary>
        /// <param name="row">row of black/white values to search</param>
        /// <param name="startRange">start/end offset of start guard pattern</param>
        /// <param name="resultString"><see cref="StringBuilder"/>to append decoded chars to</param>
        /// <returns>
        /// horizontal offset of first pixel after the "middle" that was decoded or -1 if decoding could not complete successfully
        /// </returns>
        override protected internal int decodeMiddle(BitArray row,
                                   int[] startRange,
                                   StringBuilder resultString)
        {
            int[] counters = decodeMiddleCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;
            int end = row.Size;
            int rowOffset = startRange[1];

            int lgPatternFound = 0;

            for (int x = 0; x < 6 && rowOffset < end; x++)
            {
                int bestMatch;
                if (!decodeDigit(row, counters, rowOffset, L_AND_G_PATTERNS, out bestMatch))
                    return -1;
                resultString.Append((char)('0' + bestMatch % 10));
                foreach (int counter in counters)
                {
                    rowOffset += counter;
                }
                if (bestMatch >= 10)
                {
                    lgPatternFound |= 1 << (5 - x);
                }
            }

            if (!determineFirstDigit(resultString, lgPatternFound))
                return -1;

            int[] middleRange = findGuardPattern(row, rowOffset, true, MIDDLE_PATTERN);
            if (middleRange == null)
                return -1;
            rowOffset = middleRange[1];

            for (int x = 0; x < 6 && rowOffset < end; x++)
            {
                int bestMatch;
                if (!decodeDigit(row, counters, rowOffset, L_PATTERNS, out bestMatch))
                    return -1;
                resultString.Append((char)('0' + bestMatch));
                foreach (int counter in counters)
                {
                    rowOffset += counter;
                }
            }

            return rowOffset;
        }

        /// <summary>
        /// Get the format of this decoder.
        /// <returns>The 1D format.</returns>
        /// </summary>
        override internal BarcodeFormat BarcodeFormat
        {
            get { return BarcodeFormat.EAN_13; }
        }

        /// <summary>
        /// Based on pattern of odd-even ('L' and 'G') patterns used to encoded the explicitly-encoded
        /// digits in a barcode, determines the implicitly encoded first digit and adds it to the
        /// result string.
        /// </summary>
        /// <param name="resultString">string to insert decoded first digit into</param>
        /// <param name="lgPatternFound">int whose bits indicates the pattern of odd/even L/G patterns used to</param>
        ///  encode digits
        /// <return>-1 if first digit cannot be determined</return>
        private static bool determineFirstDigit(StringBuilder resultString, int lgPatternFound)
        {
            for (int d = 0; d < 10; d++)
            {
                if (lgPatternFound == FIRST_DIGIT_ENCODINGS[d])
                {
                    resultString.Insert(0, new[] { (char)('0' + d) });
                    return true;
                }
            }
            return false;
        }
    }
}
