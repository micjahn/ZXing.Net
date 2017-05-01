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

namespace ZXing.ImageSharp
{
   /// <summary>
   /// barcode writer which creates ImageSharp Image instances
   /// </summary>
   public class BarcodeWriter : BarcodeWriter<Image>
   {
      /// <summary>
      /// contructor
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new ImageSharpRenderer();
      }
   }
}