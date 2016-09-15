/*
 * Copyright 2015 ZXing authors
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
   /// This object renders a CODE93 code as a BitMatrix
   /// </summary>
   public class Code93Writer : OneDimensionalCodeWriter
   {
      public override BitMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
      {
         if (format != BarcodeFormat.CODE_93)
         {
            throw new ArgumentException("Can only encode CODE_93, but got " + format);
         }
         return base.encode(contents, format, width, height, hints);
      }

      public override bool[] encode(String contents)
      {
         int length = contents.Length;
         if (length > 80)
         {
            throw new ArgumentException(
               "Requested contents should be less than 80 digits long, but got " + length);
         }
         //each character is encoded by 9 of 0/1's
         int[] widths = new int[9];

         //lenght of code + 2 start/stop characters + 2 checksums, each of 9 bits, plus a termination bar
         int codeWidth = (contents.Length + 2 + 2)*9 + 1;

         bool[] result = new bool[codeWidth];

         //start character (*)
         toIntArray(Code93Reader.CHARACTER_ENCODINGS[47], widths);
         int pos = appendPattern(result, 0, widths, true);

         for (int i = 0; i < length; i++)
         {
            int indexInString = Code93Reader.ALPHABET_STRING.IndexOf(contents[i]);
            toIntArray(Code93Reader.CHARACTER_ENCODINGS[indexInString], widths);
            pos += appendPattern(result, pos, widths, true);
         }

         //add two checksums
         int check1 = computeChecksumIndex(contents, 20);
         toIntArray(Code93Reader.CHARACTER_ENCODINGS[check1], widths);
         pos += appendPattern(result, pos, widths, true);

         //append the contents to reflect the first checksum added
         contents += Code93Reader.ALPHABET_STRING[check1];

         int check2 = computeChecksumIndex(contents, 15);
         toIntArray(Code93Reader.CHARACTER_ENCODINGS[check2], widths);
         pos += appendPattern(result, pos, widths, true);

         //end character (*)
         toIntArray(Code93Reader.CHARACTER_ENCODINGS[47], widths);
         pos += appendPattern(result, pos, widths, true);

         //termination bar (single black bar)
         result[pos] = true;

         return result;
      }

      private static void toIntArray(int a, int[] toReturn)
      {
         for (int i = 0; i < 9; i++)
         {
            int temp = a & (1 << (8 - i));
            toReturn[i] = temp == 0 ? 0 : 1;
         }
      }

      protected static new int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
      {
         foreach (var bit in pattern)
         {
            target[pos++] = bit != 0;
         }
         return 9;
      }

      private static int computeChecksumIndex(String contents, int maxWeight)
      {
         int weight = 1;
         int total = 0;

         for (int i = contents.Length - 1; i >= 0; i--)
         {
            int indexInString = Code93Reader.ALPHABET_STRING.IndexOf(contents[i]);
            total += indexInString*weight;
            if (++weight > maxWeight)
            {
               weight = 1;
            }
         }
         return total%47;
      }
   }
}