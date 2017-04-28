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

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to decode multiple barcodes inside a bitmap object
   /// </summary>
   public partial interface IBarcodeReader
   {
#if __UNIFIED__
      Result[] DecodeMultiple(UIKit.UIImage barcodeImage);
#else
      Result[] DecodeMultiple(MonoTouch.UIKit.UIImage barcodeImage);
#endif
   }
}
