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

using ZXing.Common;

namespace ZXing.Rendering
{
#if !(WINDOWS_PHONE || WindowsCE)
   /// <summary>
   /// Interface for a class to convert a BitMatrix to an output image format
   /// </summary>
   public interface IBarcodeRenderer<out TOutput>
#else
   /// <summary>
   /// Interface for a class to convert a BitMatrix to an output image format
   /// </summary>
   public interface IBarcodeRenderer<TOutput>
#endif
   {
      /// <summary>
      /// Renders the specified matrix to its graphically representation
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The encoded content of the barcode which should be included in the image.
      /// That can be the numbers below a 1D barcode or something other.</param>
      /// <returns></returns>
      TOutput Render(BitMatrix matrix, BarcodeFormat format, string content);

      /// <summary>
      /// Renders the specified matrix to its graphically representation
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The encoded content of the barcode which should be included in the image.
      /// That can be the numbers below a 1D barcode or something other.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      TOutput Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options);
   }
}
