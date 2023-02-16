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
using System.Text;
using System;

namespace ZXing.PDF417.Internal.Test
{
    using System.Runtime.InteropServices;
    using ZXing.Common;

    [TestFixture]
    public class PDF417DecoderTestCase
    {
        /// <summary>
        /// Tests the first sample given in ISO/IEC 15438:2015(E) - Annex H.4
        /// </summary>
        [Test]
        public void testStandardSample1()
        {
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
            int[] sampleCodes = {20, 928, 111, 100, 17, 53, 923, 1, 111, 104, 923, 3, 64, 416, 34, 923, 4, 258, 446, 67,
            // we should never reach these
            1000, 1000, 1000};

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata);

            Assert.AreEqual(0, resultMetadata.SegmentIndex);
            Assert.AreEqual("017053", resultMetadata.FileId);
            Assert.That(resultMetadata.IsLastSegment, Is.False);
            Assert.AreEqual(4, resultMetadata.SegmentCount);
            Assert.AreEqual("CEN BE", resultMetadata.Sender);
            Assert.AreEqual("ISO CH", resultMetadata.Addressee);

            //int[] optionalData = resultMetadata.OptionalData;
            //Assert.AreEqual(1, optionalData[0], "first element of optional array should be the first field identifier");
            //Assert.AreEqual(67, optionalData[optionalData.Length - 1], "last element of optional array should be the last codeword of the last field");
        }

        /// <summary>
        /// Tests the second given in ISO/IEC 15438:2015(E) - Annex H.4
        /// </summary>
        [Test]
        public void testStandardSample2()
        {
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
            int[] sampleCodes = {11, 928, 111, 103, 17, 53, 923, 1, 111, 104, 922,
            // we should never reach these
            1000, 1000, 1000};

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata);

            Assert.AreEqual(3, resultMetadata.SegmentIndex);
            Assert.AreEqual("017053", resultMetadata.FileId);
            Assert.That(resultMetadata.IsLastSegment, Is.True);
            Assert.AreEqual(4, resultMetadata.SegmentCount);
            Assert.That(resultMetadata.Addressee, Is.Null);
            Assert.That(resultMetadata.Sender, Is.Null);

