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

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to decode multiple barcodes inside a bitmap object
   /// </summary>
   public partial interface IBarcodeReaderGeneric
   {
      /// <summary>
      /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
      /// </summary>
      /// <param name="rawRGB">The barcode bitmap.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="format">The format.</param>
      /// <returns>
      /// the result data or null
      /// </returns>
      Result[] DecodeMultiple(byte[] rawRGB, int width, int height, RGBLuminanceSource.BitmapFormat format);

      /// <summary>
      /// Tries to decode barcodes within an image which is given by a luminance source.
      /// That method gives a chance to prepare a luminance source completely before calling
      /// the time consuming decoding method. On the other hand there is a chance to create
      /// a luminance source which is independent from external resources (like Bitmap objects)
      /// and the decoding call can be made in a background thread.
      /// </summary>
      /// <param name="luminanceSource">The luminance source.</param>
      /// <returns></returns>
      Result[] DecodeMultiple(LuminanceSource luminanceSource);
   }
}
