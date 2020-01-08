/*
 * Copyright 2007 ZXing authors
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
using System.Text;
using NUnit.Framework;

namespace ZXing.Common.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   [TestFixture]
   public sealed class BitMatrixTestCase
   {
      private static readonly int[] BIT_MATRIX_POINTS = {1, 2, 2, 0, 3, 1};

      [Test]
      public void testGetSet()
      {
         var matrix = new BitMatrix(33);
         Assert.AreEqual(33, matrix.Height);
         for (var y = 0; y < 33; y++)
         {
            for (var x = 0; x < 33; x++)
            {
               if (y*x%3 == 0)
               {
                  matrix[x, y] = true;
               }
            }
         }
         for (var y = 0; y < 33; y++)
         {
            for (var x = 0; x < 33; x++)
            {
               Assert.AreEqual(y*x%3 == 0, matrix[x, y]);
            }
         }
      }

      [Test]
      public void testSetRegion()
      {
         var matrix = new BitMatrix(5);
         matrix.setRegion(1, 1, 3, 3);
         for (var y = 0; y < 5; y++)
         {
            for (var x = 0; x < 5; x++)
            {
               Assert.AreEqual(y >= 1 && y <= 3 && x >= 1 && x <= 3, matrix[x, y]);
            }
         }
      }

      [Test]
      public void testEnclosing()
      {
         BitMatrix matrix = new BitMatrix(5);
         Assert.IsNull(matrix.getEnclosingRectangle());
         matrix.setRegion(1, 1, 1, 1);
         Assert.AreEqual(new int[] {1, 1, 1, 1}, matrix.getEnclosingRectangle());
         matrix.setRegion(1, 1, 3, 2);
         Assert.AreEqual(new int[] { 1, 1, 3, 2 }, matrix.getEnclosingRectangle());
         matrix.setRegion(0, 0, 5, 5);
         Assert.AreEqual(new int[] { 0, 0, 5, 5 }, matrix.getEnclosingRectangle());
      }

      [Test]
      public void testOnBit()
      {
         BitMatrix matrix = new BitMatrix(5);
         Assert.IsNull(matrix.getTopLeftOnBit());
         Assert.IsNull(matrix.getBottomRightOnBit());
         matrix.setRegion(1, 1, 1, 1);
         Assert.AreEqual(new int[] { 1, 1 }, matrix.getTopLeftOnBit());
         Assert.AreEqual(new int[] { 1, 1 }, matrix.getBottomRightOnBit());
         matrix.setRegion(1, 1, 3, 2);
         Assert.AreEqual(new int[] { 1, 1 }, matrix.getTopLeftOnBit());
         Assert.AreEqual(new int[] { 3, 2 }, matrix.getBottomRightOnBit());
         matrix.setRegion(0, 0, 5, 5);
         Assert.AreEqual(new int[] { 0, 0 }, matrix.getTopLeftOnBit());
         Assert.AreEqual(new int[] { 4, 4 }, matrix.getBottomRightOnBit());
      }

      [Test]
      public void testRectangularMatrix()
      {
         var matrix = new BitMatrix(75, 20);
         Assert.AreEqual(75, matrix.Width);
         Assert.AreEqual(20, matrix.Height);
         matrix[10, 0] = true;
         matrix[11, 1] = true;
         matrix[50, 2] = true;
         matrix[51, 3] = true;
         matrix.flip(74, 4);
         matrix.flip(0, 5);

         // Should all be on
         Assert.IsTrue(matrix[10, 0]);
         Assert.IsTrue(matrix[11, 1]);
         Assert.IsTrue(matrix[50, 2]);
         Assert.IsTrue(matrix[51, 3]);
         Assert.IsTrue(matrix[74, 4]);
         Assert.IsTrue(matrix[0, 5]);

         // Flip a couple back off
         matrix.flip(50, 2);
         matrix.flip(51, 3);
         Assert.IsFalse(matrix[50, 2]);
         Assert.IsFalse(matrix[51, 3]);
      }

      [Test]
      public void testRectangularSetRegion()
      {
         var matrix = new BitMatrix(320, 240);
         Assert.AreEqual(320, matrix.Width);
         Assert.AreEqual(240, matrix.Height);
         matrix.setRegion(105, 22, 80, 12);

         // Only bits in the region should be on
         for (var y = 0; y < 240; y++)
         {
            for (var x = 0; x < 320; x++)
            {
               Assert.AreEqual(y >= 22 && y < 34 && x >= 105 && x < 185, matrix[x, y]);
            }
         }
      }

      [Test]
      public void testGetRow()
      {
         var matrix = new BitMatrix(102, 5);
         for (int x = 0; x < 102; x++)
         {
            if ((x & 0x03) == 0)
            {
               matrix[x, 2] = true;
            }
         }

         // Should allocate
         var array = matrix.getRow(2, null);
         Assert.AreEqual(102, array.Size);

         // Should reallocate
         var array2 = new BitArray(60);
         array2 = matrix.getRow(2, array2);
         Assert.AreEqual(102, array2.Size);

         // Should use provided object, with original BitArray size
         var array3 = new BitArray(200);
         array3 = matrix.getRow(2, array3);
         Assert.AreEqual(200, array3.Size);

         for (var x = 0; x < 102; x++)
         {
            var on = (x & 0x03) == 0;
            Assert.AreEqual(on, array[x]);
            Assert.AreEqual(on, array2[x]);
            Assert.AreEqual(on, array3[x]);
         }
      }

      [Test]
      public void testRotate180Simple()
      {
         var matrix = new BitMatrix(3, 3);
         matrix[0, 0] = true;
         matrix[0, 1] = true;
         matrix[1, 2] = true;
         matrix[2, 1] = true;

         matrix.rotate180();

         Assert.IsTrue(matrix[2, 2]);
         Assert.IsTrue(matrix[2, 1]);
         Assert.IsTrue(matrix[1, 0]);
         Assert.IsTrue(matrix[0, 1]);
      }

      [Test]
      public void testRotate180()
      {
         testRotate180(7, 4);
         testRotate180(7, 5);
         testRotate180(8, 4);
         testRotate180(8, 5);
      }


      [Test]
      public void testParse()
      {
         var emptyMatrix = new BitMatrix(3, 3);
         var fullMatrix = new BitMatrix(3, 3);
         fullMatrix.setRegion(0, 0, 3, 3);
         var centerMatrix = new BitMatrix(3, 3);
         centerMatrix.setRegion(1, 1, 1, 1);
         var emptyMatrix24 = new BitMatrix(2, 4);

         Assert.AreEqual(emptyMatrix, BitMatrix.parse("   \n   \n   \n", "x", " "));
         Assert.AreEqual(emptyMatrix, BitMatrix.parse("   \n   \r\r\n   \n\r", "x", " "));
         Assert.AreEqual(emptyMatrix, BitMatrix.parse("   \n   \n   ", "x", " "));

         Assert.AreEqual(fullMatrix, BitMatrix.parse("xxx\nxxx\nxxx\n", "x", " "));

         Assert.AreEqual(centerMatrix, BitMatrix.parse("   \n x \n   \n", "x", " "));
         Assert.AreEqual(centerMatrix, BitMatrix.parse("      \n  x   \n      \n", "x ", "  "));
         try
         {
            Assert.AreEqual(centerMatrix, BitMatrix.parse("   \n xy\n   \n", "x", " "));
            Assert.Fail();
         }
         catch (ArgumentException)
         {
         }

         Assert.AreEqual(emptyMatrix24, BitMatrix.parse("  \n  \n  \n  \n", "x", " "));

         Assert.AreEqual(centerMatrix, BitMatrix.parse(centerMatrix.ToString("x", ".", "\n"), "x", "."));
      }

      [Test]
      public void testUnset()
      {
         var emptyMatrix = new BitMatrix(3, 3);
         var matrix = (BitMatrix) emptyMatrix.Clone();
         matrix[1, 1] = true;
         Assert.AreNotEqual(emptyMatrix, matrix);
         matrix[1, 1] = false;
         Assert.AreEqual(emptyMatrix, matrix);
         matrix[1, 1] = false;
         Assert.AreEqual(emptyMatrix, matrix);
      }

      [Test]
      public void testXOR()
      {
         var emptyMatrix = new BitMatrix(3, 3);
         var fullMatrix = new BitMatrix(3, 3);
         fullMatrix.setRegion(0, 0, 3, 3);
         var centerMatrix = new BitMatrix(3, 3);
         centerMatrix.setRegion(1, 1, 1, 1);
         var invertedCenterMatrix = (BitMatrix) fullMatrix.Clone();
         invertedCenterMatrix[1, 1] = false;
         var badMatrix = new BitMatrix(4, 4);

         testXOR(emptyMatrix, emptyMatrix, emptyMatrix);
         testXOR(emptyMatrix, centerMatrix, centerMatrix);
         testXOR(emptyMatrix, fullMatrix, fullMatrix);

         testXOR(centerMatrix, emptyMatrix, centerMatrix);
         testXOR(centerMatrix, centerMatrix, emptyMatrix);
         testXOR(centerMatrix, fullMatrix, invertedCenterMatrix);

         testXOR(invertedCenterMatrix, emptyMatrix, invertedCenterMatrix);
         testXOR(invertedCenterMatrix, centerMatrix, fullMatrix);
         testXOR(invertedCenterMatrix, fullMatrix, centerMatrix);

         testXOR(fullMatrix, emptyMatrix, fullMatrix);
         testXOR(fullMatrix, centerMatrix, invertedCenterMatrix);
         testXOR(fullMatrix, fullMatrix, emptyMatrix);

         try
         {
            ((BitMatrix) emptyMatrix.Clone()).xor(badMatrix);
            Assert.Fail();
         }
         catch (ArgumentException)
         {
         }

         try
         {
            ((BitMatrix) badMatrix.Clone()).xor(emptyMatrix);
            Assert.Fail();
         }
         catch (ArgumentException)
         {
         }
      }

      public static String matrixToString(BitMatrix result)
      {
         Assert.AreEqual(1, result.Height);
         StringBuilder builder = new StringBuilder(result.Width);
         for (int i = 0; i < result.Width; i++)
         {
            builder.Append(result[i, 0] ? '1' : '0');
         }
         return builder.ToString();
      }

      private static void testXOR(BitMatrix dataMatrix, BitMatrix flipMatrix, BitMatrix expectedMatrix)
      {
         var matrix = (BitMatrix) dataMatrix.Clone();
         matrix.xor(flipMatrix);
         Assert.AreEqual(expectedMatrix, matrix);
      }

      private static void testRotate180(int width, int height)
      {
         var input = getInput(width, height);
         input.rotate180();
         var expected = getExpected(width, height);

         for (int y = 0; y < height; y++)
         {
            for (int x = 0; x < width; x++)
            {
               Assert.AreEqual(expected[x, y], input[x, y], "(" + x + ',' + y + ')');
            }
         }
      }

      private static BitMatrix getExpected(int width, int height)
      {
         var result = new BitMatrix(width, height);
         for (int i = 0; i < BIT_MATRIX_POINTS.Length; i += 2)
         {
            result[width - 1 - BIT_MATRIX_POINTS[i], height - 1 - BIT_MATRIX_POINTS[i + 1]] = true;
         }
         return result;
      }

      private static BitMatrix getInput(int width, int height)
      {
         var result = new BitMatrix(width, height);
         for (int i = 0; i < BIT_MATRIX_POINTS.Length; i += 2)
         {
            result[BIT_MATRIX_POINTS[i], BIT_MATRIX_POINTS[i + 1]] = true;
         }
         return result;
      }
   }
}