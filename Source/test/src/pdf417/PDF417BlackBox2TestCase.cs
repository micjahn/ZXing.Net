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

using ZXing.Common.Test;

namespace ZXing.PDF417.Test
{
   /// <summary>
   /// This test contains 480x240 images captured from an Android device at preview resolution.
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public sealed class PDF417BlackBox2TestCase : AbstractBlackBoxTestCase
   {
      public PDF417BlackBox2TestCase()
         : base("test/data/blackbox/pdf417-2", new MultiFormatReader(), BarcodeFormat.PDF_417)
      {
         addTest(25, 25, 0, 0, 0.0f);
         addTest(25, 25, 0, 0, 180.0f);
      }
   }
}