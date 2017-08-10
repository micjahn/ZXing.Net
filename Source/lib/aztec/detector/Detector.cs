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

using ZXing.Common;
using ZXing.Common.Detector;
using ZXing.Common.ReedSolomon;

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// Encapsulates logic that can detect an Aztec Code in an image, even if the Aztec Code
   /// is rotated or skewed, or partially obscured.
   /// </summary>
   /// <author>David Olivier</author>
   public sealed class Detector
   {
      private static readonly int[] EXPECTED_CORNER_BITS =
      {
         0xee0, // 07340  XXX .XX X.. ...
         0x1dc, // 00734  ... XXX .XX X..
         0x83b, // 04073  X.. ... XXX .XX
         0x707, // 03407 .XX X.. ... XXX
      };

      private readonly BitMatrix image;

      private bool compact;
      private int nbLayers;
      private int nbDataBlocks;
      private int nbCenterLayers;
      private int shift;

      /// <summary>
      /// Initializes a new instance of the <see cref="Detector"/> class.
      /// </summary>
      /// <param name="image">The image.</param>
      public Detector(BitMatrix image)
      {
         this.image = image;
      }

      /// <summary>
      /// Detects an Aztec Code in an image.
      /// </summary>
      public AztecDetectorResult detect()
      {
         return detect(false);
      }

      /// <summary>
      /// Detects an Aztec Code in an image.
      /// </summary>
      /// <param name="isMirror">if true, image is a mirror-image of original.</param>
      /// <returns>
      /// encapsulating results of detecting an Aztec Code
      /// </returns>
      public AztecDetectorResult detect(bool isMirror)
      {
         // 1. Get the center of the aztec matrix
         var pCenter = getMatrixCenter();
         if (pCenter == null)
            return null;

         // 2. Get the center points of the four diagonal points just outside the bull's eye
         //  [topRight, bottomRight, bottomLeft, topLeft]
         var bullsEyeCorners = getBullsEyeCorners(pCenter);
         if (bullsEyeCorners == null)
         {
            return null;
         }
         if (isMirror)
         {
            ResultPoint temp = bullsEyeCorners[0];
            bullsEyeCorners[0] = bullsEyeCorners[2];
            bullsEyeCorners[2] = temp;
         }

         // 3. Get the size of the matrix and other parameters from the bull's eye
         if (!extractParameters(bullsEyeCorners))
         {
            return null;
         }

         // 4. Sample the grid
         var bits = sampleGrid(image,
                               bullsEyeCorners[shift%4],
                               bullsEyeCorners[(shift + 1)%4],
                               bullsEyeCorners[(shift + 2)%4],
                               bullsEyeCorners[(shift + 3)%4]);
         if (bits == null)
         {
            return null;
         }

         // 5. Get the corners of the matrix.
         var corners = getMatrixCornerPoints(bullsEyeCorners);
         if (corners == null)
         {
            return null;
         }

         return new AztecDetectorResult(bits, corners, compact, nbDataBlocks, nbLayers);
      }

      /// <summary>
      /// Extracts the number of data layers and data blocks from the layer around the bull's eye 
      /// </summary>
      /// <param name="bullsEyeCorners">bullEyeCornerPoints the array of bull's eye corners</param>
      /// <returns></returns>
      private bool extractParameters(ResultPoint[] bullsEyeCorners)
      {
         if (!isValid(bullsEyeCorners[0]) || !isValid(bullsEyeCorners[1]) ||
             !isValid(bullsEyeCorners[2]) || !isValid(bullsEyeCorners[3]))
         {
            return false;
         }
         
         int length = 2 * nbCenterLayers;

         // Get the bits around the bull's eye
         int[] sides =
            {
               sampleLine(bullsEyeCorners[0], bullsEyeCorners[1], length), // Right side
               sampleLine(bullsEyeCorners[1], bullsEyeCorners[2], length), // Bottom 
               sampleLine(bullsEyeCorners[2], bullsEyeCorners[3], length), // Left side
               sampleLine(bullsEyeCorners[3], bullsEyeCorners[0], length) // Top 
            };


         // bullsEyeCorners[shift] is the corner of the bulls'eye that has three 
         // orientation marks.  
         // sides[shift] is the row/column that goes from the corner with three
         // orientation marks to the corner with two.
         shift = getRotation(sides, length);
         if (shift < 0)
            return false;

         // Flatten the parameter bits into a single 28- or 40-bit long
         long parameterData = 0;
         for (int i = 0; i < 4; i++)
         {
            int side = sides[(shift + i) % 4];
            if (compact)
            {
               // Each side of the form ..XXXXXXX. where Xs are parameter data
               parameterData <<= 7;
               parameterData += (side >> 1) & 0x7F;
            }
            else
            {
               // Each side of the form ..XXXXX.XXXXX. where Xs are parameter data
               parameterData <<= 10;
               parameterData += ((side >> 2) & (0x1f << 5)) + ((side >> 1) & 0x1F);
            }
         }

         // Corrects parameter data using RS.  Returns just the data portion
         // without the error correction.
         int correctedData = getCorrectedParameterData(parameterData, compact);
         if (correctedData < 0)
            return false;

         if (compact)
         {
            // 8 bits:  2 bits layers and 6 bits data blocks
            nbLayers = (correctedData >> 6) + 1;
            nbDataBlocks = (correctedData & 0x3F) + 1;
         }
         else
         {
            // 16 bits:  5 bits layers and 11 bits data blocks
            nbLayers = (correctedData >> 11) + 1;
            nbDataBlocks = (correctedData & 0x7FF) + 1;
         }

         return true;
      }

      private static int getRotation(int[] sides, int length)
      {
         // In a normal pattern, we expect to See
         //   **    .*             D       A
         //   *      *
         //
         //   .      *
         //   ..    ..             C       B
         //
         // Grab the 3 bits from each of the sides the form the locator pattern and concatenate
         // into a 12-bit integer.  Start with the bit at A
         int cornerBits = 0;
         foreach (int side in sides)
         {
            // XX......X where X's are orientation marks
            int t = ((side >> (length - 2)) << 1) + (side & 1);
            cornerBits = (cornerBits << 3) + t;
         }
         // Mov the bottom bit to the top, so that the three bits of the locator pattern at A are
         // together.  cornerBits is now:
         //  3 orientation bits at A || 3 orientation bits at B || ... || 3 orientation bits at D
         cornerBits = ((cornerBits & 1) << 11) + (cornerBits >> 1);
         // The result shift indicates which element of BullsEyeCorners[] goes into the top-left
         // corner. Since the four rotation values have a Hamming distance of 8, we
         // can easily tolerate two errors.
         for (int shift = 0; shift < 4; shift++)
         {
            if (SupportClass.bitCount(cornerBits ^ EXPECTED_CORNER_BITS[shift]) <= 2)
            {
               return shift;
            }
         }
         return -1;
      }

      /// <summary>
      /// Corrects the parameter bits using Reed-Solomon algorithm
      /// </summary>
      /// <param name="parameterData">paremeter bits</param>
      /// <param name="compact">compact true if this is a compact Aztec code</param>
      /// <returns></returns>
      private static int getCorrectedParameterData(long parameterData, bool compact)
      {
         int numCodewords;
         int numDataCodewords;

         if (compact)
         {
            numCodewords = 7;
            numDataCodewords = 2;
         }
         else
         {
            numCodewords = 10;
            numDataCodewords = 4;
         }

         int numECCodewords = numCodewords - numDataCodewords;
         int[] parameterWords = new int[numCodewords];

         for (int i = numCodewords - 1; i >= 0; --i)
         {
            parameterWords[i] = (int)parameterData & 0xF;
            parameterData >>= 4;
         }

         var rsDecoder = new ReedSolomonDecoder(GenericGF.AZTEC_PARAM);
         if (!rsDecoder.decode(parameterWords, numECCodewords))
            return -1;

         // Toss the error correction.  Just return the data as an integer
         int result = 0;
         for (int i = 0; i < numDataCodewords; i++)
         {
            result = (result << 4) + parameterWords[i];
         }
         return result;
      }

      /// <summary>
      /// Finds the corners of a bull-eye centered on the passed point
      /// This returns the centers of the diagonal points just outside the bull's eye
      /// Returns [topRight, bottomRight, bottomLeft, topLeft]
      /// </summary>
      /// <param name="pCenter">Center point</param>
      /// <returns>The corners of the bull-eye</returns>
      private ResultPoint[] getBullsEyeCorners(Point pCenter)
      {
         Point pina = pCenter;
         Point pinb = pCenter;
         Point pinc = pCenter;
         Point pind = pCenter;

         bool color = true;

         for (nbCenterLayers = 1; nbCenterLayers < 9; nbCenterLayers++)
         {
            Point pouta = getFirstDifferent(pina, color, 1, -1);
            Point poutb = getFirstDifferent(pinb, color, 1, 1);
            Point poutc = getFirstDifferent(pinc, color, -1, 1);
            Point poutd = getFirstDifferent(pind, color, -1, -1);

            //d      a
            //
            //c      b

            if (nbCenterLayers > 2)
            {
               float q = distance(poutd, pouta)*nbCenterLayers/(distance(pind, pina)*(nbCenterLayers + 2));
               if (q < 0.75 || q > 1.25 || !isWhiteOrBlackRectangle(pouta, poutb, poutc, poutd))
               {
                  break;
               }
            }

            pina = pouta;
            pinb = poutb;
            pinc = poutc;
            pind = poutd;

            color = !color;
         }

         if (nbCenterLayers != 5 && nbCenterLayers != 7)
         {
            return null;
         }

         compact = nbCenterLayers == 5;

         // Expand the square by .5 pixel in each direction so that we're on the border
         // between the white square and the black square
         var pinax = new ResultPoint(pina.X + 0.5f, pina.Y - 0.5f);
         var pinbx = new ResultPoint(pinb.X + 0.5f, pinb.Y + 0.5f);
         var pincx = new ResultPoint(pinc.X - 0.5f, pinc.Y + 0.5f);
         var pindx = new ResultPoint(pind.X - 0.5f, pind.Y - 0.5f);

         // Expand the square so that its corners are the centers of the points
         // just outside the bull's eye.
         return expandSquare(new[] {pinax, pinbx, pincx, pindx},
                             2*nbCenterLayers - 3,
                             2*nbCenterLayers);
      }

      /// <summary>
      /// Finds a candidate center point of an Aztec code from an image
      /// </summary>
      /// <returns>the center point</returns>
      private Point getMatrixCenter()
      {
         ResultPoint pointA;
         ResultPoint pointB;
         ResultPoint pointC;
         ResultPoint pointD;
         int cx;
         int cy;

         //Get a white rectangle that can be the border of the matrix in center bull's eye or
         var whiteDetector = WhiteRectangleDetector.Create(image);
         if (whiteDetector == null)
            return null;
         ResultPoint[] cornerPoints = whiteDetector.detect();
         if (cornerPoints != null)
         {
            pointA = cornerPoints[0];
            pointB = cornerPoints[1];
            pointC = cornerPoints[2];
            pointD = cornerPoints[3];
         }
         else
         {

            // This exception can be in case the initial rectangle is white
            // In that case, surely in the bull's eye, we try to expand the rectangle.
            cx = image.Width/2;
            cy = image.Height/2;
            pointA = getFirstDifferent(new Point(cx + 7, cy - 7), false, 1, -1).toResultPoint();
            pointB = getFirstDifferent(new Point(cx + 7, cy + 7), false, 1, 1).toResultPoint();
            pointC = getFirstDifferent(new Point(cx - 7, cy + 7), false, -1, 1).toResultPoint();
            pointD = getFirstDifferent(new Point(cx - 7, cy - 7), false, -1, -1).toResultPoint();
         }

         //Compute the center of the rectangle
         cx = MathUtils.round((pointA.X + pointD.X + pointB.X + pointC.X) / 4.0f);
         cy = MathUtils.round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4.0f);

         // Redetermine the white rectangle starting from previously computed center.
         // This will ensure that we end up with a white rectangle in center bull's eye
         // in order to compute a more accurate center.
         whiteDetector = WhiteRectangleDetector.Create(image, 15, cx, cy);
         if (whiteDetector == null)
            return null;
         cornerPoints = whiteDetector.detect();
         if (cornerPoints != null)
         {
            pointA = cornerPoints[0];
            pointB = cornerPoints[1];
            pointC = cornerPoints[2];
            pointD = cornerPoints[3];
         }
         else
         {
            // This exception can be in case the initial rectangle is white
            // In that case we try to expand the rectangle.
            pointA = getFirstDifferent(new Point(cx + 7, cy - 7), false, 1, -1).toResultPoint();
            pointB = getFirstDifferent(new Point(cx + 7, cy + 7), false, 1, 1).toResultPoint();
            pointC = getFirstDifferent(new Point(cx - 7, cy + 7), false, -1, 1).toResultPoint();
            pointD = getFirstDifferent(new Point(cx - 7, cy - 7), false, -1, -1).toResultPoint();
         }

         // Recompute the center of the rectangle
         cx = MathUtils.round((pointA.X + pointD.X + pointB.X + pointC.X) / 4.0f);
         cy = MathUtils.round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4.0f);

         return new Point(cx, cy);
      }

      /// <summary>
      /// Gets the Aztec code corners from the bull's eye corners and the parameters.
      /// </summary>
      /// <param name="bullsEyeCorners">the array of bull's eye corners</param>
      /// <returns>the array of aztec code corners</returns>
      private ResultPoint[] getMatrixCornerPoints(ResultPoint[] bullsEyeCorners)
      {
         return expandSquare(bullsEyeCorners, 2 * nbCenterLayers, getDimension());
      }

      /// <summary>
      /// Creates a BitMatrix by sampling the provided image.
      /// topLeft, topRight, bottomRight, and bottomLeft are the centers of the squares on the
      /// diagonal just outside the bull's eye.
      /// </summary>
      /// <param name="image">The image.</param>
      /// <param name="topLeft">The top left.</param>
      /// <param name="bottomLeft">The bottom left.</param>
      /// <param name="bottomRight">The bottom right.</param>
      /// <param name="topRight">The top right.</param>
      /// <returns></returns>
      private BitMatrix sampleGrid(BitMatrix image,
                                   ResultPoint topLeft,
                                   ResultPoint topRight,
                                   ResultPoint bottomRight,
                                   ResultPoint bottomLeft)
      {
         GridSampler sampler = GridSampler.Instance;
         int dimension = getDimension();

         float low = dimension/2.0f - nbCenterLayers;
         float high = dimension/2.0f + nbCenterLayers;

         return sampler.sampleGrid(image,
                                   dimension,
                                   dimension,
                                   low, low, // topleft
                                   high, low, // topright
                                   high, high, // bottomright
                                   low, high, // bottomleft
                                   topLeft.X, topLeft.Y,
                                   topRight.X, topRight.Y,
                                   bottomRight.X, bottomRight.Y,
                                   bottomLeft.X, bottomLeft.Y);
      }

      /// <summary>
      /// Samples a line
      /// </summary>
      /// <param name="p1">start point (inclusive)</param>
      /// <param name="p2">end point (exclusive)</param>
      /// <param name="size">number of bits</param>
      /// <returns> the array of bits as an int (first bit is high-order bit of result)</returns>
      private int sampleLine(ResultPoint p1, ResultPoint p2, int size)
      {
         int result = 0;

         float d = distance(p1, p2);
         float moduleSize = d / size;
         float px = p1.X;
         float py = p1.Y;
         float dx = moduleSize * (p2.X - p1.X) / d;
         float dy = moduleSize * (p2.Y - p1.Y) / d;
         for (int i = 0; i < size; i++)
         {
            if (image[MathUtils.round(px + i * dx), MathUtils.round(py + i * dy)])
            {
               result |= 1 << (size - i - 1);
            }
         }
         return result;
      }

      /// <summary>
      /// Determines whether [is white or black rectangle] [the specified p1].
      /// </summary>
      /// <param name="p1">The p1.</param>
      /// <param name="p2">The p2.</param>
      /// <param name="p3">The p3.</param>
      /// <param name="p4">The p4.</param>
      /// <returns>true if the border of the rectangle passed in parameter is compound of white points only
      /// or black points only</returns>
      private bool isWhiteOrBlackRectangle(Point p1, Point p2, Point p3, Point p4)
      {
         const int corr = 3;

         p1 = new Point(p1.X - corr, p1.Y + corr);
         p2 = new Point(p2.X - corr, p2.Y - corr);
         p3 = new Point(p3.X + corr, p3.Y - corr);
         p4 = new Point(p4.X + corr, p4.Y + corr);

         int cInit = getColor(p4, p1);

         if (cInit == 0)
         {
            return false;
         }

         int c = getColor(p1, p2);

         if (c != cInit)
         {
            return false;
         }

         c = getColor(p2, p3);

         if (c != cInit)
         {
            return false;
         }

         c = getColor(p3, p4);

         return c == cInit;

      }

      /// <summary>
      /// Gets the color of a segment
      /// </summary>
      /// <param name="p1">The p1.</param>
      /// <param name="p2">The p2.</param>
      /// <returns>1 if segment more than 90% black, -1 if segment is more than 90% white, 0 else</returns>
      private int getColor(Point p1, Point p2)
      {
         float d = distance(p1, p2);
         float dx = (p2.X - p1.X) / d;
         float dy = (p2.Y - p1.Y) / d;
         int error = 0;

         float px = p1.X;
         float py = p1.Y;

         bool colorModel = image[p1.X, p1.Y];

         int iMax = (int) Math.Ceiling(d);
         for (int i = 0; i < iMax; i++)
         {
            px += dx;
            py += dy;
            if (image[MathUtils.round(px), MathUtils.round(py)] != colorModel)
            {
               error++;
            }
         }

         float errRatio = error / d;

         if (errRatio > 0.1f && errRatio < 0.9f)
         {
            return 0;
         }

         return (errRatio <= 0.1f) == colorModel ? 1 : -1;
      }

      /// <summary>
      /// Gets the coordinate of the first point with a different color in the given direction
      /// </summary>
      /// <param name="init">The init.</param>
      /// <param name="color">if set to <c>true</c> [color].</param>
      /// <param name="dx">The dx.</param>
      /// <param name="dy">The dy.</param>
      /// <returns></returns>
      private Point getFirstDifferent(Point init, bool color, int dx, int dy)
      {
         int x = init.X + dx;
         int y = init.Y + dy;

         while (isValid(x, y) && image[x, y] == color)
         {
            x += dx;
            y += dy;
         }

         x -= dx;
         y -= dy;

         while (isValid(x, y) && image[x, y] == color)
         {
            x += dx;
         }
         x -= dx;

         while (isValid(x, y) && image[x, y] == color)
         {
            y += dy;
         }
         y -= dy;

         return new Point(x, y);
      }

      /// <summary>
      /// Expand the square represented by the corner points by pushing out equally in all directions
      /// </summary>
      /// <param name="cornerPoints">the corners of the square, which has the bull's eye at its center</param>
      /// <param name="oldSide">the original length of the side of the square in the target bit matrix</param>
      /// <param name="newSide">the new length of the size of the square in the target bit matrix</param>
      /// <returns>the corners of the expanded square</returns>
      private static ResultPoint[] expandSquare(ResultPoint[] cornerPoints, float oldSide, float newSide)
      {
         float ratio = newSide/(2*oldSide);
         float dx = cornerPoints[0].X - cornerPoints[2].X;
         float dy = cornerPoints[0].Y - cornerPoints[2].Y;
         float centerx = (cornerPoints[0].X + cornerPoints[2].X)/2.0f;
         float centery = (cornerPoints[0].Y + cornerPoints[2].Y)/2.0f;

         var result0 = new ResultPoint(centerx + ratio*dx, centery + ratio*dy);
         var result2 = new ResultPoint(centerx - ratio * dx, centery - ratio * dy);

         dx = cornerPoints[1].X - cornerPoints[3].X;
         dy = cornerPoints[1].Y - cornerPoints[3].Y;
         centerx = (cornerPoints[1].X + cornerPoints[3].X)/2.0f;
         centery = (cornerPoints[1].Y + cornerPoints[3].Y)/2.0f;
         var result1 = new ResultPoint(centerx + ratio * dx, centery + ratio * dy);
         var result3 = new ResultPoint(centerx - ratio * dx, centery - ratio * dy);

         return new ResultPoint[] {result0, result1, result2, result3};
      }

      private bool isValid(int x, int y)
      {
         return x >= 0 && x < image.Width && y > 0 && y < image.Height;
      }

      private bool isValid(ResultPoint point)
      {
         int x = MathUtils.round(point.X);
         int y = MathUtils.round(point.Y);
         return isValid(x, y);
      }

      // L2 distance
      private static float distance(Point a, Point b)
      {
         return MathUtils.distance(a.X, a.Y, b.X, b.Y);
      }

      private static float distance(ResultPoint a, ResultPoint b)
      {
         return MathUtils.distance(a.X, a.Y, b.X, b.Y);
      }

      private int getDimension()
      {
         if (compact)
         {
            return 4 * nbLayers + 11;
         }
         if (nbLayers <= 4)
         {
            return 4 * nbLayers + 15;
         }
         return 4 * nbLayers + 2 * ((nbLayers - 4) / 8 + 1) + 15;
      }

      internal sealed class Point
      {
         public int X { get; private set; }
         public int Y { get; private set; }

         public ResultPoint toResultPoint()
         {
            return new ResultPoint(X, Y);
         }

         internal Point(int x, int y)
         {
            X = x;
            Y = y;
         }

         public override String ToString()
         {
            return "<" + X + ' ' + Y + '>';
         }
      }
   }
}