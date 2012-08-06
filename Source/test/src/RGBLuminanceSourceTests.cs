using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;

namespace ZXing.Test
{
   [TestFixture]
   public class RGBLuminanceSourceTests
   {
      private const string samplePicRelPath = @"../../../Source/test/data/blackbox/codabar-1/01.jpg";
      private LuminanceSource samplePicLuminance;

      [SetUp]
      public void Setup()
      {
         var bitmap = (Bitmap)Image.FromFile(samplePicRelPath);
         samplePicLuminance = new RGBLuminanceSource(bitmap);
      }

      [Test]
      public void RGBLuminanceSource_Should_Work_With_BitmapImage()
      {
         var pixelFormats = new []
                               {
                                  PixelFormats.Bgr24,
                                  PixelFormats.Bgr32,
                                  PixelFormats.Bgra32,
                                  PixelFormats.Rgb24,
                                  //PixelFormats.Bgr565, // conversion isn't accurate to compare it directly to RGB24
                                  //PixelFormats.Bgr555, // conversion isn't accurate to compare it directly to RGB24
                                  PixelFormats.Gray8,
                               };
         foreach (var pixelFormat in pixelFormats)
         {
            BitmapSource bitmapImage = new BitmapImage(new Uri(samplePicRelPath, UriKind.RelativeOrAbsolute));
            if (bitmapImage.Format != pixelFormat)
               bitmapImage = new FormatConvertedBitmap(bitmapImage, pixelFormat, null, 0);
            var rgbLuminanceSource = new RGBLuminanceSource(bitmapImage);
            Assert.That(IsEqual(samplePicLuminance.Matrix, rgbLuminanceSource.Matrix));
         }
      }

      private static bool IsEqual(byte[] left, byte[] right)
      {
         if (left.Length != right.Length)
            return false;

         for (var index = 0; index < left.Length; index++)
         {
            if (left[index] != right[index])
               return false;
         }

         return true;
      }
   }
}
