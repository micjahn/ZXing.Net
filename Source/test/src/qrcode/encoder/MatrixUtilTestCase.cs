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

using NUnit.Framework;

using ZXing.Common;

namespace ZXing.QrCode.Internal.Test
{
   /// <summary>
   /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
   /// <author>mysen@google.com (Chris Mysen) - ported from C++</author>
   /// </summary>
   [TestFixture]
   public sealed class MatrixUtilTestCase
   {

      [Test]
      public void testToString()
      {
         ByteMatrix array = new ByteMatrix(3, 3);
         array.set(0, 0, 0);
         array.set(1, 0, 1);
         array.set(2, 0, 0);
         array.set(0, 1, 1);
         array.set(1, 1, 0);
         array.set(2, 1, 1);
         array.set(0, 2, 2);
         array.set(1, 2, 2);
         array.set(2, 2, 2);
         String expected = " 0 1 0\n" + " 1 0 1\n" + "      \n";
         Assert.AreEqual(expected, array.ToString());
      }

      [Test]
      public void testClearMatrix()
      {
         ByteMatrix matrix = new ByteMatrix(2, 2);
         MatrixUtil.clearMatrix(matrix);
         Assert.AreEqual(2, matrix[0, 0]);
         Assert.AreEqual(2, matrix[1, 0]);
         Assert.AreEqual(2, matrix[0, 1]);
         Assert.AreEqual(2, matrix[1, 1]);
      }

      [Test]
      public void testEmbedBasicPatterns()
      {
         {
            // Version 1.
            String expected =
              " 1 1 1 1 1 1 1 0           0 1 1 1 1 1 1 1\n" +
              " 1 0 0 0 0 0 1 0           0 1 0 0 0 0 0 1\n" +
              " 1 0 1 1 1 0 1 0           0 1 0 1 1 1 0 1\n" +
              " 1 0 1 1 1 0 1 0           0 1 0 1 1 1 0 1\n" +
              " 1 0 1 1 1 0 1 0           0 1 0 1 1 1 0 1\n" +
              " 1 0 0 0 0 0 1 0           0 1 0 0 0 0 0 1\n" +
              " 1 1 1 1 1 1 1 0 1 0 1 0 1 0 1 1 1 1 1 1 1\n" +
              " 0 0 0 0 0 0 0 0           0 0 0 0 0 0 0 0\n" +
              "             1                            \n" +
              "             0                            \n" +
              "             1                            \n" +
              "             0                            \n" +
              "             1                            \n" +
              " 0 0 0 0 0 0 0 0 1                        \n" +
              " 1 1 1 1 1 1 1 0                          \n" +
              " 1 0 0 0 0 0 1 0                          \n" +
              " 1 0 1 1 1 0 1 0                          \n" +
              " 1 0 1 1 1 0 1 0                          \n" +
              " 1 0 1 1 1 0 1 0                          \n" +
              " 1 0 0 0 0 0 1 0                          \n" +
              " 1 1 1 1 1 1 1 0                          \n";
            ByteMatrix matrix = new ByteMatrix(21, 21);
            MatrixUtil.clearMatrix(matrix);
            MatrixUtil.embedBasicPatterns(Version.getVersionForNumber(1), matrix);
            Assert.AreEqual(expected, matrix.ToString());
         }
         {
            // Version 2.  Position adjustment pattern should apppear at right
            // bottom corner.
            String expected =
              " 1 1 1 1 1 1 1 0                   0 1 1 1 1 1 1 1\n" +
              " 1 0 0 0 0 0 1 0                   0 1 0 0 0 0 0 1\n" +
              " 1 0 1 1 1 0 1 0                   0 1 0 1 1 1 0 1\n" +
              " 1 0 1 1 1 0 1 0                   0 1 0 1 1 1 0 1\n" +
              " 1 0 1 1 1 0 1 0                   0 1 0 1 1 1 0 1\n" +
              " 1 0 0 0 0 0 1 0                   0 1 0 0 0 0 0 1\n" +
              " 1 1 1 1 1 1 1 0 1 0 1 0 1 0 1 0 1 0 1 1 1 1 1 1 1\n" +
              " 0 0 0 0 0 0 0 0                   0 0 0 0 0 0 0 0\n" +
              "             1                                    \n" +
              "             0                                    \n" +
              "             1                                    \n" +
              "             0                                    \n" +
              "             1                                    \n" +
              "             0                                    \n" +
              "             1                                    \n" +
              "             0                                    \n" +
              "             1                   1 1 1 1 1        \n" +
              " 0 0 0 0 0 0 0 0 1               1 0 0 0 1        \n" +
              " 1 1 1 1 1 1 1 0                 1 0 1 0 1        \n" +
              " 1 0 0 0 0 0 1 0                 1 0 0 0 1        \n" +
              " 1 0 1 1 1 0 1 0                 1 1 1 1 1        \n" +
              " 1 0 1 1 1 0 1 0                                  \n" +
              " 1 0 1 1 1 0 1 0                                  \n" +
              " 1 0 0 0 0 0 1 0                                  \n" +
              " 1 1 1 1 1 1 1 0                                  \n";
            ByteMatrix matrix = new ByteMatrix(25, 25);
            MatrixUtil.clearMatrix(matrix);
            MatrixUtil.embedBasicPatterns(Version.getVersionForNumber(2), matrix);
            Assert.AreEqual(expected, matrix.ToString());
         }
      }

