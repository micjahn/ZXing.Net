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

namespace ZXing.Interop.Decoding
{
   /// <summary>
   /// Encapsulates the result of decoding a barcode within an image.
   /// </summary>
   [ComVisible(true)]
   [Guid("4A5EB14C-9F1C-437E-969D-9882D9AB8AF5")]
   [InterfaceType(ComInterfaceType.InterfaceIsDual)]
   public interface IResult
   {
      /// <returns>raw text encoded by the barcode, if applicable, otherwise <code>null</code></returns>
      String Text { get; }

      /// <returns>raw bytes encoded by the barcode, if applicable, otherwise <code>null</code></returns>
      byte[] RawBytes { get; }

      /// <returns>
      /// points related to the barcode in the image. These are typically points
      /// identifying finder patterns or the corners of the barcode. The exact meaning is
      /// specific to the type of barcode that was decoded.
      /// </returns>
      ResultPoint[] ResultPoints { get; }

      /// <returns>{@link BarcodeFormat} representing the format of the barcode that was decoded</returns>
      Common.BarcodeFormat BarcodeFormat { get; }

      /// <returns>
      /// {@link Hashtable} mapping {@link ResultMetadataType} keys to values. May be
      /// <code>null</code>. This contains optional metadata about what was detected about the barcode,
      /// like orientation.
      /// </returns>
      ResultMetadataItem[] ResultMetadata { get; }

      /// <summary>
      /// Gets the timestamp.
      /// </summary>
      long Timestamp { get; }

      /// <summary>
      /// how many bits of <see cref="RawBytes"/> are valid; typically 8 times its length
      /// </summary>
      int NumBits { get; }
   }
}