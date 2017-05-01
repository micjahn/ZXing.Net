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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing.Common;
using ZXing.OneD;

namespace ZXing.CoreCompat.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="Bitmap" /> image
   /// </summary>
   public class BitmapRenderer : ZXing.Rendering.IBarcodeRenderer<Bitmap>
   {
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
      /// Gets or sets the resolution which should be used to create the bitmap
      /// If nothing is set the current system settings are used
      /// </summary>
      public float? DpiX { get; set; }

      /// <summary>
      /// Gets or sets the resolution which should be used to create the bitmap
      /// If nothing is set the current system settings are used
      /// </summary>
      public float? DpiY { get; set; }

      /// <summary>
      /// Gets or sets the text font.
      /// </summary>
      /// <value>
      /// The text font.
      /// </value>
      public Font TextFont { get; set; }

      private static readonly Font DefaultTextFont;

      static BitmapRenderer()
      {
         try
         {
            DefaultTextFont = new Font("Arial", 10, FontStyle.Regular);
         }
         catch (Exception exc)
         {
            // have to ignore, no better idea
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
      /// </summary>
      public BitmapRenderer()
      {
         Foreground = Color.Black;
         Background = Color.White;
         TextFont = DefaultTextFont;
      }

      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, new EncodingOptions());
      }

      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
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
         var pixelsizeWidth = width/matrix.Width;
         var pixelsizeHeight = height/matrix.Height;

         // create the bitmap and lock the bits because we need the stride
         // which is the width of the image and possible padding bytes
         var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

         var dpiX = DpiX ?? DpiY;
         var dpiY = DpiY ?? DpiX;
         if (dpiX != null)
            bmp.SetResolution(dpiX.Value, dpiY.Value);

         using (var g = Graphics.FromImage(bmp))
         {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
               PixelFormat.Format24bppRgb);
            try
            {
               var pixels = new byte[bmpData.Stride*height];
               var padding = bmpData.Stride - (3*width);
               var index = 0;
               var color = Background;

               // going through the lines of the matrix
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
                        color = matrix[x, y] ? Foreground : Background;
                        // stretching the columns by the scaling factor
                        for (var pixelsizeWidthProcessed = 0;
                           pixelsizeWidthProcessed < pixelsizeWidth;
                           pixelsizeWidthProcessed++)
                        {
                           pixels[index++] = color.B;
                           pixels[index++] = color.G;
                           pixels[index++] = color.R;
                        }
                     }
                     // fill up to the right if the barcode doesn't fully fit in 
                     for (var x = pixelsizeWidth*matrix.Width; x < width; x++)
                     {
                        pixels[index++] = Background.B;
                        pixels[index++] = Background.G;
                        pixels[index++] = Background.R;
                     }
                     index += padding;
                  }
               }
               // fill up to the bottom if the barcode doesn't fully fit in 
               for (var y = pixelsizeHeight*matrix.Height; y < height; y++)
               {
                  for (var x = 0; x < width; x++)
                  {
                     pixels[index++] = Background.B;
                     pixels[index++] = Background.G;
                     pixels[index++] = Background.R;
                  }
                  index += padding;
               }
               // fill the bottom area with the background color if the content should be written below the barcode
               if (outputContent)
               {
                  var textAreaHeight = font.Height;

                  emptyArea = height + 10 > textAreaHeight ? textAreaHeight : 0;

                  if (emptyArea > 0)
                  {
                     index = (width*3 + padding)*(height - emptyArea);
                     for (int y = height - emptyArea; y < height; y++)
                     {
                        for (var x = 0; x < width; x++)
                        {
                           pixels[index++] = Background.B;
                           pixels[index++] = Background.G;
                           pixels[index++] = Background.R;
                        }
                        index += padding;
                     }
                  }
               }

               //Copy the data from the byte array into BitmapData.Scan0
               Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            }
            finally
            {
               //Unlock the pixels
               bmp.UnlockBits(bmpData);
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
               var brush = new SolidBrush(Foreground);
               var drawFormat = new StringFormat {Alignment = StringAlignment.Center};
               g.DrawString(content, font, brush, pixelsizeWidth*matrix.Width/2, height - emptyArea, drawFormat);
            }
         }

         return bmp;
      }
   }
}