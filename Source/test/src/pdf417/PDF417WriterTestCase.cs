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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using NUnit.Framework;

namespace ZXing.PDF417.Test
{
    public class PDF417WriterTestCase
    {
        [Test]
        public void testDataMatrixImageWriter()
        {
            var hints = new PDF417EncodingOptions
            {
                Margin = 0
            };
            const int size = 64;
            var writer = new PDF417Writer();
            var matrix = writer.encode("Hello Google", BarcodeFormat.PDF_417, size, size, hints.Hints);
            Assert.IsNotNull(matrix);
            var expected =
                "X X X X X X X X   X   X   X       X X X X   X   X   X X X X         X X   X   X           X X         X X X X   X X     X     X X X     X X   X           X       X X     X X X X X   X   X   X X X X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X   X X X X         X X   X   X           X X         X X X X   X X     X     X X X     X X   X           X       X X     X X X X X   X   X   X X X X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X   X X X X         X X   X   X           X X         X X X X   X X     X     X X X     X X   X           X       X X     X X X X X   X   X   X X X X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X   X X X X         X X   X   X           X X         X X X X   X X     X     X X X     X X   X           X       X X     X X X X X   X   X   X X X X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X         X         X   X X     X     X X X X X X     X X X           X   X X       X   X X X   X           X X     X     X X X X X X   X   X   X X X       X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X         X         X   X X     X     X X X X X X     X X X           X   X X       X   X X X   X           X X     X     X X X X X X   X   X   X X X       X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X         X         X   X X     X     X X X X X X     X X X           X   X X       X   X X X   X           X X     X     X X X X X X   X   X   X X X       X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X   X   X         X         X   X X     X     X X X X X X     X X X           X   X X       X   X X X   X           X X     X     X X X X X X   X   X   X X X       X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X     X X X X             X   X X X       X       X X       X     X X   X X     X X X X       X X       X X X X X     X     X   X   X   X         X X X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X     X X X X             X   X X X       X       X X       X     X X   X X     X X X X       X X       X X X X X     X     X   X   X   X         X X X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X     X X X X             X   X X X       X       X X       X     X X   X X     X X X X       X X       X X X X X     X     X   X   X   X         X X X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X     X X X X             X   X X X       X       X X       X     X X   X X     X X X X       X X       X X X X X     X     X   X   X   X         X X X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X X X X     X X X X       X         X X       X X     X     X   X     X X X X       X X X X   X       X X       X X         X   X X   X   X X X X     X X X X X   X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X X X X     X X X X       X         X X       X X     X     X   X     X X X X       X X X X   X       X X       X X         X   X X   X   X X X X     X X X X X   X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X X X X     X X X X       X         X X       X X     X     X   X     X X X X       X X X X   X       X X       X X         X   X X   X   X X X X     X X X X X   X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X   X   X X X X     X X X X       X         X X       X X     X     X   X     X X X X       X X X X   X       X X       X X         X   X X   X   X X X X     X X X X X   X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X   X   X X X           X       X X     X       X X X     X       X X X X X X   X X   X     X X     X   X X X   X     X X X X X       X X X   X   X X X     X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X   X   X X X           X       X X     X       X X X     X       X X X X X X   X X   X     X X     X   X X X   X     X X X X X       X X X   X   X X X     X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X   X   X X X           X       X X     X       X X X     X       X X X X X X   X X   X     X X     X   X X X   X     X X X X X       X X X   X   X X X     X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X   X   X X X           X       X X     X       X X X     X       X X X X X X   X X   X     X X     X   X X X   X     X X X X X       X X X   X   X X X     X X         X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X X   X   X X X X   X X     X           X   X       X X X X   X       X   X         X X X X     X           X X X   X       X X   X X X X   X   X X X X X   X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X X   X   X X X X   X X     X           X   X       X X X X   X       X   X         X X X X     X           X X X   X       X X   X X X X   X   X X X X X   X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X X   X   X X X X   X X     X           X   X       X X X X   X       X   X         X X X X     X           X X X   X       X X   X X X X   X   X X X X X   X X     X X X X X X X   X       X   X     X \r\n" +
                "X X X X X X X X   X   X   X       X X X X X   X   X X X X   X X     X           X   X       X X X X   X       X   X         X X X X     X           X X X   X       X X   X X X X   X   X X X X X   X X     X X X X X X X   X       X   X     X \r\n";
            Assert.AreEqual(expected, matrix.ToString());
        }

