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

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Guenther Grau</author>
    /// <author>creatale GmbH (christoph.schulz@creatale.de)</author>
    public static class PDF417CodewordDecoder
    {
        /// <summary>
        /// The ratios table
        /// </summary>
        private static readonly float[][] RATIOS_TABLE; // = new float[PDF417Common.SYMBOL_TABLE.Length][PDF417Common.BARS_IN_MODULE];

        /// <summary>
        /// Initializes the <see cref="ZXing.PDF417.Internal.PDF417CodewordDecoder"/> class &amp; Pre-computes the symbol ratio table.
        /// </summary>
        static PDF417CodewordDecoder()
        {
            // Jagged arrays in Java assign the memory automatically, but C# has no equivalent. (Jon Skeet says so!)
            // http://stackoverflow.com/a/5313879/266252
            RATIOS_TABLE = new float[PDF417Common.SYMBOL_TABLE.Length][];
            for (int s = 0; s < RATIOS_TABLE.Length; s++)
            {
                RATIOS_TABLE[s] = new float[PDF417Common.BARS_IN_MODULE];
            }

            // Pre-computes the symbol ratio table.
            for (int i = 0; i < PDF417Common.SYMBOL_TABLE.Length; i++)
            {
                int currentSymbol = PDF417Common.SYMBOL_TABLE[i];
                int currentBit = currentSymbol & 0x1;
                for (int j = 0; j < PDF417Common.BARS_IN_MODULE; j++)
                {
                    float size = 0.0f;
                    while ((currentSymbol & 0x1) == currentBit)
                    {
                        size += 1.0f;
                        currentSymbol >>= 1;
                    }
                    currentBit = currentSymbol & 0x1;
                    RATIOS_TABLE[i][PDF417Common.BARS_IN_MODULE - j - 1] = size / PDF417Common.MODULES_IN_CODEWORD;
                }
            }
        }

        /// <summary>
        /// Gets the decoded value.
        /// </summary>
        /// <returns>The decoded value.</returns>
        /// <param name="moduleBitCount">Module bit count.</param>
        public static int getDecodedValue(int[] moduleBitCount)
        {
            int decodedValue = getDecodedCodewordValue(sampleBitCounts(moduleBitCount));
            if (decodedValue != PDF417Common.INVALID_CODEWORD)
            {
                return decodedValue;
            }
            return getClosestDecodedValue(moduleBitCount);
        }

        /// <summary>
        /// Samples the bit counts.
        /// </summary>
        /// <returns>The bit counts.</returns>
        /// <param name="moduleBitCount">Module bit count.</param>
        private static int[] sampleBitCounts(int[] moduleBitCount)
        {
            float bitCountSum = ZXing.Common.Detector.MathUtils.sum(moduleBitCount);
            int[] result = new int[PDF417Common.BARS_IN_MODULE];
            int bitCountIndex = 0;
            int sumPreviousBits = 0;
            for (int i = 0; i < PDF417Common.MODULES_IN_CODEWORD; i++)
            {
                float sampleIndex =
                   bitCountSum / (2 * PDF417Common.MODULES_IN_CODEWORD) +
                   (i * bitCountSum) / PDF417Common.MODULES_IN_CODEWORD;
                if (sumPreviousBits + moduleBitCount[bitCountIndex] <= sampleIndex)
                {
                    sumPreviousBits += moduleBitCount[bitCountIndex];
                    bitCountIndex++;
                }
                result[bitCountIndex]++;
            }
            return result;
        }

        /// <summary>
        /// Gets the decoded codeword value.
        /// </summary>
        /// <returns>The decoded codeword value.</returns>
        /// <param name="moduleBitCount">Module bit count.</param>
        private static int getDecodedCodewordValue(int[] moduleBitCount)
        {
            int decodedValue = getBitValue(moduleBitCount);
            return PDF417Common.getCodeword(decodedValue) == PDF417Common.INVALID_CODEWORD ? PDF417Common.INVALID_CODEWORD : decodedValue;
        }

        /// <summary>
        /// Gets the bit value.
        /// </summary>
        /// <returns>The bit value.</returns>
        /// <param name="moduleBitCount">Module bit count.</param>
        private static int getBitValue(int[] moduleBitCount)
        {
            ulong result = 0;
            for (ulong i = 0; i < (ulong)moduleBitCount.Length; i++)
            {
                for (int bit = 0; bit < moduleBitCount[i]; bit++)
                {
                    result = (result << 1) | (i % 2ul == 0ul ? 1ul : 0ul); // C# was warning about using the bit-wise 'OR' here with a mix of int/longs.
                }
            }
            return (int)result;
        }

        /// <summary>
        /// Gets the closest decoded value.
        /// </summary>
        /// <returns>The closest decoded value.</returns>
        /// <param name="moduleBitCount">Module bit count.</param>
        private static int getClosestDecodedValue(int[] moduleBitCount)
        {
            int bitCountSum = ZXing.Common.Detector.MathUtils.sum(moduleBitCount);
            float[] bitCountRatios = new float[PDF417Common.BARS_IN_MODULE];
            if (bitCountSum > 1)
            {
                for (int i = 0; i < bitCountRatios.Length; i++)
                {
                    bitCountRatios[i] = moduleBitCount[i] / (float)bitCountSum;
                }
            }
            float bestMatchError = float.MaxValue;
            int bestMatch = PDF417Common.INVALID_CODEWORD;
            for (int j = 0; j < RATIOS_TABLE.Length; j++)
            {
                float error = 0.0f;
                float[] ratioTableRow = RATIOS_TABLE[j];
                for (int k = 0; k < PDF417Common.BARS_IN_MODULE; k++)
                {
                    float diff = ratioTableRow[k] - bitCountRatios[k];
                    error += diff * diff;
                    if (error >= bestMatchError)
                    {
                        break;
                    }
                }
                if (error < bestMatchError)
                {
                    bestMatchError = error;
                    bestMatch = PDF417Common.SYMBOL_TABLE[j];
                }
            }
            return bestMatch;
        }
    }
}