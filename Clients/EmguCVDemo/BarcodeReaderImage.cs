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

using Emgu.CV;

using ZXing;

namespace EmguCVDemo
{
   /// <summary>
   /// A barcode reader which accepts an Image instance from EmguCV
   /// </summary>
   internal class BarcodeReaderImage : BarcodeReaderGeneric<Image<Emgu.CV.Structure.Bgr, byte>>, IBarcodeReaderImage
   {
      private static readonly Func<Image<Emgu.CV.Structure.Bgr, byte>, LuminanceSource> defaultCreateLuminanceSource =
         (image) => new ImageLuminanceSource(image);

      public BarcodeReaderImage()
         : base(null, defaultCreateLuminanceSource, null)
      {
      }
   }
}
