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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using ZXing.Common;
using ZXing.PDF417;

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

      private static readonly RGBLuminanceSource SOURCE = new RGBLuminanceSource(new byte[]
      {
         0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0xFF, 0xFF, 0xFF,
         0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF,
         0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0x00
      }, 3, 3);

      [Test]
      public void testCrop()
      {
         Assert.IsTrue(SOURCE.CropSupported);
         LuminanceSource cropped = SOURCE.crop(1, 1, 1, 1);
         Assert.AreEqual(1, cropped.Height);
         Assert.AreEqual(1, cropped.Width);
         // java and .Net differs, not sure, why
         //var expectedInJava = new byte[] {0x7F};
         var expected = new byte[] { 0x95 };
         Assert.AreEqual(expected, cropped.getRow(0, null));
      }

      [Test]
      public void testMatrix()
      {
         // java and .Net differs, not sure, why
         //var expectedInJava = new byte[] {0x00, 0x7F, 0xFF, 0x3F, 0x7F, 0x3F, 0x3F, 0x7F, 0x3F};
         var expected = new byte[] {0x00, 0x7F, 0xFF, 0x4c, 0x95, 0x1c, 0x1c, 0x95, 0x4c};
         Assert.AreEqual(expected, SOURCE.Matrix);
         var croppedFullWidth = SOURCE.crop(0, 1, 3, 2);
         Assert.AreEqual(new byte[] { 0x4c, 0x95, 0x1c, 0x1c, 0x95, 0x4c }, croppedFullWidth.Matrix);
         var croppedCorner = SOURCE.crop(1, 1, 2, 2);
         Assert.AreEqual(new byte[] { 0x95, 0x1c, 0x95, 0x4c }, croppedCorner.Matrix);
      }

      [Test]
      public void testGetRow()
      {
         // java and .Net differs, not sure, why
         //var expectedInJava = new byte[] {0x3F, 0x7F, 0x3F};
         var expected = new byte[] {0x1c, 0x95, 0x4c};
         Assert.AreEqual(expected, SOURCE.getRow(2, new byte[3]));
      }

      [Test]
      public void testToString()
      {
         // java and .Net differs, not sure, why
         //var expectedInJava = "#+ \n#+#\n#+#\n";
         var expected = "#+ \n+.#\n#.+\n";
         Assert.AreEqual(expected, SOURCE.ToString());
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

      [Test]
      public void Hex2Pdf417()
      {
         var hexStr = "fe3009333137303130323031f9200134fe300120fc2006";

         //hexStr =
            //"30303142415243303030333739373830323541463937363330303130303106414142415243fe3009333137303130323031f9200134fe30fb2006373937383032f420114f6e6c696e652d4d61686e616e74726167fe200a30312e31322e32303038d6200430314b53fe20023030ce200a3538303831486167656edb200158fc200158f4200430314153fe2007303120476d6248e2200b59616e64757520476d6248b9200430314153fe200d30335765737472696e67203235e9200b3434373837426f6368756dec200144ce203c30314153475620303147657363688466747366816872656e64657220476573656c6c736368616674657220204a756c69656e204c81747a656e696368bd2012303142414e4b203030314445383334333035fe300b3130303431343136363239f5200b57454c4144454431424f43b8200430314147fe2003303131de20034a6f6be120034a6f6bd4200430314147fe200e30334f737473747261e165203235ea200e343535323548617474696e67656eef200144ce200430314147fe200e3034313434373832426f6368756d96200730314153504b20fe300135f72008526563686e756e67e6200435313031fe35e5200c313531323031313730313035fe300131fb30f2201030315a494e53203030382c3132204231912009303141535053203031fb300134fe30323135313230313137303130355665727a756773706175736368616c652067656d2e20f52032383820424742204162732e2034c6200642424e525748fe300133fb300131fb300138fb300135f7300131e9300131c420";
         var byteArray = Enumerable.Range(0, hexStr.Length / 2).Select(x => Convert.ToByte(hexStr.Substring(x * 2, 2), 16)).ToArray();
         var byteArrayAsString = new String(byteArray.Select(b => (char)b).ToArray());

         // encode the string as PDF417
         var writer = new BarcodeWriter
         {
            Format = BarcodeFormat.PDF_417,
            Options = new PDF417EncodingOptions
            {
               Height = 200,
               Width = 200,
               Margin = 10
            }
         };
         var bitmap = writer.Write(byteArrayAsString);
         bitmap.Save("test.bmp", ImageFormat.Jpeg);

         // try to decode the PDF417
         var reader = new BarcodeReader
         {
            Options = new DecodingOptions
            {
               PossibleFormats = new List<BarcodeFormat>
               {
                  BarcodeFormat.PDF_417
               },
               PureBarcode = true
            }
         };
         var result = reader.Decode(bitmap);

         // make sure, the result is the same as the original hex
         var resultBackToBytes = result.Text.Select(c => (byte)c).ToArray();
         var resultAsHexString = String.Join("", resultBackToBytes.Select(b => b.ToString("x2")));

         Assert.That(resultAsHexString, Is.EqualTo(hexStr));
      }
   }
}
