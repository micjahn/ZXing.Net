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

namespace ZXing.Aztec.Test
{
   /// <summary>
   /// <author>David Olivier</author>
   /// </summary>
   public sealed class AztecBlackBox1TestCase : AbstractBlackBoxTestCase
   {
      public AztecBlackBox1TestCase()
         : base("test/data/blackbox/aztec-1", new AztecReader(), BarcodeFormat.AZTEC)
      {
         addTest(14, 14, 0.0f);
         addTest(14, 14, 90.0f);
         addTest(14, 14, 180.0f);
         addTest(14, 14, 270.0f);
      }
   }
}
