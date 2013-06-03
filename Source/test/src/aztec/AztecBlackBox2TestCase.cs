/*
 * Copyright 2011 ZXing authors
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

namespace ZXing.Aztec.Test
{
   /// <summary>
   /// A test of Aztec barcodes under real world lighting conditions, taken with a mobile phone.
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public sealed class AztecBlackBox2TestCase : AbstractBlackBoxTestCase
   {
      public AztecBlackBox2TestCase()
         : base("test/data/blackbox/aztec-2", new AztecReader(), BarcodeFormat.AZTEC)
      {
         addTest(5, 5, 0.0f);
         addTest(4, 4, 90.0f);
         addTest(6, 6, 180.0f);
         addTest(3, 3, 270.0f);
      }
   }
}
