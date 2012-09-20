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
using System.Collections.Generic;

#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
using System.Drawing;
#else
using UnityEngine;
#endif
#elif NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to decode the barcode inside a bitmap object
   /// </summary>
   public interface IBarcodeReader
   {
      /// <summary>
      /// event is executed when a result point was found
      /// </summary>
      event Action<ResultPoint> ResultPointFound;

      /// <summary>
      /// event is executed when a result was found via decode
      /// </summary>
      event Action<Result> ResultFound;

      /// <summary>
      /// Gets or sets a flag which cause a deeper look into the bitmap
      /// </summary>
      /// <value>
      ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
      /// </value>
      bool TryHarder { get; set; }

      /// <summary>
      /// Image is a pure monochrome image of a barcode. Doesn't matter what it maps to;
      /// use {@link Boolean#TRUE}.
      /// </summary>
      /// <value>
      ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
      /// </value>
      bool PureBarcode { get; set; }

      /// <summary>
      /// Specifies what character encoding to use when decoding, where applicable (type String)
      /// </summary>
      /// <value>
      /// The character set.
      /// </value>
      string CharacterSet { get; set; }

      /// <summary>
      /// Image is known to be of one of a few possible formats.
      /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
      /// </summary>
      /// <value>
      /// The possible formats.
      /// </value>
      IList<BarcodeFormat> PossibleFormats { get; set; }

      /// <summary>
      /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
      /// </summary>
      /// <param name="rawRGB">The image as RGB24 array.</param>
      /// <returns>the result data or null</returns>
      Result Decode(byte[] rawRGB, int width, int height);

#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result Decode(Bitmap barcodeBitmap);
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="rawColor32">The image as Color32 array.</param>
      /// <returns>the result data or null</returns>
      Result Decode(Color32[] rawColor32, int width, int height);
#endif
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result Decode(WriteableBitmap barcodeBitmap);
#endif
   }
}
