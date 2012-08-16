/*
* Copyright 2009 ZXing authors
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

using System.Collections.Generic;

namespace ZXing.Multi
{
   /// <summary>
   /// Implementation of this interface attempt to read several barcodes from one image.
   /// <author>Sean Owen</author>
   /// 	<seealso cref="Reader"/>
   /// </summary>
   public interface MultipleBarcodeReader
   {
      /// <summary>
      /// Decodes the multiple.
      /// </summary>
      /// <param name="image">The image.</param>
      /// <returns></returns>
      Result[] decodeMultiple(BinaryBitmap image);

      /// <summary>
      /// Decodes the multiple.
      /// </summary>
      /// <param name="image">The image.</param>
      /// <param name="hints">The hints.</param>
      /// <returns></returns>
      Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints);
   }
}