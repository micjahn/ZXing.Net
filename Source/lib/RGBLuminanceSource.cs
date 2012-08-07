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
   /// <summary>
   /// Luminance source class which support different formats of images.
   /// </summary>
   public partial class RGBLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RGBLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected RGBLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="RGBLuminanceSource"/> class.
      /// It supports a byte array with 3 bytes per pixel (RGB24).
      /// </summary>
      /// <param name="rgbRawBytes">The RGB raw bytes.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      public RGBLuminanceSource(byte[] rgbRawBytes, int width, int height)
         : base(width, height)
      {
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         luminances = new byte[width * height];
         for (int y = 0; y < height; y++)
         {
            int offset = y * width;
            for (int x = 0; x < width; x++)
            {
               int r = rgbRawBytes[offset * 3 + x * 3];
               int g = rgbRawBytes[offset * 3 + x * 3 + 1];
               int b = rgbRawBytes[offset * 3 + x * 3 + 2];
               if (r == g && g == b)
               {
                  // Image is already greyscale, so pick any channel.
                  luminances[offset + x] = (byte)r;
               }
               else
               {
                  // Calculate luminance cheaply, favoring green.
                  luminances[offset + x] = (byte)((r + g + g + b) >> 2);
               }
            }
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="RGBLuminanceSource"/> class.
      /// It supports a byte array with 1 byte per pixel (Gray8).
      /// That means the whole array consists of the luminance values (grayscale).
      /// </summary>
      /// <param name="luminanceArray">The luminance array.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="is8Bit">if set to <c>true</c> [is8 bit].</param>
      public RGBLuminanceSource(byte[] luminanceArray, int width, int height, bool is8Bit)
         : base(luminanceArray, width, height)
      {
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
         return new RGBLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}