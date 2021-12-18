
namespace ZXing.SkiaSharp.Test
{
    using System;

    using Aztec;

    using global::SkiaSharp;

    using NUnit.Framework;

    [TestFixture]
    public class RenderingPerformanceTests
    {
        [Test]
        [Explicit("only for manually measuring the rendering speed")]
        public void Measure()
        {
            var writer = new BarcodeWriter
            {
                // Format = BarcodeFormat.EAN_13,
                Format = BarcodeFormat.AZTEC,
                Options = new AztecEncodingOptions
                {
                    Width = 1000,
                    Height = 1000
                }
            };
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Restart();
            writer.Write("123456789012");
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            stopWatch.Restart();
            writer.Write("123456789012");
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);

            var bitmap = writer.Write("123456789012");
            using (var fileStream = System.IO.File.Create("C:\\ZXing.Net\\test.png"))
            using (var dst = new SKManagedWStream(fileStream))
            {
                bitmap.Encode(dst, SKEncodedImageFormat.Png, 100);
            }
        }
    }
}
