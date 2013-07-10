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

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec.Test
{
   /// <summary>
   /// Tests for the Detector
   /// @author Frank Yellin
   /// </summary>
   public class DetectorTest
   {
      private static readonly Encoding LATIN_1 = Encoding.GetEncoding("ISO-8859-1");

      [Test]
      public void testErrorInParameterLocatorZeroZero()
      {
         // Layers=1, CodeWords=1.  So the parameter info and its Reed-Solomon info
         // will be completely zero!
         testErrorInParameterLocator("X");
      }

      [Test]
      public void testErrorInParameterLocatorCompact()
      {
         testErrorInParameterLocator("This is an example Aztec symbol for Wikipedia.");
      }

      [Test]
      public void testErrorInParameterLocatorNotCompact()
      {
         const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYabcdefghijklmnopqrstuvwxyz";
         testErrorInParameterLocator(alphabet + alphabet + alphabet);
      }

      // Test that we can tolerate errors in the parameter locator bits
      private static void testErrorInParameterLocator(String data)
      {
         AztecCode aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 25);
         int layers = aztec.Layers;
         bool compact = aztec.isCompact;
         List<Detector.Point> orientationPoints = getOrientationPoints(aztec);
         Random random = new Random(aztec.Matrix.GetHashCode()); // random, but repeatable
         foreach (BitMatrix matrix in getRotations(aztec.Matrix))
         {
            // Each time through this loop, we reshuffle the corners, to get a different set of errors
            Shuffle(orientationPoints, random);
            for (int errors = 1; errors <= 3; errors++)
            {
               // Add another error to one of the parameter locator bits
               matrix.flip(orientationPoints[errors].X, orientationPoints[errors].Y);
               // The detector can't yet deal with bitmaps in which each square is only 1x1 pixel.
               // We zoom it larger.
               AztecDetectorResult r = new Detector(makeLarger(matrix, 3)).detect();
               if (r != null)
               { 
               if (errors < 3)
               {
                  Assert.NotNull(r);
                  Assert.AreEqual(r.NbLayers, layers);
                  Assert.AreEqual(r.Compact, compact);
               }
               else
               {
                  Assert.Fail("Should not succeed with more than two errors");
               }
               }
               else
               {
                  Assert.AreEqual(3, errors, "Should only fail with three errors");
               }
            }
         }
      }

      // Zooms a bit matrix so that each bit is factor x factor
      private static BitMatrix makeLarger(BitMatrix input, int factor)
      {
         int width = input.Width;
         BitMatrix output = new BitMatrix(width*factor);
         for (int inputY = 0; inputY < width; inputY++)
         {
            for (int inputX = 0; inputX < width; inputX++)
            {
               if (input[inputX, inputY])
               {
                  output.setRegion(inputX*factor, inputY*factor, factor, factor);
               }
            }
         }
         return output;
      }

      // Returns a list of the four rotations of the BitMatrix.  The identity rotation is
      // explicitly a copy, so that it can be modified without affecting the original matrix.
      private static List<BitMatrix> getRotations(BitMatrix input)
      {
         int width = input.Width;
         var matrix0 = new BitMatrix(width);
         var matrix90 = new BitMatrix(width);
         var matrix180 = new BitMatrix(width);
         var matrix270 = new BitMatrix(width);
         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < width; y++)
            {
               if (input[x, y])
               {
                  matrix0[x, y] = true;
                  matrix90[y, width - x - 1] = true;
                  matrix180[width - x - 1, width - y - 1] = true;
                  matrix270[width - y - 1, x] = true;
               }
            }
         }
         return new List<BitMatrix> { matrix0, matrix90, matrix180, matrix270 };
      }

      private static List<Detector.Point> getOrientationPoints(AztecCode code)
      {
         int center = code.Matrix.Width/2;
         int offset = code.isCompact ? 5 : 7;
         List<Detector.Point> result = new List<Detector.Point>();
         for (int xSign = -1; xSign <= 1; xSign += 2)
         {
            for (int ySign = -1; ySign <= 1; ySign += 2)
            {
               result.Add(new Detector.Point(center + xSign*offset, center + ySign*offset));
               result.Add(new Detector.Point(center + xSign * (offset - 1), center + ySign * offset));
               result.Add(new Detector.Point(center + xSign * offset, center + ySign * (offset - 1)));
            }
         }
         return result;
      }

      public static void Shuffle<T>(IList<T> list, Random random)
      {
         int n = list.Count;
         while (n > 1)
         {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
         }
      }
   }
}
