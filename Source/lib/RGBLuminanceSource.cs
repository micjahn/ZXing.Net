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
   public partial class RGBLuminanceSource : BaseLuminanceSource
   {
      public RGBLuminanceSource(byte[] rgbRawBytes, int width, int height)
         : base(width, height)
      {
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
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

      public RGBLuminanceSource(byte[] luminanceArray, int width, int height, bool is8Bit)
         : base(luminanceArray, width, height)
      {
      }
   }
}