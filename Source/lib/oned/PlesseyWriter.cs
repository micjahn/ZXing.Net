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

using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// This object renders a Plessey code as a <see cref="BitMatrix"/>.
   /// </summary>
   public sealed class PlesseyWriter : OneDimensionalCodeWriter
   {
      private const String ALPHABET_STRING = "0123456789ABCDEF";
      private static readonly int[] startWidths = new[] { 14, 11, 14, 11, 5, 20, 14, 11 };
      private static readonly int[] terminationWidths = new[] { 25 };
      private static readonly int[] endWidths = new[] { 20, 5, 20, 5, 14, 11, 14, 11 };
      private static readonly int[][] numberWidths = new[]
                                                        {
                                                           new[] { 5, 20, 5, 20, 5, 20, 5, 20 },     // 0
                                                           new[] { 14, 11, 5, 20, 5, 20, 5, 20 },    // 1
                                                           new[] { 5, 20, 14, 11, 5, 20, 5, 20 },    // 2
                                                           new[] { 14, 11, 14, 11, 5, 20, 5, 20 },   // 3
                                                           new[] { 5, 20, 5, 20, 14, 11, 5, 20 },    // 4
                                                           new[] { 14, 11, 5, 20, 14, 11, 5, 20 },   // 5
                                                           new[] { 5, 20, 14, 11, 14, 11, 5, 20 },   // 6
                                                           new[] { 14, 11, 14, 11, 14, 11, 5, 20 },  // 7
                                                           new[] { 5, 20, 5, 20, 5, 20, 14, 11 },    // 8
                                                           new[] { 14, 11, 5, 20, 5, 20, 14, 11 },   // 9
                                                           new[] { 5, 20, 14, 11, 5, 20, 14, 11 },   // A / 10
                                                           new[] { 14, 11, 14, 11, 5, 20, 14, 11 },  // B / 11
                                                           new[] { 5, 20, 5, 20, 14, 11, 14, 11 },   // C / 12
                                                           new[] { 14, 11, 5, 20, 14, 11, 14, 11 },  // D / 13
                                                           new[] { 5, 20, 14, 11, 14, 11, 14, 11 },  // E / 14
                                                           new[] { 14, 11, 14, 11, 14, 11, 14, 11 }, // F / 15
                                                        };
      private static readonly byte[] crcGrid = new byte[] { 1, 1, 1, 1, 0, 1, 0, 0, 1 };
      private static readonly int[] crc0Widths = new[] {5, 20};
      private static readonly int[] crc1Widths = new[] {14, 11};

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
         if (format != BarcodeFormat.PLESSEY)
         {
            throw new ArgumentException("Can only encode Plessey, but got " + format);
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
         var length = contents.Length;
         for (var i = 0; i < length; i++)
         {
            int indexInString = ALPHABET_STRING.IndexOf(contents[i]);
            if (indexInString < 0)
               throw new ArgumentException("Requested contents contains a not encodable character: '" + contents[i] + "'");
         }

         // quiet zone + start pattern + data + crc + termination bar + end pattern + quiet zone
         var codeWidth = 100 + 100 + length * 100 + 25 * 8 + 25 + 100 + 100;
         var result = new bool[codeWidth];
         var crcBuffer = new byte[4*length + 8];
         var crcBufferPos = 0;
         var pos = 100;
         // start pattern
         pos += appendPattern(result, pos, startWidths, true);
         // data
         for (var i = 0; i < length; i++)
         {
            var indexInString = ALPHABET_STRING.IndexOf(contents[i]);
            var widths = numberWidths[indexInString];
            pos += appendPattern(result, pos, widths, true);
            // remember the position number for crc calculation
            crcBuffer[crcBufferPos++] = (byte)(indexInString & 1);
            crcBuffer[crcBufferPos++] = (byte)((indexInString >> 1) & 1);
            crcBuffer[crcBufferPos++] = (byte)((indexInString >> 2) & 1);
            crcBuffer[crcBufferPos++] = (byte)((indexInString >> 3) & 1);
         }
         // CRC calculation
         for (var i = 0; i < (4 * length); i++)
         {
            if (crcBuffer[i] != 0)
            {
               for (var j = 0; j < 9; j++)
               {
                  crcBuffer[i + j] ^= crcGrid[j];
               }
            }
         }
         // append CRC pattern
         for (var i = 0; i < 8; i++)
         {
            switch (crcBuffer[length * 4 + i])
            {
               case 0:
                  pos += appendPattern(result, pos, crc0Widths, true);
                  break;
               case 1:
                  pos += appendPattern(result, pos, crc1Widths, true);
                  break;
            }
         }
         // termination bar
         pos += appendPattern(result, pos, terminationWidths, true);
         // end pattern
         appendPattern(result, pos, endWidths, false);
         return result;
      }
   }
}