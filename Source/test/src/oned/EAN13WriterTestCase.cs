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

using NUnit.Framework;

using ZXing.Common.Test;

namespace ZXing.OneD.Test
{
   /// <summary>
   /// <author>Ari Pollak</author>
   /// </summary>
   [TestFixture]
   public sealed class EAN13WriterTestCase
   {
      [Test]
      public void testEncode()
      {
         var testStr = "00010100010110100111011001100100110111101001110101010110011011011001000010101110010011101000100101000";
         var result = new EAN13Writer().encode("5901234123457", BarcodeFormat.EAN_13, testStr.Length, 0);
         Assert.AreEqual(testStr, BitMatrixTestCase.matrixToString(result));
      }
   }
}