            //int[] optionalData = resultMetadata.OptionalData;
            //Assert.AreEqual(1, optionalData[0], "first element of optional array should be the first field identifier");
            //Assert.AreEqual(104, optionalData[optionalData.Length - 1], "last element of optional array should be the last codeword of the last field");
        }

        /// <summary>
        /// Tests the example given in ISO/IEC 15438:2015(E) - Annex H.6
        /// </summary>
        [Test]
        public void testStandardSample3()
        {
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
            int[] sampleCodes = {7, 928, 111, 100, 100, 200, 300,
      0}; // Final dummy ECC codeword required to avoid ArrayIndexOutOfBounds

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata);

            Assert.AreEqual(0, resultMetadata.SegmentIndex);
            Assert.AreEqual("100200300", resultMetadata.FileId);
            Assert.False(resultMetadata.IsLastSegment);
            Assert.AreEqual(-1, resultMetadata.SegmentCount);
            Assert.IsNull(resultMetadata.Addressee);
            Assert.IsNull(resultMetadata.Sender);
            Assert.IsNull(resultMetadata.OptionalData);

            // Check that symbol containing no data except Macro is accepted (see note in Annex H.2)
            DecoderResult decoderResult = DecodedBitStreamParser.decode(sampleCodes, "0", System.Text.Encoding.GetEncoding(PDF417HighLevelEncoder.DEFAULT_ENCODING_NAME));
            Assert.AreEqual("", decoderResult.Text);
            Assert.IsNotNull(decoderResult.Other);
        }

        [Test]
        public void testSampleWithFilename()
        {
            int[] sampleCodes = {23, 477, 928, 111, 100, 0, 252, 21, 86, 923, 0, 815, 251, 133, 12, 148, 537, 593,
            599, 923, 1, 111, 102, 98, 311, 355, 522, 920, 779, 40, 628, 33, 749, 267, 506, 213, 928, 465, 248,
            493, 72, 780, 699, 780, 493, 755, 84, 198, 628, 368, 156, 198, 809, 19, 113};
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 3, resultMetadata);

            Assert.AreEqual(0, resultMetadata.SegmentIndex);
            Assert.AreEqual("000252021086", resultMetadata.FileId);
            Assert.That(resultMetadata.IsLastSegment, Is.False);
            Assert.AreEqual(2, resultMetadata.SegmentCount);
            Assert.That(resultMetadata.Addressee, Is.Null);
            Assert.That(resultMetadata.Sender, Is.Null);
            Assert.AreEqual("filename.txt", resultMetadata.FileName);
        }

        [Test]
        public void testSampleWithNumericValues()
        {
            int[] sampleCodes = {25, 477, 928, 111, 100, 0, 252, 21, 86, 923, 2, 2, 0, 1, 0, 0, 0, 923, 5, 130, 923,
        6, 1, 500, 13, 0};
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 3, resultMetadata);

            Assert.AreEqual(0, resultMetadata.SegmentIndex);
            Assert.AreEqual("000252021086", resultMetadata.FileId);
            Assert.That(resultMetadata.IsLastSegment, Is.False);

            Assert.AreEqual(180980729000000L, resultMetadata.Timestamp);
            Assert.AreEqual(30, resultMetadata.FileSize);
            Assert.AreEqual(260013, resultMetadata.Checksum);
        }


        [Test]
        public void testSampleWithMacroTerminatorOnly()
        {
            int[] sampleCodes = { 7, 477, 928, 222, 198, 0, 922 };
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

            DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 3, resultMetadata);

            Assert.AreEqual(99998, resultMetadata.SegmentIndex);
            Assert.AreEqual("000", resultMetadata.FileId);
            Assert.IsTrue(resultMetadata.IsLastSegment);
            Assert.AreEqual(-1, resultMetadata.SegmentCount);
            Assert.IsNull(resultMetadata.OptionalData);
        }

        [Test]
        public void testSampleWithBadSequenceIndexMacro()
        {
            int[]
                sampleCodes = { 3, 928, 222, 0 };
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

            Assert.AreEqual(-1, DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata));
        }

        [Test]

        public void testSampleWithNoFileIdMacro()
        {
            int[]
            sampleCodes = { 4, 928, 222, 198, 0 };
            PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

            Assert.AreEqual(-1, DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata));
        }

        [Test]
        public void testSampleWithNoDataNoMacro()
        {
            int[]
            sampleCodes = { 3, 899, 899, 0 };

            Assert.IsNull(DecodedBitStreamParser.decode(sampleCodes, "0", System.Text.Encoding.GetEncoding(PDF417HighLevelEncoder.DEFAULT_ENCODING_NAME)));
        }

        [Test]
        public void testUppercase()
        {
            //encodeDecode("", 0);
            performEncodeTest('A', new int[] { 3, 4, 5, 6, 4, 4, 5, 5 });
        }

        [Test]
        public void testNumeric()
        {
            performEncodeTest('1', new int[] { 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10 });
        }

        [Test]
        public void testByte()
        {
            performEncodeTest('\u00c4', new int[] { 3, 4, 5, 6, 7, 7, 8 });
        }

        [Test]
        public void testUppercaseLowercaseMix1()
        {
            encodeDecode("aA", 4);
            encodeDecode("aAa", 5);
            encodeDecode("Aa", 4);
            encodeDecode("Aaa", 5);
            encodeDecode("AaA", 5);
            encodeDecode("AaaA", 6);
            encodeDecode("Aaaa", 6);
            encodeDecode("AaAaA", 5);
            encodeDecode("AaaAaaA", 6);
            encodeDecode("AaaAAaaA", 7);
        }

        [Test]
        public void testPunctuation()
        {
            performEncodeTest(';', new int[] { 3, 4, 5, 6, 6, 7, 8 });
            encodeDecode(";;;;;;;;;;;;;;;;", 17);
        }

        [Test]
        public void testUppercaseLowercaseMix2()
        {
            performPermutationTest(new char[] { 'A', 'a' }, 10, 8972);
        }

        [Test]
        public void testUppercaseNumericMix()
        {
            performPermutationTest(new char[] { 'A', '1' }, 14, 192510);
        }

        [Test]
        public void testUppercaseMixedMix()
        {
            performPermutationTest(new char[] { 'A', '1', ' ', ';' }, 7, 106060);
        }

        [Test]
        public void testUppercasePunctuationMix()
        {
            performPermutationTest(new char[] { 'A', ';' }, 10, 8967);
        }

        [Test]
        public void testUppercaseByteMix()
        {
            performPermutationTest(new char[] { 'A', '\u00c4' }, 10, 11222);
        }

        [Test]
        public void testLowercaseByteMix()
        {
            performPermutationTest(new char[] { 'a', '\u00c4' }, 10, 11233);
        }

        public void testUppercaseLowercaseNumericMix()
        {
            performPermutationTest(new char[] { 'A', 'a', '1' }, 7, 15491);
        }

        [Test]
        public void testUppercaseLowercasePunctuationMix()
        {
            performPermutationTest(new char[] { 'A', 'a', ';' }, 7, 15491);
        }

        [Test]
        public void testUppercaseLowercaseByteMix()
        {
            performPermutationTest(new char[] { 'A', 'a', '\u00c4' }, 7, 17288);
        }

        [Test]
        public void testLowercasePunctuationByteMix()
        {
            performPermutationTest(new char[] { 'a', ';', '\u00c4' }, 7, 17427);
        }

        [Test]
        public void testUppercaseLowercaseNumericPunctuationMix()
        {
            performPermutationTest(new char[] { 'A', 'a', '1', ';' }, 7, 120479);
        }

        [Test]
        public void testBinaryData()
        {
            var bytes = new byte[500];
            var random = new Random(0);
            int total = 0;
            for (int i = 0; i < 10000; i++)
            {
                random.NextBytes(bytes);
                total += encodeDecode(Encoding.GetEncoding(PDF417HighLevelEncoder.DEFAULT_ENCODING_NAME).GetString(bytes));
            }
            Assert.AreEqual(4190032, total); // in java 4190044 (?)
        }

        [Test]
        public void testECIEnglishHiragana()
        {
            //multi ECI UTF-8, UTF-16 and ISO-8859-1
            performECITest(new char[] { 'a', '1', '\u3040' }, new double[] { 20f, 1f, 10f }, 105640, 111542); // Java: 105825, 110914);
        }

        [Test]
        public void testECIEnglishKatakana()
        {
            //multi ECI UTF-8, UTF-16 and ISO-8859-1
            performECITest(new char[] { 'a', '1', '\u30a0' }, new double[] { 20f, 1f, 10f }, 109526, 111542); // Java: 109177, 110914);
        }

        [Test]
        public void testECIEnglishHalfWidthKatakana()
        {
            //single ECI
            performECITest(new char[] { 'a', '1', '\uff80' }, new double[] { 20f, 1f, 10f }, 79589, 111542); // Java: 80617, 110914);
        }

        [Test]
        public void testECIEnglishChinese()
        {
            //single ECI
            performECITest(new char[] { 'a', '1', '\u4e00' }, new double[] { 20f, 1f, 10f }, 95604, 111542); // Java: 95797, 110914);
        }

        [Test]
        public void testECIGermanCyrillic()
        {
            //single ECI since the German Umlaut is in ISO-8859-1
            performECITest(new char[] { 'a', '1', '\u00c4', '\u042f' }, new double[] { 20f, 1f, 1f, 10f }, 79744, 95266); // Java: 80755, 96007);
        }

        [Test]
        public void testECIEnglishCzechCyrillic1()
        {
            //multi ECI between ISO-8859-2 and ISO-8859-5
            performECITest(new char[] { 'a', '1', '\u010c', '\u042f' }, new double[] { 10f, 1f, 10f, 10f }, 103640, 126382); // Java: 102824, 124525);
        }

        [Test]
        public void testECIEnglishCzechCyrillic2()
        {
            //multi ECI between ISO-8859-2 and ISO-8859-5
            performECITest(new char[] { 'a', '1', '\u010c', '\u042f' }, new double[] { 40f, 1f, 10f, 10f }, 80622, 87520); // Java: 81321, 88236);
        }

        [Test]
        public void testECIEnglishArabicCyrillic()
        {
            //multi ECI between UTF-8 (ISO-8859-6 is excluded in CharacterSetECI) and ISO-8859-5
            performECITest(new char[] { 'a', '1', '\u0620', '\u042f' }, new double[] { 10f, 1f, 10f, 10f }, 120162, 126382); // Java: 118510, 124525);
        }

        [Test]
        public void testBinaryMultiECI()
        {
            //Test the cases described in 5.5.5.3 "ECI and Byte Compaction mode using latch 924 and 901"
            performDecodeTest(new int[] { 5, 927, 4, 913, 200 }, "\u010c");
            performDecodeTest(new int[] { 9, 927, 4, 913, 200, 927, 7, 913, 207 }, "\u010c\u042f");
            performDecodeTest(new int[] { 9, 927, 4, 901, 200, 927, 7, 901, 207 }, "\u010c\u042f");
            performDecodeTest(new int[] { 8, 927, 4, 901, 200, 927, 7, 207 }, "\u010c\u042f");
            performDecodeTest(new int[] { 14, 927, 4, 901, 200, 927, 7, 207, 927, 4, 200, 927, 7, 207 },
                 "\u010c\u042f\u010c\u042f");
            performDecodeTest(new int[] { 16, 927, 4, 924, 336, 432, 197, 51, 300, 927, 7, 348, 231, 311, 858, 567 },
                "\u010c\u010c\u010c\u010c\u010c\u010c\u042f\u042f\u042f\u042f\u042f\u042f");
        }

        private static void encodeDecode(String input, int expectedLength)
        {
            Assert.AreEqual(expectedLength, encodeDecode(input));
        }

        private static int encodeDecode(String input)
        {
            return encodeDecode(input, null, false, true);
        }

        private static int encodeDecode(String input, Encoding charset, bool autoECI, bool decode)
      {
            var s = PDF417HighLevelEncoder.encodeHighLevel(input, Compaction.AUTO, charset, false, autoECI);
            if (decode)
            {
                var codewords = new int[s.Length + 1];
                codewords[0] = codewords.Length;
                for (int i = 1; i < codewords.Length; i++)
                {
                    codewords[i] = s[i - 1];
                }
                performDecodeTest(codewords, input);
            }
            return s.Length + 1;
        }

        private static int getEndIndex(int length, char[] chars)
        {
            double decimalLength = Math.Log10(chars.Length);
            return (int)Math.Ceiling(Math.Pow(10, decimalLength * length));
        }

        private static String generatePermutation(int index, int length, char[] chars)
        {
            int N = chars.Length;
            var baseNNumber = DecimalToArbitrarySystem(index, N);
            while (baseNNumber.Length < length)
            {
                baseNNumber = "0" + baseNNumber;
            }
            var prefix = string.Empty;
            for (int i = 0; i < baseNNumber.Length; i++)
            {
                prefix += chars[baseNNumber[i] - '0'];
            }
            return prefix;
        }

        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        private static void performPermutationTest(char[] chars, int length, int expectedTotal)
        {
            int endIndex = getEndIndex(length, chars);
            int total = 0;
            for (int i = 0; i < endIndex; i++)
            {
                total += encodeDecode(generatePermutation(i, length, chars));
            }
            Assert.AreEqual(expectedTotal, total);
        }

        private static void performEncodeTest(char c, int[] expectedLengths) {
            for (int i = 0; i < expectedLengths.Length; i++) {
                var sb = new StringBuilder();
                for (int j = 0; j <= i; j++)
                {
                    sb.Append(c);
                }
                encodeDecode(sb.ToString(), expectedLengths[i]);
            }
        }

        private static void performDecodeTest(int[] codewords, String expectedResult)
        {
            var result = DecodedBitStreamParser.decode(codewords, "0", null);
            Assert.AreEqual(expectedResult, result.Text);
        }

        private static void performECITest(char[] chars,
                                     double[] weights,
                                     int expectedMinLength,
                                     int expectedUTFLength)
        {
            var random = new Random(0);
            int minLength = 0;
            int utfLength = 0;
            for (int i = 0; i < 1000; i++)
            {
                String s = generateText(random, 100, chars, weights);
                minLength += encodeDecode(s, null, true, true);
                utfLength += encodeDecode(s, Encoding.UTF8, false, true);
            }
            Assert.AreEqual(expectedMinLength, minLength);
            Assert.AreEqual(expectedUTFLength, utfLength);
        }

        private static String generateText(Random random, int maxWidth, char[] chars, double[] weights)
        {
            StringBuilder result = new StringBuilder();
            int maxWordWidth = 7;
            double total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
            }
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] /= total;
            }
            int cnt = 0;
            do
            {
                double maxValue = 0;
                int maxIndex = 0;
                for (int j = 0; j < weights.Length; j++)
                {
                    double value = random.NextDouble() * weights[j];
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxIndex = j;
                    }
                }
                double wordLength = maxWordWidth * random.NextDouble();
                if (wordLength > 0 && result.Length > 0)
                {
                    result.Append(' ');
                }
                for (int j = 0; j < wordLength; j++)
                {
                    char c = chars[maxIndex];
                    if (j == 0 && c >= 'a' && c <= 'z' && (random.Next() % 1) == 1)
                    {
                        c = (char)(c - 'a' + 'A');
                    }
                    result.Append(c);
                }
                if (cnt % 2 != 0 && (random.Next() % 1) == 1)
                {
                    result.Append('.');
                }
                cnt++;
            }
            while (result.Length < maxWidth - maxWordWidth);
            return result.ToString();
        }
    }
}