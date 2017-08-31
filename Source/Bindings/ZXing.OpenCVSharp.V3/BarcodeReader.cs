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

using OpenCvSharp;

namespace ZXing.OpenCV
{
   /// <summary>
   /// a barcode reader class which can be used with the Mat type from OpenCVSharp
   /// </summary>
   public class BarcodeReader :  BarcodeReader<Mat>
   {
      /// <summary>
      /// define a custom function for creation of a luminance source with our specialized Mat-supporting class
      /// </summary>
      private static readonly Func<Mat, LuminanceSource> defaultCreateLuminanceSource =
         (image) => new MatLuminanceSource(image);

      /// <summary>
      /// constructor which uses a custom luminance source with Mat support
      /// </summary>
      public BarcodeReader()
         : base(null, defaultCreateLuminanceSource, null)
      {
      }
   }
}
