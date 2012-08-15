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

using System.Windows.Media.Imaging;

namespace ZXing
{
   public partial class BitmapLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected BitmapLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      public BitmapLuminanceSource(WriteableBitmap writeableBitmap)
         : base(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight)
      {
         var height = writeableBitmap.PixelHeight;
         var width = writeableBitmap.PixelWidth;

         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         luminances = new byte[width * height];
         System.Windows.Media.Color c;
         for (int y = 0; y < height; y++)
         {
            int offset = y * width;
            for (int x = 0; x < width; x++)
            {
               int srcPixel = writeableBitmap.Pixels[x + offset];
               c = System.Windows.Media.Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                     (byte)((srcPixel >> 0x10) & 0xff),
                     (byte)((srcPixel >> 8) & 0xff),
                     (byte)(srcPixel & 0xff));
               luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
            }
         }
      }

      /// <summary>
      /// Should create a new luminance source with the right class type.
      /// The method is used in methods crop and rotate.
      /// </summary>
      /// <param name="newLuminances">The new luminances.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns></returns>
      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new BitmapLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}