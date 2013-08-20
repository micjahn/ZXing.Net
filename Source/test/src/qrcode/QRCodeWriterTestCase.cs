/*
 * Copyright 2008 ZXing authors
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
using System.IO;
#if !SILVERLIGHT
using System.Drawing;
using System.Drawing.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif
using NUnit.Framework;

using ZXing.Common;
using ZXing.QrCode.Internal;

namespace ZXing.QrCode.Test
{
   /// <summary>
   /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
   /// <author>dswitkin@google.com (Daniel Switkin) - ported and expanded from C++</author>
   /// </summary>
   [TestFixture]
   public sealed class QRCodeWriterTestCase
   {

      private static String BASE_IMAGE_PATH = "test/data/golden/qrcode/";
#if !SILVERLIGHT
      private static Bitmap loadImage(String fileName)
#else
      private static WriteableBitmap loadImage(String fileName)
#endif
      {
         String file = BASE_IMAGE_PATH + fileName;
         if (!File.Exists(file))
         {
            // try starting with 'core' since the test base is often given as the project root
            file = "..\\..\\..\\Source\\" + BASE_IMAGE_PATH + fileName;
         }
         Assert.IsTrue(File.Exists(file), "Please run from the 'core' directory");
#if !SILVERLIGHT
         return (Bitmap)Bitmap.FromFile(file);
#else
         var wb = new WriteableBitmap(0, 0);
         wb.SetSource(File.OpenRead(file));
         return wb;
#endif
      }

#if !SILVERLIGHT
      // In case the golden images are not monochromatic, convert the RGB values to greyscale.
      private static BitMatrix createMatrixFromImage(Bitmap image)
      {
         int width = image.Width;
         int height = image.Height;

         var data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                                   PixelFormat.Format24bppRgb);
         //image.getRGB(0, 0, width, height, pixels, 0, width);

         BitMatrix matrix = new BitMatrix(width, height);
         try
         {
            unsafe
            {
               for (int y = 0; y < height; y++)
               {
                  var bitmapRow = (byte*)data.Scan0 + (y * data.Stride);
                  for (int x = 0; x < width; x++)
                  {
                     int pixelR = bitmapRow[3 * x + 0];
                     int pixelG = bitmapRow[3 * x + 1];
                     int pixelB = bitmapRow[3 * x + 2];
                     int luminance = (306 * pixelR +
                                      601 * pixelG +
                                      117 * pixelB) >> 10;
                     if (luminance <= 0x7F)
                     {
                        matrix[x, y] = true;
                     }
                  }
               }
            }
         }
         finally
         {
            image.UnlockBits(data);
         }
         return matrix;
      }
#else
      // In case the golden images are not monochromatic, convert the RGB values to greyscale.
      private static BitMatrix createMatrixFromImage(WriteableBitmap image)
      {
         int width = image.PixelWidth;
         int height = image.PixelHeight;

         BitMatrix matrix = new BitMatrix(width, height);
         for (int y = 0; y < height; y++)
         {
            int offset = y * width;
            for (int x = 0; x < width; x++)
            {
               int srcPixel = image.Pixels[x + offset];
               var c = Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                     (byte)((srcPixel >> 0x10) & 0xff),
                     (byte)((srcPixel >> 8) & 0xff),
                     (byte)(srcPixel & 0xff));
               int luminance = (306 * c.R +
                                601 * c.G +
                                117 * c.B) >> 10;
               if (luminance <= 0x7F)
               {
                  matrix[x, y] = true;
               }
            }
         }
         
         return matrix;
      }
#endif

      [Test]
      public void testQRCodeWriter()
      {
         // The QR should be multiplied up to fit, with extra padding if necessary
         int bigEnough = 256;
         QRCodeWriter writer = new QRCodeWriter();
         BitMatrix matrix = writer.encode("http://www.google.com/", BarcodeFormat.QR_CODE, bigEnough,
             bigEnough, null);
         Assert.NotNull(matrix);
         Assert.AreEqual(bigEnough, matrix.Width);
         Assert.AreEqual(bigEnough, matrix.Height);

         // The QR will not fit in this size, so the matrix should come back bigger
         int tooSmall = 20;
         matrix = writer.encode("http://www.google.com/", BarcodeFormat.QR_CODE, tooSmall,
             tooSmall, null);
         Assert.NotNull(matrix);
         Assert.IsTrue(tooSmall < matrix.Width);
         Assert.IsTrue(tooSmall < matrix.Height);

         // We should also be able to handle non-square requests by padding them
         int strangeWidth = 500;
         int strangeHeight = 100;
         matrix = writer.encode("http://www.google.com/", BarcodeFormat.QR_CODE, strangeWidth,
             strangeHeight, null);
         Assert.NotNull(matrix);
         Assert.AreEqual(strangeWidth, matrix.Width);
         Assert.AreEqual(strangeHeight, matrix.Height);
      }

      private static void compareToGoldenFile(String contents,
                                              ErrorCorrectionLevel ecLevel,
                                              int resolution,
                                              String fileName)
      {
         var image = loadImage(fileName);
         Assert.NotNull(image);
         BitMatrix goldenResult = createMatrixFromImage(image);
         Assert.NotNull(goldenResult);

         QRCodeWriter writer = new QRCodeWriter();
         IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
         hints[EncodeHintType.ERROR_CORRECTION] = ecLevel;
         BitMatrix generatedResult = writer.encode(contents, BarcodeFormat.QR_CODE, resolution,
             resolution, hints);

         Assert.AreEqual(resolution, generatedResult.Width);
         Assert.AreEqual(resolution, generatedResult.Height);
         Assert.AreEqual(goldenResult, generatedResult);
      }

      // Golden images are generated with "qrcode_sample.cc". The images are checked with both eye balls
      // and cell phones. We expect pixel-perfect results, because the error correction level is known,
      // and the pixel dimensions matches exactly. 
      [Test]
      public void testRegressionTest()
      {
         compareToGoldenFile("http://www.google.com/", ErrorCorrectionLevel.M, 99,
             "renderer-test-01.png");
      }

#if !SILVERLIGHT
      [Test]
      [Explicit]
      public void test_Random_Encoding_Decoding_Cycles_Up_To_1000()
       {
           int bigEnough = 256;

           byte[] data = new byte[256];
           Random random = new Random(2344);

           for (int i = 0; i < 1000; i++)
           {
              random.NextBytes(data);
              //string content = "U/QcYPdz4MTR2nD2+vv88mZVnLA9/h+EGrEu3mwRIP65DlM6vLwlAwv/Ztd5LkHsio3UEJ29C1XUl0ZGRAFYv7pxPeyowjWqL5ilPZhICutvQlTePBBg+wP+ZiR2378Jp6YcB/FVRMdXKuAEGM29i41a1gKseYKpEEHpqlwRNE/Zm5bxKwL5Gv2NhxIvXOM1QNqWGwm9XC0jcvawbJprRfaRK3w3y2CKYbwEH/FwerRds2mBehhFHD5ozbgLSa1iIkIbnjBn/XV6DLpNuD08s/hCUrgx6crdSw89z/2nfxcOov2vVNuE9rbzB25e+GQBLBq/yfb1MTh3PlMhKS530w==";
              string content = Convert.ToBase64String(data);

              BarcodeWriter writer = new BarcodeWriter
                 {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                       {
                          Height = bigEnough,
                          Width = bigEnough
                       }
                 };
              Bitmap bmp = writer.Write(content);

              var reader = new BarcodeReader
                 {
                    Options = new DecodingOptions
                       {
                          PureBarcode = true,
                          PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.QR_CODE}
                       }
                 };
              var decodedResult = reader.Decode(bmp);

              Assert.IsNotNull(decodedResult);
              Assert.AreEqual(content, decodedResult.Text);
           }
       }
#endif
   }
}