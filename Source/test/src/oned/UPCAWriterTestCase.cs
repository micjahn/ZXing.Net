/*
 * Copyright 2010 ZXing authors
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
   /// <author>qwandor@google.com (Andrew Walbran)</author>
   /// </summary>
   [TestFixture]
   public sealed class UPCAWriterTestCase
   {
      [Test]
      public void testEncode()
      {
         var testStr = "00010101000110110111011000100010110101111011110101010111001011101001001110110011011011001011100101000";
         var result = new UPCAWriter().encode("485963095124", BarcodeFormat.UPC_A, testStr.Length, 0);
         Assert.AreEqual(testStr, BitMatrixTestCase.matrixToString(result));
      }

      [Test]
      public void testAddChecksumAndEncode()
      {
         var testStr = "00010100110010010011011110101000110110001010111101010100010010010001110100111001011001101101100101000";
         var result = new UPCAWriter().encode("12345678901", BarcodeFormat.UPC_A, testStr.Length, 0);
         Assert.AreEqual(testStr, BitMatrixTestCase.matrixToString(result));
      }
   }
}
