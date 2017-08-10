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
using System.Text;
using ZXing.Common;

namespace ZXing.OneD.RSS
{
   /// <summary>
   /// Decodes RSS-14, including truncated and stacked variants. See ISO/IEC 24724:2006.
   /// </summary>
   public sealed class RSS14Reader : AbstractRSSReader
   {
      private static readonly int[] OUTSIDE_EVEN_TOTAL_SUBSET = { 1, 10, 34, 70, 126 };
      private static readonly int[] INSIDE_ODD_TOTAL_SUBSET = { 4, 20, 48, 81 };
      private static readonly int[] OUTSIDE_GSUM = { 0, 161, 961, 2015, 2715 };
      private static readonly int[] INSIDE_GSUM = { 0, 336, 1036, 1516 };
      private static readonly int[] OUTSIDE_ODD_WIDEST = { 8, 6, 4, 3, 1 };
      private static readonly int[] INSIDE_ODD_WIDEST = { 2, 4, 6, 8 };

      private static readonly int[][] FINDER_PATTERNS = {
                                                           new[] {3, 8, 2, 1},
                                                           new[] {3, 5, 5, 1},
                                                           new[] {3, 3, 7, 1},
                                                           new[] {3, 1, 9, 1},
                                                           new[] {2, 7, 4, 1},
                                                           new[] {2, 5, 6, 1},
                                                           new[] {2, 3, 8, 1},
                                                           new[] {1, 5, 7, 1},
                                                           new[] {1, 3, 9, 1},
                                                        };

      private readonly List<Pair> possibleLeftPairs;
      private readonly List<Pair> possibleRightPairs;

      /// <summary>
      /// Initializes a new instance of the <see cref="RSS14Reader"/> class.
      /// </summary>
      public RSS14Reader()
      {
         possibleLeftPairs = new List<Pair>();
         possibleRightPairs = new List<Pair>();
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
      override public Result decodeRow(int rowNumber,
                              BitArray row,
                              IDictionary<DecodeHintType, object> hints)
      {
         Pair leftPair = decodePair(row, false, rowNumber, hints);
         addOrTally(possibleLeftPairs, leftPair);
         row.reverse();
         Pair rightPair = decodePair(row, true, rowNumber, hints);
         addOrTally(possibleRightPairs, rightPair);
         row.reverse();
         int lefSize = possibleLeftPairs.Count;
         for (int i = 0; i < lefSize; i++)
         {
            Pair left = possibleLeftPairs[i];
            if (left.Count > 1)
            {
               int rightSize = possibleRightPairs.Count;
               for (int j = 0; j < rightSize; j++)
               {
                  Pair right = possibleRightPairs[j];
                  if (right.Count > 1 &&
                      checkChecksum(left, right))
                  {
                     return constructResult(left, right);
                  }
               }
            }
         }
         return null;
      }

      private static void addOrTally(IList<Pair> possiblePairs, Pair pair)
      {
         if (pair == null)
         {
            return;
         }
         bool found = false;
         foreach (Pair other in possiblePairs)
         {
            if (other.Value == pair.Value)
            {
               other.incrementCount();
               found = true;
               break;
            }
         }
         if (!found)
         {
            possiblePairs.Add(pair);
         }
      }

      /// <summary>
      /// Resets this instance.
      /// </summary>
      public override void reset()
      {
         possibleLeftPairs.Clear();
         possibleRightPairs.Clear();
      }

      private static Result constructResult(Pair leftPair, Pair rightPair)
      {
         long symbolValue = 4537077L * leftPair.Value + rightPair.Value;
         String text = symbolValue.ToString();

         StringBuilder buffer = new StringBuilder(14);
         for (int i = 13 - text.Length; i > 0; i--)
         {
            buffer.Append('0');
         }
         buffer.Append(text);

         int checkDigit = 0;
         for (int i = 0; i < 13; i++)
         {
            int digit = buffer[i] - '0';
            checkDigit += (i & 0x01) == 0 ? 3 * digit : digit;
         }
         checkDigit = 10 - (checkDigit % 10);
         if (checkDigit == 10)
         {
            checkDigit = 0;
         }
         buffer.Append(checkDigit);

         ResultPoint[] leftPoints = leftPair.FinderPattern.ResultPoints;
         ResultPoint[] rightPoints = rightPair.FinderPattern.ResultPoints;
         return new Result(
             buffer.ToString(),
             null,
             new ResultPoint[] { leftPoints[0], leftPoints[1], rightPoints[0], rightPoints[1], },
             BarcodeFormat.RSS_14);
      }

      private static bool checkChecksum(Pair leftPair, Pair rightPair)
      {
         //int leftFPValue = leftPair.FinderPattern.Value;
         //int rightFPValue = rightPair.FinderPattern.Value;
         //if ((leftFPValue == 0 && rightFPValue == 8) ||
         //    (leftFPValue == 8 && rightFPValue == 0))
         //{
         //}
         int checkValue = (leftPair.ChecksumPortion + 16 * rightPair.ChecksumPortion) % 79;
         int targetCheckValue =
             9 * leftPair.FinderPattern.Value + rightPair.FinderPattern.Value;
         if (targetCheckValue > 72)
         {
            targetCheckValue--;
         }
         if (targetCheckValue > 8)
         {
            targetCheckValue--;
         }
         return checkValue == targetCheckValue;
      }

      private Pair decodePair(BitArray row, bool right, int rowNumber, IDictionary<DecodeHintType, object> hints)
      {
         int[] startEnd = findFinderPattern(row, right);
         if (startEnd == null)
            return null;
         FinderPattern pattern = parseFoundFinderPattern(row, rowNumber, right, startEnd);
         if (pattern == null)
            return null;

         ResultPointCallback resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? null :
            (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];

         if (resultPointCallback != null)
         {
            float center = (startEnd[0] + startEnd[1]) / 2.0f;
            if (right)
            {
               // row is actually reversed
               center = row.Size - 1 - center;
            }
            resultPointCallback(new ResultPoint(center, rowNumber));
         }

         DataCharacter outside = decodeDataCharacter(row, pattern, true);
         if (outside == null)
            return null;
         DataCharacter inside = decodeDataCharacter(row, pattern, false);
         if (inside == null)
            return null;
         return new Pair(1597*outside.Value + inside.Value,
                         outside.ChecksumPortion + 4*inside.ChecksumPortion,
                         pattern);
      }

      private DataCharacter decodeDataCharacter(BitArray row, FinderPattern pattern, bool outsideChar)
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

         if (outsideChar)
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
         }

