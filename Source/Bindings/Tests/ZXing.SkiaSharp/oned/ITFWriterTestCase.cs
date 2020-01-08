/*
 * Copyright 2017 ZXing authors
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
    public class ITFWriterTestCase
    {
        [TestCase("00123456789012", "0000010101010111000111000101110100010101110001110111010001010001110100011100010101000101011100011101011101000111000101110100010101110001110100000", TestName = "ITFtestEncode")]
        public void testEncode(String input, String expected)
        {
            var result = new ITFWriter().encode(input, BarcodeFormat.ITF, 0, 0);
            Assert.AreEqual(expected, BitMatrixTestCase.matrixToString(result));
        }

        [Test]
        public void testEncodeIllegalCharacters()
        {
            Assert.Throws<ArgumentException>(() => new ITFWriter().encode("00123456789abc", BarcodeFormat.ITF, 0, 0));
        }
    }
}