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

namespace ZXing
{
   public partial class RGBLuminanceSource
   {
      public RGBLuminanceSource(System.Windows.Media.Imaging.WriteableBitmap writeableBitmap)
         : this(writeableBitmap, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight)
      {
      }

      public RGBLuminanceSource(System.Windows.Media.Imaging.WriteableBitmap writeableBitmap, int width, int height)
         : base(width, height)
      {
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
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
   }
}