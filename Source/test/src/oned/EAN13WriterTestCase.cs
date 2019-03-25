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

using System;
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
        [TestCase("5901234123457", "00001010001011010011101100110010011011110100111010101011001101101100100001010111001001110100010010100000", TestName = "EAN13testEncode")]
        [TestCase("590123412345", "00001010001011010011101100110010011011110100111010101011001101101100100001010111001001110100010010100000", TestName = "EAN13testAddChecksumAndEncode")]
        public void testEncode(string content, string encoding)
        {
            var result = new EAN13Writer().encode(content, BarcodeFormat.EAN_13, encoding.Length, 0);
            Assert.AreEqual(encoding, BitMatrixTestCase.matrixToString(result));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void testEncodeIllegalCharacters()
        {
           new EAN13Writer().encode("5901234123abc", BarcodeFormat.EAN_13, 0, 0);
        }
    }
}
