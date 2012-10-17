/*
 * Copyright (C) 2010 ZXing authors
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

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;
using System.Collections.Generic;

using ZXing.Common;
using ZXing.OneD.RSS.Expanded.Decoders;

namespace ZXing.OneD.RSS.Expanded
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   public sealed class RSSExpandedReader : AbstractRSSReader
   {
      private static readonly int[] SYMBOL_WIDEST = { 7, 5, 4, 3, 1 };
      private static readonly int[] EVEN_TOTAL_SUBSET = { 4, 20, 52, 104, 204 };
      private static readonly int[] GSUM = { 0, 348, 1388, 2948, 3988 };

      private static readonly int[][] FINDER_PATTERNS = {
                                                  new[] {1, 8, 4, 1}, // A
                                                  new[] {3, 6, 4, 1}, // B
                                                  new[] {3, 4, 6, 1}, // C
                                                  new[] {3, 2, 8, 1}, // D
                                                  new[] {2, 6, 5, 1}, // E
                                                  new[] {2, 2, 9, 1} // F
                                               };

      private static readonly int[][] WEIGHTS = {
                                          new[] {1, 3, 9, 27, 81, 32, 96, 77},
                                          new[] {20, 60, 180, 118, 143, 7, 21, 63},
                                          new[] {189, 145, 13, 39, 117, 140, 209, 205},
                                          new[] {193, 157, 49, 147, 19, 57, 171, 91},
                                          new[] {62, 186, 136, 197, 169, 85, 44, 132},
                                          new[] {185, 133, 188, 142, 4, 12, 36, 108},
                                          new[] {113, 128, 173, 97, 80, 29, 87, 50},
                                          new[] {150, 28, 84, 41, 123, 158, 52, 156},
                                          new[] {46, 138, 203, 187, 139, 206, 196, 166},
                                          new[] {76, 17, 51, 153, 37, 111, 122, 155},
                                          new[] {43, 129, 176, 106, 107, 110, 119, 146},
                                          new[] {16, 48, 144, 10, 30, 90, 59, 177},
                                          new[] {109, 116, 137, 200, 178, 112, 125, 164},
                                          new[] {70, 210, 208, 202, 184, 130, 179, 115},
                                          new[] {134, 191, 151, 31, 93, 68, 204, 190},
                                          new[] {148, 22, 66, 198, 172, 94, 71, 2},
                                          new[] {6, 18, 54, 162, 64, 192, 154, 40},
                                          new[] {120, 149, 25, 75, 14, 42, 126, 167},
                                          new[] {79, 26, 78, 23, 69, 207, 199, 175},
                                          new[] {103, 98, 83, 38, 114, 131, 182, 124},
                                          new[] {161, 61, 183, 127, 170, 88, 53, 159},
                                          new[] {55, 165, 73, 8, 24, 72, 5, 15},
                                          new[] {45, 135, 194, 160, 58, 174, 100, 89}
                                       };

      private const int FINDER_PAT_A = 0;
      private const int FINDER_PAT_B = 1;
      private const int FINDER_PAT_C = 2;
      private const int FINDER_PAT_D = 3;
      private const int FINDER_PAT_E = 4;
      private const int FINDER_PAT_F = 5;

      private static readonly int[][] FINDER_PATTERN_SEQUENCES = {
         new[] { FINDER_PAT_A, FINDER_PAT_A },
         new[] { FINDER_PAT_A, FINDER_PAT_B, FINDER_PAT_B },
         new[] { FINDER_PAT_A, FINDER_PAT_C, FINDER_PAT_B, FINDER_PAT_D },
         new[] { FINDER_PAT_A, FINDER_PAT_E, FINDER_PAT_B, FINDER_PAT_D, FINDER_PAT_C },
         new[] { FINDER_PAT_A, FINDER_PAT_E, FINDER_PAT_B, FINDER_PAT_D, FINDER_PAT_D, FINDER_PAT_F },
         new[] { FINDER_PAT_A, FINDER_PAT_E, FINDER_PAT_B, FINDER_PAT_D, FINDER_PAT_E, FINDER_PAT_F, FINDER_PAT_F },
         new[] { FINDER_PAT_A, FINDER_PAT_A, FINDER_PAT_B, FINDER_PAT_B, FINDER_PAT_C, FINDER_PAT_C, FINDER_PAT_D, FINDER_PAT_D },
         new[] { FINDER_PAT_A, FINDER_PAT_A, FINDER_PAT_B, FINDER_PAT_B, FINDER_PAT_C, FINDER_PAT_C, FINDER_PAT_D, FINDER_PAT_E, FINDER_PAT_E },
         new[] { FINDER_PAT_A, FINDER_PAT_A, FINDER_PAT_B, FINDER_PAT_B, FINDER_PAT_C, FINDER_PAT_C, FINDER_PAT_D, FINDER_PAT_E, FINDER_PAT_F, FINDER_PAT_F },
         new[] { FINDER_PAT_A, FINDER_PAT_A, FINDER_PAT_B, FINDER_PAT_B, FINDER_PAT_C, FINDER_PAT_D, FINDER_PAT_D, FINDER_PAT_E, FINDER_PAT_E, FINDER_PAT_F, FINDER_PAT_F },
      };

      private static readonly int LONGEST_SEQUENCE_SIZE = FINDER_PATTERN_SEQUENCES[FINDER_PATTERN_SEQUENCES.Length - 1].Length;

      private const int MAX_PAIRS = 11;

      private readonly List<ExpandedPair> pairs = new List<ExpandedPair>(MAX_PAIRS);
      private readonly int[] startEnd = new int[2];
      private readonly int[] currentSequence = new int[LONGEST_SEQUENCE_SIZE];

      internal List<ExpandedPair> Pairs { get { return pairs; } }

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
      override public Result decodeRow(int rowNumber,
                              BitArray row,
                              IDictionary<DecodeHintType, object> hints)
      {
         reset();
         if (decodeRow2pairs(rowNumber, row) == false)
            return null;
         return constructResult(pairs);
      }

      /// <summary>
      /// Resets this instance.
      /// </summary>
      public override void reset()
      {
         pairs.Clear();
      }

      // Not private for testing
      internal bool decodeRow2pairs(int rowNumber, BitArray row)
      {
         while (true)
         {
            ExpandedPair nextPair = retrieveNextPair(row, pairs, rowNumber);
            if (nextPair == null)
               return false;

            pairs.Add(nextPair);

            if (nextPair.MayBeLast)
            {
               if (checkChecksum())
               {
                  return true;
               }
               if (nextPair.MustBeLast)
               {
                  return false;
               }
            }
         }
      }

      private static Result constructResult(List<ExpandedPair> pairs)
      {
         BitArray binary = BitArrayBuilder.buildBitArray(pairs);

         AbstractExpandedDecoder decoder = AbstractExpandedDecoder.createDecoder(binary);
         String resultingString = decoder.parseInformation();
         if (resultingString == null)
            return null;

         ResultPoint[] firstPoints = pairs[0].FinderPattern.ResultPoints;
         ResultPoint[] lastPoints = pairs[pairs.Count - 1].FinderPattern.ResultPoints;

         return new Result(
               resultingString,
               null,
               new ResultPoint[] { firstPoints[0], firstPoints[1], lastPoints[0], lastPoints[1] },
               BarcodeFormat.RSS_EXPANDED
           );
      }

      private bool checkChecksum()
      {
         ExpandedPair firstPair = pairs[0];
         DataCharacter checkCharacter = firstPair.LeftChar;
         DataCharacter firstCharacter = firstPair.RightChar;

         int checksum = firstCharacter.ChecksumPortion;
         int s = 2;

         for (int i = 1; i < pairs.Count; ++i)
         {
            ExpandedPair currentPair = pairs[i];
            checksum += currentPair.LeftChar.ChecksumPortion;
            s++;
            DataCharacter currentRightChar = currentPair.RightChar;
            if (currentRightChar != null)
            {
               checksum += currentRightChar.ChecksumPortion;
               s++;
            }
         }

         checksum %= 211;

         int checkCharacterValue = 211 * (s - 4) + checksum;

         return checkCharacterValue == checkCharacter.Value;
      }

      private static int getNextSecondBar(BitArray row, int initialPos)
      {
         int currentPos;
         if (row[initialPos])
         {
            currentPos = row.getNextUnset(initialPos);
            currentPos = row.getNextSet(currentPos);
         }
         else
         {
            currentPos = row.getNextSet(initialPos);
            currentPos = row.getNextUnset(currentPos);
         }
         return currentPos;
      }

      // not private for testing
      internal ExpandedPair retrieveNextPair(BitArray row, List<ExpandedPair> previousPairs, int rowNumber)
      {
         bool isOddPattern = previousPairs.Count % 2 == 0;

         FinderPattern pattern;

         bool keepFinding = true;
         int forcedOffset = -1;
         do
         {
            if (!findNextPair(row, previousPairs, forcedOffset))
               return null;
            pattern = parseFoundFinderPattern(row, rowNumber, isOddPattern);
            if (pattern == null)
            {
               forcedOffset = getNextSecondBar(row, startEnd[0]);
            }
            else
            {
               keepFinding = false;
            }
         } while (keepFinding);

         bool mayBeLast;
         if (!checkPairSequence(previousPairs, pattern, out mayBeLast))
            return null;

         DataCharacter leftChar = decodeDataCharacter(row, pattern, isOddPattern, true);
         if (leftChar == null)
            return null;
         DataCharacter rightChar = decodeDataCharacter(row, pattern, isOddPattern, false);
         if (rightChar == null && !mayBeLast)
            return null;

         return new ExpandedPair(leftChar, rightChar, pattern, mayBeLast);
      }

      private bool checkPairSequence(List<ExpandedPair> previousPairs, FinderPattern pattern, out bool mayBeLast)
      {
         mayBeLast = false;
         int currentSequenceLength = previousPairs.Count + 1;
         if (currentSequenceLength > currentSequence.Length)
         {
            return false;
         }

         for (int pos = 0; pos < previousPairs.Count; ++pos)
         {
            currentSequence[pos] = previousPairs[pos].FinderPattern.Value;
         }

         currentSequence[currentSequenceLength - 1] = pattern.Value;

         foreach (int[] validSequence in FINDER_PATTERN_SEQUENCES)
         {
            if (validSequence.Length >= currentSequenceLength)
            {
               bool valid = true;
               for (int pos = 0; pos < currentSequenceLength; ++pos)
               {
                  if (currentSequence[pos] != validSequence[pos])
                  {
                     valid = false;
                     break;
                  }
               }

               if (valid)
               {
                  mayBeLast = currentSequenceLength == validSequence.Length;
                  return true;
               }
            }
         }

         return false;
      }

      private bool findNextPair(BitArray row, List<ExpandedPair> previousPairs, int forcedOffset)
      {
         int[] counters = getDecodeFinderCounters();
         counters[0] = 0;
         counters[1] = 0;
         counters[2] = 0;
         counters[3] = 0;

         int width = row.Size;

         int rowOffset;
         if (forcedOffset >= 0)
         {
            rowOffset = forcedOffset;
         }
         else if (previousPairs.Count == 0)
         {
            rowOffset = 0;
         }
         else
         {
            ExpandedPair lastPair = previousPairs[previousPairs.Count - 1];
            rowOffset = lastPair.FinderPattern.StartEnd[1];
         }
         bool searchingEvenPair = previousPairs.Count % 2 != 0;

         bool isWhite = false;
         while (rowOffset < width)
         {
            isWhite = !row[rowOffset];
            if (!isWhite)
            {
               break;
            }
            rowOffset++;
         }

         int counterPosition = 0;
         int patternStart = rowOffset;
         for (int x = rowOffset; x < width; x++)
         {
            if (row[x] ^ isWhite)
            {
               counters[counterPosition]++;
            }
            else
            {
               if (counterPosition == 3)
               {
                  if (searchingEvenPair)
                  {
                     reverseCounters(counters);
                  }

                  if (isFinderPattern(counters))
                  {
                     startEnd[0] = patternStart;
                     startEnd[1] = x;
                     return true;
                  }

                  if (searchingEvenPair)
                  {
                     reverseCounters(counters);
                  }

                  patternStart += counters[0] + counters[1];
                  counters[0] = counters[2];
                  counters[1] = counters[3];
                  counters[2] = 0;
                  counters[3] = 0;
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
         return false;
      }

      private static void reverseCounters(int[] counters)
      {
         int length = counters.Length;
         for (int i = 0; i < length / 2; ++i)
         {
            int tmp = counters[i];
            counters[i] = counters[length - i - 1];
            counters[length - i - 1] = tmp;
         }
      }

      private FinderPattern parseFoundFinderPattern(BitArray row, int rowNumber, bool oddPattern)
      {
         // Actually we found elements 2-5.
         int firstCounter;
         int start;
         int end;

         if (oddPattern)
         {
            // If pattern number is odd, we need to locate element 1 *before* the current block.

            int firstElementStart = startEnd[0] - 1;
            // Locate element 1
            while (firstElementStart >= 0 && !row[firstElementStart])
            {
               firstElementStart--;
            }

            firstElementStart++;
            firstCounter = startEnd[0] - firstElementStart;
            start = firstElementStart;
            end = startEnd[1];

         }
         else
         {
            // If pattern number is even, the pattern is reversed, so we need to locate element 1 *after* the current block.
            start = startEnd[0];
            end = row.getNextUnset(startEnd[1] + 1);
            firstCounter = end - startEnd[1];
         }

         // Make 'counters' hold 1-4
         int[] counters = getDecodeFinderCounters();
         Array.Copy(counters, 0, counters, 1, counters.Length - 1);

         counters[0] = firstCounter;
         int value;
         if (!parseFinderValue(counters, FINDER_PATTERNS, out value))
            return null;

         return new FinderPattern(value, new int[] { start, end }, start, end, rowNumber);
      }

      internal DataCharacter decodeDataCharacter(BitArray row,
                                        FinderPattern pattern,
                                        bool isOddPattern,
                                        bool leftChar)
      {
         int[] counters = getDataCharacterCounters();
         counters[0] = 0;
         counters[1] = 0;
         counters[2] = 0;
         counters[3] = 0;
         counters[4] = 0;
         counters[5] = 0;
         counters[6] = 0;
         counters[7] = 0;

         if (leftChar)
         {
            if (!recordPatternInReverse(row, pattern.StartEnd[0], counters))
               return null;
         }
         else
         {
            if (!recordPattern(row, pattern.StartEnd[1] + 1, counters))
               return null;
            // reverse it
            for (int i = 0, j = counters.Length - 1; i < j; i++, j--)
            {
               int temp = counters[i];
               counters[i] = counters[j];
               counters[j] = temp;
            }
         }//counters[] has the pixels of the module

         int numModules = 17; //left and right data characters have all the same length
         float elementWidth = (float)count(counters) / (float)numModules;

         int[] oddCounts = getOddCounts();
         int[] evenCounts = getEvenCounts();
         float[] oddRoundingErrors = getOddRoundingErrors();
         float[] evenRoundingErrors = getEvenRoundingErrors();

         for (int i = 0; i < counters.Length; i++)
         {
            float divided = 1.0f * counters[i] / elementWidth;
            int rounded = (int)(divided + 0.5f); // Round
            if (rounded < 1)
            {
               rounded = 1;
            }
            else if (rounded > 8)
            {
               rounded = 8;
            }
            int offset = i >> 1;
            if ((i & 0x01) == 0)
            {
               oddCounts[offset] = rounded;
               oddRoundingErrors[offset] = divided - rounded;
            }
            else
            {
               evenCounts[offset] = rounded;
               evenRoundingErrors[offset] = divided - rounded;
            }
         }

         if (!adjustOddEvenCounts(numModules))
            return null;

         int weightRowNumber = 4 * pattern.Value + (isOddPattern ? 0 : 2) + (leftChar ? 0 : 1) - 1;

         int oddSum = 0;
         int oddChecksumPortion = 0;
         for (int i = oddCounts.Length - 1; i >= 0; i--)
         {
            if (isNotA1left(pattern, isOddPattern, leftChar))
            {
               int weight = WEIGHTS[weightRowNumber][2 * i];
               oddChecksumPortion += oddCounts[i] * weight;
            }
            oddSum += oddCounts[i];
         }
         int evenChecksumPortion = 0;
         int evenSum = 0;
         for (int i = evenCounts.Length - 1; i >= 0; i--)
         {
            if (isNotA1left(pattern, isOddPattern, leftChar))
            {
               int weight = WEIGHTS[weightRowNumber][2 * i + 1];
               evenChecksumPortion += evenCounts[i] * weight;
            }
            evenSum += evenCounts[i];
         }
         int checksumPortion = oddChecksumPortion + evenChecksumPortion;

         if ((oddSum & 0x01) != 0 || oddSum > 13 || oddSum < 4)
         {
            return null;
         }

         int group = (13 - oddSum) / 2;
         int oddWidest = SYMBOL_WIDEST[group];
         int evenWidest = 9 - oddWidest;
         int vOdd = RSSUtils.getRSSvalue(oddCounts, oddWidest, true);
         int vEven = RSSUtils.getRSSvalue(evenCounts, evenWidest, false);
         int tEven = EVEN_TOTAL_SUBSET[group];
         int gSum = GSUM[group];
         int value = vOdd * tEven + vEven + gSum;

         return new DataCharacter(value, checksumPortion);
      }

      private static bool isNotA1left(FinderPattern pattern, bool isOddPattern, bool leftChar)
      {
         // A1: pattern.getValue is 0 (A), and it's an oddPattern, and it is a left char
         return !(pattern.Value == 0 && isOddPattern && leftChar);
      }

      private bool adjustOddEvenCounts(int numModules)
      {
         int oddSum = count(getOddCounts());
         int evenSum = count(getEvenCounts());
         int mismatch = oddSum + evenSum - numModules;
         bool oddParityBad = (oddSum & 0x01) == 1;
         bool evenParityBad = (evenSum & 0x01) == 0;

         bool incrementOdd = false;
         bool decrementOdd = false;

         if (oddSum > 13)
         {
            decrementOdd = true;
         }
         else if (oddSum < 4)
         {
            incrementOdd = true;
         }
         bool incrementEven = false;
         bool decrementEven = false;
         if (evenSum > 13)
         {
            decrementEven = true;
         }
         else if (evenSum < 4)
         {
            incrementEven = true;
         }

         if (mismatch == 1)
         {
            if (oddParityBad)
            {
               if (evenParityBad)
               {
                  return false;
               }
               decrementOdd = true;
            }
            else
            {
               if (!evenParityBad)
               {
                  return false;
               }
               decrementEven = true;
            }
         }
         else if (mismatch == -1)
         {
            if (oddParityBad)
            {
               if (evenParityBad)
               {
                  return false;
               }
               incrementOdd = true;
            }
            else
            {
               if (!evenParityBad)
               {
                  return false;
               }
               incrementEven = true;
            }
         }
         else if (mismatch == 0)
         {
            if (oddParityBad)
            {
               if (!evenParityBad)
               {
                  return false;
               }
               // Both bad
               if (oddSum < evenSum)
               {
                  incrementOdd = true;
                  decrementEven = true;
               }
               else
               {
                  decrementOdd = true;
                  incrementEven = true;
               }
            }
            else
            {
               if (evenParityBad)
               {
                  return false;
               }
               // Nothing to do!
            }
         }
         else
         {
            return false;
         }

         if (incrementOdd)
         {
            if (decrementOdd)
            {
               return false;
            }
            increment(getOddCounts(), getOddRoundingErrors());
         }
         if (decrementOdd)
         {
            decrement(getOddCounts(), getOddRoundingErrors());
         }
         if (incrementEven)
         {
            if (decrementEven)
            {
               return false;
            }
            increment(getEvenCounts(), getOddRoundingErrors());
         }
         if (decrementEven)
         {
            decrement(getEvenCounts(), getEvenRoundingErrors());
         }

         return true;
      }
   }
}