/*
 * Copyright (C) 2014 ZXing authors
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
using System.Text;

using NUnit.Framework;
using ZXing.Common;
using static ZXing.Datamatrix.Encoder.MinimalEncoder;

namespace ZXing.PDF417.Internal.Test
{
    public sealed class PDF417EncoderTestCase
    {
        private static String PDF417PFX = "\u039f\u001A\u0385";

        [Test]
        public void testEncodeAuto()
        {
            var input = "ABCD";
            Assert.That(checkEncodeAutoWithSpecialChars(input, Compaction.AUTO), Is.EqualTo(PDF417PFX + input));
        }

        [Test]
        public void testEncodeAutoWithSpecialChars()
        {
            // Just check if this does not throw an exception
            checkEncodeAutoWithSpecialChars("1%§s ?aG$", Compaction.AUTO);
            checkEncodeAutoWithSpecialChars("日本語", Compaction.AUTO);
            checkEncodeAutoWithSpecialChars("₸ 5555", Compaction.AUTO);
            checkEncodeAutoWithSpecialChars("€ 123,45", Compaction.AUTO);
            checkEncodeAutoWithSpecialChars("€ 123,45", Compaction.BYTE);
            checkEncodeAutoWithSpecialChars("123,45", Compaction.TEXT);

            // Greek alphabet
            var cp437 = CharacterSetECI.getEncoding("IBM437");
            Assert.That(cp437, Is.Not.Null);
            byte[] cp437Array = { (byte)224, (byte)225, (byte)226, (byte)227, (byte)228 }; //αßΓπΣ
            var greek = cp437.GetString(cp437Array);
            Assert.That("αßΓπΣ", Is.EqualTo(greek));
            checkEncodeAutoWithSpecialChars(greek, Compaction.AUTO);
            checkEncodeAutoWithSpecialChars(greek, Compaction.BYTE);
            PDF417HighLevelEncoder.encodeHighLevel(greek, Compaction.AUTO, cp437, false, true);
            PDF417HighLevelEncoder.encodeHighLevel(greek, Compaction.AUTO, cp437, false, false);

            try
            {
                // detect when a TEXT Compaction is applied to a non text input
                checkEncodeAutoWithSpecialChars("€ 123,45", Compaction.TEXT);
            }
            catch (WriterException e)
            {
                Assert.That(e.Message, Is.Not.Null);
                Assert.That(e.Message.Contains("8364"), Is.True);
                Assert.That(e.Message.Contains("Compaction.TEXT"), Is.True);
                Assert.That(e.Message.Contains("Compaction.AUTO"), Is.True);
            }

            try
            {
                // detect when a TEXT Compaction is applied to a non text input
                var input = "Hello! " + (char)128;
                checkEncodeAutoWithSpecialChars(input, Compaction.TEXT);
            }
            catch (WriterException e)
            {
                Assert.That(e.Message, Is.Not.Null);
                Assert.That(e.Message.Contains("128"), Is.True);
                Assert.That(e.Message.Contains("Compaction.TEXT"), Is.True);
                Assert.That(e.Message.Contains("Compaction.AUTO"), Is.True);
            }

            try
            {
                // detect when a TEXT Compaction is applied to a non text input
                // https://github.com/zxing/zxing/issues/1761
                var content = "€ 123,45";
                var hints = new System.Collections.Generic.Dictionary<EncodeHintType, object>();
                hints[EncodeHintType.ERROR_CORRECTION] = 4;
                hints[EncodeHintType.PDF417_DIMENSIONS] = new Dimensions(7, 7, 1, 300);
                hints[EncodeHintType.MARGIN] = 0;
                hints[EncodeHintType.CHARACTER_SET] = "ISO-8859-15";
                hints[EncodeHintType.PDF417_COMPACTION] = Compaction.TEXT;

                (new MultiFormatWriter()).encode(content, BarcodeFormat.PDF_417, 200, 100, hints);
            }
            catch (WriterException e)
            {
                Assert.That(e.Message, Is.Not.Null);
                Assert.That(e.Message.Contains("8364"), Is.True);
                Assert.That(e.Message.Contains("Compaction.TEXT"), Is.True);
                Assert.That(e.Message.Contains("Compaction.AUTO"), Is.True);
            }
        }

        public String checkEncodeAutoWithSpecialChars(String input, Compaction compaction)
        {
            return PDF417HighLevelEncoder.encodeHighLevel(input, compaction, Encoding.UTF8, false, false);
        }

        [Test]
        public void testCheckCharset()
        {
            var input = "Hello!";
            var errorMessage = Guid.NewGuid().ToString();

            // no exception
            PDF417HighLevelEncoder.checkCharset(input, 255, errorMessage);
            PDF417HighLevelEncoder.checkCharset(input, 1255, errorMessage);
            PDF417HighLevelEncoder.checkCharset(input, 111, errorMessage);

            try
            {
                // should throw an exception for character 'o' because it exceeds upper limit 110
                PDF417HighLevelEncoder.checkCharset(input, 110, errorMessage);
            }
            catch (WriterException e)
            {
                Assert.That(e.Message, Is.Not.Null);
                Assert.That(e.Message.Contains("111"), Is.True);
                Assert.That(e.Message.Contains(errorMessage), Is.True);
            }
        }

        [Test]
        public void testEncodeIso88591WithSpecialChars()
        {
            // Just check if this does not throw an exception
            PDF417HighLevelEncoder.encodeHighLevel("asdfg§asd", Compaction.AUTO, Encoding.GetEncoding("ISO8859-1"), false, false);
        }

        [Test]
        public void testEncodeText()
        {
            var encoded = PDF417HighLevelEncoder.encodeHighLevel(
               "ABCD", Compaction.TEXT, Encoding.UTF8, false, false);
            Assert.AreEqual("Ο\u001A\u0001?", encoded);
        }

        [Test]
        public void testEncodeNumeric()
        {
            var encoded = PDF417HighLevelEncoder.encodeHighLevel(
               "1234", Compaction.NUMERIC, Encoding.UTF8, false, false);
            Assert.AreEqual("\u039f\u001A\u0386\f\u01b2", encoded);
        }

        [Test]
        public void testEncodeByte()
        {
            var encoded = PDF417HighLevelEncoder.encodeHighLevel(
               "abcd", Compaction.BYTE, Encoding.UTF8, false, false);
            Assert.AreEqual("\u039f\u001A\u0385abcd", encoded);
        }

        [Test]
        public void testEncodeEmptyString()
        {
            Assert.Throws<ArgumentException>(() => PDF417HighLevelEncoder.encodeHighLevel("", Compaction.AUTO, null, false, false));
        }


        [Test]
        public void testDimensions()
        {
            // test https://github.com/zxing/zxing/issues/1831
            String input = "0000000001000000022200000003330444400888888881010101010";
            testDimensions(input, new Dimensions(1, 30, 7, 10));
            testDimensions(input, new Dimensions(1, 40, 1, 7));
            testDimensions(input, new Dimensions(10, 30, 1, 5));
            testDimensions(input, new Dimensions(1, 3, 1, 15));
            testDimensions(input, new Dimensions(5, 30, 7, 7));
            testDimensions(input, new Dimensions(12, 12, 1, 17));
            testDimensions(input, new Dimensions(1, 30, 7, 8));
        }

        public static void testDimensions(String input, Dimensions dimensions)
        {
            var sourceCodeWords = 20;
            var errorCorrectionCodeWords = 8;

            //var calculated = PDF417.determineDimensions(dimensions.MinCols, dimensions.MaxCols,
            //      dimensions.MinRows, dimensions.MaxRows, sourceCodeWords, errorCorrectionCodeWords);
            var aspectRatio = 4;
            var pdf417 = new PDF417();
            pdf417.setDimensions(dimensions.MaxCols, dimensions.MinCols, dimensions.MaxRows, dimensions.MinRows);
            var calculated = pdf417.determineDimensions(sourceCodeWords, errorCorrectionCodeWords, 0, 0, ref aspectRatio);

            Assert.That(calculated, Is.Not.Null);
            Assert.That(calculated.Length, Is.EqualTo(2));
            Assert.That(dimensions.MinCols <= calculated[0], Is.True);
            Assert.That(dimensions.MaxCols >= calculated[0], Is.True);
            Assert.That(dimensions.MinRows <= calculated[1], Is.True);
            Assert.That(dimensions.MaxRows >= calculated[1], Is.True);
            Assert.That(generatePDF417BitMatrix(input, 371, null, dimensions), Is.Not.Null);
        }

        public static BitMatrix generatePDF417BitMatrix(String barcodeText, int width, int? heightRequested, Dimensions dimensions)
        {
            var barcodeWriter = new PDF417Writer();
            var height = heightRequested == null ? width / 4 : heightRequested.Value;
            var hints = new System.Collections.Generic.Dictionary<EncodeHintType, object>();
            hints[EncodeHintType.MARGIN] = 0;
            hints[EncodeHintType.PDF417_DIMENSIONS] = dimensions;
            return barcodeWriter.encode(barcodeText, BarcodeFormat.PDF_417, width, height, hints);
        }
    }
}