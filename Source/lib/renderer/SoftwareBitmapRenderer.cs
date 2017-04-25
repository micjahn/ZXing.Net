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
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media;

using ZXing.Common;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="SoftwareBitmap" />
   /// </summary>
   public class SoftwareBitmapRenderer : IBarcodeRenderer<SoftwareBitmap>
   {
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>
      /// The foreground color.
      /// </value>
      public Color Foreground { get; set; }
      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>
      /// The background color.
      /// </value>
      public Color Background { get; set; }
      /// <summary>
      /// Gets or sets the font family.
      /// </summary>
      /// <value>
      /// The font family.
      /// </value>
      public FontFamily FontFamily { get; set; }
      /// <summary>
      /// Gets or sets the size of the font.
      /// </summary>
      /// <value>
      /// The size of the font.
      /// </value>
      public double FontSize { get; set; }

      private static readonly FontFamily DefaultFontFamily = new FontFamily("Arial");

      /// <summary>
      /// Initializes a new instance of the <see cref="WriteableBitmapRenderer"/> class.
      /// </summary>
      public SoftwareBitmapRenderer()
      {
         Foreground = Colors.Black;
         Background = Colors.White;
         FontFamily = DefaultFontFamily;
         FontSize = 10.0;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public SoftwareBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      public virtual SoftwareBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int width = matrix.Width;
         int height = matrix.Height;
         bool outputContent = (options == null || !options.PureBarcode) &&
                              !String.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 ||
                                                                 format == BarcodeFormat.CODE_128 ||
                                                                 format == BarcodeFormat.EAN_13 ||
                                                                 format == BarcodeFormat.EAN_8 ||
                                                                 format == BarcodeFormat.CODABAR ||
                                                                 format == BarcodeFormat.ITF ||
                                                                 format == BarcodeFormat.UPC_A ||
                                                                 format == BarcodeFormat.MSI ||
                                                                 format == BarcodeFormat.PLESSEY);
         int emptyArea = outputContent ? 16 : 0;
         int pixelsize = 1;

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
            // calculating the scaling factor
            pixelsize = width / matrix.Width;
            if (pixelsize > height / matrix.Height)
            {
               pixelsize = height / matrix.Height;
            }
         }

         var foreground = new [] { Foreground.B, Foreground.G, Foreground.R, Foreground.A };
         var background = new [] { Background.B, Background.G, Background.R, Background.A };
         var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, width, height, BitmapAlphaMode.Ignore);
         var index = 0;

         using (var bitmapBuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write))
         using (var reference = bitmapBuffer.CreateReference())
         {
            unsafe
            {
               byte* data;
               uint capacity;
               ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

               for (int y = 0; y < matrix.Height - emptyArea; y++)
               {
                  for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
                  {
                     for (var x = 0; x < matrix.Width; x++)
                     {
                        var color = matrix[x, y] ? foreground : background;
                        for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                        {
                           data[index++] = color[0];
                           data[index++] = color[1];
                           data[index++] = color[2];
                           data[index++] = color[3];
                        }
                     }
                     for (var x = pixelsize * matrix.Width; x < width; x++)
                     {
                        data[index++] = background[0];
                        data[index++] = background[1];
                        data[index++] = background[2];
                        data[index++] = background[3];
                     }
                  }
               }
               for (int y = matrix.Height * pixelsize - emptyArea; y < height; y++)
               {
                  for (var x = 0; x < width; x++)
                  {
                     data[index++] = background[0];
                     data[index++] = background[1];
                     data[index++] = background[2];
                     data[index++] = background[3];
                  }
               }
            }
         }

         return softwareBitmap;
      }
   }
}