      [Test]
      public void testEmbedTypeInfo()
      {
         // Type info bits = 100000011001110.
         String expected =
           "                 0                        \n" +
           "                 1                        \n" +
           "                 1                        \n" +
           "                 1                        \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                                          \n" +
           "                 1                        \n" +
           " 1 0 0 0 0 0   0 1         1 1 0 0 1 1 1 0\n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                 0                        \n" +
           "                 1                        \n";
         ByteMatrix matrix = new ByteMatrix(21, 21);
         MatrixUtil.clearMatrix(matrix);
         MatrixUtil.embedTypeInfo(ErrorCorrectionLevel.M, 5, matrix);
         Assert.AreEqual(expected, matrix.ToString());
      }

      [Test]
      public void testEmbedVersionInfo()
      {
         // Version info bits = 000111 110010 010100
         String expected =
           "                     0 0 1                \n" +
           "                     0 1 0                \n" +
           "                     0 1 0                \n" +
           "                     0 1 1                \n" +
           "                     1 1 1                \n" +
           "                     0 0 0                \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           " 0 0 0 0 1 0                              \n" +
           " 0 1 1 1 1 0                              \n" +
           " 1 0 0 1 1 0                              \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n" +
           "                                          \n";
         // Actually, version 7 QR Code has 45x45 matrix but we use 21x21 here
         // since 45x45 matrix is too big to depict.
         ByteMatrix matrix = new ByteMatrix(21, 21);
         MatrixUtil.clearMatrix(matrix);
         MatrixUtil.maybeEmbedVersionInfo(Version.getVersionForNumber(7), matrix);
         Assert.AreEqual(expected, matrix.ToString());
      }

      [Test]
      public void testEmbedDataBits()
      {
         // Cells other than basic patterns should be filled with zero.
         String expected =
           " 1 1 1 1 1 1 1 0 0 0 0 0 0 0 1 1 1 1 1 1 1\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0 0 0 1\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 1 0 1 1 1 0 1\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 1 0 1 1 1 0 1\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 1 0 1 1 1 0 1\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0 0 0 1\n" +
           " 1 1 1 1 1 1 1 0 1 0 1 0 1 0 1 1 1 1 1 1 1\n" +
           " 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n" +
           " 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0\n";
         BitArray bits = new BitArray();
         ByteMatrix matrix = new ByteMatrix(21, 21);
         MatrixUtil.clearMatrix(matrix);
         MatrixUtil.embedBasicPatterns(Version.getVersionForNumber(1), matrix);
         MatrixUtil.embedDataBits(bits, -1, matrix);
         Assert.AreEqual(expected, matrix.ToString());
      }

