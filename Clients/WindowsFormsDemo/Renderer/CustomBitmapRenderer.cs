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

using System.Drawing;
using System.Drawing.Drawing2D;

using ZXing.Common;

namespace ZXing.Rendering
{
   public class CustomBitmapRenderer : BitmapRenderer
   {
      public Color BackgroundGradientColor { get; set; }
      public Color ForegroundGradientColor { get; set; }

      public CustomBitmapRenderer()
      {
         BackgroundGradientColor = Color.Aqua;
         Background = Color.GreenYellow;
         ForegroundGradientColor = Color.RoyalBlue;
         Foreground = Color.MidnightBlue;
      }
      public override Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int width = matrix.Width;
         int height = matrix.Height;

         var backgroundBrush = new LinearGradientBrush(
            new Rectangle(0, 0, width, height), BackgroundGradientColor, Background, LinearGradientMode.Vertical);
         var foregroundBrush = new LinearGradientBrush(
            new Rectangle(0, 0, width, height), ForegroundGradientColor, Foreground, LinearGradientMode.ForwardDiagonal);

         var bmp = new Bitmap(width, height);
         var gg = Graphics.FromImage(bmp);
         gg.Clear(Background);

         for (int x = 0; x < width - 1; x++)
         {
            for (int y = 0; y < height - 1; y++)
            {
               if (matrix[x, y])
               {
                  gg.FillRectangle(foregroundBrush, x, y, 1, 1);
               }
               else
               {
                  gg.FillRectangle(backgroundBrush, x, y, 1, 1);
               }
            }
         }

         return bmp;
      }
   }
}
