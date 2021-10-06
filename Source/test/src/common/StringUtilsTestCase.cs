/*
 * Copyright 2012 ZXing authors
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

namespace ZXing.Common.Test
{
    [TestFixture]
    public class StringUtilsTestCase
    {
        [Test]
        public void testShortShiftJIS_1()
        {
            // 金魚
            doTest(new byte[] { 0x8b, 0xe0, 0x8b, 0x9b, }, StringUtils.SHIFT_JIS_ENCODING, "SJIS");
        }

        [Test]
        public void testShortISO88591_1()
        {
            // båd
            doTest(new byte[] { 0x62, 0xe5, 0x64, }, StringUtils.ISO88591_ENCODING, "ISO-8859-1");
        }

        [Test]
        public void testShortUTF81()
        {
            // Español
            doTest(new byte[] { (byte) 0x45, (byte) 0x73, (byte) 0x70, (byte) 0x61, (byte) 0xc3,
                        (byte) 0xb1, (byte) 0x6f, (byte) 0x6c },
                   Encoding.UTF8, "UTF-8");
        }

        [Test]
        public void testMixedShiftJIS_1()
        {
            // Hello 金!
            doTest(new byte[]
                      {
                      0x48, 0x65, 0x6c, 0x6c, 0x6f,
                      0x20, 0x8b, 0xe0, 0x21,
                      },
                   StringUtils.SHIFT_JIS_ENCODING, "SJIS");
        }

        [Test]
        public void testUTF16BE()
        {
            // 调压柜
            doTest(new byte[] { (byte) 0xFE, (byte) 0xFF, (byte) 0x8c, (byte) 0x03, (byte) 0x53, (byte) 0x8b,
                        (byte) 0x67, (byte) 0xdc, },
              Encoding.Unicode, Encoding.Unicode.WebName.ToUpper());
        }

        [Test]
        public void testUTF16LE()
        {
            // 调压柜
            doTest(new byte[] { (byte) 0xFF, (byte) 0xFE, (byte) 0x03, (byte) 0x8c, (byte) 0x8b, (byte) 0x53,
                        (byte) 0xdc, (byte) 0x67, },
              Encoding.Unicode, Encoding.Unicode.WebName.ToUpper());
        }

        private static void doTest(byte[] bytes, Encoding encoding, String encodingName)
        {
            Encoding guessedCharset = StringUtils.guessCharset(bytes, null);
            String guessedEncoding = StringUtils.guessEncoding(bytes, null);
            Assert.AreEqual(encoding, guessedCharset);
            Assert.AreEqual(encodingName, guessedEncoding);
        }

        /**
         * Utility for printing out a string in given encoding as a Java statement, since it's better
         * to write that into the Java source file rather than risk character encoding issues in the
         * source file itself
         */
        public static void main(String[] args)
        {
            var text = args[0];
            var charset = Encoding.GetEncoding(args[1]);
            var declaration = new StringBuilder();
            declaration.Append("new byte[] { ");
            foreach (byte b in charset.GetBytes(text))
            {
                declaration.Append("(byte) 0x");
                declaration.Append((b & 0xFF).ToString("X"));
                int value = b & 0xFF;
                if (value < 0x10)
                {
                    declaration.Append('0');
                }
                declaration.Append(value.ToString("X"));
                declaration.Append(", ");
            }
            declaration.Append('}');
            Console.WriteLine(declaration);
        }
    }
}