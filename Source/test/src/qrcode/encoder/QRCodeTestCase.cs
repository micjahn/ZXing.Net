/**
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

using NUnit.Framework;

namespace ZXing.QrCode.Internal.Test
{
   /// <summary>
   /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
   /// <author>mysen@google.com (Chris Mysen) - ported from C++</author>
   /// </summary>
   [TestFixture]
   public sealed class QRCodeTestCase
   {

      [Test]
      public void test()
      {
         var qrCode = new QRCode();
         // Initially the QR Code should be invalid.
         Assert.IsFalse(qrCode.Valid);

         // First, test simple setters and getters.
         // We use numbers of version 7-H.
         qrCode.Mode = Mode.BYTE;
         qrCode.ECLevel = ErrorCorrectionLevel.H;
         qrCode.Version = 7;
         qrCode.MatrixWidth = 45;
         qrCode.MaskPattern = 3;
         qrCode.NumTotalBytes = 196;
         qrCode.NumDataBytes = 66;
         qrCode.NumECBytes = 130;
         qrCode.NumRSBlocks = 5;

         Assert.AreEqual(Mode.BYTE, qrCode.Mode);
         Assert.AreEqual(ErrorCorrectionLevel.H, qrCode.ECLevel);
         Assert.AreEqual(7, qrCode.Version);
         Assert.AreEqual(45, qrCode.MatrixWidth);
         Assert.AreEqual(3, qrCode.MaskPattern);
         Assert.AreEqual(196, qrCode.NumTotalBytes);
         Assert.AreEqual(66, qrCode.NumDataBytes);
         Assert.AreEqual(130, qrCode.NumECBytes);
         Assert.AreEqual(5, qrCode.NumRSBlocks);

         // It still should be invalid.
         Assert.IsFalse(qrCode.Valid);

         // Prepare the matrix.
         var matrix = new ByteMatrix(45, 45);
         // Just set bogus zero/one values.
         for (int y = 0; y < 45; ++y)
         {
            for (int x = 0; x < 45; ++x)
            {
               matrix.set(x, y, (y + x) % 2 == 1);
            }
         }

         // Set the matrix.
         qrCode.Matrix = matrix;
         Assert.AreEqual(matrix, qrCode.Matrix);

         // Finally, it should be valid.
         Assert.IsTrue(qrCode.Valid);

         // Make sure "at()" returns the same value.
         for (int y = 0; y < 45; ++y)
         {
            for (int x = 0; x < 45; ++x)
            {
               Assert.AreEqual((y + x) % 2, qrCode.at(x, y));
            }
         }
      }

      [Test]
      public void testToString()
      {
         {
            var qrCode = new QRCode();
            const string expected = "<<\n" +
                                    " mode: \n" +
                                    " ecLevel: \n" +
                                    " version: -1\n" +
                                    " matrixWidth: -1\n" +
                                    " maskPattern: -1\n" +
                                    " numTotalBytes: -1\n" +
                                    " numDataBytes: -1\n" +
                                    " numECBytes: -1\n" +
                                    " numRSBlocks: -1\n" +
                                    " matrix: null\n" +
                                    ">>\n";
            Assert.AreEqual(expected, qrCode.ToString());
         }
         {
            const string expected = "<<\n" +
                                    " mode: BYTE\n" +
                                    " ecLevel: H\n" +
                                    " version: 1\n" +
                                    " matrixWidth: 21\n" +
                                    " maskPattern: 3\n" +
                                    " numTotalBytes: 26\n" +
                                    " numDataBytes: 9\n" +
                                    " numECBytes: 17\n" +
                                    " numRSBlocks: 1\n" +
                                    " matrix:\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    " 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1\n" +
                                    " 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0\n" +
                                    ">>\n";
            var qrCode = new QRCode
                            {
                               Mode = Mode.BYTE,
                               ECLevel = ErrorCorrectionLevel.H,
                               Version = 1,
                               MatrixWidth = 21,
                               MaskPattern = 3,
                               NumTotalBytes = 26,
                               NumDataBytes = 9,
                               NumECBytes = 17,
                               NumRSBlocks = 1
                            };
            var matrix = new ByteMatrix(21, 21);
            for (int y = 0; y < 21; ++y)
            {
               for (int x = 0; x < 21; ++x)
               {
                  matrix.set(x, y, (y + x) % 2 == 1);
               }
            }
            qrCode.Matrix = matrix;
            Assert.IsTrue(qrCode.Valid);
            Assert.AreEqual(expected, qrCode.ToString());
         }
      }

      [Test]
      public void testIsValidMaskPattern()
      {
         Assert.IsFalse(QRCode.isValidMaskPattern(-1));
         Assert.IsTrue(QRCode.isValidMaskPattern(0));
         Assert.IsTrue(QRCode.isValidMaskPattern(7));
         Assert.IsFalse(QRCode.isValidMaskPattern(8));
      }
   }
}