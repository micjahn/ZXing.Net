/*
 * Copyright 2013 ZXing authors
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

using ZXing.Common;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417.Test
{
   [TestFixture]
   public sealed class DetectorTest
   {
      private static readonly int[] BIT_SET_INDEX = {1, 2, 3, 5};
      private static readonly int[] BIT_MATRIX_POINTS = {1, 2, 2, 0, 3, 1};

      [Test]
      public void testMirror()
      {
         testMirror(7);
         testMirror(8);
      }

      private static void testMirror(int size)
      {
         BitArray result = new BitArray(size);
         Detector.mirror(getInput(size), result);
         Assert.AreEqual(getExpected(size).ToString(), result.ToString());
      }

      [Test]
      public void testRotate180()
      {
         testRotate180(7, 4);
         testRotate180(7, 5);
         testRotate180(8, 4);
         testRotate180(8, 5);
      }

      private static void testRotate180(int width, int height)
      {
         BitMatrix input = getInput(width, height);
         Detector.rotate180(input);
         BitMatrix expected = getExpected(width, height);

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

      private static BitArray getExpected(int size)
      {
         var expected = new BitArray(size);
         foreach (int index in BIT_SET_INDEX)
         {
            expected[size - 1 - index] = true;
         }
         return expected;
      }

      private static BitArray getInput(int size)
      {
         var input = new BitArray(size);
         foreach (int index in BIT_SET_INDEX)
         {
            input[index] = true;
         }
         return input;
      }
   }
}
