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
         var aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 25, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
         var random = new Random(aztec.Matrix.GetHashCode()); // pseudo-random, but deterministic
         var layers = aztec.Layers;
         var compact = aztec.isCompact;
         var orientationPoints = getOrientationPoints(aztec);
         foreach (bool isMirror in new[] {false, true})
         {
            foreach (BitMatrix matrix in getRotations(aztec.Matrix))
            {
               // Systematically try every possible 1- and 2-bit error.
               for (int error1 = 0; error1 < orientationPoints.Count; error1++)
               {
                  for (int error2 = error1; error2 < orientationPoints.Count; error2++)
                  {
                     BitMatrix copy = isMirror ? transpose(matrix) : clone(matrix);
                     copy.flip(orientationPoints[error1].X, orientationPoints[error1].Y);
                     if (error2 > error1)
                     {
                        // if error2 == error1, we only test a single error
                        copy.flip(orientationPoints[error2].X, orientationPoints[error2].Y);
                     }
                     // The detector doesn't seem to work when matrix bits are only 1x1.  So magnify.
                     AztecDetectorResult r = new Detector(makeLarger(copy, 3)).detect(isMirror);
                     Assert.IsNotNull(r);
                     Assert.AreEqual(r.NbLayers, layers);
                     Assert.AreEqual(r.Compact, compact);
                     DecoderResult res = new Internal.Decoder().decode(r);
                     Assert.AreEqual(data, res.Text);
                  }
               }
               // Try a few random three-bit errors;
               for (int i = 0; i < 5; i++)
               {
                  BitMatrix copy = clone(matrix);
                  ISet<int> errors = new SortedSet<int>();
                  while (errors.Count < 3)
                  {
                     // Quick and dirty way of getting three distinct integers between 1 and n.
                     errors.Add(random.Next(orientationPoints.Count));
                  }
                  foreach (int error in errors)
                  {
                     copy.flip(orientationPoints[error].X, orientationPoints[error].Y);
                  }
                  var result = new Detector(makeLarger(copy, 3)).detect(false);
                  if (result != null)
                  {
                     Assert.Fail("Should not reach here");
                  }
               }
            }
         }
      }

      // Zooms a bit matrix so that each bit is factor x factor
      private static BitMatrix makeLarger(BitMatrix input, int factor)
      {
         var width = input.Width;
         var output = new BitMatrix(width*factor);
         for (var inputY = 0; inputY < width; inputY++)
         {
            for (var inputX = 0; inputX < width; inputX++)
            {
               if (input[inputX, inputY])
               {
                  output.setRegion(inputX*factor, inputY*factor, factor, factor);
               }
            }
         }
         return output;
      }

      // Returns a list of the four rotations of the BitMatrix.
      private static List<BitMatrix> getRotations(BitMatrix matrix0)
      {
         BitMatrix matrix90 = rotateRight(matrix0);
         BitMatrix matrix180 = rotateRight(matrix90);
         BitMatrix matrix270 = rotateRight(matrix180);
         return new List<BitMatrix> {matrix0, matrix90, matrix180, matrix270};
      }

      // Rotates a square BitMatrix to the right by 90 degrees
      private static BitMatrix rotateRight(BitMatrix input)
      {
         var width = input.Width;
         var result = new BitMatrix(width);
         for (var x = 0; x < width; x++)
         {
            for (var y = 0; y < width; y++)
            {
               if (input[x, y])
               {
                  result[y, width - x - 1] = true;
               }
            }
         }
         return result;
      }

      // Returns the transpose of a bit matrix, which is equivalent to rotating the
      // matrix to the right, and then flipping it left-to-right
      private static BitMatrix transpose(BitMatrix input)
      {
         var width = input.Width;
         var result = new BitMatrix(width);
         for (var x = 0; x < width; x++)
         {
            for (var y = 0; y < width; y++)
            {
               if (input[x, y])
               {
                  result[y, x] = true;
               }
            }
         }
         return result;
      }

      private static BitMatrix clone(BitMatrix input)
      {
         var width = input.Width;
         var result = new BitMatrix(width);
         for (var x = 0; x < width; x++)
         {
            for (var y = 0; y < width; y++)
            {
               if (input[x, y])
               {
                  result[x, y] = true;
               }
            }
         }
         return result;
      }

      private static List<Detector.Point> getOrientationPoints(AztecCode code)
      {
         var center = code.Matrix.Width/2;
         var offset = code.isCompact ? 5 : 7;
         var result = new List<Detector.Point>();
         for (var xSign = -1; xSign <= 1; xSign += 2)
         {
            for (var ySign = -1; ySign <= 1; ySign += 2)
            {
               result.Add(new Detector.Point(center + xSign*offset, center + ySign*offset));
               result.Add(new Detector.Point(center + xSign*(offset - 1), center + ySign*offset));
               result.Add(new Detector.Point(center + xSign*offset, center + ySign*(offset - 1)));
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
