/*
 * Copyright 2016 ZXing authors
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
using NUnit.Framework;
using ZXing.Common.Test;

namespace ZXing.OneD.Test
{
   public class UPCEWriterTestCase
   {
      [TestCase("05096893", "0000000000010101110010100111000101101011110110111001011101010100000000000", TestName = "testEncode")]
      [TestCase("12345670", "0000000000010100100110111101010001101110010000101001000101010100000000000", TestName = "testEncodeSystem1")]
      [TestCase("0509689", "0000000000010101110010100111000101101011110110111001011101010100000000000", TestName = "testAddChecksumAndEncode")]
      public void testEncode(String content, String encoding)
      {
         var result = new UPCEWriter().encode(content, BarcodeFormat.UPC_E, encoding.Length, 0);
         Assert.AreEqual(encoding, BitMatrixTestCase.matrixToString(result));
      }
   }
}