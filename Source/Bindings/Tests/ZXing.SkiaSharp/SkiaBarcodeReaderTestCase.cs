using SkiaSharp;
using System;
using ZXing.Common.Test;

namespace ZXing.SkiaSharp
{
    /// <summary>
    /// <author>Sean Owen</author>
    /// </summary>
    public sealed class SkiaBarcodeReaderTestCase : TestCaseBase<SKBitmap>
    {
        public SkiaBarcodeReaderTestCase()
           : base("../../test/data/blackbox/code39-1", new BarcodeReader(), BarcodeFormat.CODE_39)
        {
            addTest(1, 1, 0.0f);
            addTest(1, 1, 90.0f);
            addTest(1, 1, 180.0f);
            addTest(1, 1, 270.0f);
        }

        protected override SKBitmap openFromFile(string filePath)
        {
            return SKBitmap.Decode(filePath);
        }

        protected override SKBitmap rotateImage(SKBitmap original, float degrees)
        {
            double radians = Math.PI * degrees / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2.0f, rotatedHeight / 2.0f);
                surface.RotateDegrees(degrees);
                surface.Translate(-originalWidth / 2.0f, -originalHeight / 2.0f);
                surface.DrawBitmap(original, 0, 0);
            }

            return rotatedBitmap;
        }
    }
}
