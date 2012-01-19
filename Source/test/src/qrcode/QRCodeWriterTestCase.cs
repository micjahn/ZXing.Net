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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;
using NUnit.Framework;

namespace com.google.zxing.qrcode
{



   /// <summary>
   /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
   /// <author>dswitkin@google.com (Daniel Switkin) - ported and expanded from C++</author>
   /// </summary>
   [TestFixture]
   public sealed class QRCodeWriterTestCase
   {

      private static String BASE_IMAGE_PATH = "test/data/golden/qrcode/";

      private static Bitmap loadImage(String fileName)
      {
         String file = BASE_IMAGE_PATH + fileName;
         if (!File.Exists(file))
         {
            // try starting with 'core' since the test base is often given as the project root
            file = "..\\..\\..\\Source\\" + BASE_IMAGE_PATH + fileName;
         }
         Assert.IsTrue(File.Exists(file), "Please run from the 'core' directory");
         return (Bitmap)Bitmap.FromFile(file);
      }

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
                  var bitmapRow = (byte*) data.Scan0 + (y*data.Stride);
                  for (int x = 0; x < width; x++)
                  {
                     int pixel = bitmapRow[3*x];
                     int luminance = (306*((pixel >> 16) & 0xFF) +
                                      601*((pixel >> 8) & 0xFF) +
                                      117*(pixel & 0xFF)) >> 10;
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

         Bitmap image = loadImage(fileName);
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

         compareToGoldenFile("12345", ErrorCorrectionLevel.L, 58, "renderer-test-02.png");

         // Test in Katakana in Shift_JIS.
         // TODO: this test is bogus now that byte mode has been basically fixed to assuming ISO-8859-1 encoding
         //  The real solution is to implement Kanji mode, in which case the golden file will be wrong again
         /*
         compareToGoldenFile(
             new String(new sbyte[] {(byte)0x83, 0x65, (byte)0x83, 0x58, (byte)0x83, 0x67}, "Shift_JIS"),
             ErrorCorrectionLevel.H, 145,
             "renderer-test-03.png");
         */
      }
   }
}