/*
 * Copyright 2012 ZXing.Net authors
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Rendering
{
   public class WriteableBitmapRenderer : IBarcodeRenderer<WriteableBitmap>
   {
      public Color Foreground { get; set; }
      public Color Background { get; set; }
      public FontFamily FontFamily { get; set; }
      public double FontSize { get; set; }
      public FontStretch FontStretch { get; set; }
      public FontStyle FontStyle { get; set; }
      public FontWeight FontWeight { get; set; }

      private static readonly FontFamily DefaultFontFamily = new FontFamily("Arial");

      public WriteableBitmapRenderer()
      {
         Foreground = Colors.Black;
         Background = Colors.White;
         FontFamily = DefaultFontFamily;
         FontSize = 10.0;
         FontStretch = FontStretches.Normal;
         FontStyle = FontStyles.Normal;
         FontWeight = FontWeights.Normal;
      }

      public WriteableBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      virtual public WriteableBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int foreground = Foreground.A << 24 | Foreground.B << 16 | Foreground.G << 8 | Foreground.R;
         int background = Background.A << 24 | Background.B << 16 | Background.G << 8 | Background.R;
         int width = matrix.Width;
         int height = matrix.Height;
         bool outputContent = !String.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 ||
                                                                 format == BarcodeFormat.CODE_128 ||
                                                                 format == BarcodeFormat.EAN_13 ||
                                                                 format == BarcodeFormat.EAN_8 ||
                                                                 format == BarcodeFormat.CODABAR ||
                                                                 format == BarcodeFormat.ITF ||
                                                                 format == BarcodeFormat.UPC_A);
         int emptyArea = outputContent ? 16 : 0;

         var bmp = new WriteableBitmap(width, height);
         var pixels = bmp.Pixels;
         var index = 0;

         for (int y = 0; y < height - emptyArea; y++)
         {
            for (var x = 0; x < width; x++)
            {
               var color = matrix[x, y] ? foreground : background;
               pixels[index++] = color;
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
