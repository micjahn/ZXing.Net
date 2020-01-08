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
      [TestCase("485963095124", "00001010100011011011101100010001011010111101111010101011100101110100100111011001101101100101110010100000", TestName = "UPCAtestEncode")]
      [TestCase("12345678901", "00001010011001001001101111010100011011000101011110101010001001001000111010011100101100110110110010100000", TestName = "UPCAtestAddChecksumAndEncode")]
      public void testEncode(string content, string encoding)
      {
         var result = new UPCAWriter().encode(content, BarcodeFormat.UPC_A, encoding.Length, 0);
         Assert.AreEqual(encoding, BitMatrixTestCase.matrixToString(result));
      }
   }
}
