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
using System.Drawing;

using ZXing.Interop.Common;

namespace ZXing.Interop.Decoding
{
   /// <summary>
   /// A smart class to decode the barcode inside a bitmap object
   /// </summary>
   [ComVisible(true)]
   [Guid("2EDCFB5F-307A-4E21-89CB-AFC99DDDE618")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class BarcodeReader : IBarcodeReader
   {
      private readonly ZXing.BarcodeReader wrappedReader;

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeReader()
      {
         wrappedReader = new ZXing.BarcodeReader();
      }

      public DecodingOptions Options
      {
         get { return new DecodingOptions(wrappedReader.Options); }
         set { wrappedReader.Options = value.wrappedDecodingOptions; }
      }

      public Result DecodeImageBytes([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]ref byte[] rawRGB,
         [In] int width,
         [In] int height,
         [In] BitmapFormat format)
      {
         return new Result(wrappedReader.Decode(rawRGB, width, height, format.ToZXing()));
      }

      public Result DecodeImageFile(String barcodeBitmapFilePath)
      {
         try
         {
            using (var bitmap = (Bitmap) Bitmap.FromFile(barcodeBitmapFilePath))
            {
               return new Result(wrappedReader.Decode(bitmap));
            }
         }
         catch (Exception)
         {
            return null;
         }
      }
   }
}
