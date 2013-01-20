/*
 * Copyright 2013 ZXing.Net authors
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
   /// Decodes MSI barcodes.
   /// </summary>
   public sealed class MSIReader : OneDReader
   {
      internal static String ALPHABET_STRING = "0123456789";
      private static readonly char[] ALPHABET = ALPHABET_STRING.ToCharArray();

      /// <summary>
      /// These represent the encodings of characters, as patterns of wide and narrow bars.
      /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow,
      /// with 1s representing "wide" and 0s representing narrow.
      /// </summary>
      internal static int[] CHARACTER_ENCODINGS = {
                                                     0x924, 0x926, 0x934, 0x936, 0x9A4, 0x9A6, 0x9B4, 0x9B6, 0xD24, 0xD26 // 0-9
                                                  };

      private const int START_ENCODING = 0x06;
      private const int END_ENCODING = 0x09;

      private readonly bool usingCheckDigit;
      private readonly StringBuilder decodeRowResult;
      private readonly int[] counters;
      private int averageCounterWidth;

      /// <summary>
      /// Creates a reader that assumes all encoded data is data, and does not treat the final
      /// character as a check digit.
      /// </summary>
      public MSIReader()
         : this(false)
      {
      }

      /// <summary>
      /// Creates a reader that can be configured to check the last character as a check digit,
      /// </summary>
      /// <param name="usingCheckDigit">if true, treat the last data character as a check digit, not
      /// data, and verify that the checksum passes.</param>
      public MSIReader(bool usingCheckDigit)
      {
         this.usingCheckDigit = usingCheckDigit;
         decodeRowResult = new StringBuilder(20);
         counters = new int[8];
      }

      /// <summary>
      ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
      /// an image.</p>
      /// </summary>
      /// <param name="rowNumber">row number from top of the row</param>
      /// <param name="row">the black/white pixel data of the row</param>
      /// <param name="hints">decode hints</param>
      /// <returns><see cref="Result"/>containing encoded string and start/end of barcode</returns>
      override public Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
      {
         for (var index = 0; index < counters.Length; index++)
            counters[index] = 0;
         decodeRowResult.Length = 0;

         int[] start = findStartPattern(row, counters);
         if (start == null)
            return null;

         // Read off white space    
         int nextStart = row.getNextSet(start[1]);

         char decodedChar;
         int lastStart = nextStart;
         int pattern;
         do
         {
            if (!recordPattern(row, nextStart, counters, 8))
            {
               // not enough bars for a number but perhaps enough for the end pattern
               var endPattern = findEndPattern(row, nextStart, counters);
               if (endPattern == null)
                  return null;
               lastStart = nextStart;
               nextStart = endPattern[1];
               break;
            }
            pattern = toPattern(counters, 8);
            if (!patternToChar(pattern, out decodedChar))
            {
               // pattern doesn't result in an encoded number
               // but it could be the end pattern followed by some black areas
               var endPattern = findEndPattern(row, nextStart, counters);
               if (endPattern == null)
                  return null;
               lastStart = nextStart;
               nextStart = endPattern[1];
               break;
            }
            decodeRowResult.Append(decodedChar);
            lastStart = nextStart;
            foreach (int counter in counters)
            {
               nextStart += counter;
            }
            // Read off white space
            nextStart = row.getNextSet(nextStart);
         } while (decodedChar != '*');
         
         // at least 3 digits to prevent false positives within other kind
         // of codes like PDF417
         if (decodeRowResult.Length < 3)
         {
            return null;
         }

         var rawBytes = Encoding.UTF8.GetBytes(decodeRowResult.ToString());
         var resultString = decodeRowResult.ToString();

         if (usingCheckDigit)
         {
            var resultStringWithoutChecksum = resultString.Substring(0, resultString.Length - 1);
            int checkSum = CalculateChecksumLuhn(resultStringWithoutChecksum);
            if ((char)(checkSum + 48) != resultString[resultStringWithoutChecksum.Length])
            {
               return null;
            }
         }

         float left = (float)(start[1] + start[0]) / 2.0f;
         float right = (float)(nextStart + lastStart) / 2.0f;

         var resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                                      ? null
                                      : (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(left, rowNumber));
            resultPointCallback(new ResultPoint(right, rowNumber));
         }

         return new Result(
            resultString,
            rawBytes,
            new[]
               {
                  new ResultPoint(left, rowNumber),
                  new ResultPoint(right, rowNumber)
               },
            BarcodeFormat.MSI);
      }

      private int[] findStartPattern(BitArray row, int[] counters)
      {
         const int patternLength = 2;

         int width = row.Size;
         int rowOffset = row.getNextSet(0);

         int counterPosition = 0;
         int patternStart = rowOffset;
         bool isWhite = false;

         counters[0] = 0;
         counters[1] = 0;
         for (int i = rowOffset; i < width; i++)
         {
            if (row[i] ^ isWhite)
            {
               counters[counterPosition]++;
            }
            else
            {
               if (counterPosition == patternLength - 1)
               {
                  // narrow and wide areas should be as near as possible to factor 2
                  // lets say we will check 1.5 <= factor <= 5
                  var factorNarrowToWide = ((float)counters[0]) / ((float)counters[1]);
                  if (factorNarrowToWide >= 1.5 && factorNarrowToWide <= 5)
                  {
                     calculateAverageCounterWidth(counters, patternLength);
                     if (toPattern(counters, patternLength) == START_ENCODING)
                     {
                        // Look for whitespace before start pattern, >= 50% of width of start pattern
                        if (row.isRange(Math.Max(0, patternStart - ((i - patternStart) >> 1)), patternStart, false))
                        {
                           return new int[] {patternStart, i};
                        }
                     }
                  }
                  patternStart += counters[0] + counters[1];
                  Array.Copy(counters, 2, counters, 0, patternLength - 2);
                  counters[patternLength - 2] = 0;
                  counters[patternLength - 1] = 0;
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

      private int[] findEndPattern(BitArray row, int rowOffset, int[] counters)
      {
         const int patternLength = 3;

         int width = row.Size;

         int counterPosition = 0;
         int patternStart = rowOffset;
         bool isWhite = false;

         counters[0] = 0;
         counters[1] = 0;
         counters[2] = 0;
         for (int i = rowOffset; i < width; i++)
         {
            if (row[i] ^ isWhite)
            {
               counters[counterPosition]++;
            }
            else
            {
               if (counterPosition == patternLength - 1)
               {
                  var factorNarrowToWide = ((float)counters[1]) / ((float)counters[0]);
                  if (factorNarrowToWide >= 1.5 && factorNarrowToWide <= 5)
                  {
                     if (toPattern(counters, patternLength) == END_ENCODING)
                     {
                        // Look for whitespace after end pattern, >= 50% of width of end pattern
                        var minEndOfWhite = Math.Min(row.Size - 1, i + ((i - patternStart) >> 1));
                        if (row.isRange(i, minEndOfWhite, false))
                        {
                           return new int[] {patternStart, i};
                        }
                     }
                  }
                  return null;
               }
               counterPosition++;
               counters[counterPosition] = 1;
               isWhite = !isWhite;
            }
         }
         return null;
      }

      private void calculateAverageCounterWidth(int[] counters, int patternLength)
      {
         // look for the minimum and the maximum width of the bars
         // there are only two sizes for MSI barcodes
         // all numbers are encoded as a chain of the pattern 100 and 110
         // the complete pattern of one number always starts with 1 or 11 (black bar(s))
         int minCounter = Int32.MaxValue;
         int maxCounter = 0;
         for (var index = 0; index < patternLength; index++)
         {
            var counter = counters[index];
            if (counter < minCounter)
            {
               minCounter = counter;
            }
            if (counter > maxCounter)
            {
               maxCounter = counter;
            }
         }
         // calculate the average of the minimum and maximum width
         // using some bit shift to get a higher resolution without floating point arithmetic
         averageCounterWidth = ((maxCounter << 8) + (minCounter << 8)) / 2;
      }

      private int toPattern(int[] counters, int patternLength)
      {
         // calculating the encoded value from the pattern
         int pattern = 0;
         int bit = 1;
         int doubleBit = 3;
         for (var index = 0; index < patternLength; index++)
         {
            var counter = counters[index];
            if ((counter << 8) < averageCounterWidth)
            {
               pattern = (pattern << 1) | bit;
            }
            else
            {
               pattern = (pattern << 2) | doubleBit;
            }
            bit = bit ^ 1;
            doubleBit = doubleBit ^ 3;
         }

         return pattern;
      }

      private static bool patternToChar(int pattern, out char c)
      {
         for (int i = 0; i < CHARACTER_ENCODINGS.Length; i++)
         {
            if (CHARACTER_ENCODINGS[i] == pattern)
            {
               c = ALPHABET[i];
               return true;
            }
         }
         c = '*';
         return false;
      }

      private static readonly int[] doubleAndCrossSum = new [] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

      private static int CalculateChecksumLuhn(string number)
      {
         var checksum = 0;

         for (var index = number.Length - 2; index >= 0; index -= 2)
         {
            var digit = number[index] - 48;
            checksum += digit;
         }
         for (var index = number.Length - 1; index >= 0; index -= 2)
         {
            var digit = doubleAndCrossSum[number[index] - 48];
            checksum += digit;
         }

         return (10 - (checksum % 10)) % 10;
      }
   }
}