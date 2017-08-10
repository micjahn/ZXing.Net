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

namespace ZXing.Rendering
{
   /// <summary>
   /// represents the generated code as a byte array with pixel data (4 byte per pixel, BGRA)
   /// </summary>
   public sealed partial class PixelData
   {
      /// <summary>
      /// converts the pixel data to a bitmap object
      /// </summary>
      /// <returns></returns>
      [System.CLSCompliant(false)]
      public Android.Graphics.Bitmap ToBitmap()
      {
         var pixels = Pixels;
         var colors = new int[Width*Height];
         for (var index = 0; index < Width*Height; index++)
         {
            colors[index] =
               pixels[index*4] << 24 |
               pixels[index*4 + 1] << 16 |
               pixels[index*4 + 2] << 8 |
               pixels[index*4 + 3];
         }
         return Android.Graphics.Bitmap.CreateBitmap(colors, Width, Height, Android.Graphics.Bitmap.Config.Argb8888);
      }
   }
}
