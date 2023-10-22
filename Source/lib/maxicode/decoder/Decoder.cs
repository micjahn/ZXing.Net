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
using ZXing.Common.ReedSolomon;

namespace ZXing.Maxicode.Internal
{
    /// <summary>
    /// <p>The main class which implements MaxiCode decoding -- as opposed to locating and extracting
    /// the MaxiCode from an image.</p>
    ///
    /// <author>Manuel Kasten</author>
    /// </summary>
    public sealed class Decoder
    {
        private const int ALL = 0;
        private const int EVEN = 1;
        private const int ODD = 2;

        private readonly ReedSolomonDecoder rsDecoder;

        /// <summary>
        /// constructor
        /// </summary>
        public Decoder()
        {
            rsDecoder = new ReedSolomonDecoder(GenericGF.MAXICODE_FIELD_64);
        }
        /// <summary>
        /// decode the bits
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public DecoderResult decode(BitMatrix bits)
        {
            return decode(bits, null);
        }
        /// <summary>
        /// decode the bits
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public DecoderResult decode(BitMatrix bits,
                                    IDictionary<DecodeHintType, object> hints)
        {
            BitMatrixParser parser = new BitMatrixParser(bits);
            byte[] codewords = parser.readCodewords();

            int errorsCorrected = 0;
            if (!correctErrors(codewords, 0, 10, 10, ALL, out errorsCorrected))
                return null;

            int mode = codewords[0] & 0x0F;
            byte[] datawords;
            int errorsCorrectedLastRun = 0;
            switch (mode)
            {
                case 2:
                case 3:
                case 4:
                    if (!correctErrors(codewords, 20, 84, 40, EVEN, out errorsCorrectedLastRun))
                        return null;
                    errorsCorrected += errorsCorrectedLastRun;
                    if (!correctErrors(codewords, 20, 84, 40, ODD, out errorsCorrectedLastRun))
                        return null;
                    errorsCorrected += errorsCorrectedLastRun;
                    datawords = new byte[94];
                    break;
                case 5:
                    if (!correctErrors(codewords, 20, 68, 56, EVEN, out errorsCorrectedLastRun))
                        return null;
                    errorsCorrected += errorsCorrectedLastRun;
                    if (!correctErrors(codewords, 20, 68, 56, ODD, out errorsCorrectedLastRun))
                        return null;
                    errorsCorrected += errorsCorrectedLastRun;
                    datawords = new byte[78];
                    break;
                default:
                    return null;
            }

            Array.Copy(codewords, 0, datawords, 0, 10);
            Array.Copy(codewords, 20, datawords, 10, datawords.Length - 10);

            var result = DecodedBitStreamParser.decode(datawords, mode);
            result.ErrorsCorrected = errorsCorrected;
            return result;
        }

        private bool correctErrors(byte[] codewordBytes,
                                   int start,
                                   int dataCodewords,
                                   int ecCodewords,
                                   int mode,
                                   out int errorsCorrected)
        {
            int codewords = dataCodewords + ecCodewords;

            // in EVEN or ODD mode only half the codewords
            int divisor = mode == ALL ? 1 : 2;

            // First read into an array of ints
            int[] codewordsInts = new int[codewords / divisor];
            for (int i = 0; i < codewords; i++)
            {
                if ((mode == ALL) || (i % 2 == (mode - 1)))
                {
                    codewordsInts[i / divisor] = codewordBytes[i + start] & 0xFF;
                }
            }

            if (!rsDecoder.decodeWithECCount(codewordsInts, ecCodewords / divisor, out errorsCorrected))
                return false;

            // Copy back into array of bytes -- only need to worry about the bytes that were data
            // We don't care about errors in the error-correction codewords
            for (int i = 0; i < dataCodewords; i++)
            {
                if ((mode == ALL) || (i % 2 == (mode - 1)))
                {
                    codewordBytes[i + start] = (byte)codewordsInts[i / divisor];
                }
            }

            return true;
        }
    }
}