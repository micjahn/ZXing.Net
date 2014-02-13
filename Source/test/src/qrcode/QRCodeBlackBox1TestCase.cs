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

namespace ZXing.QrCode.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class QRCodeBlackBox1TestCase : AbstractBlackBoxTestCase
   {
      public QRCodeBlackBox1TestCase()
         : base("test/data/blackbox/qrcode-1", new MultiFormatReader(), BarcodeFormat.QR_CODE)
      {
         addTest(18, 18, 0.0f);     // Java: addTest(17, 17, 0.0f);
         addTest(15, 15, 90.0f);    // Java: addTest(14, 14, 0.0f);
         addTest(18, 18, 180.0f);   // Java: addTest(17, 17, 0.0f);
         addTest(15, 15, 270.0f);   // Java: addTest(14, 14, 0.0f);
      }
   }
}
