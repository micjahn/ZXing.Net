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
   /// This object renders an UPC-E code as a {@link BitMatrix}.
   /// @author 0979097955s@gmail.com (RX)
   /// </summary>
   public class UPCEWriter : UPCEANWriter
   {

      private const int CODE_WIDTH = 3 + // start guard
                                     (7*6) + // bars
                                     6; // end guard

      public override BitMatrix encode(String contents,
         BarcodeFormat format,
         int width,
         int height,
         IDictionary<EncodeHintType, object> hints)
      {
         if (format != BarcodeFormat.UPC_E)
         {
            throw new ArgumentException("Can only encode UPC_E, but got " + format);
         }

         return base.encode(contents, format, width, height, hints);
      }

      public override bool[] encode(String contents)
      {
         int length = contents.Length;
         switch (length)
         {
            case 7:
               // No check digit present, calculate it and add it
               var check = UPCEANReader.getStandardUPCEANChecksum(UPCEReader.convertUPCEtoUPCA(contents));
               if (check == null)
               {
                  throw new ArgumentException("Checksum can't be calculated");
               }
               contents += check.Value;
               break;
            case 8:
               try
               {
                  if (!UPCEANReader.checkStandardUPCEANChecksum(contents))
                  {
                     throw new ArgumentException("Contents do not pass checksum");
                  }
               }
               catch (FormatException ignored)
               {
                  throw new ArgumentException("Illegal contents", ignored);
               }
               break;
            default:
               throw new ArgumentException("Requested contents should be 8 digits long, but got " + length);
         }

         int firstDigit = int.Parse(contents.Substring(0, 1));
         if (firstDigit != 0 && firstDigit != 1)
         {
            throw new ArgumentException("Number system must be 0 or 1");
         }

         var checkDigit = int.Parse(contents.Substring(7, 1));
         var parities = UPCEReader.NUMSYS_AND_CHECK_DIGIT_PATTERNS[firstDigit][checkDigit];
         var result = new bool[CODE_WIDTH];
         var pos = 0;

         pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

         for (var i = 1; i <= 6; i++)
         {
            var digit = int.Parse(contents.Substring(i, 1));
            if ((parities >> (6 - i) & 1) == 1)
            {
               digit += 10;
            }
            pos += appendPattern(result, pos, UPCEANReader.L_AND_G_PATTERNS[digit], false);
         }

         appendPattern(result, pos, UPCEANReader.END_PATTERN, false);

         return result;
      }
   }
}