      [Test]
      public void testBuildMatrix()
      {
         // From http://www.swetake.com/qr/qr7.html
         String expected =
           " 1 1 1 1 1 1 1 0 0 1 1 0 0 0 1 1 1 1 1 1 1\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0 0 0 1\n" +
           " 1 0 1 1 1 0 1 0 0 0 0 1 0 0 1 0 1 1 1 0 1\n" +
           " 1 0 1 1 1 0 1 0 0 1 1 0 0 0 1 0 1 1 1 0 1\n" +
           " 1 0 1 1 1 0 1 0 1 1 0 0 1 0 1 0 1 1 1 0 1\n" +
           " 1 0 0 0 0 0 1 0 0 0 1 1 1 0 1 0 0 0 0 0 1\n" +
           " 1 1 1 1 1 1 1 0 1 0 1 0 1 0 1 1 1 1 1 1 1\n" +
           " 0 0 0 0 0 0 0 0 1 1 0 1 1 0 0 0 0 0 0 0 0\n" +
           " 0 0 1 1 0 0 1 1 1 0 0 1 1 1 1 0 1 0 0 0 0\n" +
           " 1 0 1 0 1 0 0 0 0 0 1 1 1 0 0 1 0 1 1 1 0\n" +
           " 1 1 1 1 0 1 1 0 1 0 1 1 1 0 0 1 1 1 0 1 0\n" +
           " 1 0 1 0 1 1 0 1 1 1 0 0 1 1 1 0 0 1 0 1 0\n" +
           " 0 0 1 0 0 1 1 1 0 0 0 0 0 0 1 0 1 1 1 1 1\n" +
           " 0 0 0 0 0 0 0 0 1 1 0 1 0 0 0 0 0 1 0 1 1\n" +
           " 1 1 1 1 1 1 1 0 1 1 1 1 0 0 0 0 1 0 1 1 0\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 1 0 1 1 1 0 0 0 0 0\n" +
           " 1 0 1 1 1 0 1 0 0 1 0 0 1 1 0 0 1 0 0 1 1\n" +
           " 1 0 1 1 1 0 1 0 1 1 0 1 0 0 0 0 0 1 1 1 0\n" +
           " 1 0 1 1 1 0 1 0 1 1 1 1 0 0 0 0 1 1 1 0 0\n" +
           " 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 1 0 1 0 0\n" +
           " 1 1 1 1 1 1 1 0 0 0 1 1 1 1 1 0 1 0 0 1 0\n";
         int[] bytes = {32, 65, 205, 69, 41, 220, 46, 128, 236,
        42, 159, 74, 221, 244, 169, 239, 150, 138,
        70, 237, 85, 224, 96, 74, 219 , 61};
         BitArray bits = new BitArray();
         foreach (char c in bytes)
         {
            bits.appendBits(c, 8);
         }
         ByteMatrix matrix = new ByteMatrix(21, 21);
         MatrixUtil.buildMatrix(bits,
                                ErrorCorrectionLevel.H,
                                Version.getVersionForNumber(1),  // Version 1
                                3,  // Mask pattern 3
                                matrix);
         Assert.AreEqual(expected, matrix.ToString());
      }

      [Test]
      public void testFindMSBSet()
      {
         Assert.AreEqual(0, MatrixUtil.findMSBSet(0));
         Assert.AreEqual(1, MatrixUtil.findMSBSet(1));
         Assert.AreEqual(8, MatrixUtil.findMSBSet(0x80));
         Assert.AreEqual(32, MatrixUtil.findMSBSet(-2147483648 /*0x80000000*/));
      }

      [Test]
      public void testCalculateBCHCode()
      {
         // Encoding of type information.
         // From Appendix C in JISX0510:2004 (p 65)
         Assert.AreEqual(0xdc, MatrixUtil.calculateBCHCode(5, 0x537));
         // From http://www.swetake.com/qr/qr6.html
         Assert.AreEqual(0x1c2, MatrixUtil.calculateBCHCode(0x13, 0x537));
         // From http://www.swetake.com/qr/qr11.html
         Assert.AreEqual(0x214, MatrixUtil.calculateBCHCode(0x1b, 0x537));

         // Encofing of version information.
         // From Appendix D in JISX0510:2004 (p 68)
         Assert.AreEqual(0xc94, MatrixUtil.calculateBCHCode(7, 0x1f25));
         Assert.AreEqual(0x5bc, MatrixUtil.calculateBCHCode(8, 0x1f25));
         Assert.AreEqual(0xa99, MatrixUtil.calculateBCHCode(9, 0x1f25));
         Assert.AreEqual(0x4d3, MatrixUtil.calculateBCHCode(10, 0x1f25));
         Assert.AreEqual(0x9a6, MatrixUtil.calculateBCHCode(20, 0x1f25));
         Assert.AreEqual(0xd75, MatrixUtil.calculateBCHCode(30, 0x1f25));
         Assert.AreEqual(0xc69, MatrixUtil.calculateBCHCode(40, 0x1f25));
      }

      // We don't test a lot of cases in this function since we've already
      // tested them in TEST(calculateBCHCode).
      [Test]
      public void testMakeVersionInfoBits()
      {
         // From Appendix D in JISX0510:2004 (p 68)
         BitArray bits = new BitArray();
         MatrixUtil.makeVersionInfoBits(Version.getVersionForNumber(7), bits);
         Assert.AreEqual(" ...XXXXX ..X..X.X ..", bits.ToString());
      }

      // We don't test a lot of cases in this function since we've already
      // tested them in TEST(calculateBCHCode).
      [Test]
      public void testMakeTypeInfoInfoBits()
      {
         // From Appendix C in JISX0510:2004 (p 65)
         BitArray bits = new BitArray();
         MatrixUtil.makeTypeInfoBits(ErrorCorrectionLevel.M, 5, bits);
         Assert.AreEqual(" X......X X..XXX.", bits.ToString());
      }
   }
}