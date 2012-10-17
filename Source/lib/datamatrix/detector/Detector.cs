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

using ZXing.Common;
using ZXing.Common.Detector;

namespace ZXing.Datamatrix.Internal
{
   /// <summary>
   /// <p>Encapsulates logic that can detect a Data Matrix Code in an image, even if the Data Matrix Code
   /// is rotated or skewed, or partially obscured.</p>
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class Detector
   {
      private readonly BitMatrix image;
      private readonly WhiteRectangleDetector rectangleDetector;

      /// <summary>
      /// Initializes a new instance of the <see cref="Detector"/> class.
      /// </summary>
      /// <param name="image">The image.</param>
      public Detector(BitMatrix image)
      {
         this.image = image;
         rectangleDetector = WhiteRectangleDetector.Create(image);
      }

      /// <summary>
      /// <p>Detects a Data Matrix Code in an image.</p>
      /// </summary>
      /// <returns><see cref="DetectorResult" />encapsulating results of detecting a Data Matrix Code or null</returns>
      public DetectorResult detect()
      {
         if (rectangleDetector == null)
            // can be null, if the image is to small
            return null;
         ResultPoint[] cornerPoints = rectangleDetector.detect();
         if (cornerPoints == null)
            return null;
         var pointA = cornerPoints[0];
         var pointB = cornerPoints[1];
         var pointC = cornerPoints[2];
         var pointD = cornerPoints[3];

         // Point A and D are across the diagonal from one another,
         // as are B and C. Figure out which are the solid black lines
         // by counting transitions
         var transitions = new List<ResultPointsAndTransitions>(4);
         transitions.Add(transitionsBetween(pointA, pointB));
         transitions.Add(transitionsBetween(pointA, pointC));
         transitions.Add(transitionsBetween(pointB, pointD));
         transitions.Add(transitionsBetween(pointC, pointD));
         transitions.Sort(new ResultPointsAndTransitionsComparator());

         // Sort by number of transitions. First two will be the two solid sides; last two
         // will be the two alternating black/white sides
         var lSideOne = transitions[0];
         var lSideTwo = transitions[1];

         // Figure out which point is their intersection by tallying up the number of times we see the
         // endpoints in the four endpoints. One will show up twice.
         var pointCount = new Dictionary<ResultPoint, int>();
         increment(pointCount, lSideOne.From);
         increment(pointCount, lSideOne.To);
         increment(pointCount, lSideTwo.From);
         increment(pointCount, lSideTwo.To);

         ResultPoint maybeTopLeft = null;
         ResultPoint bottomLeft = null;
         ResultPoint maybeBottomRight = null;
         foreach (var entry in pointCount)
         {
            ResultPoint point = entry.Key;
            int value = entry.Value;
            if (value == 2)
            {
               bottomLeft = point; // this is definitely the bottom left, then -- end of two L sides
            }
            else
            {
               // Otherwise it's either top left or bottom right -- just assign the two arbitrarily now
               if (maybeTopLeft == null)
               {
                  maybeTopLeft = point;
               }
               else
               {
                  maybeBottomRight = point;
               }
            }
         }

         if (maybeTopLeft == null || bottomLeft == null || maybeBottomRight == null)
         {
            return null;
         }

         // Bottom left is correct but top left and bottom right might be switched
         ResultPoint[] corners = { maybeTopLeft, bottomLeft, maybeBottomRight };
         // Use the dot product trick to sort them out
         ResultPoint.orderBestPatterns(corners);

         // Now we know which is which:
         ResultPoint bottomRight = corners[0];
         bottomLeft = corners[1];
         ResultPoint topLeft = corners[2];

         // Which point didn't we find in relation to the "L" sides? that's the top right corner
         ResultPoint topRight;
         if (!pointCount.ContainsKey(pointA))
         {
            topRight = pointA;
         }
         else if (!pointCount.ContainsKey(pointB))
         {
            topRight = pointB;
         }
         else if (!pointCount.ContainsKey(pointC))
         {
            topRight = pointC;
         }
         else
         {
            topRight = pointD;
         }

         // Next determine the dimension by tracing along the top or right side and counting black/white
         // transitions. Since we start inside a black module, we should see a number of transitions
         // equal to 1 less than the code dimension. Well, actually 2 less, because we are going to
         // end on a black module:

         // The top right point is actually the corner of a module, which is one of the two black modules
         // adjacent to the white module at the top right. Tracing to that corner from either the top left
         // or bottom right should work here.

         int dimensionTop = transitionsBetween(topLeft, topRight).Transitions;
         int dimensionRight = transitionsBetween(bottomRight, topRight).Transitions;

         if ((dimensionTop & 0x01) == 1)
         {
            // it can't be odd, so, round... up?
            dimensionTop++;
         }
         dimensionTop += 2;

         if ((dimensionRight & 0x01) == 1)
         {
            // it can't be odd, so, round... up?
            dimensionRight++;
         }
         dimensionRight += 2;

         BitMatrix bits;
         ResultPoint correctedTopRight;

         // Rectanguar symbols are 6x16, 6x28, 10x24, 10x32, 14x32, or 14x44. If one dimension is more
         // than twice the other, it's certainly rectangular, but to cut a bit more slack we accept it as
         // rectangular if the bigger side is at least 7/4 times the other:
         if (4 * dimensionTop >= 7 * dimensionRight || 4 * dimensionRight >= 7 * dimensionTop)
         {
            // The matrix is rectangular

            correctedTopRight =
                correctTopRightRectangular(bottomLeft, bottomRight, topLeft, topRight, dimensionTop, dimensionRight);
            if (correctedTopRight == null)
            {
               correctedTopRight = topRight;
            }

            dimensionTop = transitionsBetween(topLeft, correctedTopRight).Transitions;
            dimensionRight = transitionsBetween(bottomRight, correctedTopRight).Transitions;

            if ((dimensionTop & 0x01) == 1)
            {
               // it can't be odd, so, round... up?
               dimensionTop++;
            }

            if ((dimensionRight & 0x01) == 1)
            {
               // it can't be odd, so, round... up?
               dimensionRight++;
            }

            bits = sampleGrid(image, topLeft, bottomLeft, bottomRight, correctedTopRight, dimensionTop, dimensionRight);
         }
         else
         {
            // The matrix is square

            int dimension = Math.Min(dimensionRight, dimensionTop);
            // correct top right point to match the white module
            correctedTopRight = correctTopRight(bottomLeft, bottomRight, topLeft, topRight, dimension);
            if (correctedTopRight == null)
            {
               correctedTopRight = topRight;
            }

            // Redetermine the dimension using the corrected top right point
            int dimensionCorrected = Math.Max(transitionsBetween(topLeft, correctedTopRight).Transitions,
                                      transitionsBetween(bottomRight, correctedTopRight).Transitions);
            dimensionCorrected++;
            if ((dimensionCorrected & 0x01) == 1)
            {
               dimensionCorrected++;
            }

            bits = sampleGrid(image,
                              topLeft,
                              bottomLeft,
                              bottomRight,
                              correctedTopRight,
                              dimensionCorrected,
                              dimensionCorrected);
         }
         if (bits == null)
            return null;

         return new DetectorResult(bits, new ResultPoint[] { topLeft, bottomLeft, bottomRight, correctedTopRight });
      }

      /// <summary>
      /// Calculates the position of the white top right module using the output of the rectangle detector
      /// for a rectangular matrix
      /// </summary>
      private ResultPoint correctTopRightRectangular(ResultPoint bottomLeft,
                                                     ResultPoint bottomRight,
                                                     ResultPoint topLeft,
                                                     ResultPoint topRight,
                                                     int dimensionTop,
                                                     int dimensionRight)
      {

         float corr = distance(bottomLeft, bottomRight) / (float)dimensionTop;
         int norm = distance(topLeft, topRight);
         if (norm == 0)
            return null;
         float cos = (topRight.X - topLeft.X) / norm;
         float sin = (topRight.Y - topLeft.Y) / norm;

         ResultPoint c1 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

         corr = distance(bottomLeft, topLeft) / (float)dimensionRight;
         norm = distance(bottomRight, topRight);
         if (norm == 0)
            return null;
         cos = (topRight.X - bottomRight.X) / norm;
         sin = (topRight.Y - bottomRight.Y) / norm;

         ResultPoint c2 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

         if (!isValid(c1))
         {
            if (isValid(c2))
            {
               return c2;
            }
            return null;
         }
         if (!isValid(c2))
         {
            return c1;
         }

         int l1 = Math.Abs(dimensionTop - transitionsBetween(topLeft, c1).Transitions) +
                    Math.Abs(dimensionRight - transitionsBetween(bottomRight, c1).Transitions);
         int l2 = Math.Abs(dimensionTop - transitionsBetween(topLeft, c2).Transitions) +
           Math.Abs(dimensionRight - transitionsBetween(bottomRight, c2).Transitions);

         if (l1 <= l2)
         {
            return c1;
         }

         return c2;
      }

      /// <summary>
      /// Calculates the position of the white top right module using the output of the rectangle detector
      /// for a square matrix
      /// </summary>
      private ResultPoint correctTopRight(ResultPoint bottomLeft,
                                          ResultPoint bottomRight,
                                          ResultPoint topLeft,
                                          ResultPoint topRight,
                                          int dimension)
      {

         float corr = distance(bottomLeft, bottomRight) / (float)dimension;
         int norm = distance(topLeft, topRight);
         if (norm == 0)
            return null;
         float cos = (topRight.X - topLeft.X) / norm;
         float sin = (topRight.Y - topLeft.Y) / norm;

         ResultPoint c1 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

         corr = distance(bottomLeft, topLeft) / (float)dimension;
         norm = distance(bottomRight, topRight);
         if (norm == 0)
            return null;
         cos = (topRight.X - bottomRight.X) / norm;
         sin = (topRight.Y - bottomRight.Y) / norm;

         ResultPoint c2 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

         if (!isValid(c1))
         {
            if (isValid(c2))
            {
               return c2;
            }
            return null;
         }
         if (!isValid(c2))
         {
            return c1;
         }

         int l1 = Math.Abs(transitionsBetween(topLeft, c1).Transitions -
                           transitionsBetween(bottomRight, c1).Transitions);
         int l2 = Math.Abs(transitionsBetween(topLeft, c2).Transitions -
                         transitionsBetween(bottomRight, c2).Transitions);

         return l1 <= l2 ? c1 : c2;
      }

      private bool isValid(ResultPoint p)
      {
         return p.X >= 0 && p.X < image.Width && p.Y > 0 && p.Y < image.Height;
      }

      // L2 distance
      private static int distance(ResultPoint a, ResultPoint b)
      {
         return MathUtils.round(ResultPoint.distance(a, b));
      }

      /// <summary>
      /// Increments the Integer associated with a key by one.
      /// </summary>
      private static void increment(IDictionary<ResultPoint, int> table, ResultPoint key)
      {
         if (table.ContainsKey(key))
         {
            int value = table[key];
            table[key] = value + 1;
         }
         else
         {
            table[key] = 1;
         }
      }

      private static BitMatrix sampleGrid(BitMatrix image,
                                          ResultPoint topLeft,
                                          ResultPoint bottomLeft,
                                          ResultPoint bottomRight,
                                          ResultPoint topRight,
                                          int dimensionX,
                                          int dimensionY)
      {

         GridSampler sampler = GridSampler.Instance;

         return sampler.sampleGrid(image,
                                   dimensionX,
                                   dimensionY,
                                   0.5f,
                                   0.5f,
                                   dimensionX - 0.5f,
                                   0.5f,
                                   dimensionX - 0.5f,
                                   dimensionY - 0.5f,
                                   0.5f,
                                   dimensionY - 0.5f,
                                   topLeft.X,
                                   topLeft.Y,
                                   topRight.X,
                                   topRight.Y,
                                   bottomRight.X,
                                   bottomRight.Y,
                                   bottomLeft.X,
                                   bottomLeft.Y);
      }

      /// <summary>
      /// Counts the number of black/white transitions between two points, using something like Bresenham's algorithm.
      /// </summary>
      private ResultPointsAndTransitions transitionsBetween(ResultPoint from, ResultPoint to)
      {
         // See QR Code Detector, sizeOfBlackWhiteBlackRun()
         int fromX = (int)from.X;
         int fromY = (int)from.Y;
         int toX = (int)to.X;
         int toY = (int)to.Y;
         bool steep = Math.Abs(toY - fromY) > Math.Abs(toX - fromX);
         if (steep)
         {
            int temp = fromX;
            fromX = fromY;
            fromY = temp;
            temp = toX;
            toX = toY;
            toY = temp;
         }

         int dx = Math.Abs(toX - fromX);
         int dy = Math.Abs(toY - fromY);
         int error = -dx >> 1;
         int ystep = fromY < toY ? 1 : -1;
         int xstep = fromX < toX ? 1 : -1;
         int transitions = 0;
         bool inBlack = image[steep ? fromY : fromX, steep ? fromX : fromY];
         for (int x = fromX, y = fromY; x != toX; x += xstep)
         {
            bool isBlack = image[steep ? y : x, steep ? x : y];
            if (isBlack != inBlack)
            {
               transitions++;
               inBlack = isBlack;
            }
            error += dy;
            if (error > 0)
            {
               if (y == toY)
               {
                  break;
               }
               y += ystep;
               error -= dx;
            }
         }
         return new ResultPointsAndTransitions(from, to, transitions);
      }

      /// <summary>
      /// Simply encapsulates two points and a number of transitions between them.
      /// </summary>
      private sealed class ResultPointsAndTransitions
      {
         public ResultPoint From { get; private set; }
         public ResultPoint To { get; private set; }
         public int Transitions { get; private set; }

         public ResultPointsAndTransitions(ResultPoint from, ResultPoint to, int transitions)
         {
            From = from;
            To = to;
            Transitions = transitions;
         }

         override public String ToString()
         {
            return From + "/" + To + '/' + Transitions;
         }
      }

      /// <summary>
      /// Orders ResultPointsAndTransitions by number of transitions, ascending.
      /// </summary>
      private sealed class ResultPointsAndTransitionsComparator : IComparer<ResultPointsAndTransitions>
      {
         public int Compare(ResultPointsAndTransitions o1, ResultPointsAndTransitions o2)
         {
            return o1.Transitions - o2.Transitions;
         }
      }
   }
}