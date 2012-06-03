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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using ZXing.OneD;

namespace ZXing.Common
{
   public sealed partial class BitMatrix
   {
      public Bitmap ToBitmap()
      {
         return ToBitmap(BarcodeFormat.EAN_8, null);
      }

      /// <summary>
      /// Converts this ByteMatrix to a black and white bitmap.
      /// </summary>
      /// <returns>A black and white bitmap converted from this ByteMatrix.</returns>
      public Bitmap ToBitmap(BarcodeFormat format, String content)
      {
         const byte BLACK = 0;
         const byte WHITE = 255;
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
         var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
         var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
         try
         {
            var pixels = new byte[bmpData.Stride*height];
            var padding = bmpData.Stride - (3 * width);
            var index = 0;

            for (int y = 0; y < height - emptyArea; y++)
            {
               for (var x = 0; x < width; x++)
               {
                  var color = this[x, y] ? BLACK : WHITE;
                  pixels[index++] = color;
                  pixels[index++] = color;
                  pixels[index++] = color;
               }
               index += padding;
            }
            for (int y = (height - emptyArea) * bmpData.Stride; y < pixels.Length; y++)
            {
               pixels[y] = WHITE;
            }

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
         }
         finally
         {
            //Unlock the pixels
            bmp.UnlockBits(bmpData);
         }

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
            var font = new Font("Arial", 10, FontStyle.Regular);
            using (var g = Graphics.FromImage(bmp))
            {
               var drawFormat = new StringFormat {Alignment = StringAlignment.Center};
               g.DrawString(content, font, Brushes.Black, width / 2, height - 14, drawFormat);
            }
         }

         return bmp;
      }
   }
}