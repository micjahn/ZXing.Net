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

using System;
using System.Runtime.InteropServices;

using ZXing.Interop.Common;

namespace ZXing.Interop.Decoding
{
   /// <summary>
   /// Interface for a smart class to decode the barcode inside a bitmap object
   /// </summary>
   [ComVisible(true)]
   [Guid("015B084E-0AAB-4004-8104-ADC87E3E366F")]
   [InterfaceType(ComInterfaceType.InterfaceIsDual)]
   public interface IBarcodeReader
   {
      /// <summary>
      /// Specifies some options which influence the decoding process
      /// </summary>
      DecodingOptions Options { get; set; }

      /// <summary>
      /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
      /// </summary>
      /// <param name="rawRGB">The image as RGB24 array.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="format">The format.</param>
      /// <returns>
      /// the result data or null
      /// </returns>
      Result DecodeImageBytes(
         [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]ref byte[] rawRGB, 
         [In] int width,
         [In] int height,
         [In] BitmapFormat format);

      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result DecodeImageFile(String barcodeBitmapFilePath);
   }
}
