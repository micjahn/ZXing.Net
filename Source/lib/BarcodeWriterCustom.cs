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

using System;

using ZXing.Common;
using ZXing.Rendering;

namespace ZXing
{
   /// <summary>
   /// A base class for specific barcode writers with specific formats of barcode images.
   /// </summary>
   /// <typeparam name="TOutput">The type of the output.</typeparam>
   public class BarcodeWriter<TOutput> : BarcodeWriterGeneric, IBarcodeWriter<TOutput>
   {
      /// <summary>
      /// Gets or sets the renderer which should be used to render the encoded BitMatrix.
      /// </summary>
      public IBarcodeRenderer<TOutput> Renderer { get; set; }

      /// <summary>
      /// Encodes the specified contents and returns a rendered instance of the barcode.
      /// For rendering the instance of the property Renderer is used and has to be set before
      /// calling that method.
      /// </summary>
      /// <param name="contents">The contents.</param>
      /// <returns></returns>
      public TOutput Write(string contents)
      {
         if (Renderer == null)
         {
            throw new InvalidOperationException("You have to set a renderer instance.");
         }

         var matrix = Encode(contents);

         return Renderer.Render(matrix, Format, contents, Options);
      }

      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// For rendering the instance of the property Renderer is used and has to be set before
      /// calling that method.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <returns></returns>
      public TOutput Write(BitMatrix matrix)
      {
         if (Renderer == null)
         {
            throw new InvalidOperationException("You have to set a renderer instance.");
         }

         return Renderer.Render(matrix, Format, null, Options);
      }
   }
}
