
namespace ZXing.ImageSharp.Test
{
    using System;

    using NUnit.Framework;

    using SelectedPixelFormat = SixLabors.ImageSharp.PixelFormats.Argb32;

    [TestFixture]
    public class RenderingTests
    {
        [Test]
        [Explicit("only for manually measuring the rendering speed")]
        public void Measure_Performance()
        {
            var writer = new BarcodeWriter<SelectedPixelFormat>
            {
                Format = BarcodeFormat.AZTEC,
                Options = new ZXing.Aztec.AztecEncodingOptions
                {
                    Width = 1000,
                    Height = 1000
                }
            };
            var stopWatch = new System.Diagnostics.Stopwatch();

            stopWatch.Restart();
            var bmp1 = writer.Write("123456789012");
            stopWatch.Stop();
            Assert.That(bmp1, Is.Not.Null);
            Console.WriteLine(stopWatch.Elapsed);

            stopWatch.Restart();
            var bmp2 = writer.Write("123456789012");
            stopWatch.Stop();
            Assert.That(bmp2, Is.Not.Null);
            Console.WriteLine(stopWatch.Elapsed);

            var image = writer.Write("123456789012");
            using (var fileStream = System.IO.File.Create("test.png"))
            {
                image.Save(fileStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            }
        }

        [TestCase(BarcodeFormat.AZTEC, "123456789012")]
        [TestCase(BarcodeFormat.CODE_128, "N-c9fc4114210805000022")]
        public void Roundtrip_Test(BarcodeFormat format, string content)
        {
            var writer = new BarcodeWriter<SelectedPixelFormat>
            {
                Format = format,
                Options = new Common.EncodingOptions
                {
                    Width = 1000,
                    Height = 1000
                }
            };
            var image = writer.Write(content);
            using (var fileStream = System.IO.File.Create(content + ".png"))
            {
                image.Save(fileStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            }

            var reader = new BarcodeReader<SelectedPixelFormat>()
            {
                Options = new Common.DecodingOptions
                {
                    PureBarcode = true
                }
            };
            var decodingResult = reader.Decode(image);

            Assert.That(decodingResult?.Text, Is.Not.Null);
            Assert.That(decodingResult?.Text, Is.EqualTo(content));
        }
    }
}
