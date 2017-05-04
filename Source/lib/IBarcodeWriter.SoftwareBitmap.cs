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

using Windows.Graphics.Imaging;
using ZXing.Common;

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to encode some content into a barcode
   /// </summary>
   [System.CLSCompliant(false)]
   public partial interface IBarcodeWriterSoftwareBitmap
   {
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      SoftwareBitmap Write(string contents);

      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      SoftwareBitmap Write(BitMatrix matrix);
   }
}
