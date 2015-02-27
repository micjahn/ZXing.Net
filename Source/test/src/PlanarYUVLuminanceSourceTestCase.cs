/*
 * Copyright 2014 ZXing authors
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

namespace ZXing.Test
{
   public sealed class PlanarYUVLuminanceSourceTestCase
   {
      private static readonly byte[] YUV =
         {
            128, 129, 129, 130, 131, 133,
            136, 141, 149, 162, 183, 217,
            128, 127, 127, 126, 125, 123,
            120, 115, 107, 94, 73, 39,
            255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255,
         };

      private const int COLS = 6;
      private const int ROWS = 4;
      private static readonly byte[] Y = new byte[COLS*ROWS];

      static PlanarYUVLuminanceSourceTestCase()
      {
         Array.Copy(YUV, 0, Y, 0, Y.Length);
      }

      [Test]
      public void testNoCrop()
      {
         var source =
            new PlanarYUVLuminanceSource(YUV, COLS, ROWS, 0, 0, COLS, ROWS, false);
         assertEquals(Y, 0, source.Matrix, 0, Y.Length);
         for (int r = 0; r < ROWS; r++)
         {
            assertEquals(Y, r*COLS, source.getRow(r, null), 0, COLS);
         }
      }

      [Test]
      public void testCrop()
      {
         var source =
            new PlanarYUVLuminanceSource(YUV, COLS, ROWS, 1, 1, COLS - 2, ROWS - 2, false);
         for (int r = 0; r < ROWS - 2; r++)
         {
            assertEquals(Y, (r + 1)*COLS + 1, source.getRow(r, null), 0, COLS - 2);
         }
      }

      private static void assertEquals(byte[] expected, int expectedFrom,
                                       byte[] actual, int actualFrom,
                                       int length)
      {
         for (int i = 0; i < length; i++)
         {
            Assert.AreEqual(expected[expectedFrom + i], actual[actualFrom + i]);
         }
      }
   }
}