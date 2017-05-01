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

using ZXing.Common;

namespace ZXing
{
   /// <summary>
   /// A base class for specific barcode writers with specific formats of barcode images.
   /// </summary>
   public class BarcodeWriterGeneric : IBarcodeWriterGeneric
   {
      private EncodingOptions options;

      /// <summary>
      /// Gets or sets the barcode format.
      /// The value is only suitable if the MultiFormatWriter is used.
      /// </summary>
      public BarcodeFormat Format { get; set; }

      /// <summary>
      /// Gets or sets the options container for the encoding and renderer process.
      /// </summary>
      public EncodingOptions Options
      {
         get
         {
            return (options ?? (options = new EncodingOptions { Height = 100, Width = 100 }));
         }
         set
         {
            options = value;
         }
      }

      /// <summary>
      /// Gets or sets the writer which encodes the content to a BitMatrix.
      /// If no value is set the MultiFormatWriter is used.
      /// </summary>
      public Writer Encoder { get; set; }

      /// <summary>
      /// 
      /// </summary>
      public BarcodeWriterGeneric()
      {
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="encoder"></param>
      public BarcodeWriterGeneric(Writer encoder)
      {
         Encoder = encoder;
      }

      /// <summary>
      /// Encodes the specified contents and returns a BitMatrix array.
      /// That array has to be rendered manually or with a IBarcodeRenderer.
      /// </summary>
      /// <param name="contents">The contents.</param>
      /// <returns></returns>
      public BitMatrix Encode(string contents)
      {
         var encoder = Encoder ?? new MultiFormatWriter();
         var currentOptions = Options;
         return encoder.encode(contents, Format, currentOptions.Width, currentOptions.Height, currentOptions.Hints);
      }
   }
}