         int numModules = outsideChar ? 16 : 15;
         float elementWidth = (float)ZXing.Common.Detector.MathUtils.sum(counters) / (float)numModules;

         int[] oddCounts = this.getOddCounts();
         int[] evenCounts = this.getEvenCounts();
         float[] oddRoundingErrors = this.getOddRoundingErrors();
         float[] evenRoundingErrors = this.getEvenRoundingErrors();

         for (int i = 0; i < counters.Length; i++)
         {
            float value = (float)counters[i] / elementWidth;
            int rounded = (int)(value + 0.5f); // Round
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
               oddRoundingErrors[offset] = value - rounded;
            }
            else
            {
               evenCounts[offset] = rounded;
               evenRoundingErrors[offset] = value - rounded;
            }
         }

         if (!adjustOddEvenCounts(outsideChar, numModules))
            return null;

         int oddSum = 0;
         int oddChecksumPortion = 0;
         for (int i = oddCounts.Length - 1; i >= 0; i--)
         {
            oddChecksumPortion *= 9;
            oddChecksumPortion += oddCounts[i];
            oddSum += oddCounts[i];
         }
         int evenChecksumPortion = 0;
         int evenSum = 0;
         for (int i = evenCounts.Length - 1; i >= 0; i--)
         {
            evenChecksumPortion *= 9;
            evenChecksumPortion += evenCounts[i];
            evenSum += evenCounts[i];
         }
         int checksumPortion = oddChecksumPortion + 3 * evenChecksumPortion;

