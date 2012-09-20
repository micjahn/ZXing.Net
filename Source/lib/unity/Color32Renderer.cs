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

using ZXing.Common;
using ZXing.Rendering;

namespace ZXing
{
   /// <summary>
   /// a barcode renderer which returns a Color32 array
   /// </summary>
   public class Color32Renderer : IBarcodeRenderer<Color32[]>
   {
      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public Color32[] Render(BitMatrix matrix, BarcodeFormat format, string content)
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
      public Color32[] Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var result = new Color32[matrix.Width * matrix.Height];
         var offset = matrix.Height - 1;
         const byte black = 0;
         const byte white = 255;

         for (int y = 0; y < matrix.Height; y++)
         {
            var ba = matrix.getRow(offset - y, null);
            int[] bits = ba.Array;

            for (int x = 0; x < bits.Length; x++)
            {
               for (int i = 0; i < 32; i++)
               {
                  int bit = (bits[x] >> i) & 1;
                  if (bit == 1)
                     result[256 * y + x * 32 + i] = new Color32(black, black, black, 255);
                  else
                     result[256 * y + x * 32 + i] = new Color32(white, white, white, 255);
               }
            }
         }

         return result;
      }
   }
}
