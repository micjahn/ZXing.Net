﻿/*
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
#if !(PORTABLE || NETCOREAPP1_1)
using System.Drawing;
#endif
#else
using UnityEngine;
#endif
#elif NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.Common;

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to decode multiple barcodes inside a bitmap object
   /// </summary>
#if UNITY
   [System.CLSCompliant(false)]
#endif
   public interface IMultipleBarcodeReader
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
      [Obsolete("Please use the Options.TryHarder property instead.")]
      bool TryHarder { get; set; }

      /// <summary>
      /// Image is a pure monochrome image of a barcode. Doesn't matter what it maps to;
      /// use {@link Boolean#TRUE}.
      /// </summary>
      /// <value>
      ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
      /// </value>
      [Obsolete("Please use the Options.PureBarcode property instead.")]
      bool PureBarcode { get; set; }

      /// <summary>
      /// Specifies what character encoding to use when decoding, where applicable (type String)
      /// </summary>
      /// <value>
      /// The character set.
      /// </value>
      [Obsolete("Please use the Options.CharacterSet property instead.")]
      string CharacterSet { get; set; }

      /// <summary>
      /// Image is known to be of one of a few possible formats.
      /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
      /// </summary>
      /// <value>
      /// The possible formats.
      /// </value>
      [Obsolete("Please use the Options.PossibleFormats property instead.")]
      IList<BarcodeFormat> PossibleFormats { get; set; }

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

#if MONOTOUCH
#if __UNIFIED__
      Result[] DecodeMultiple(UIKit.UIImage barcodeImage);
#else
      Result[] DecodeMultiple(MonoTouch.UIKit.UIImage barcodeImage);
#endif
#elif MONOANDROID
      Result[] DecodeMultiple(Android.Graphics.Bitmap barcodeImage);
#else
#if !(PORTABLE || NETCOREAPP1_1)
#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result[] DecodeMultiple(Bitmap barcodeBitmap);
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="rawColor32">The image as Color32 array.</param>
      /// <returns>the result data or null</returns>
      [System.CLSCompliant(false)]
      Result[] DecodeMultiple(Color32[] rawColor32, int width, int height);
#endif
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result[] DecodeMultiple(WriteableBitmap barcodeBitmap);
#endif
#endif
#endif
   }
}
