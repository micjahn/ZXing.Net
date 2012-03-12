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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZXing.Test
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
      private WriteableBitmap image;
      private readonly int left;
      private readonly int top;
      private Guid guid = Guid.NewGuid();

      public BufferedImageLuminanceSource(WriteableBitmap image)
         : this(image, 0, 0, image.PixelWidth, image.PixelHeight)
      {
      }

      public BufferedImageLuminanceSource(WriteableBitmap image, int left, int top, int width, int height)
         : base(width, height)
      {
         int sourceWidth = image.PixelWidth;
         int sourceHeight = image.PixelHeight;
         if (left + width > sourceWidth || top + height > sourceHeight)
         {
            throw new ArgumentException("Crop rectangle does not fit within image data.");
         }
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

         var offset = (top + y) * image.PixelWidth;
         var x = 0;
         for (var index = offset + left; index < width; index++)
         {
            int srcPixel = image.Pixels[index];
            var c = Color.FromArgb((byte) ((srcPixel >> 0x18) & 0xff),
                                                        (byte) ((srcPixel >> 0x10) & 0xff),
                                                        (byte) ((srcPixel >> 8) & 0xff),
                                                        (byte) (srcPixel & 0xff));
            row[x] = (sbyte) (0.3*c.R + 0.59*c.G + 0.11*c.B);
            x++;
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

            for (int y = 0; y < height; y++)
            {
               int offset = y * width;
               for (int x = 0; x < width; x++)
               {
                  int srcPixel = image.Pixels[x + offset];
                  var c = Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                        (byte)((srcPixel >> 0x10) & 0xff),
                        (byte)((srcPixel >> 8) & 0xff),
                        (byte)(srcPixel & 0xff));
                  matrix[offset + x] = (sbyte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
               }
            }

            return matrix;
         }
      }

      public bool isCropSupported()
      {
         return true;
      }

      public override LuminanceSource crop(int l, int t, int width, int height)
      {
         return new BufferedImageLuminanceSource(image, left + l, top + t, width, height);
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

      public override LuminanceSource rotateCounterClockwise()
      {
         int width = image.PixelWidth;
         int height = image.PixelHeight;
         int full = Math.Max(width, height);
         int sourceWidth = image.PixelWidth;

         Image tempImage2 = new Image();
         tempImage2.Width = full;
         tempImage2.Height = full;
         tempImage2.Source = image;

         // New bitmap has swapped width/height
         WriteableBitmap wb1 = new WriteableBitmap(height, width);


         TransformGroup transformGroup = new TransformGroup();

         // Rotate around centre
         RotateTransform rotate = new RotateTransform();
         rotate.Angle = 90;
         rotate.CenterX = full / 2;
         rotate.CenterY = full / 2;
         transformGroup.Children.Add(rotate);

         // and transform back to top left corner of new image
         TranslateTransform translate = new TranslateTransform();
         translate.X = -(full - height) / 2;
         translate.Y = -(full - width) / 2;
         transformGroup.Children.Add(translate);

         wb1.Render(tempImage2, transformGroup);
         wb1.Invalidate();

         return new BufferedImageLuminanceSource(wb1, top, sourceWidth - (left + width), Height, width);
      }
   }
}