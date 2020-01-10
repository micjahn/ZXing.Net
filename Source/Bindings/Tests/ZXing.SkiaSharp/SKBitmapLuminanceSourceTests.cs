/*
* Copyright 2013 ZXing.Net authors
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
using SkiaSharp;
using System;
using System.IO;

namespace ZXing.SkiaSharp.Test
{
    [TestFixture]
    public class SKBitmapLuminanceSourceTests
    {
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format1bppIndexed.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format4bppIndexed.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format8bppIndexed.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format16bppRgb565.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format24bppRgb.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format32bppRgb.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format32bppArgb.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format16bppRgb555.bmp", "abc")]
        [TestCase("../../../../../test/data/luminance/qr-code-abc-Format16bppArgb1555.bmp", "abc")]
        public void Should_Calculate_Luminance_And_Decode(string fileName, string content)
        {
            using (var bitmap = loadImage(fileName))
            {
                var luminance = new SKBitmapLuminanceSource(bitmap);
                var reader = new BarcodeReader();
                var result = reader.Decode(luminance);
                Assert.That(result?.Text, Is.EqualTo(content));
            }
        }

        private static SKBitmap loadImage(String fileName)
        {
            String file = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
            if (!File.Exists(file))
            {
                // try starting with 'core' since the test base is often given as the project root
                file = Path.GetFullPath("..\\..\\..\\Source\\" + fileName);
            }
            Assert.IsTrue(File.Exists(file), "Please run from the 'core' directory");

            return SKBitmap.Decode(file);
        }
    }
}