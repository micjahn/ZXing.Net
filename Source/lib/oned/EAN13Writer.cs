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
using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// This object renders an EAN13 code as a <see cref="BitMatrix"/>.
   /// <author>aripollak@gmail.com (Ari Pollak)</author>
   /// </summary>
   public sealed class EAN13Writer : UPCEANWriter
   {
      private const int CODE_WIDTH = 3 + // start guard
          (7 * 6) + // left bars
          5 + // middle guard
          (7 * 6) + // right bars
          3; // end guard

      /// <summary>
      /// Encode the contents following specified format.
      /// {@code width} and {@code height} are required size. This method may return bigger size
      /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
      /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
      /// or {@code height}, {@code IllegalArgumentException} is thrown.
      /// </summary>
      /// <param name="contents"></param>
      /// <param name="format"></param>
      /// <param name="width"></param>
      /// <param name="height"></param>
      /// <param name="hints"></param>
      /// <returns></returns>
      public override BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height,
                              IDictionary<EncodeHintType, object> hints)
      {
         if (format != BarcodeFormat.EAN_13)
         {
            throw new ArgumentException("Can only encode EAN_13, but got " + format);
         }

         return base.encode(contents, format, width, height, hints);
      }

      /// <summary>
      /// Encode the contents to byte array expression of one-dimensional barcode.
      /// Start code and end code should be included in result, and side margins should not be included.
      /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
      /// </summary>
      /// <param name="contents"></param>
      /// <returns></returns>
      override public bool[] encode(String contents)
      {
         if (contents.Length < 12 || contents.Length > 13)
         {
            throw new ArgumentException(
                "Requested contents should be 12 (without checksum digit) or 13 digits long, but got " + contents.Length);
         }
         foreach (var ch in contents)
         {
            if (!Char.IsDigit(ch))
               throw new ArgumentException("Requested contents should only contain digits, but got '" + ch + "'");
         }

         if (contents.Length == 12)
         {
            contents = CalculateChecksumDigitModulo10(contents);
         }
         else
         {
            if (!UPCEANReader.checkStandardUPCEANChecksum(contents))
            {
               throw new ArgumentException("Contents do not pass checksum");
            }
         }

         int firstDigit = Int32.Parse(contents.Substring(0, 1));
         int parities = EAN13Reader.FIRST_DIGIT_ENCODINGS[firstDigit];
         var result = new bool[CODE_WIDTH];
         int pos = 0;

         pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

         // See {@link #EAN13Reader} for a description of how the first digit & left bars are encoded
         for (int i = 1; i <= 6; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, 1));
            if ((parities >> (6 - i) & 1) == 1)
            {
               digit += 10;
            }
            pos += appendPattern(result, pos, UPCEANReader.L_AND_G_PATTERNS[digit], false);
         }

         pos += appendPattern(result, pos, UPCEANReader.MIDDLE_PATTERN, false);

         for (int i = 7; i <= 12; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, 1));
            pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], true);
         }
         appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

         return result;
      }
   }
}
