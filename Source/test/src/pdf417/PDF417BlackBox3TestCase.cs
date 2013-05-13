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
   /// Regression tests
   /// </summary>
   public sealed class PDF417BlackBox3TestCase : AbstractBlackBoxTestCase
   {
      public PDF417BlackBox3TestCase()
         : base("test/data/blackbox/pdf417-3", new MultiFormatReader(), BarcodeFormat.PDF_417)
      {
         addTest(18, 18, 0, 0, 0.0f);
         addTest(18, 18, 0, 0, 180.0f);
      }
   }
}