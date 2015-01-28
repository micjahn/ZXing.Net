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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="Bitmap" /> image
   /// </summary>
   public class BitmapRenderer : IBarcodeRenderer<Bitmap>
   {
#if WindowsCE
      private static Brush Black = new SolidBrush(Color.Black);
#endif

      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      public Color Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      public Color Background { get; set; }

      /// <summary>
      /// Gets or sets the text font.
      /// </summary>
      /// <value>
      /// The text font.
      /// </value>
      public Font TextFont { get; set; }

      private static readonly Font DefaultTextFont = new Font("Arial", 10, FontStyle.Regular);

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
      /// </summary>
      public BitmapRenderer()
      {
         Foreground = Color.Black;
         Background = Color.White;
         TextFont = DefaultTextFont;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
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
      virtual public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
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
            pixelsize = width/matrix.Width;
            if (pixelsize > height/matrix.Height)
            {
               pixelsize = height/matrix.Height;
            }
         }

         // create the bitmap and lock the bits because we need the stride
         // which is the width of the image and possible padding bytes
         var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
#if !WindowsCE
         bmp.SetResolution(96, 96);
#endif
         var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
         try
         {
            var pixels = new byte[bmpData.Stride*height];
            var padding = bmpData.Stride - (3*width);
            var index = 0;
            var color = Background;

            for (int y = 0; y < matrix.Height - emptyArea; y++)
            {
               for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
               {
                  for (var x = 0; x < matrix.Width; x++)
                  {
                     color = matrix[x, y] ? Foreground : Background;
                     for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                     {
                        pixels[index++] = color.B;
                        pixels[index++] = color.G;
                        pixels[index++] = color.R;
                     }
                  }
                  for (var x = pixelsize * matrix.Width; x < width; x++)
                  {
                     pixels[index++] = Background.B;
                     pixels[index++] = Background.G;
                     pixels[index++] = Background.R;
                  }
                  index += padding;
               }
            }
            for (int y = matrix.Height * pixelsize - emptyArea; y < height; y++)
            {
               for (var x = 0; x < width; x++)
               {
                  pixels[index++] = Background.B;
                  pixels[index++] = Background.G;
                  pixels[index++] = Background.R;
               }
               index += padding;
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
            var font = TextFont ?? DefaultTextFont;
            using (var g = Graphics.FromImage(bmp))
            {
               var drawFormat = new StringFormat {Alignment = StringAlignment.Center};
#if WindowsCE
               g.DrawString(content, font, Black, width / 2, height - 14, drawFormat);
#else
               g.DrawString(content, font, Brushes.Black, width/2, height - 14, drawFormat);
#endif
            }
         }

         return bmp;
      }
   }
}
