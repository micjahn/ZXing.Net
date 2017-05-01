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
using ZXing.ImageSharp.Rendering;

namespace ZXing
{
   /// <summary>
   /// extensions methods which are working directly on any BarcodeWriterGeneric implementation
   /// </summary>
   public static class BarcodeWriterExtensions
   {
      /// <summary>
      /// uses the BarcodeWriterGeneric implementation and the <see cref="ImageSharpRenderer"/> class for decoding
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="content"></param>
      /// <returns></returns>
      public static Image WriteAsImageSharp(this IBarcodeWriterGeneric writer, string content)
      {
         var bitmatrix = writer.Encode(content);
         var renderer = new ImageSharpRenderer();
         return renderer.Render(bitmatrix, writer.Format, content, writer.Options);
      }
   }
}
