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
using System.Drawing;
using System.Drawing.Imaging;

namespace com.google.zxing
{
   /// <summary>
   /// This LuminanceSource implementation is meant for J2SE clients and our blackbox unit tests.
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// <author>Sean Owen</author>
   /// <author>code@elektrowolle.de (Wolfgang Jung)</author>
   /// </summary>
   public sealed class BufferedImageLuminanceSource : LuminanceSource
   {
      private Bitmap image;
      private int left;
      private int top;
      private Guid guid = Guid.NewGuid();

      public BufferedImageLuminanceSource(Bitmap image)
         : this(image, 0, 0, image.Width, image.Height)
      {
      }

      public BufferedImageLuminanceSource(Bitmap image, int left, int top, int width, int height)
         : base(width, height)
      {
         int sourceWidth = image.Width;
         int sourceHeight = image.Height;
         if (left + width > sourceWidth || top + height > sourceHeight)
         {
            throw new ArgumentException("Crop rectangle does not fit within image data.");
         }
         // Create a grayscale copy, no need to calculate the luminance manually
         this.image = MakeGrayscale3(image);
         this.left = left;
         this.top = top;
      }

      override public sbyte[] getRow(int y, sbyte[] row)
      {
         if (y < 0 || y >= Height)
         {
            throw new ArgumentException("Requested row is outside the image: " + y);
         }
         int width = Width;
         if (row == null || row.Length < width)
         {
            row = new sbyte[width];
         }
         unsafe
         {
            // The underlying raster of image consists of bytes with the luminance values
            var data = image.LockBits(new Rectangle(left, top + y, width, 1), ImageLockMode.ReadOnly,
                                      PixelFormat.Format24bppRgb);

            try
            {
               byte* bitmapRow = (byte*) data.Scan0;

               for (int x = 0; x < width; x++)
               {
                  row[x] = (sbyte) bitmapRow[x*3];
               }
            }
            finally
            {
               image.UnlockBits(data);
            }
         }
         return row;
      }

      override public sbyte[] Matrix
      {
         get
         {
            int width = Width;
            int height = Height;
            int area = width * height;
            sbyte[] matrix = new sbyte[area];
            // The underlying raster of image consists of area bytes with the luminance values
            unsafe
            {
               // The underlying raster of image consists of bytes with the luminance values
               var data = image.LockBits(new Rectangle(left, top, width, height), ImageLockMode.ReadOnly,
                                         PixelFormat.Format24bppRgb);
               try
               {
                  for (int y = 0; y < height; y++)
                  {
                     var bitmapRow = (byte*)data.Scan0 + (y * data.Stride);
                     var offset = y*width;
                     for (int x = 0; x < width; x++)
                     {
                        matrix[offset + x] = (sbyte)bitmapRow[x * 3];
                     }
                  }
               }
               finally
               {
                  image.UnlockBits(data);
               }
            }
#if DEBUGDATA
            SaveArray(matrix);
#endif
            return matrix;
         }
      }

      public bool isCropSupported()
      {
         return true;
      }

      public LuminanceSource crop(int left, int top, int width, int height)
      {
         return new BufferedImageLuminanceSource(image, this.left + left, this.top + top, width, height);
      }

      /// <summary>
      /// This is always true, since the image is a gray-scale image.
      ///
      /// <returns>true</returns>
      /// </summary>
      public bool isRotateSupported()
      {
         return true;
      }

      public LuminanceSource rotateCounterClockwise()
      {
         //if (!isRotateSupported()) {
         //  throw new IllegalStateException("Rotate not supported");
         //}
         int sourceWidth = image.Width;

         var rotatedImage = MakeGrayscale3(image);
         rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

         int width = Width;
         return new BufferedImageLuminanceSource(rotatedImage, top, sourceWidth - (left + width), Height, width);
      }

      public Bitmap MakeGrayscale3(Bitmap original)
      {
         //create a blank bitmap the same size as original
         Bitmap newBitmap = new Bitmap(original.Width, original.Height);

         //get a graphics object from the new image
         Graphics g = Graphics.FromImage(newBitmap);

         //create the grayscale ColorMatrix
         ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
               {
                  new float[] {.3f, .3f, .3f, 0, 0},
                  new float[] {.59f, .59f, .59f, 0, 0},
                  new float[] {.11f, .11f, .11f, 0, 0},
                  new float[] {0, 0, 0, 1, 0},
                  new float[] {0, 0, 0, 0, 1}
               });

         //create some image attributes
         ImageAttributes attributes = new ImageAttributes();

         //set the color matrix attribute
         attributes.SetColorMatrix(colorMatrix);

         //draw the original image on the new image
         //using the grayscale color matrix
         g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

         //dispose the Graphics object
         g.Dispose();

#if DEBUGDATA
         newBitmap.Save("data\\grayscale" + guid + ".png");
#endif

         return newBitmap;
      }

#if DEBUGDATA
      private void SaveArray(sbyte[] matrix)
      {
         var builder = new System.Text.StringBuilder(Width);
         for (var y = 0; y < Height; y++)
         {
            var offset = y*Width;
            for (var x = 0; x < Width; x++)
            {
               builder.Append(matrix[offset + x] == 0 ? "O" : "X");
            }
            builder.AppendLine("");
         }
         System.IO.File.WriteAllText("data\\image" + guid + ".txt", builder.ToString());
      }
#endif
   }
}