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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using ZXing.Interop.Common;

namespace ZXing.Interop.Encoding
{
   [ComVisible(true)]
   [Guid("7CA422BA-2ADE-4C33-A45C-D456A3441FD9")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class BarcodeWriter : IBarcodeWriter
   {
      private EncodingOptions options;

      /// <summary>
      /// Gets or sets the barcode format.
      /// The value is only suitable if the MultiFormatWriter is used.
      /// </summary>
      public Common.BarcodeFormat Format { get; set; }

      /// <summary>
      /// Gets or sets the options container for the encoding and renderer process.
      /// </summary>
      public EncodingOptions Options
      {
         get
         {
            return (options ?? (options = new EncodingOptions()));
         }
         set
         {
            options = value;
         }
      }

      public PixelData Write(string contents)
      {
         var writer = new BarcodeWriterPixelData
         {
            Format = Format.ToZXing(),
            Options = options.wrappedEncodingOptions
         };
         return writer.Write(contents).ToInterop();
      }

      public void WritePngToFile(string contents, string fileName)
      {
         if (string.IsNullOrEmpty(fileName))
            throw new ArgumentNullException(nameof(fileName));

         var writer = new ZXing.BarcodeWriter
         {
            Format = Format.ToZXing(),
            Options = options.wrappedEncodingOptions
         };
         using (var bitmap = writer.Write(contents))
         {
            bitmap.Save(fileName, ImageFormat.Png);
         }
      }
   }
}
