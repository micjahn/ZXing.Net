/*
 * Copyright 2017 ZXing.Net authors
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

using ImageSharp;
using ImageSharp.PixelFormats;

using ZXing.Common;

namespace ZXing.ImageSharp.Rendering
{
   /// <summary>
   /// IBarcodeRenderer implementation which creates an ImageSharp Image object from the barcode BitMatrix
   /// </summary>
   public class ImageSharpRenderer : ZXing.Rendering.IBarcodeRenderer<Image>
   {
      /// <summary>
      /// renders the image
      /// </summary>
      /// <param name="matrix"></param>
      /// <param name="format"></param>
      /// <param name="content"></param>
      /// <returns></returns>
      public Image Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, new EncodingOptions());
      }

      /// <summary>
      /// renders the image
      /// </summary>
      /// <param name="matrix"></param>
      /// <param name="format"></param>
      /// <param name="content"></param>
      /// <param name="options"></param>
      /// <returns></returns>
      public Image Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int width = matrix.Width;
         int heigth = matrix.Height;
         const uint black = 0xFF000000;
         const uint white = 0xFFFFFFFF;
         
         int pixelsize = 1;

         if (options != null)
         {
            if (options.Width > width)
            {
               width = options.Width;
            }
            if (options.Height > heigth)
            {
               heigth = options.Height;
            }
            // calculating the scaling factor
            pixelsize = width/matrix.Width;
            if (pixelsize > heigth/matrix.Height)
            {
               pixelsize = heigth/matrix.Height;
            }
         }

         var result = new Image(matrix.Width, matrix.Height);
         using (var pixelAccessor = result.Lock())
         {
            for (int y = 0; y < matrix.Height; y++)
            {
               for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
               {
                  var rowOffset = pixelsize*y + pixelsizeHeight;

                  for (var x = 0; x < matrix.Width; x++)
                  {
                     var color = matrix[x, y] ? black : white;
                     for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                     {
                        pixelAccessor[pixelsize*x + pixelsizeWidth, rowOffset] = new Rgba32(color);
                     }
                  }
                  for (var x = pixelsize*matrix.Width; x < width; x++)
                  {
                     pixelAccessor[x, rowOffset] = new Rgba32(white);
                  }
               }
            }
         }

         return result;
      }
   }
}