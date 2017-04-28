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

using Microsoft.Kinect;

namespace ZXing.Kinect
{
   /// <summary>
   /// A smart class to decode the barcode inside a ColorImageFrame object
   /// </summary>
   public class BarcodeReader : BarcodeReader<ColorImageFrame>
   {
      private static readonly Func<ColorImageFrame, LuminanceSource> defaultCreateLuminanceSource =
         bitmap => new ColorImageFrameLuminanceSource(bitmap);

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeReader()
         : this(null, defaultCreateLuminanceSource, null)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
      /// If null then MultiFormatReader is used</param>
      /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a ColorImageFrame.
      /// If null, default is used</param>
      /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
      /// If null then HybridBinarizer is used</param>
      public BarcodeReader(Reader reader,
         Func<ColorImageFrame, LuminanceSource> createLuminanceSource,
         Func<LuminanceSource, Binarizer> createBinarizer
         )
         : base(reader, createLuminanceSource ?? defaultCreateLuminanceSource, createBinarizer)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
      /// If null then MultiFormatReader is used</param>
      /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
      /// If null, default is used</param>
      /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
      /// If null then HybridBinarizer is used</param>
      public BarcodeReader(Reader reader,
         Func<ColorImageFrame, LuminanceSource> createLuminanceSource,
         Func<LuminanceSource, Binarizer> createBinarizer,
         Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> createRGBLuminanceSource
         )
         : base(reader, createLuminanceSource ?? defaultCreateLuminanceSource, createBinarizer, createRGBLuminanceSource)
      {
      }
   }
}
