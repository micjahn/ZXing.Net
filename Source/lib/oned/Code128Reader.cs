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

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// <p>Decodes Code 128 barcodes.</p>
    ///
    /// <author>Sean Owen</author>
    /// </summary>
    public sealed class Code128Reader : OneDReader
    {
        internal static int[][] CODE_PATTERNS = {
                                                new[] {2, 1, 2, 2, 2, 2}, // 0
                                                new[] {2, 2, 2, 1, 2, 2},
                                                new[] {2, 2, 2, 2, 2, 1},
                                                new[] {1, 2, 1, 2, 2, 3},
                                                new[] {1, 2, 1, 3, 2, 2},
                                                new[] {1, 3, 1, 2, 2, 2}, // 5
                                                new[] {1, 2, 2, 2, 1, 3},
                                                new[] {1, 2, 2, 3, 1, 2},
                                                new[] {1, 3, 2, 2, 1, 2},
                                                new[] {2, 2, 1, 2, 1, 3},
                                                new[] {2, 2, 1, 3, 1, 2}, // 10
                                                new[] {2, 3, 1, 2, 1, 2},
                                                new[] {1, 1, 2, 2, 3, 2},
                                                new[] {1, 2, 2, 1, 3, 2},
                                                new[] {1, 2, 2, 2, 3, 1},
                                                new[] {1, 1, 3, 2, 2, 2}, // 15
                                                new[] {1, 2, 3, 1, 2, 2},
                                                new[] {1, 2, 3, 2, 2, 1},
                                                new[] {2, 2, 3, 2, 1, 1},
                                                new[] {2, 2, 1, 1, 3, 2},
                                                new[] {2, 2, 1, 2, 3, 1}, // 20
                                                new[] {2, 1, 3, 2, 1, 2},
                                                new[] {2, 2, 3, 1, 1, 2},
                                                new[] {3, 1, 2, 1, 3, 1},
                                                new[] {3, 1, 1, 2, 2, 2},
                                                new[] {3, 2, 1, 1, 2, 2}, // 25
                                                new[] {3, 2, 1, 2, 2, 1},
                                                new[] {3, 1, 2, 2, 1, 2},
                                                new[] {3, 2, 2, 1, 1, 2},
                                                new[] {3, 2, 2, 2, 1, 1},
                                                new[] {2, 1, 2, 1, 2, 3}, // 30
                                                new[] {2, 1, 2, 3, 2, 1},
                                                new[] {2, 3, 2, 1, 2, 1},
                                                new[] {1, 1, 1, 3, 2, 3},
                                                new[] {1, 3, 1, 1, 2, 3},
                                                new[] {1, 3, 1, 3, 2, 1}, // 35
                                                new[] {1, 1, 2, 3, 1, 3},
                                                new[] {1, 3, 2, 1, 1, 3},
                                                new[] {1, 3, 2, 3, 1, 1},
                                                new[] {2, 1, 1, 3, 1, 3},
                                                new[] {2, 3, 1, 1, 1, 3}, // 40
                                                new[] {2, 3, 1, 3, 1, 1},
                                                new[] {1, 1, 2, 1, 3, 3},
                                                new[] {1, 1, 2, 3, 3, 1},
                                                new[] {1, 3, 2, 1, 3, 1},
                                                new[] {1, 1, 3, 1, 2, 3}, // 45
                                                new[] {1, 1, 3, 3, 2, 1},
                                                new[] {1, 3, 3, 1, 2, 1},
                                                new[] {3, 1, 3, 1, 2, 1},
                                                new[] {2, 1, 1, 3, 3, 1},
                                                new[] {2, 3, 1, 1, 3, 1}, // 50
                                                new[] {2, 1, 3, 1, 1, 3},
                                                new[] {2, 1, 3, 3, 1, 1},
                                                new[] {2, 1, 3, 1, 3, 1},
                                                new[] {3, 1, 1, 1, 2, 3},
                                                new[] {3, 1, 1, 3, 2, 1}, // 55
                                                new[] {3, 3, 1, 1, 2, 1},
                                                new[] {3, 1, 2, 1, 1, 3},
                                                new[] {3, 1, 2, 3, 1, 1},
                                                new[] {3, 3, 2, 1, 1, 1},
                                                new[] {3, 1, 4, 1, 1, 1}, // 60
                                                new[] {2, 2, 1, 4, 1, 1},
                                                new[] {4, 3, 1, 1, 1, 1},
                                                new[] {1, 1, 1, 2, 2, 4},
                                                new[] {1, 1, 1, 4, 2, 2},
                                                new[] {1, 2, 1, 1, 2, 4}, // 65
                                                new[] {1, 2, 1, 4, 2, 1},
                                                new[] {1, 4, 1, 1, 2, 2},
                                                new[] {1, 4, 1, 2, 2, 1},
                                                new[] {1, 1, 2, 2, 1, 4},
                                                new[] {1, 1, 2, 4, 1, 2}, // 70
                                                new[] {1, 2, 2, 1, 1, 4},
                                                new[] {1, 2, 2, 4, 1, 1},
                                                new[] {1, 4, 2, 1, 1, 2},
                                                new[] {1, 4, 2, 2, 1, 1},
                                                new[] {2, 4, 1, 2, 1, 1}, // 75
                                                new[] {2, 2, 1, 1, 1, 4},
                                                new[] {4, 1, 3, 1, 1, 1},
                                                new[] {2, 4, 1, 1, 1, 2},
                                                new[] {1, 3, 4, 1, 1, 1},
                                                new[] {1, 1, 1, 2, 4, 2}, // 80
                                                new[] {1, 2, 1, 1, 4, 2},
                                                new[] {1, 2, 1, 2, 4, 1},
                                                new[] {1, 1, 4, 2, 1, 2},
                                                new[] {1, 2, 4, 1, 1, 2},
                                                new[] {1, 2, 4, 2, 1, 1}, // 85
                                                new[] {4, 1, 1, 2, 1, 2},
                                                new[] {4, 2, 1, 1, 1, 2},
                                                new[] {4, 2, 1, 2, 1, 1},
                                                new[] {2, 1, 2, 1, 4, 1},
                                                new[] {2, 1, 4, 1, 2, 1}, // 90
                                                new[] {4, 1, 2, 1, 2, 1},
                                                new[] {1, 1, 1, 1, 4, 3},
                                                new[] {1, 1, 1, 3, 4, 1},
                                                new[] {1, 3, 1, 1, 4, 1},
                                                new[] {1, 1, 4, 1, 1, 3}, // 95
                                                new[] {1, 1, 4, 3, 1, 1},
                                                new[] {4, 1, 1, 1, 1, 3},
                                                new[] {4, 1, 1, 3, 1, 1},
                                                new[] {1, 1, 3, 1, 4, 1},
                                                new[] {1, 1, 4, 1, 3, 1}, // 100
                                                new[] {3, 1, 1, 1, 4, 1},
                                                new[] {4, 1, 1, 1, 3, 1},
                                                new[] {2, 1, 1, 4, 1, 2},
                                                new[] {2, 1, 1, 2, 1, 4},
                                                new[] {2, 1, 1, 2, 3, 2}, // 105
                                                new[] {2, 3, 3, 1, 1, 1, 2}
                                             };

        internal static int[][] EDGE_DIFFERENCES_PATTERNS = {
                                                new[] {3, 3, 4, 4, 6},  // 0
                                                new[] {4, 4, 3, 3, 6},
                                                new[] {4, 4, 4, 4, 6},
                                                new[] {3, 3, 3, 4, 4},
                                                new[] {3, 3, 4, 5, 4},
                                                new[] {4, 4, 3, 4, 4},  // 5
                                                new[] {3, 4, 4, 3, 4},
                                                new[] {3, 4, 5, 4, 4},
                                                new[] {4, 5, 4, 3, 4},
                                                new[] {4, 3, 3, 3, 4},
                                                new[] {4, 3, 4, 4, 4},  // 10
                                                new[] {5, 4, 3, 3, 4},
                                                new[] {2, 3, 4, 5, 6},
                                                new[] {3, 4, 3, 4, 6},
                                                new[] {3, 4, 4, 5, 6},
                                                new[] {2, 4, 5, 4, 6},  // 15
                                                new[] {3, 5, 4, 3, 6},
                                                new[] {3, 5, 5, 4, 6},
                                                new[] {4, 5, 5, 3, 6},
                                                new[] {4, 3, 2, 4, 6},
                                                new[] {4, 3, 3, 5, 6},  // 20
                                                new[] {3, 4, 5, 3, 6},
                                                new[] {4, 5, 4, 2, 6},
                                                new[] {4, 3, 3, 4, 8},
                                                new[] {4, 2, 3, 4, 6},
                                                new[] {5, 3, 2, 3, 6},  // 25
                                                new[] {5, 3, 3, 4, 6},
                                                new[] {4, 3, 4, 3, 6},
                                                new[] {5, 4, 3, 2, 6},
                                                new[] {5, 4, 4, 3, 6},
                                                new[] {3, 3, 3, 3, 6},  // 30
                                                new[] {3, 3, 5, 5, 6},
                                                new[] {5, 5, 3, 3, 6},
                                                new[] {2, 2, 4, 5, 4},
                                                new[] {4, 4, 2, 3, 4},
                                                new[] {4, 4, 4, 5, 4},  // 35
                                                new[] {2, 3, 5, 4, 4},
                                                new[] {4, 5, 3, 2, 4},
                                                new[] {4, 5, 5, 4, 4},
                                                new[] {3, 2, 4, 4, 4},
                                                new[] {5, 4, 2, 2, 4},  // 40
                                                new[] {5, 4, 4, 4, 4},
                                                new[] {2, 3, 3, 4, 6},
                                                new[] {2, 3, 5, 6, 6},
                                                new[] {4, 5, 3, 4, 6},
                                                new[] {2, 4, 4, 3, 6},  // 45
                                                new[] {2, 4, 6, 5, 6},
                                                new[] {4, 6, 4, 3, 6},
                                                new[] {4, 4, 4, 3, 8},
                                                new[] {3, 2, 4, 6, 6},
                                                new[] {5, 4, 2, 4, 6},  // 50
                                                new[] {3, 4, 4, 2, 6},
                                                new[] {3, 4, 6, 4, 6},
                                                new[] {3, 4, 4, 4, 8},
                                                new[] {4, 2, 2, 3, 6},
                                                new[] {4, 2, 4, 5, 6},  // 55
                                                new[] {6, 4, 2, 3, 6},
                                                new[] {4, 3, 3, 2, 6},
                                                new[] {4, 3, 5, 4, 6},
                                                new[] {6, 5, 3, 2, 6},
                                                new[] {4, 5, 5, 2, 8},  // 60
                                                new[] {4, 3, 5, 5, 4},
                                                new[] {7, 4, 2, 2, 6},
                                                new[] {2, 2, 3, 4, 4},
                                                new[] {2, 2, 5, 6, 4},
                                                new[] {3, 3, 2, 3, 4},  // 65
                                                new[] {3, 3, 5, 6, 4},
                                                new[] {5, 5, 2, 3, 4},
                                                new[] {5, 5, 3, 4, 4},
                                                new[] {2, 3, 4, 3, 4},
                                                new[] {2, 3, 6, 5, 4},  // 70
                                                new[] {3, 4, 3, 2, 4},
                                                new[] {3, 4, 6, 5, 4},
                                                new[] {5, 6, 3, 2, 4},
                                                new[] {5, 6, 4, 3, 4},
                                                new[] {6, 5, 3, 3, 4},  // 75
                                                new[] {4, 3, 2, 2, 4},
                                                new[] {5, 4, 4, 2, 8},
                                                new[] {6, 5, 2, 2, 4},
                                                new[] {4, 7, 5, 2, 6},
                                                new[] {2, 2, 3, 6, 6},  // 80
                                                new[] {3, 3, 2, 5, 6},
                                                new[] {3, 3, 3, 6, 6},
                                                new[] {2, 5, 6, 3, 6},
                                                new[] {3, 6, 5, 2, 6},
                                                new[] {3, 6, 6, 3, 6},  // 85
                                                new[] {5, 2, 3, 3, 6},
                                                new[] {6, 3, 2, 2, 6},
                                                new[] {6, 3, 3, 3, 6},
                                                new[] {3, 3, 3, 5, 8},
                                                new[] {3, 5, 5, 3, 8},  // 90
                                                new[] {5, 3, 3, 3, 8},
                                                new[] {2, 2, 2, 5, 6},
                                                new[] {2, 2, 4, 7, 6},
                                                new[] {4, 4, 2, 5, 6},
                                                new[] {2, 5, 5, 2, 6},  // 95
                                                new[] {2, 5, 7, 4, 6},
                                                new[] {5, 2, 2, 2, 6},
                                                new[] {5, 2, 4, 4, 6},
                                                new[] {2, 4, 4, 5, 8},
                                                new[] {2, 5, 5, 4, 8},  // 100
                                                new[] {4, 2, 2, 5, 8},
                                                new[] {5, 2, 2, 4, 8},
                                                new[] {3, 2, 5, 5, 4},
                                                new[] {3, 2, 3, 3, 4},
                                                new[] {3, 2, 3, 5, 6},  // 105
                                                new[] {5, 6, 4, 2, 6},
                                                new[] {3, 2, 2, 4, 6}
                                             };

        private static readonly int MAX_AVG_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.25f);
        private static readonly int MAX_INDIVIDUAL_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.7f);

        private const int CODE_SHIFT = 98;

        private const int CODE_CODE_C = 99;
        private const int CODE_CODE_B = 100;
        private const int CODE_CODE_A = 101;

        private const int CODE_FNC_1 = 102;
        private const int CODE_FNC_2 = 97;
        private const int CODE_FNC_3 = 96;
        private const int CODE_FNC_4_A = 101;
        private const int CODE_FNC_4_B = 100;

        private const int CODE_START_A = 103;
        private const int CODE_START_B = 104;
        private const int CODE_START_C = 105;
        private const int CODE_STOP = 106;

        private static int[] findStartPattern(BitArray row)
        {
            int blop = 0;

            int width = row.Size;
            int rowOffset = row.getNextSet(0);

            int counterPosition = 0;
            int[] counters = new int[6];
            int patternStart = rowOffset;
            bool isWhite = false;
            int patternLength = counters.Length;

            for (int i = rowOffset; i < width; i++)
            {
                blop = row.Size;

                if (row[i] != isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        int edgeDifferences_id = getEdgeDifferences(counters);

                        if ((edgeDifferences_id == CODE_START_A) || (edgeDifferences_id == CODE_START_B) || (edgeDifferences_id == CODE_START_C))
                        {
                            return new int[] { patternStart, i, edgeDifferences_id };
                        }
                        
                        patternStart += counters[0] + counters[1];
                        Array.Copy(counters, 2, counters, 0, counterPosition - 1);
                        counters[counterPosition - 1] = 0;
                        counters[counterPosition] = 0;
                        counterPosition--;
                    }
                    else
                    {
                        counterPosition++;
                    }
                    counters[counterPosition] = 1;
                    isWhite = !isWhite;
                }
            }
            return null;
        }

        private static bool decodeCode(BitArray row, int[] counters, int rowOffset, out int code)
        {
            code = -1;
            if (!recordPattern(row, rowOffset, counters))
                return false;

            int bestVariance = MAX_AVG_VARIANCE; // worst variance we'll accept
            for (int d = 0; d < CODE_PATTERNS.Length; d++)
            {
                int[] pattern = CODE_PATTERNS[d];
                int variance = patternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE);
                if (variance < bestVariance)
                {
                    bestVariance = variance;
                    code = d;
                }
            }
            // TODO We're overlooking the fact that the STOP pattern has 7 values, not 6.
            return code >= 0;
        }


        /// <summary>
        /// Determines how closely a set of observed counts of runs of black/white values matches a given
        /// target pattern. This is reported as the ratio of the total variance from the expected pattern
        /// proportions across all pattern elements, to the length of the pattern.
        /// </summary>
        /// <param name="counters">observed counters</param>
        /// <param name="pattern">expected pattern</param>
        /// <param name="maxIndividualVariance">The most any counter can differ before we give up</param>
        /// <returns>ratio of total variance between counters and pattern compared to total pattern size,
        ///  where the ratio has been multiplied by 256. So, 0 means no variance (perfect match); 256 means
        ///  the total variance between counters and patterns equals the pattern length, higher values mean
        ///  even more variance</returns>
        private static int getEdgeDifferences(int[] counters)        
        {
            int numCounters = counters.Length;
            int total = 0;
            
            int[] pattern = new int[5];

            // Get length of the patern 
            for (int i = 0; i < numCounters; i++)
            {
                total += counters[i];
            }

            // Set the calculated parten
            for (int i = 0; i < numCounters-2; i++)
            {
                int e_i = counters[i] + counters[i + 1];
                if ((1.5 * total / 11 <= e_i) &&  (e_i < 2.5 * total / 11))
                {
                    pattern[i] = 2;
                }
                else if ((2.5 * total / 11 <= e_i) && (e_i < 3.5 * total / 11))
                {
                    pattern[i] = 3;
                }
                else if ((3.5 * total / 11 <= e_i) && (e_i < 4.5 * total / 11))
                {
                    pattern[i] = 4;
                }
                else if ((4.5 * total / 11 <= e_i) && (e_i < 5.5 * total / 11))
                {
                    pattern[i] = 5;
                }
                else if ((5.5 * total / 11 <= e_i) && (e_i < 6.5 * total / 11))
                {
                    pattern[i] = 6;
                }
                else if ((6.5 * total / 11 <= e_i) && (e_i < 7.5 * total / 11))
                {
                    pattern[i] = 7;
                }
            }

            // Find the parttent in the edge differences table
            for (int d = 0; d < EDGE_DIFFERENCES_PATTERNS.Length; d++)
            {
                if(    pattern[0] == EDGE_DIFFERENCES_PATTERNS[d][0]
                    && pattern[1] == EDGE_DIFFERENCES_PATTERNS[d][1]
                    && pattern[2] == EDGE_DIFFERENCES_PATTERNS[d][2]
                    && pattern[3] == EDGE_DIFFERENCES_PATTERNS[d][3])

                {
                    // Pattern found, check its avalidity
                    int V = EDGE_DIFFERENCES_PATTERNS[d][4];

                    double minRange = ((V - 1.75) * total) / 11;
                    double maxRange = ((V + 1.75) * total) / 11;

                    int barLength = counters[0] + counters[2] + counters[4];

                    if ((minRange < barLength) && (barLength < maxRange))
                    {
                        return d;
                    }
                }
            }

            // Not found
            return -1;
        }

        private static bool decodeCode_ed(BitArray row, int[] counters, int rowOffset, out int code)
        {
            code = -1;

            if (!recordPattern(row, rowOffset, counters))
                return false;

            code = getEdgeDifferences(counters);

            if (code >= 0)
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
        /// an image.</p>
        /// </summary>
        /// <param name="rowNumber">row number from top of the row</param>
        /// <param name="row">the black/white pixel data of the row</param>
        /// <param name="hints">decode hints</param>
        /// <returns>
        ///   <see cref="Result"/>containing encoded string and start/end of barcode or null, if an error occurs or barcode cannot be found
        /// </returns>
        public override Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
        {
            bool convertFNC1 = hints != null && hints.ContainsKey(DecodeHintType.ASSUME_GS1);

            int symbologyModifier = 0;

            int[] startPatternInfo = findStartPattern(row);
            if (startPatternInfo == null)
                return null;
            int startCode = startPatternInfo[2];

            var rawCodes = new List<byte>(20);
            rawCodes.Add((byte)startCode);

            int codeSet;
            switch (startCode)
            {
                case CODE_START_A:
                    codeSet = CODE_CODE_A;
                    break;
                case CODE_START_B:
                    codeSet = CODE_CODE_B;
                    break;
                case CODE_START_C:
                    codeSet = CODE_CODE_C;
                    break;
                default:
                    return null;
            }

            bool done = false;
            bool isNextShifted = false;

            var result = new StringBuilder(20);

            int lastStart = startPatternInfo[0];
            int nextStart = startPatternInfo[1];
            int[] counters = new int[6];

            int lastCode = 0;
            int code = 0;
            int checksumTotal = startCode;
            int multiplier = 0;
            bool lastCharacterWasPrintable = true;
            bool upperMode = false;
            bool shiftUpperMode = false;

            while (!done)
            {
                bool unshift = isNextShifted;
                isNextShifted = false;

                // Save off last code
                lastCode = code;

                // Decode another code from image
                if (!decodeCode_ed(row, counters, nextStart, out code))
                {
                    if (!decodeCode(row, counters, nextStart, out code))
                    {
                        return null;
                    }
                }

                rawCodes.Add((byte)code);

                // Remember whether the last code was printable or not (excluding CODE_STOP)
                if (code != CODE_STOP)
                {
                    lastCharacterWasPrintable = true;
                }

                // Add to checksum computation (if not CODE_STOP of course)
                if (code != CODE_STOP)
                {
                    multiplier++;
                    checksumTotal += multiplier * code;
                }

                // Advance to where the next code will to start
                lastStart = nextStart;
                foreach (int counter in counters)
                {
                    nextStart += counter;
                }

                // Take care of illegal start codes
                switch (code)
                {
                    case CODE_START_A:
                    case CODE_START_B:
                    case CODE_START_C:
                        return null;
                }

                switch (codeSet)
                {

                    case CODE_CODE_A:
                        if (code < 64)
                        {
                            if (shiftUpperMode == upperMode)
                            {
                                result.Append((char)(' ' + code));
                            }
                            else
                            {
                                result.Append((char)(' ' + code + 128));
                            }
                            shiftUpperMode = false;
                        }
                        else if (code < 96)
                        {
                            if (shiftUpperMode == upperMode)
                            {
                                result.Append((char)(code - 64));
                            }
                            else
                            {
                                result.Append((char)(code + 64));
                            }
                            shiftUpperMode = false;
                        }
                        else
                        {
                            // Don't let CODE_STOP, which always appears, affect whether whether we think the last
                            // code was printable or not.
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                    if (result.Length == 0)
                                    {
                                        // FNC1 at first or second character determines the symbology
                                        symbologyModifier = 1;
                                    }
                                    else if (result.Length == 1)
                                    {
                                        symbologyModifier = 2;
                                    }
                                    if (convertFNC1)
                                    {
                                        if (result.Length == 0)
                                        {
                                            // GS1 specification 5.4.3.7. and 5.4.6.4. If the first char after the start code
                                            // is FNC1 then this is GS1-128. We add the symbology identifier.
                                            result.Append("]C1");
                                        }
                                        else
                                        {
                                            // GS1 specification 5.4.7.5. Every subsequent FNC1 is returned as ASCII 29 (GS)
                                            result.Append((char)29);
                                        }
                                    }
                                    break;
                                case CODE_FNC_2:
                                    symbologyModifier = 4;
                                    break;
                                case CODE_FNC_3:
                                    // do nothing?
                                    break;
                                case CODE_FNC_4_A:
                                    if (!upperMode && shiftUpperMode)
                                    {
                                        upperMode = true;
                                        shiftUpperMode = false;
                                    }
                                    else if (upperMode && shiftUpperMode)
                                    {
                                        upperMode = false;
                                        shiftUpperMode = false;
                                    }
                                    else
                                    {
                                        shiftUpperMode = true;
                                    }
                                    break;
                                case CODE_SHIFT:
                                    isNextShifted = true;
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_CODE_B:
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_CODE_C:
                                    codeSet = CODE_CODE_C;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                    case CODE_CODE_B:
                        if (code < 96)
                        {
                            if (shiftUpperMode == upperMode)
                            {
                                result.Append((char)(' ' + code));
                            }
                            else
                            {
                                result.Append((char)(' ' + code + 128));
                            }
                            shiftUpperMode = false;
                        }
                        else
                        {
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                    if (result.Length == 0)
                                    {
                                        // FNC1 at first or second character determines the symbology
                                        symbologyModifier = 1;
                                    }
                                    else if (result.Length == 1)
                                    {
                                        symbologyModifier = 2;
                                    }
                                    if (convertFNC1)
                                    {
                                        if (result.Length == 0)
                                        {
                                            // GS1 specification 5.4.3.7. and 5.4.6.4. If the first char after the start code
                                            // is FNC1 then this is GS1-128. We add the symbology identifier.
                                            result.Append("]C1");
                                        }
                                        else
                                        {
                                            // GS1 specification 5.4.7.5. Every subsequent FNC1 is returned as ASCII 29 (GS)
                                            result.Append((char)29);
                                        }
                                    }
                                    break;
                                case CODE_FNC_2:
                                    symbologyModifier = 4;
                                    break;
                                case CODE_FNC_3:
                                    // do nothing?
                                    break;
                                case CODE_FNC_4_B:
                                    if (!upperMode && shiftUpperMode)
                                    {
                                        upperMode = true;
                                        shiftUpperMode = false;
                                    }
                                    else if (upperMode && shiftUpperMode)
                                    {
                                        upperMode = false;
                                        shiftUpperMode = false;
                                    }
                                    else
                                    {
                                        shiftUpperMode = true;
                                    }
                                    break;
                                case CODE_SHIFT:
                                    isNextShifted = true;
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_A:
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_C:
                                    codeSet = CODE_CODE_C;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                    case CODE_CODE_C:
                        if (code < 100)
                        {
                            if (code < 10)
                            {
                                result.Append('0');
                            }
                            result.Append(code);
                        }
                        else
                        {
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                    if (result.Length == 0)
                                    {
                                        // FNC1 at first or second character determines the symbology
                                        symbologyModifier = 1;
                                    }
                                    else if (result.Length == 1)
                                    {
                                        symbologyModifier = 2;
                                    }
                                    if (convertFNC1)
                                    {
                                        if (result.Length == 0)
                                        {
                                            // GS1 specification 5.4.3.7. and 5.4.6.4. If the first char after the start code
                                            // is FNC1 then this is GS1-128. We add the symbology identifier.
                                            result.Append("]C1");
                                        }
                                        else
                                        {
                                            // GS1 specification 5.4.7.5. Every subsequent FNC1 is returned as ASCII 29 (GS)
                                            result.Append((char)29);
                                        }
                                    }
                                    break;
                                case CODE_CODE_A:
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_B:
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                }

                // Unshift back to another code set if we were shifted
                if (unshift)
                {
                    codeSet = codeSet == CODE_CODE_A ? CODE_CODE_B : CODE_CODE_A;
                }

            }

            int lastPatternSize = nextStart - lastStart;

            // Check for ample whitespace following pattern, but, to do this we first need to remember that
            // we fudged decoding CODE_STOP since it actually has 7 bars, not 6. There is a black bar left
            // to read off. Would be slightly better to properly read. Here we just skip it:
            nextStart = row.getNextUnset(nextStart);
            if (!row.isRange(nextStart,
                             Math.Min(row.Size, nextStart + (nextStart - lastStart) / 2),
                             false))
            {
                return null;
            }

            // Pull out from sum the value of the penultimate check code
            checksumTotal -= multiplier * lastCode;
            // lastCode is the checksum then:
            if (checksumTotal % 103 != lastCode)
            {
                return null;
            }

            // Need to pull out the check digits from string
            int resultLength = result.Length;
            if (resultLength == 0)
            {
                // false positive
                return null;
            }

            // Only bother if the result had at least one character, and if the checksum digit happened to
            // be a printable character. If it was just interpreted as a control code, nothing to remove.
            if (resultLength > 0 && lastCharacterWasPrintable)
            {
                if (codeSet == CODE_CODE_C)
                {
                    result.Remove(resultLength - 2, 2);
                }
                else
                {
                    result.Remove(resultLength - 1, 1);
                }
            }

            float left = (startPatternInfo[1] + startPatternInfo[0]) / 2.0f;
            float right = lastStart + lastPatternSize / 2.0f;

            var resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                                ? null
                                : (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
            if (resultPointCallback != null)
            {
                resultPointCallback(new ResultPoint(left, rowNumber));
                resultPointCallback(new ResultPoint(right, rowNumber));
            }

            int rawCodesSize = rawCodes.Count;
            var rawBytes = new byte[rawCodesSize];
            for (int i = 0; i < rawCodesSize; i++)
            {
                rawBytes[i] = rawCodes[i];
            }

            var resultObject = new Result(
               result.ToString(),
               rawBytes,
               new[]
                  {
                  new ResultPoint(left, rowNumber),
                  new ResultPoint(right, rowNumber)
                  },
               BarcodeFormat.CODE_128);
            resultObject.putMetadata(ResultMetadataType.SYMBOLOGY_IDENTIFIER, "]C" + symbologyModifier);
            return resultObject;
        }
    }
}
