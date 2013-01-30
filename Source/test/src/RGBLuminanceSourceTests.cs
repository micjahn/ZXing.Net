/*
* Copyright 2012 ZXing.Net authors
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
using System.IO;
using System.IO.Compression;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;

namespace ZXing.Test
{
   [TestFixture]
   public class RGBLuminanceSourceTests
   {
      private const string samplePicRelPath = @"../../../Source/test/data/luminance/01.jpg";
      private const string samplePicRelResultPath = @"../../../Source/test/data/luminance/01Result.txt.gz";
      private string samplePicRelResult;

      [SetUp]
      public void Setup()
      {
         using (var stream = File.OpenRead(samplePicRelResultPath))
         using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress, true))
         using (var reader = new StreamReader(deflateStream))
            samplePicRelResult = reader.ReadToEnd();

         using (var stream = File.OpenRead(cropSamplePicRelResultPath))
         using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress, true))
         using (var reader = new StreamReader(deflateStream))
            cropSamplePicRelResult = reader.ReadToEnd();
      }

      [Test]
      public void RGBLuminanceSource_Should_Work_With_BitmapImage()
      {
         var pixelFormats = new []
                               {
                                  PixelFormats.Bgr24,
                                  PixelFormats.Bgr32,
                                  // PixelFormats.Bgra32, // TODO: alpha channel calculation isn't fully accurate
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
            var rgbLuminanceSource = new BitmapSourceLuminanceSource(bitmapImage);
            var rgbLuminanceSourceResult = rgbLuminanceSource.ToString();
            Assert.That(samplePicRelResult.Equals(rgbLuminanceSourceResult));
         }
      }

      [Test]
      public void RGB565LuminanceSource_Should_Give_The_Same_Result_As_RGBLuminanceSource_For_BGR565()
      {
         BitmapSource bitmapImage = new BitmapImage(new Uri(samplePicRelPath, UriKind.RelativeOrAbsolute));
         bitmapImage = new FormatConvertedBitmap(bitmapImage, PixelFormats.Bgr565, null, 0);
         var bytes = new byte[bitmapImage.PixelHeight*bitmapImage.PixelWidth*2];
         bitmapImage.CopyPixels(bytes, bitmapImage.PixelWidth * 2, 0);

         var rgb565LuminanceSource = new RGB565LuminanceSource(bytes, bitmapImage.PixelWidth, bitmapImage.PixelHeight);

         var rgbLuminanceSource = new BitmapSourceLuminanceSource(bitmapImage);

         Assert.AreEqual(rgbLuminanceSource.ToString(), rgb565LuminanceSource.ToString());
      }

      private const string cropSamplePicRelPath = @"../../../Source/test/data/luminance/crop_sample.png";
      private const string cropSamplePicRelResultPath = @"../../../Source/test/data/luminance/crop_sample.txt.gz";
      private string cropSamplePicRelResult;

      [Test]
      public void Should_Support_Cropping()
      {
         BitmapSource bitmapImage = new BitmapImage(new Uri(cropSamplePicRelPath, UriKind.RelativeOrAbsolute));
         var rgbLuminanceSource = new BitmapSourceLuminanceSource(bitmapImage);
         var croppedImage = rgbLuminanceSource.crop(0, 0, rgbLuminanceSource.Width / 2, rgbLuminanceSource.Height/5);
         var result = croppedImage.ToString();
         Assert.AreEqual(cropSamplePicRelResult, result);
      }
   }
}
