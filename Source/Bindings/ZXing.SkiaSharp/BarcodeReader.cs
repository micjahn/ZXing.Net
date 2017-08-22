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

using SkiaSharp;

namespace ZXing.SkiaSharp
{
   /// <summary>
   /// a barcode reader class which can be used with the SKBitmap type from SkiaSharp
   /// </summary>
   public class BarcodeReader :  BarcodeReader<SKBitmap>
   {
      /// <summary>
      /// define a custom function for creation of a luminance source with our specialized SKBitmap-supporting class
      /// </summary>
      private static readonly Func<SKBitmap, LuminanceSource> defaultCreateLuminanceSource =
         (image) => new SKBitmapLuminanceSource(image);

      /// <summary>
      /// constructor which uses a custom luminance source with SKImage support
      /// </summary>
      public BarcodeReader()
         : base(null, defaultCreateLuminanceSource, null)
      {
      }
   }
}
