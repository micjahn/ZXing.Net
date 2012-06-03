/*
* Copyright 2007 ZXing authors
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
using System.Windows.Media.Imaging;

using ZXing.OneD;

namespace ZXing.Common
{
   public sealed partial class BitMatrix
   {
      public WriteableBitmap ToBitmap()
      {
         return ToBitmap(BarcodeFormat.EAN_8, null);
      }

      /// <summary>
      /// Converts this ByteMatrix to a black and white bitmap.
      /// </summary>
      /// <returns>A black and white bitmap converted from this ByteMatrix.</returns>
      public WriteableBitmap ToBitmap(BarcodeFormat format, String content)
      {
         const int BLACK = 0x00FF0000 << 8;
         const int WHITE = int.MaxValue;
         int width = Width;
         int height = Height;
         bool outputContent = !String.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 ||
                                                                 format == BarcodeFormat.CODE_128 ||
                                                                 format == BarcodeFormat.EAN_13 ||
                                                                 format == BarcodeFormat.EAN_8 ||
                                                                 format == BarcodeFormat.CODABAR ||
                                                                 format == BarcodeFormat.ITF ||
                                                                 format == BarcodeFormat.UPC_A);
         int emptyArea = outputContent ? 16 : 0;

         // create the bitmap and lock the bits because we need the stride
         // which is the width of the image and possible padding bytes
         var bmp = new WriteableBitmap(width, height);
         var pixels = bmp.Pixels;

         for (int y = 0; y < height - emptyArea; y++)
         {
            var offset = y * width;
            for (var x = 0; x < width; x++)
            {
               var color = this[x, y] ? BLACK : WHITE;
               pixels[offset + x] = color;
            }
         }
         bmp.Invalidate();

         if (outputContent)
         {
            switch (format)
            {
               case BarcodeFormat.EAN_8:
                  if (content.Length < 8)
                     content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                  content = content.Insert(4, "   ");
                  break;
               case BarcodeFormat.EAN_13:
                  if (content.Length < 13)
                     content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                  content = content.Insert(7, "   ");
                  content = content.Insert(1, "   ");
                  break;
            }
            /* doesn't correctly work at the moment
             * renders at the wrong position
            var txt1 = new TextBlock {Text = content, FontSize = 10, Foreground = new SolidColorBrush(Colors.Black)};
            bmp.Render(txt1, new RotateTransform { Angle = 0, CenterX = width / 2, CenterY = height - 14});
            bmp.Invalidate();
             * */
         }

         return bmp;
      }
   }
}