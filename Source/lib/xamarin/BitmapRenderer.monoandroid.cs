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

using Android.Graphics;

using ZXing.Common;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="Bitmap" /> image
   /// </summary>
   public class BitmapRenderer : IBarcodeRenderer<Bitmap>
   {
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      [System.CLSCompliant(false)]
      public Color Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      [System.CLSCompliant(false)]
      public Color Background { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
      /// </summary>
      public BitmapRenderer()
      {
         Foreground = Color.Black;
         Background = Color.White;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      [System.CLSCompliant(false)]
      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, new EncodingOptions());
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
      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var width = matrix.Width;
         var height = matrix.Height;
         var pixels = new int[width * height];
         var outputIndex = 0;
         var fColor = Foreground.ToArgb();
         var bColor = Background.ToArgb();

         for (var y = 0; y < height; y++)
         {
            for (var x = 0; x < width; x++)
            {
               pixels[outputIndex] = matrix[x, y] ? fColor : bColor;
               outputIndex++;
            }
         }

         var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
         bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
         return bitmap;
      }
   }
}