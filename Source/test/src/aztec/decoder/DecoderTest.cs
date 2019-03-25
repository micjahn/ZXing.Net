/*
 * Copyright 2014 ZXing authors
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

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec.Test
{
    public sealed class DecoderTest
    {
        private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

        [Test]
        public void testAztecResult()
        {
            BitMatrix matrix = BitMatrix.parse(
                "X X X X X     X X X       X X X     X X X     \n" +
                "X X X     X X X     X X X X     X X X     X X \n" +
                "  X   X X       X   X   X X X X     X     X X \n" +
                "  X   X X     X X     X     X   X       X   X \n" +
                "  X X   X X         X               X X     X \n" +
                "  X X   X X X X X X X X X X X X X X X     X   \n" +
                "  X X X X X                       X   X X X   \n" +
                "  X   X   X   X X X X X X X X X   X X X   X X \n" +
                "  X   X X X   X               X   X X       X \n" +
                "  X X   X X   X   X X X X X   X   X X X X   X \n" +
                "  X X   X X   X   X       X   X   X   X X X   \n" +
                "  X   X   X   X   X   X   X   X   X   X   X   \n" +
                "  X X X   X   X   X       X   X   X X   X X   \n" +
                "  X X X X X   X   X X X X X   X   X X X   X X \n" +
                "X X   X X X   X               X   X   X X   X \n" +
                "  X       X   X X X X X X X X X   X   X     X \n" +
                "  X X   X X                       X X   X X   \n" +
                "  X X X   X X X X X X X X X X X X X X   X X   \n" +
                "X     X     X     X X   X X               X X \n" +
                "X   X X X X X   X X X X X     X   X   X     X \n" +
                "X X X   X X X X           X X X       X     X \n" +
                "X X     X X X     X X X X     X X X     X X   \n" +
                "    X X X     X X X       X X X     X X X X   \n",
                "X ", "  ");
            AztecDetectorResult r = new AztecDetectorResult(matrix, NO_POINTS, false, 30, 2);
            DecoderResult result = new Decoder().decode(r);
            Assert.AreEqual("88888TTTTTTTTTTTTTTTTTTTTTTTTTTTTTT", result.Text);
            Assert.AreEqual(
                new byte[]
                {
                    245, 85, 85, 117, 107, 90, 214, 181, 173, 107,
                    90, 214, 181, 173, 107, 90, 214, 181, 173, 107,
                    90, 214, 176
                },
                result.RawBytes);
            Assert.AreEqual(180, result.NumBits);
        }

        [Test]
        public void testDecodeTooManyErrors()
        {
            var matrix = BitMatrix.parse(""
                                         + "X X . X . . . X X . . . X . . X X X . X . X X X X X . \n"
                                         + "X X . . X X . . . . . X X . . . X X . . . X . X . . X \n"
                                         + "X . . . X X . . X X X . X X . X X X X . X X . . X . . \n"
                                         + ". . . . X . X X . . X X . X X . X . X X X X . X . . X \n"
                                         + "X X X . . X X X X X . . . . . X X . . . X . X . X . X \n"
                                         + "X X . . . . . . . . X . . . X . X X X . X . . X . . . \n"
                                         + "X X . . X . . . . . X X . . . . . X . . . . X . . X X \n"
                                         + ". . . X . X . X . . . . . X X X X X X . . . . . . X X \n"
                                         + "X . . . X . X X X X X X . . X X X . X . X X X X X X . \n"
                                         + "X . . X X X . X X X X X X X X X X X X X . . . X . X X \n"
                                         + ". . . . X X . . . X . . . . . . . X X . . . X X . X . \n"
                                         + ". . . X X X . . X X . X X X X X . X . . X . . . . . . \n"
                                         + "X . . . . X . X . X . X . . . X . X . X X . X X . X X \n"
                                         + "X . X . . X . X . X . X . X . X . X . . . . . X . X X \n"
                                         + "X . X X X . . X . X . X . . . X . X . X X X . . . X X \n"
                                         + "X X X X X X X X . X . X X X X X . X . X . X . X X X . \n"
                                         + ". . . . . . . X . X . . . . . . . X X X X . . . X X X \n"
                                         + "X X . . X . . X . X X X X X X X X X X X X X . . X . X \n"
                                         + "X X X . X X X X . . X X X X . . X . . . . X . . X X X \n"
                                         + ". . . . X . X X X . . . . X X X X . . X X X X . . . . \n"
                                         + ". . X . . X . X . . . X . X X . X X . X . . . X . X . \n"
                                         + "X X . . X . . X X X X X X X . . X . X X X X X X X . . \n"
                                         + "X . X X . . X X . . . . . X . . . . . . X X . X X X . \n"
                                         + "X . . X X . . X X . X . X . . . . X . X . . X . . X . \n"
                                         + "X . X . X . . X . X X X X X X X X . X X X X . . X X . \n"
                                         + "X X X X . . . X . . X X X . X X . . X . . . . X X X . \n"
                                         + "X X . X . X . . . X . X . . . . X X . X . . X X . . . \n",
                "X ", ". ");
            var r = new AztecDetectorResult(matrix, NO_POINTS, true, 16, 4);
            Assert.That(new Decoder().decode(r), Is.Null);
        }

        [Test]
        public void testDecodeTooManyErrors2()
        {
            var matrix = BitMatrix.parse(""
                                         + ". X X . . X . X X . . . X . . X X X . . . X X . X X . \n"
                                         + "X X . X X . . X . . . X X . . . X X . X X X . X . X X \n"
                                         + ". . . . X . . . X X X . X X . X X X X . X X . . X . . \n"
                                         + "X . X X . . X . . . X X . X X . X . X X . . . . . X . \n"
                                         + "X X . X . . X . X X . . . . . X X . . . . . X . . . X \n"
                                         + "X . . X . . . . . . X . . . X . X X X X X X X . . . X \n"
                                         + "X . . X X . . X . . X X . . . . . X . . . . . X X X . \n"
                                         + ". . X X X X . X . . . . . X X X X X X . . . . . . X X \n"
                                         + "X . . . X . X X X X X X . . X X X . X . X X X X X X . \n"
                                         + "X . . X X X . X X X X X X X X X X X X X . . . X . X X \n"
                                         + ". . . . X X . . . X . . . . . . . X X . . . X X . X . \n"
                                         + ". . . X X X . . X X . X X X X X . X . . X . . . . . . \n"
                                         + "X . . . . X . X . X . X . . . X . X . X X . X X . X X \n"
                                         + "X . X . . X . X . X . X . X . X . X . . . . . X . X X \n"
                                         + "X . X X X . . X . X . X . . . X . X . X X X . . . X X \n"
                                         + "X X X X X X X X . X . X X X X X . X . X . X . X X X . \n"
                                         + ". . . . . . . X . X . . . . . . . X X X X . . . X X X \n"
                                         + "X X . . X . . X . X X X X X X X X X X X X X . . X . X \n"
                                         + "X X X . X X X X . . X X X X . . X . . . . X . . X X X \n"
                                         + ". . X X X X X . X . . . . X X X X . . X X X . X . X . \n"
                                         + ". . X X . X . X . . . X . X X . X X . . . . X X . . . \n"
                                         + "X . . . X . X . X X X X X X . . X . X X X X X . X . . \n"
                                         + ". X . . . X X X . . . . . X . . . . . X X X X X . X . \n"
                                         + "X . . X . X X X X . X . X . . . . X . X X . X . . X . \n"
                                         + "X . . . X X . X . X X X X X X X X . X X X X . . X X . \n"
                                         + ". X X X X . . X . . X X X . X X . . X . . . . X X X . \n"
                                         + "X X . . . X X . . X . X . . . . X X . X . . X . X . X \n",
                "X ", ". ");
            var r = new AztecDetectorResult(matrix, NO_POINTS, true, 16, 4);
            Assert.That(new Decoder().decode(r), Is.Null);
        }

        private static void assertEqualByteArrays(byte[] b1, byte[] b2)
        {
            Assert.That(b1.Length, Is.EqualTo(b2.Length));
            for (int i = 0; i < b1.Length; i++)
            {
                Assert.That(b1[i], Is.EqualTo(b2[i]));
            }
        }

        [Test]
        public void testRawBytes()
        {
            var bool0 = new bool[0];
            var bool1 = new bool[] {true};
            var bool7 = new bool[] {true, false, true, false, true, false, true};
            var bool8 = new bool[] {true, false, true, false, true, false, true, false};
            var bool9 = new bool[]
            {
                true, false, true, false, true, false, true, false,
                true
            };
            var bool16 = new bool[]
            {
                false, true, true, false, false, false, true, true,
                true, true, false, false, false, false, false, true
            };
            var byte0 = new byte[0];
            var byte1 = new byte[] {128};
            var byte7 = new byte[] {170};
            var byte8 = new byte[] {170};
            var byte9 = new byte[] {170, 128};
            var byte16 = new byte[] {99, 193};

            assertEqualByteArrays(byte0, Decoder.convertBoolArrayToByteArray(bool0));
            assertEqualByteArrays(byte1, Decoder.convertBoolArrayToByteArray(bool1));
            assertEqualByteArrays(byte7, Decoder.convertBoolArrayToByteArray(bool7));
            assertEqualByteArrays(byte8, Decoder.convertBoolArrayToByteArray(bool8));
            assertEqualByteArrays(byte9, Decoder.convertBoolArrayToByteArray(bool9));
            assertEqualByteArrays(byte16, Decoder.convertBoolArrayToByteArray(bool16));
        }

#if !SILVERLIGHT
        [Test]
        public void roundTripTestMixedMode()
        {
            var base64Content = "QAECAwQFBgcLGxwdHh9/QA==";
            var byteContent = System.Convert.FromBase64String(base64Content);
            var stringContent = System.Text.Encoding.GetEncoding("ISO-8859-15").GetString(byteContent);
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.AZTEC
            };
            var bitmap = writer.Write(stringContent);
            var reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            Assert.That(result.Text, Is.EqualTo(stringContent));
        }
#endif
    }
}