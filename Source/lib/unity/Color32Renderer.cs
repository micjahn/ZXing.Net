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
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>
      /// The foreground color.
      /// </value>
      [System.CLSCompliant(false)]
      public Color32 Foreground { get; set; }
      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>
      /// The background color.
      /// </value>
      [System.CLSCompliant(false)]
      public Color32 Background { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="Color32Renderer"/> class.
      /// </summary>
      public Color32Renderer()
      {
         Foreground = Color.black;
         Background = Color.white;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      [System.CLSCompliant(false)]
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
      [System.CLSCompliant(false)]
      public Color32[] Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var result = new Color32[matrix.Width * matrix.Height];
         var offset = matrix.Height - 1;
         var foreground = Foreground;
         var background = Background;
         var divisionLeftover = matrix.Width % 32;
         var bitRowRemainder = divisionLeftover > 0 ? divisionLeftover : 32;

         for (int y = 0; y < matrix.Height; y++)
         {
            var ba = matrix.getRow(offset - y, null);
            int[] bits = ba.Array;

            for (int x = 0; x < bits.Length; x++)
            {
               int finalIndex = 32;

               if (x == bits.Length - 1)
               {
                  finalIndex = bitRowRemainder;
               }

               for (int i = 0; i < finalIndex; i++)
               {
                  int bit = (bits[x] >> i) & 1;

                  if (bit == 1)
                  {
                     result[matrix.Width * y + x * 32 + i] = new Color32(foreground.r, foreground.g, foreground.b, foreground.a);
                  }
                  else
                  {
                     result[matrix.Width * y + x * 32 + i] = new Color32(background.r, background.g, background.b, background.a);
                  }
               }
            }
         }

         return result;
      }
   }
}
