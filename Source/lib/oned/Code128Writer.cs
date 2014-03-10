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
using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// This object renders a CODE128 code as a <see cref="BitMatrix" />.
   /// 
   /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
   /// </summary>
   public sealed class Code128Writer : OneDimensionalCodeWriter
   {
      private const int CODE_START_B = 104;
      private const int CODE_START_C = 105;
      private const int CODE_CODE_B = 100;
      private const int CODE_CODE_C = 99;
      private const int CODE_STOP = 106;

      // Dummy characters used to specify control characters in input
      private const char ESCAPE_FNC_1 = '\u00f1';
      private const char ESCAPE_FNC_2 = '\u00f2';
      private const char ESCAPE_FNC_3 = '\u00f3';
      private const char ESCAPE_FNC_4 = '\u00f4';

      private const int CODE_FNC_1 = 102;   // Code A, Code B, Code C
      private const int CODE_FNC_2 = 97;    // Code A, Code B
      private const int CODE_FNC_3 = 96;    // Code A, Code B
      private const int CODE_FNC_4_B = 100; // Code B

      private bool forceCodesetB;

      public override BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height,
                              IDictionary<EncodeHintType, object> hints)
      {
         if (format != BarcodeFormat.CODE_128)
         {
            throw new ArgumentException("Can only encode CODE_128, but got " + format);
         }

         forceCodesetB = (hints != null &&
                          hints.ContainsKey(EncodeHintType.CODE128_FORCE_CODESET_B) &&
                          (bool) hints[EncodeHintType.CODE128_FORCE_CODESET_B]);

         return base.encode(contents, format, width, height, hints);
      }

      override public bool[] encode(String contents)
      {
         int length = contents.Length;
         // Check length
         if (length < 1 || length > 80)
         {
            throw new ArgumentException(
                "Contents length should be between 1 and 80 characters, but got " + length);
         }
         // Check content
         for (int i = 0; i < length; i++)
         {
            char c = contents[i];
            if (c < ' ' || c > '~')
            {
               switch (c)
               {
                  case ESCAPE_FNC_1:
                  case ESCAPE_FNC_2:
                  case ESCAPE_FNC_3:
                  case ESCAPE_FNC_4:
                     break;
                  default:
                     throw new ArgumentException("Bad character in input: " + c);
               }
            }
         }

         var patterns = new List<int[]>(); // temporary storage for patterns
         int checkSum = 0;
         int checkWeight = 1;
         int codeSet = 0; // selected code (CODE_CODE_B or CODE_CODE_C)
         int position = 0; // position in contents

         while (position < length)
         {
            //Select code to use
            int requiredDigitCount = codeSet == CODE_CODE_C ? 2 : 4;
            int newCodeSet;
            if (isDigits(contents, position, requiredDigitCount))
            {
               newCodeSet = forceCodesetB ? CODE_CODE_B : CODE_CODE_C;
            }
            else
            {
               newCodeSet = CODE_CODE_B;
            }

            //Get the pattern index
            int patternIndex;
            if (newCodeSet == codeSet)
            {
               // Encode the current character
               // First handle escapes
               switch (contents[position])
               {
                  case ESCAPE_FNC_1:
                     patternIndex = CODE_FNC_1;
                     break;
                  case ESCAPE_FNC_2:
                     patternIndex = CODE_FNC_2;
                     break;
                  case ESCAPE_FNC_3:
                     patternIndex = CODE_FNC_3;
                     break;
                  case ESCAPE_FNC_4:
                     patternIndex = CODE_FNC_4_B; // FIXME if this ever outputs Code A
                     break;
                  default:
                     // Then handle normal characters otherwise
                     if (codeSet == CODE_CODE_B)
                     {
                        patternIndex = contents[position] - ' ';
                     }
                     else
                     { // CODE_CODE_C
                        patternIndex = Int32.Parse(contents.Substring(position, 2));
                        position++; // Also incremented below
                     }
                     break;
               }
               position++;
            }
            else
            {
               // Should we change the current code?
               // Do we have a code set?
               if (codeSet == 0)
               {
                  // No, we don't have a code set
                  if (newCodeSet == CODE_CODE_B)
                  {
                     patternIndex = CODE_START_B;
                  }
                  else
                  {
                     // CODE_CODE_C
                     patternIndex = CODE_START_C;
                  }
               }
               else
               {
                  // Yes, we have a code set
                  patternIndex = newCodeSet;
               }
               codeSet = newCodeSet;
            }

            // Get the pattern
            patterns.Add(Code128Reader.CODE_PATTERNS[patternIndex]);

            // Compute checksum
            checkSum += patternIndex * checkWeight;
            if (position != 0)
            {
               checkWeight++;
            }
         }

         // Compute and append checksum
         checkSum %= 103;
         patterns.Add(Code128Reader.CODE_PATTERNS[checkSum]);

         // Append stop code
         patterns.Add(Code128Reader.CODE_PATTERNS[CODE_STOP]);

         // Compute code width
         int codeWidth = 0;
         foreach (int[] pattern in patterns)
         {
            foreach (int width in pattern)
            {
               codeWidth += width;
            }
         }

         // Compute result
         var result = new bool[codeWidth];
         int pos = 0;
         foreach (int[] pattern in patterns)
         {
            pos += appendPattern(result, pos, pattern, true);
         }

         return result;
      }

      private static bool isDigits(String value, int start, int length)
      {
         int end = start + length;
         int last = value.Length;
         for (int i = start; i < end && i < last; i++)
         {
            char c = value[i];
            if (c < '0' || c > '9')
            {
               if (c != ESCAPE_FNC_1)
               {
                  return false;
               }
               end++; // ignore FNC_1
            }
         }
         return end <= last; // end > last if we've run out of string
      }
   }
}