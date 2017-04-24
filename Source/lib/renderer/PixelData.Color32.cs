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

using UnityEngine;

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
      public Color32[] ToColor32()
      {
         var result = new Color32[Width * Height];
         var resultIndex = 0;

         for (int y = 0; y < Height; y++)
         {
            var row = y*Width;
            for (int x = 0; x < Width; x++)
            {
               var pixelIndex = row + 4*x;
               result[resultIndex] = new Color32(Pixels[pixelIndex + 2], Pixels[pixelIndex + 1], Pixels[pixelIndex], Pixels[pixelIndex + 3]);
               resultIndex++;
            }
         }

         return result;
      }
   }
}
