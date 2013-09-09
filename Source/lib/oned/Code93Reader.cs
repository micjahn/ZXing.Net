/*
 * Copyright 2010 ZXing authors
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
   ///   <p>Decodes Code 93 barcodes.</p>
   /// 	<author>Sean Owen</author>
   /// <see cref="Code39Reader" />
   /// </summary>
   public sealed class Code93Reader : OneDReader
   {
      // Note that 'abcd' are dummy characters in place of control characters.
      private const String ALPHABET_STRING = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%abcd*";
      private static readonly char[] ALPHABET = ALPHABET_STRING.ToCharArray();

      /// <summary>
      /// These represent the encodings of characters, as patterns of wide and narrow bars.
      /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow.
      /// </summary>
      private static readonly int[] CHARACTER_ENCODINGS = {
                                                    0x114, 0x148, 0x144, 0x142, 0x128, 0x124, 0x122, 0x150, 0x112, 0x10A, // 0-9
                                                    0x1A8, 0x1A4, 0x1A2, 0x194, 0x192, 0x18A, 0x168, 0x164, 0x162, 0x134, // A-J
                                                    0x11A, 0x158, 0x14C, 0x146, 0x12C, 0x116, 0x1B4, 0x1B2, 0x1AC, 0x1A6, // K-T
                                                    0x196, 0x19A, 0x16C, 0x166, 0x136, 0x13A, // U-Z
                                                    0x12E, 0x1D4, 0x1D2, 0x1CA, 0x16E, 0x176, 0x1AE, // - - %
                                                    0x126, 0x1DA, 0x1D6, 0x132, 0x15E, // Control chars? $-*
                                                 };
      private static readonly int ASTERISK_ENCODING = CHARACTER_ENCODINGS[47];

      private readonly StringBuilder decodeRowResult;
      private readonly int[] counters;

      /// <summary>
      /// Initializes a new instance of the <see cref="Code93Reader"/> class.
      /// </summary>
      public Code93Reader()
      {
         decodeRowResult = new StringBuilder(20);
         counters = new int[6];
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
         
         int[] start = findAsteriskPattern(row);
         if (start == null)
            return null;

         // Read off white space    
         int nextStart = row.getNextSet(start[1]);
         int end = row.Size;

         char decodedChar;
         int lastStart;
         do
         {
            if (!recordPattern(row, nextStart, counters))
               return null;

            int pattern = toPattern(counters);
            if (pattern < 0)
            {
               return null;
            }
            if (!patternToChar(pattern, out decodedChar))
               return null;
            decodeRowResult.Append(decodedChar);
            lastStart = nextStart;
            foreach (int counter in counters)
            {
               nextStart += counter;
            }
            // Read off white space
            nextStart = row.getNextSet(nextStart);
         } while (decodedChar != '*');
         decodeRowResult.Remove(decodeRowResult.Length - 1, 1); // remove asterisk

         int lastPatternSize = 0;
         foreach (int counter in counters)
         {
            lastPatternSize += counter;
         }

         // Should be at least one more black module
         if (nextStart == end || !row[nextStart])
         {
            return null;
         }

         if (decodeRowResult.Length < 2)
         {
            // false positive -- need at least 2 checksum digits
            return null;
         }

         if (!checkChecksums(decodeRowResult))
            return null;
         // Remove checksum digits
         decodeRowResult.Length = decodeRowResult.Length - 2;

         String resultString = decodeExtended(decodeRowResult);
         if (resultString == null)
            return null;

         float left = (start[1] + start[0])/2.0f;
         float right = lastStart + lastPatternSize / 2.0f;

         var resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                                      ? null
                                      : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(left, rowNumber));
            resultPointCallback(new ResultPoint(right, rowNumber));
         }

         return new Result(
            resultString,
            null,
            new[]
               {
                  new ResultPoint(left, rowNumber),
                  new ResultPoint(right, rowNumber)
               },
            BarcodeFormat.CODE_93);
      }

      private int[] findAsteriskPattern(BitArray row)
      {
         int width = row.Size;
         int rowOffset = row.getNextSet(0);

         for (var index = 0; index < counters.Length; index++)
            counters[index] = 0;
         int counterPosition = 0;
         int patternStart = rowOffset;
         bool isWhite = false;
         int patternLength = counters.Length;

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
                  if (toPattern(counters) == ASTERISK_ENCODING)
                  {
                     return new int[] { patternStart, i };
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

      private static int toPattern(int[] counters)
      {
         int max = counters.Length;
         int sum = 0;
         foreach (var counter in counters)
         {
            sum += counter;
         }
         int pattern = 0;
         for (int i = 0; i < max; i++)
         {
            int scaledShifted = (counters[i] << INTEGER_MATH_SHIFT) * 9 / sum;
            int scaledUnshifted = scaledShifted >> INTEGER_MATH_SHIFT;
            if ((scaledShifted & 0xFF) > 0x7F)
            {
               scaledUnshifted++;
            }
            if (scaledUnshifted < 1 || scaledUnshifted > 4)
            {
               return -1;
            }
            if ((i & 0x01) == 0)
            {
               for (int j = 0; j < scaledUnshifted; j++)
               {
                  pattern = (pattern << 1) | 0x01;
               }
            }
            else
            {
               pattern <<= scaledUnshifted;
            }
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

      private static String decodeExtended(StringBuilder encoded)
      {
         int length = encoded.Length;
         StringBuilder decoded = new StringBuilder(length);
         for (int i = 0; i < length; i++)
         {
            char c = encoded[i];
            if (c >= 'a' && c <= 'd')
            {
               if (i >= length - 1)
               {
                  return null;
               }
               char next = encoded[i + 1];
               char decodedChar = '\0';
               switch (c)
               {
                  case 'd':
                     // +A to +Z map to a to z
                     if (next >= 'A' && next <= 'Z')
                     {
                        decodedChar = (char)(next + 32);
                     }
                     else
                     {
                        return null;
                     }
                     break;
                  case 'a':
                     // $A to $Z map to control codes SH to SB
                     if (next >= 'A' && next <= 'Z')
                     {
                        decodedChar = (char)(next - 64);
                     }
                     else
                     {
                        return null;
                     }
                     break;
                  case 'b':
                     // %A to %E map to control codes ESC to US
                     if (next >= 'A' && next <= 'E')
                     {
                        decodedChar = (char)(next - 38);
                     }
                     else if (next >= 'F' && next <= 'W')
                     {
                        decodedChar = (char)(next - 11);
                     }
                     else
                     {
                        return null;
                     }
                     break;
                  case 'c':
                     // /A to /O map to ! to , and /Z maps to :
                     if (next >= 'A' && next <= 'O')
                     {
                        decodedChar = (char)(next - 32);
                     }
                     else if (next == 'Z')
                     {
                        decodedChar = ':';
                     }
                     else
                     {
                        return null;
                     }
                     break;
               }
               decoded.Append(decodedChar);
               // bump up i again since we read two characters
               i++;
            }
            else
            {
               decoded.Append(c);
            }
         }
         return decoded.ToString();
      }

      private static bool checkChecksums(StringBuilder result)
      {
         int length = result.Length;
         if (!checkOneChecksum(result, length - 2, 20))
            return false;
         if (!checkOneChecksum(result, length - 1, 15))
            return false;
         return true;
      }

      private static bool checkOneChecksum(StringBuilder result, int checkPosition, int weightMax)
      {
         int weight = 1;
         int total = 0;
         for (int i = checkPosition - 1; i >= 0; i--)
         {
            total += weight * ALPHABET_STRING.IndexOf(result[i]);
            if (++weight > weightMax)
            {
               weight = 1;
            }
         }
         if (result[checkPosition] != ALPHABET[total % 47])
         {
            return false;
         }
         return true;
      }
   }
}