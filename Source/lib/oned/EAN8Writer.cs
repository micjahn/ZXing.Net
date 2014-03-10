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
   /// This object renders an EAN8 code as a <see cref="BitMatrix"/>.
   /// <author>aripollak@gmail.com (Ari Pollak)</author>
   /// </summary>
   public sealed class EAN8Writer : UPCEANWriter
   {
      private const int CODE_WIDTH = 3 + // start guard
          (7 * 4) + // left bars
          5 + // middle guard
          (7 * 4) + // right bars
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
         if (format != BarcodeFormat.EAN_8)
         {
            throw new ArgumentException("Can only encode EAN_8, but got "
                + format);
         }

         return base.encode(contents, format, width, height, hints);
      }

      /// <summary>
      /// </summary>
      /// <returns>
      /// a byte array of horizontal pixels (false = white, true = black)
      /// </returns>
      override public bool[] encode(String contents)
      {
         if (contents.Length < 7 || contents.Length > 8)
         {
            throw new ArgumentException(
                "Requested contents should be 7 (without checksum digit) or 8 digits long, but got " + contents.Length);
         }
         foreach (var ch in contents)
         {
            if (!Char.IsDigit(ch))
               throw new ArgumentException("Requested contents should only contain digits, but got '" + ch + "'");
         }
         if (contents.Length == 7)
            contents = CalculateChecksumDigitModulo10(contents);

         var result = new bool[CODE_WIDTH];
         int pos = 0;

         pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

         for (int i = 0; i <= 3; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, 1));
            pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], false);
         }

         pos += appendPattern(result, pos, UPCEANReader.MIDDLE_PATTERN, false);

         for (int i = 4; i <= 7; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, 1));
            pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], true);
         }
         appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

         return result;
      }
   }
}