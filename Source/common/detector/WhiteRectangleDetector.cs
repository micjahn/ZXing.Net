/*
 * Copyright 2010 ZXing authors
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

namespace com.google.zxing.common.detector
{
   /**
    * <p>
    * Detects a candidate barcode-like rectangular region within an image. It
    * starts around the center of the image, increases the size of the candidate
    * region until it finds a white rectangular region. By keeping track of the
    * last black points it encountered, it determines the corners of the barcode.
    * </p>
    *
    * @author David Olivier
    */
   public sealed class WhiteRectangleDetector
   {

      private static int INIT_SIZE = 30;
      private static int CORR = 1;

      private BitMatrix image;
      private int height;
      private int width;
      private int leftInit;
      private int rightInit;
      private int downInit;
      private int upInit;

      /**
       * @throws NotFoundException if image is too small
       */
      public WhiteRectangleDetector(BitMatrix image)
      {
         this.image = image;
         height = image.Height;
         width = image.Width;
         leftInit = (width - INIT_SIZE) >> 1;
         rightInit = (width + INIT_SIZE) >> 1;
         upInit = (height - INIT_SIZE) >> 1;
         downInit = (height + INIT_SIZE) >> 1;
         if (upInit < 0 || leftInit < 0 || downInit >= height || rightInit >= width)
         {
            throw NotFoundException.Instance;
         }
      }

      /**
       * @throws NotFoundException if image is too small
       */
      public WhiteRectangleDetector(BitMatrix image, int initSize, int x, int y)
      {
         this.image = image;
         height = image.Height;
         width = image.Width;
         int halfsize = initSize >> 1;
         leftInit = x - halfsize;
         rightInit = x + halfsize;
         upInit = y - halfsize;
         downInit = y + halfsize;
         if (upInit < 0 || leftInit < 0 || downInit >= height || rightInit >= width)
         {
            throw NotFoundException.Instance;
         }
      }

      /**
       * <p>
       * Detects a candidate barcode-like rectangular region within an image. It
       * starts around the center of the image, increases the size of the candidate
       * region until it finds a white rectangular region.
       * </p>
       *
       * @return {@link ResultPoint}[] describing the corners of the rectangular
       *         region. The first and last points are opposed on the diagonal, as
       *         are the second and third. The first point will be the topmost
       *         point and the last, the bottommost. The second point will be
       *         leftmost and the third, the rightmost
       * @throws NotFoundException if no Data Matrix Code can be found
       */
      public ResultPoint[] detect()
      {

         int left = leftInit;
         int right = rightInit;
         int up = upInit;
         int down = downInit;
         bool sizeExceeded = false;
         bool aBlackPointFoundOnBorder = true;
         bool atLeastOneBlackPointFoundOnBorder = false;

         while (aBlackPointFoundOnBorder)
         {

            aBlackPointFoundOnBorder = false;

            // .....
            // .   |
            // .....
            bool rightBorderNotWhite = true;
            while (rightBorderNotWhite && right < width)
            {
               rightBorderNotWhite = containsBlackPoint(up, down, right, false);
               if (rightBorderNotWhite)
               {
                  right++;
                  aBlackPointFoundOnBorder = true;
               }
            }

            if (right >= width)
            {
               sizeExceeded = true;
               break;
            }

            // .....
            // .   .
            // .___.
            bool bottomBorderNotWhite = true;
            while (bottomBorderNotWhite && down < height)
            {
               bottomBorderNotWhite = containsBlackPoint(left, right, down, true);
               if (bottomBorderNotWhite)
               {
                  down++;
                  aBlackPointFoundOnBorder = true;
               }
            }

            if (down >= height)
            {
               sizeExceeded = true;
               break;
            }

            // .....
            // |   .
            // .....
            bool leftBorderNotWhite = true;
            while (leftBorderNotWhite && left >= 0)
            {
               leftBorderNotWhite = containsBlackPoint(up, down, left, false);
               if (leftBorderNotWhite)
               {
                  left--;
                  aBlackPointFoundOnBorder = true;
               }
            }

            if (left < 0)
            {
               sizeExceeded = true;
               break;
            }

            // .___.
            // .   .
            // .....
            bool topBorderNotWhite = true;
            while (topBorderNotWhite && up >= 0)
            {
               topBorderNotWhite = containsBlackPoint(left, right, up, true);
               if (topBorderNotWhite)
               {
                  up--;
                  aBlackPointFoundOnBorder = true;
               }
            }

            if (up < 0)
            {
               sizeExceeded = true;
               break;
            }

            if (aBlackPointFoundOnBorder)
            {
               atLeastOneBlackPointFoundOnBorder = true;
            }

         }

         if (!sizeExceeded && atLeastOneBlackPointFoundOnBorder)
         {

            int maxSize = right - left;

            ResultPoint z = null;
            for (int i = 1; i < maxSize; i++)
            {
               z = getBlackPointOnSegment(left, down - i, left + i, down);
               if (z != null)
               {
                  break;
               }
            }

            if (z == null)
            {
               throw NotFoundException.Instance;
            }

            ResultPoint t = null;
            //go down right
            for (int i = 1; i < maxSize; i++)
            {
               t = getBlackPointOnSegment(left, up + i, left + i, up);
               if (t != null)
               {
                  break;
               }
            }

            if (t == null)
            {
               throw NotFoundException.Instance;
            }

            ResultPoint x = null;
            //go down left
            for (int i = 1; i < maxSize; i++)
            {
               x = getBlackPointOnSegment(right, up + i, right - i, up);
               if (x != null)
               {
                  break;
               }
            }

            if (x == null)
            {
               throw NotFoundException.Instance;
            }

            ResultPoint y = null;
            //go up left
            for (int i = 1; i < maxSize; i++)
            {
               y = getBlackPointOnSegment(right, down - i, right - i, down);
               if (y != null)
               {
                  break;
               }
            }

            if (y == null)
            {
               throw NotFoundException.Instance;
            }

            return centerEdges(y, z, x, t);

         }
         else
         {
            throw NotFoundException.Instance;
         }
      }

      /**
       * Ends up being a bit faster than Math.round(). This merely rounds its
       * argument to the nearest int, where x.5 rounds up.
       */
      private static int round(float d)
      {
         return (int)(d + 0.5f);
      }

      private ResultPoint getBlackPointOnSegment(float aX, float aY, float bX, float bY)
      {
         int dist = distanceL2(aX, aY, bX, bY);
         float xStep = (bX - aX) / dist;
         float yStep = (bY - aY) / dist;

         for (int i = 0; i < dist; i++)
         {
            int x = round(aX + i * xStep);
            int y = round(aY + i * yStep);
            if (image.get_Renamed(x, y))
            {
               return new ResultPoint(x, y);
            }
         }
         return null;
      }

      private static int distanceL2(float aX, float aY, float bX, float bY)
      {
         float xDiff = aX - bX;
         float yDiff = aY - bY;
         return round((float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff));
      }

      /**
       * recenters the points of a constant distance towards the center
       *
       * @param y bottom most point
       * @param z left most point
       * @param x right most point
       * @param t top most point
       * @return {@link ResultPoint}[] describing the corners of the rectangular
       *         region. The first and last points are opposed on the diagonal, as
       *         are the second and third. The first point will be the topmost
       *         point and the last, the bottommost. The second point will be
       *         leftmost and the third, the rightmost
       */
      private ResultPoint[] centerEdges(ResultPoint y, ResultPoint z,
                                        ResultPoint x, ResultPoint t)
      {

         //
         //       t            t
         //  z                      x
         //        x    OR    z
         //   y                    y
         //

         float yi = y.X;
         float yj = y.Y;
         float zi = z.X;
         float zj = z.Y;
         float xi = x.X;
         float xj = x.Y;
         float ti = t.X;
         float tj = t.Y;

         if (yi < width / 2)
         {
            return new[]{
          new ResultPoint(ti - CORR, tj + CORR),
          new ResultPoint(zi + CORR, zj + CORR),
          new ResultPoint(xi - CORR, xj - CORR),
          new ResultPoint(yi + CORR, yj - CORR)};
         }
         else
         {
            return new[]{
          new ResultPoint(ti + CORR, tj + CORR),
          new ResultPoint(zi + CORR, zj - CORR),
          new ResultPoint(xi - CORR, xj + CORR),
          new ResultPoint(yi - CORR, yj - CORR)};
         }
      }

      /**
       * Determines whether a segment contains a black point
       *
       * @param a          min value of the scanned coordinate
       * @param b          max value of the scanned coordinate
       * @param fixed      value of fixed coordinate
       * @param horizontal set to true if scan must be horizontal, false if vertical
       * @return true if a black point has been found, else false.
       */
      private bool containsBlackPoint(int a, int b, int @fixed, bool horizontal)
      {

         if (horizontal)
         {
            for (int x = a; x <= b; x++)
            {
               if (image.get_Renamed(x, @fixed))
               {
                  return true;
               }
            }
         }
         else
         {
            for (int y = a; y <= b; y++)
            {
               if (image.get_Renamed(@fixed, y))
               {
                  return true;
               }
            }
         }

         return false;
      }

   }
}