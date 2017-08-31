/*
 * Copyright 2017 ZXing.Net authors
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

using SkiaSharp;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.SkiaSharp.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="SKBitmap" /> image
   /// </summary>
   public class SKBitmapRenderer : ZXing.Rendering.IBarcodeRenderer<SKBitmap>
   {
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      public SKColor Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      public SKColor Background { get; set; }

      /// <summary>
      /// Gets or sets the text font.
      /// </summary>
      /// <value>
      /// The text font.
      /// </value>
      public SKTypeface TextFont { get; set; }

      /// <summary>
      /// Gets or sets the height of the text
      /// </summary>
      public float TextSize { get; set; }

      private static readonly SKTypeface DefaultTextFont;

      static SKBitmapRenderer()
      {
         try
         {
            DefaultTextFont = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
         }
         catch (Exception)
         {
            // have to ignore, no better idea
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SKBitmapRenderer"/> class.
      /// </summary>
      public SKBitmapRenderer()
      {
         Foreground = SKColors.Black;
         Background = SKColors.White;
         TextFont = DefaultTextFont;
         TextSize = 10;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, new EncodingOptions());
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var width = matrix.Width;
         var height = matrix.Height;
         var font = TextFont ?? DefaultTextFont;
         var emptyArea = 0;
         var outputContent = font != null &&
                             (options == null || !options.PureBarcode) &&
                             !String.IsNullOrEmpty(content) &&
                             (format == BarcodeFormat.CODE_39 ||
                              format == BarcodeFormat.CODE_93 ||
                              format == BarcodeFormat.CODE_128 ||
                              format == BarcodeFormat.EAN_13 ||
                              format == BarcodeFormat.EAN_8 ||
                              format == BarcodeFormat.CODABAR ||
                              format == BarcodeFormat.ITF ||
                              format == BarcodeFormat.UPC_A ||
                              format == BarcodeFormat.UPC_E ||
                              format == BarcodeFormat.MSI ||
                              format == BarcodeFormat.PLESSEY);

         if (options != null)
         {
            if (options.Width > width)
            {
               width = options.Width;
            }
            if (options.Height > height)
            {
               height = options.Height;
            }
         }

         // calculating the scaling factor
         var pixelsizeWidth = width / matrix.Width;
         var pixelsizeHeight = height / matrix.Height;

         using (var surface = SKSurface.Create(width, height, SKColorType.Bgra8888, SKAlphaType.Premul))
         using (var paint = new SKPaint())
         {
            var myCanvas = surface.Canvas;
            paint.IsAntialias = true;
            paint.Color = Foreground;
            paint.Typeface = font;
            paint.TextSize = TextSize < 1 ? 10 : TextSize;

            for (int y = 0; y < matrix.Height; y++)
            {
               // stretching the line by the scaling factor
               for (var pixelsizeHeightProcessed = 0;
                  pixelsizeHeightProcessed < pixelsizeHeight;
                  pixelsizeHeightProcessed++)
               {
                  // going through the columns of the current line
                  for (var x = 0; x < matrix.Width; x++)
                  {
                     var color = matrix[x, y] ? Foreground : Background;
                     // stretching the columns by the scaling factor
                     for (var pixelsizeWidthProcessed = 0;
                        pixelsizeWidthProcessed < pixelsizeWidth;
                        pixelsizeWidthProcessed++)
                     {
                        myCanvas.DrawPoint(x*pixelsizeWidth + pixelsizeWidthProcessed,
                           y*pixelsizeHeight + pixelsizeHeightProcessed, color);
                     }
                  }
                  // fill up to the right if the barcode doesn't fully fit in 
                  for (var x = pixelsizeWidth*matrix.Width; x < width; x++)
                  {
                     myCanvas.DrawPoint(x, y*pixelsizeHeight + pixelsizeHeightProcessed, Background);
                  }
               }
            }
            // fill up to the bottom if the barcode doesn't fully fit in 
            for (var y = pixelsizeHeight*matrix.Height; y < height; y++)
            {
               for (var x = 0; x < width; x++)
               {
                  myCanvas.DrawPoint(x, y, Background);
               }
            }
            // fill the bottom area with the background color if the content should be written below the barcode
            if (outputContent)
            {
               var textAreaHeight = (int)paint.TextSize;

               emptyArea = height + 10 > textAreaHeight ? textAreaHeight : 0;

               if (emptyArea > 0)
               {
                  for (int y = height - emptyArea; y < height; y++)
                  {
                     for (var x = 0; x < width; x++)
                     {
                        myCanvas.DrawPoint(x, y, Background);
                     }
                  }
               }
            }

            // output content text below the barcode
            if (emptyArea > 0)
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
                  default:
                     break;
               }
               var textWidth = paint.MeasureText(content);
               var x = (pixelsizeWidth*matrix.Width - textWidth)/2;
               var y = height - 1;
               x = x < 0 ? 0 : x;
               myCanvas.DrawText(content, x, y, paint);
            }
            myCanvas.Flush();

            return SKBitmap.FromImage(surface.Snapshot());
         }
      }
   }
}