        [TestCase("UTF-8")]
        [TestCase("ISO-8859-15")]
        [TestCase("ISO-8859-1")]
        public void test0To256AsBytesRoundTrip(string encodingStr)
        {
            var encoding = System.Text.Encoding.GetEncoding(encodingStr);
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new PDF417EncodingOptions
                {
                    Width = 1,
                    Height = 1,
                    CharacterSet = encodingStr
                }
            };
            var reader = new BarcodeReader
            {
                Options =
                {
                    PureBarcode = true,
                    PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.PDF_417}
                }
            };
            var errors = new Dictionary<string, Exception>();
            for (int i = 0; i < 256; i++)
            {
                var content = encoding.GetString(new[] {(byte)i});
                try
                {
                    var bitmap = writer.Write(content);
                    var result = reader.Decode(bitmap);
                    if (result == null)
                        throw new InvalidOperationException("cant be decoded");
                    Assert.That(result.Text, Is.EqualTo(content));
                }
                catch (Exception e)
                {
                    errors[content] = e;
                }
            }

            foreach (var error in errors)
            {
                var bytes = encoding.GetBytes(error.Key);
                Console.WriteLine($"Content: {error.Key} ({bytes[0]}); Error: {error.Value.Message}");
            }

            if (errors.Count > 0)
                throw new AssertionException("not every content could be encoded and decoded");
        }

        [TestCase("UTF-8")]
        [TestCase("ISO-8859-15")]
        [TestCase("ISO-8859-1")]
        public void test0To256AsOneStringRoundTrip(string encodingStr)
        {
            var encoding = System.Text.Encoding.GetEncoding(encodingStr);
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new PDF417EncodingOptions
                {
                    Width = 1,
                    Height = 1,
                    CharacterSet = encodingStr
                }
            };
            var reader = new BarcodeReader
            {
                Options =
                {
                    PureBarcode = true,
                    PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.PDF_417}
                }
            };
            var content = new StringBuilder();
            for (int i = 0; i < 256; i++)
            {
                content.Append(encoding.GetString(new[] {(byte) i}));
            }
            Console.WriteLine(content.ToString());
            var bitmap = writer.Write(content.ToString());
            bitmap.Save($"D:\\test-{encodingStr}.png", ImageFormat.Png);
            var result = reader.Decode(bitmap);
            if (result == null)
                throw new InvalidOperationException("cant be decoded");
            Assert.That(result.Text, Is.EqualTo(content.ToString()));
        }

        [TestCase('1')]
        [TestCase('a')]
        [TestCase('A')]
        [TestCase('€')]
        public void testCharactersForLengthUntil256RoundTrip(char character)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new PDF417EncodingOptions
                {
                    Width = 1,
                    Height = 1,
                    CharacterSet = "UTF-8"
                }
            };
            var reader = new BarcodeReader();
            reader.Options.PureBarcode = true;
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.PDF_417
            };
            var errors = new Dictionary<string, Exception>();
            var content = new StringBuilder();
            for (int i = 0; i < 256; i++)
            {
                content.Append(character);
                try
                {
                    var bitmap = writer.Write(content.ToString());
                    var result = reader.Decode(bitmap);
                    if (result == null)
                        throw new InvalidOperationException("cant be decoded");
                    Assert.That(result.Text, Is.EqualTo(content.ToString()));
                }
                catch (Exception e)
                {
                    errors[content.ToString()] = e;
                }
            }

            foreach (var error in errors)
            {
                Console.WriteLine($"Content: {error.Key}; Error: {error.Value.Message}");
            }

            if (errors.Count > 0)
                throw new AssertionException("not every content could be encoded and decoded");
        }

    }
}