         if (outsideChar)
         {
            if ((oddSum & 0x01) != 0 || oddSum > 12 || oddSum < 4)
            {
               return null;
            }
            int group = (12 - oddSum) / 2;
            int oddWidest = OUTSIDE_ODD_WIDEST[group];
            int evenWidest = 9 - oddWidest;
            int vOdd = RSSUtils.getRSSvalue(oddCounts, oddWidest, false);
            int vEven = RSSUtils.getRSSvalue(evenCounts, evenWidest, true);
            int tEven = OUTSIDE_EVEN_TOTAL_SUBSET[group];
            int gSum = OUTSIDE_GSUM[group];
            return new DataCharacter(vOdd * tEven + vEven + gSum, checksumPortion);
         }
         else
         {
            if ((evenSum & 0x01) != 0 || evenSum > 10 || evenSum < 4)
            {
               return null;
            }
            int group = (10 - evenSum) / 2;
            int oddWidest = INSIDE_ODD_WIDEST[group];
            int evenWidest = 9 - oddWidest;
            int vOdd = RSSUtils.getRSSvalue(oddCounts, oddWidest, true);
            int vEven = RSSUtils.getRSSvalue(evenCounts, evenWidest, false);
            int tOdd = INSIDE_ODD_TOTAL_SUBSET[group];
            int gSum = INSIDE_GSUM[group];
            return new DataCharacter(vEven * tOdd + vOdd + gSum, checksumPortion);
         }
      }

      private int[] findFinderPattern(BitArray row, bool rightFinderPattern)
      {

         int[] counters = getDecodeFinderCounters();
         counters[0] = 0;
         counters[1] = 0;
         counters[2] = 0;
         counters[3] = 0;

         int width = row.Size;
         bool isWhite = false;
         int rowOffset = 0;
         while (rowOffset < width)
         {
            isWhite = !row[rowOffset];
            if (rightFinderPattern == isWhite)
            {
               // Will encounter white first when searching for right finder pattern
               break;
            }
            rowOffset++;
         }

         int counterPosition = 0;
         int patternStart = rowOffset;
         for (int x = rowOffset; x < width; x++)
         {
            if (row[x] != isWhite)
            {
               counters[counterPosition]++;
            }
            else
            {
               if (counterPosition == 3)
               {
                  if (isFinderPattern(counters))
                  {
                     return new int[] { patternStart, x };
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
         return null;
      }

      private FinderPattern parseFoundFinderPattern(BitArray row, int rowNumber, bool right, int[] startEnd)
      {
         // Actually we found elements 2-5
         bool firstIsBlack = row[startEnd[0]];
         int firstElementStart = startEnd[0] - 1;
         // Locate element 1
         while (firstElementStart >= 0 && firstIsBlack != row[firstElementStart])
         {
            firstElementStart--;
         }
         firstElementStart++;
         int firstCounter = startEnd[0] - firstElementStart;
         // Make 'counters' hold 1-4
         int[] counters = getDecodeFinderCounters();
         Array.Copy(counters, 0, counters, 1, counters.Length - 1);
         counters[0] = firstCounter;
         int value;
         if (!parseFinderValue(counters, FINDER_PATTERNS, out value))
            return null;
         int start = firstElementStart;
         int end = startEnd[1];
         if (right)
         {
            // row is actually reversed
            start = row.Size - 1 - start;
            end = row.Size - 1 - end;
         }
         return new FinderPattern(value, new int[] { firstElementStart, startEnd[1] }, start, end, rowNumber);
      }

      private bool adjustOddEvenCounts(bool outsideChar, int numModules)
      {
         int oddSum = ZXing.Common.Detector.MathUtils.sum(getOddCounts());
         int evenSum = ZXing.Common.Detector.MathUtils.sum(getEvenCounts());
         int mismatch = oddSum + evenSum - numModules;
         bool oddParityBad = (oddSum & 0x01) == (outsideChar ? 1 : 0);
         bool evenParityBad = (evenSum & 0x01) == 1;

         bool incrementOdd = false;
         bool decrementOdd = false;
         bool incrementEven = false;
         bool decrementEven = false;

         if (outsideChar)
         {
            if (oddSum > 12)
            {
               decrementOdd = true;
            }
            else if (oddSum < 4)
            {
               incrementOdd = true;
            }
            if (evenSum > 12)
            {
               decrementEven = true;
            }
            else if (evenSum < 4)
            {
               incrementEven = true;
            }
         }
         else
         {
            if (oddSum > 11)
            {
               decrementOdd = true;
            }
            else if (oddSum < 5)
            {
               incrementOdd = true;
            }
            if (evenSum > 10)
            {
               decrementEven = true;
            }
            else if (evenSum < 4)
            {
               incrementEven = true;
            }
         }

         /*if (mismatch == 2) {
           if (!(oddParityBad && evenParityBad)) {
             throw ReaderException.Instance;
           }
           decrementOdd = true;
           decrementEven = true;
         } else if (mismatch == -2) {
           if (!(oddParityBad && evenParityBad)) {
             throw ReaderException.Instance;
           }
           incrementOdd = true;
           incrementEven = true;
         } else */
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