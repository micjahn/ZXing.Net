/*
 * Copyright 2008 ZXing authors
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

using ZXing.Common.Test;
using ZXing.QrCode;

namespace ZXing.Multi.QrCode.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class GenericMultipleBarcodeReaderTestCase : AbstractBlackBoxTestCase
   {
      public GenericMultipleBarcodeReaderTestCase()
         : base("test/data/blackbox/multi-qrcode-1", new GenericMultipleBarcodeReader(new QRCodeReader()), BarcodeFormat.QR_CODE)
      {
         addTest(0, 0, 0.0f);
         addTest(1, 1, 90.0f);
         addTest(0, 0, 180.0f);
         addTest(1, 1, 270.0f);
      }
   }
}
