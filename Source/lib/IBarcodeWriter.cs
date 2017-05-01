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
   /// Interface for a smart class to encode some content into a barcode
   /// </summary>
   public partial interface IBarcodeWriter
   {
      /// <summary>
      /// Get or sets the barcode format which should be generated
      /// (only suitable if MultiFormatWriter is used for property Encoder which is the default)
      /// </summary>
      BarcodeFormat Format { get; set; }

      /// <summary>
      /// Gets or sets the options container for the encoding and renderer process.
      /// </summary>
      EncodingOptions Options { get; set; }

      /// <summary>
      /// Gets or sets the writer which encodes the content to a BitMatrix.
      /// If no value is set the MultiFormatWriter is used.
      /// </summary>
      Writer Encoder { get; set; }

      /// <summary>
      /// Encodes the specified contents.
      /// </summary>
      /// <param name="contents">The contents.</param>
      /// <returns></returns>
      BitMatrix Encode(string contents);
   }
}
