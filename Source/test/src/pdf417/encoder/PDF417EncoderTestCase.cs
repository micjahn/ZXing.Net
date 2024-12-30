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
    }
}