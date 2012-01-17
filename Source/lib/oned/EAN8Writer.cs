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

using com.google.zxing.common;

namespace com.google.zxing.oned
{
   /// <summary>
   /// This object renders an EAN8 code as a <see cref="BitMatrix" />.
   ///
   /// <author>aripollak@gmail.com (Ari Pollak)</author>
   /// </summary>
   public sealed class EAN8Writer : UPCEANWriter
   {

      private static int CODE_WIDTH = 3 + // start guard
          (7 * 4) + // left bars
          5 + // middle guard
          (7 * 4) + // right bars
          3; // end guard

      public BitMatrix encode(String contents,
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

      /// <summary> @return a byte array of horizontal pixels (0 = white, 1 = black)</summary>
      override public sbyte[] encode(String contents)
      {
         if (contents.Length != 8)
         {
            throw new ArgumentException(
                "Requested contents should be 8 digits long, but got " + contents.Length);
         }

         sbyte[] result = new sbyte[CODE_WIDTH];
         int pos = 0;

         pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, 1);

         for (int i = 0; i <= 3; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, i + 1));
            pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], 0);
         }

         pos += appendPattern(result, pos, UPCEANReader.MIDDLE_PATTERN, 0);

         for (int i = 4; i <= 7; i++)
         {
            int digit = Int32.Parse(contents.Substring(i, i + 1));
            pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], 1);
         }
         pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, 1);

         return result;
      }
   }
}
