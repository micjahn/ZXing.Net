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
using System.Drawing;
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
            using (var bitmap = LoadSKBitmap(fileName))
            {
                var luminance = new SKBitmapLuminanceSource(bitmap);
                var reader = new BarcodeReader();
                var result = reader.Decode(luminance);
                Assert.That(result?.Text, Is.EqualTo(content));
            }
        }

        [TestCase("../../../../../test/data/blackbox/falsepositives/20.png")]
        [TestCase("../../../../../test/data/blackbox/upce-2/36.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-5/15.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-4/05.png")]
        [TestCase("../../../../../test/data/blackbox/code128-2/37.png")]
        [TestCase("../../../../../test/data/blackbox/partial/16.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-4/25.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-3/33.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-2/19.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpandedstacked-1/45.png")]
        [TestCase("../../../../../test/data/blackbox/upca-2/35.png")]
        [TestCase("../../../../../test/data/blackbox/rss14-2/1.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpandedstacked-1/12.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-2/13.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/49.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpandedstacked-1/39.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-2/28.png")]
        [TestCase("../../../../../test/data/blackbox/code39-3/08.png")]
        [TestCase("../../../../../test/data/blackbox/upce-2/03.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-5/06.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/18.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-3/28.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-1/10.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/114.png")]
        [TestCase("../../../../../test/data/blackbox/pdf417-3/19.png")]
        [TestCase("../../../../../test/data/blackbox/upca-6/04.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-1/30.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-4/44.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/115.png")]
        [TestCase("../../../../../test/data/benchmark/android-2/ean13-1.png")]
        [TestCase("../../../../../test/data/blackbox/codabar-1/09.png")]
        [TestCase("../../../../../test/data/blackbox/itf-2/08.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-3/23.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-5/03.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-5/02.png")]
        [TestCase("../../../../../test/data/blackbox/pdf417-3/04.png")]
        [TestCase("../../../../../test/data/blackbox/partial/23.png")]
        [TestCase("../../../../../test/data/blackbox/upce-2/21.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-1/31.png")]
        [TestCase("../../../../../test/data/blackbox/rss14-1/4.png")]
        [TestCase("../../../../../test/data/blackbox/falsepositives/18.png")]
        [TestCase("../../../../../test/data/blackbox/datamatrix-2/13.png")]
        [TestCase("../../../../../test/data/blackbox/partial/08.png")]
        [TestCase("../../../../../test/data/blackbox/code128-2/33.png")]
        [TestCase("../../../../../test/data/blackbox/upcean-extension-1/1.png")]
        [TestCase("../../../../../test/data/blackbox/aztec-1/lorem-075x075.png")]
        [TestCase("../../../../../test/data/blackbox/datamatrix-2/14.png")]
        [TestCase("../../../../../test/data/blackbox/codabar-1/02.png")]
        [TestCase("../../../../../test/data/blackbox/partial/15.png")]
        [TestCase("../../../../../test/data/blackbox/upca-2/18.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/25.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-1/13.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-5/13.png")]
        [TestCase("../../../../../test/data/blackbox/itf-2/05.png")]
        [TestCase("../../../../../test/data/blackbox/imb-1/04.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/106.png")]
        [TestCase("../../../../../test/data/blackbox/upca-6/05.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-3/10.png")]
        [TestCase("../../../../../test/data/benchmark/android-2/fail-2.png")]
        [TestCase("../../../../../test/data/blackbox/datamatrix-1/GUID.png")]
        [TestCase("../../../../../test/data/blackbox/upca-4/5.png")]
        [TestCase("../../../../../test/data/blackbox/upca-1/29.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-4/14.png")]
        [TestCase("../../../../../test/data/blackbox/falsepositives-2/12.png")]
        [TestCase("../../../../../test/data/blackbox/upcean-extension-1/2.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-1/3.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-2/23.png")]
        [TestCase("../../../../../test/data/blackbox/codabar-1/12.png")]
        [TestCase("../../../../../test/data/blackbox/itf-2/10.png")]
        [TestCase("../../../../../test/data/blackbox/code128-2/34.png")]
        [TestCase("../../../../../test/data/blackbox/datamatrix-2/04.png")]
        [TestCase("../../../../../test/data/blackbox/partial/31.png")]
        [TestCase("../../../../../test/data/blackbox/upca-2/49.png")]
        [TestCase("../../../../../test/data/blackbox/rssexpanded-3/102.png")]
        [TestCase("../../../../../test/data/blackbox/upca-5/15.png")]
        [TestCase("../../../../../test/data/blackbox/imb-1/09.png")]
        [TestCase("../../../../../test/data/blackbox/ean13-3/14.png")]
        [TestCase("../../../../../test/data/blackbox/pdf417-2/14.png")]
        [TestCase("../../../../../test/data/blackbox/qrcode-4/22.png")]
        public void Luminance_Is_Within_Margin_Of_Error(string fileName)
        {
            byte[] controlLuminanceSourceMatrix;
            using (var control = LoadBitmapImage(fileName))
            {
                var bitmapLuminanceSource = new BitmapLuminanceSource(control);
                controlLuminanceSourceMatrix = bitmapLuminanceSource.Matrix;
            }

            byte[] testLuminanceSourceMatrix;
            using (var bitmap = LoadSKBitmap(fileName))
            {
                var skBitmapLuminanceSource = new SKBitmapLuminanceSource(bitmap);
                testLuminanceSourceMatrix = skBitmapLuminanceSource.Matrix;
            }

            Assert.AreEqual(controlLuminanceSourceMatrix.Length, testLuminanceSourceMatrix.Length);

            for (int index = 0; index < controlLuminanceSourceMatrix.Length; index++)
            {
                var controlColor = controlLuminanceSourceMatrix[index];
                var testColor = testLuminanceSourceMatrix[index];

                Assert.That(testColor, Is.EqualTo(controlColor).Within(3));
            }
        }

        private static SKBitmap LoadSKBitmap(string fileName)
        {
            string file = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
            if (!File.Exists(file))
            {
                // try starting with 'core' since the test base is often given as the project root
                file = Path.GetFullPath("..\\..\\..\\Source\\" + fileName);
            }
            Assert.IsTrue(File.Exists(file), "Please run from the 'core' directory");

            return SKBitmap.Decode(file);
        }

        private static Bitmap LoadBitmapImage(string fileName)
        {
            string file = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
            if (!File.Exists(file))
            {
                // try starting with 'core' since the test base is often given as the project root
                file = Path.GetFullPath("..\\..\\..\\Source\\" + fileName);
            }
            Assert.IsTrue(File.Exists(file), "Please run from the 'core' directory");

            return (Bitmap)Image.FromFile(file);
        }

    }
}