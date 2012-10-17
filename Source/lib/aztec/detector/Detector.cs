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

using ZXing.Common;
using ZXing.Common.Detector;
using ZXing.Common.ReedSolomon;

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// <p>Encapsulates logic that can detect an Aztec Code in an image, even if the Aztec Code
   /// is rotated or skewed, or partially obscured.</p>
   /// </summary>
   /// <author>David Olivier</author>
   public sealed class Detector
   {
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
      /// <returns>encapsulating results of detecting an Aztec Code</returns>
      public AztecDetectorResult detect()
      {
         // 1. Get the center of the aztec matrix
         Point pCenter = getMatrixCenter();
         if (pCenter == null)
            return null;

         // 2. Get the corners of the center bull's eye
         Point[] bullEyeCornerPoints = getBullEyeCornerPoints(pCenter);
         if (bullEyeCornerPoints == null)
            return null;

         // 3. Get the size of the matrix from the bull's eye
         if (!extractParameters(bullEyeCornerPoints))
            return null;

         // 4. Get the corners of the matrix
         ResultPoint[] corners = getMatrixCornerPoints(bullEyeCornerPoints);
         if (corners == null)
            return null;

         // 5. Sample the grid
         BitMatrix bits = sampleGrid(image, corners[shift % 4], corners[(shift + 3) % 4], corners[(shift + 2) % 4], corners[(shift + 1) % 4]);
         if (bits == null)
            return null;

         return new AztecDetectorResult(bits, corners, compact, nbDataBlocks, nbLayers);
      }

      /// <summary>
      /// Extracts the number of data layers and data blocks from the layer around the bull's eye 
      /// </summary>
      /// <param name="bullEyeCornerPoints">bullEyeCornerPoints the array of bull's eye corners</param>
      /// <returns></returns>
      private bool extractParameters(Point[] bullEyeCornerPoints)
      {
         // Get the bits around the bull's eye
         bool[] resab = sampleLine(bullEyeCornerPoints[0], bullEyeCornerPoints[1], 2 * nbCenterLayers + 1);
         bool[] resbc = sampleLine(bullEyeCornerPoints[1], bullEyeCornerPoints[2], 2 * nbCenterLayers + 1);
         bool[] rescd = sampleLine(bullEyeCornerPoints[2], bullEyeCornerPoints[3], 2 * nbCenterLayers + 1);
         bool[] resda = sampleLine(bullEyeCornerPoints[3], bullEyeCornerPoints[0], 2 * nbCenterLayers + 1);

         // Determine the orientation of the matrix
         if (resab[0] && resab[2 * nbCenterLayers])
         {
            shift = 0;
         }
         else if (resbc[0] && resbc[2 * nbCenterLayers])
         {
            shift = 1;
         }
         else if (rescd[0] && rescd[2 * nbCenterLayers])
         {
            shift = 2;
         }
         else if (resda[0] && resda[2 * nbCenterLayers])
         {
            shift = 3;
         }
         else
         {
            return false;
         }

         //d      a
         //
         //c      b

         // Flatten the bits in a single array
         bool[] parameterData;
         bool[] shiftedParameterData;
         if (compact)
         {
            shiftedParameterData = new bool[28];
            for (int i = 0; i < 7; i++)
            {
               shiftedParameterData[i] = resab[2 + i];
               shiftedParameterData[i + 7] = resbc[2 + i];
               shiftedParameterData[i + 14] = rescd[2 + i];
               shiftedParameterData[i + 21] = resda[2 + i];
            }

            parameterData = new bool[28];
            for (int i = 0; i < 28; i++)
            {
               parameterData[i] = shiftedParameterData[(i + shift * 7) % 28];
            }
         }
         else
         {
            shiftedParameterData = new bool[40];
            for (int i = 0; i < 11; i++)
            {
               if (i < 5)
               {
                  shiftedParameterData[i] = resab[2 + i];
                  shiftedParameterData[i + 10] = resbc[2 + i];
                  shiftedParameterData[i + 20] = rescd[2 + i];
                  shiftedParameterData[i + 30] = resda[2 + i];
               }
               if (i > 5)
               {
                  shiftedParameterData[i - 1] = resab[2 + i];
                  shiftedParameterData[i + 10 - 1] = resbc[2 + i];
                  shiftedParameterData[i + 20 - 1] = rescd[2 + i];
                  shiftedParameterData[i + 30 - 1] = resda[2 + i];
               }
            }

            parameterData = new bool[40];
            for (int i = 0; i < 40; i++)
            {
               parameterData[i] = shiftedParameterData[(i + shift * 10) % 40];
            }
         }

         // corrects the error using RS algorithm
         if (!correctParameterData(parameterData, compact))
            return false;

         // gets the parameters from the bit array
         getParameters(parameterData);

         return true;
      }

      /// <summary>
      /// Gets the Aztec code corners from the bull's eye corners and the parameters
      /// </summary>
      /// <param name="bullEyeCornerPoints">the array of bull's eye corners</param>
      /// <returns>the array of aztec code corners</returns>
      private ResultPoint[] getMatrixCornerPoints(Point[] bullEyeCornerPoints)
      {
         float ratio = (2 * nbLayers + (nbLayers > 4 ? 1 : 0) + (nbLayers - 4) / 8)
             / (2.0f * nbCenterLayers);

         int dx = bullEyeCornerPoints[0].x - bullEyeCornerPoints[2].x;
         dx += dx > 0 ? 1 : -1;
         int dy = bullEyeCornerPoints[0].y - bullEyeCornerPoints[2].y;
         dy += dy > 0 ? 1 : -1;

         int targetcx = MathUtils.round(bullEyeCornerPoints[2].x - ratio * dx);
         int targetcy = MathUtils.round(bullEyeCornerPoints[2].y - ratio * dy);

         int targetax = MathUtils.round(bullEyeCornerPoints[0].x + ratio * dx);
         int targetay = MathUtils.round(bullEyeCornerPoints[0].y + ratio * dy);

         dx = bullEyeCornerPoints[1].x - bullEyeCornerPoints[3].x;
         dx += dx > 0 ? 1 : -1;
         dy = bullEyeCornerPoints[1].y - bullEyeCornerPoints[3].y;
         dy += dy > 0 ? 1 : -1;

         int targetdx = MathUtils.round(bullEyeCornerPoints[3].x - ratio * dx);
         int targetdy = MathUtils.round(bullEyeCornerPoints[3].y - ratio * dy);
         int targetbx = MathUtils.round(bullEyeCornerPoints[1].x + ratio * dx);
         int targetby = MathUtils.round(bullEyeCornerPoints[1].y + ratio * dy);

         if (!isValid(targetax, targetay) || !isValid(targetbx, targetby) || !isValid(targetcx, targetcy) || !isValid(targetdx, targetdy))
         {
            return null;
         }

         return new[] { new ResultPoint(targetax, targetay), new ResultPoint(targetbx, targetby), new ResultPoint(targetcx, targetcy), new ResultPoint(targetdx, targetdy) };
      }

      /// <summary>
      /// Corrects the parameter bits using Reed-Solomon algorithm
      /// </summary>
      /// <param name="parameterData">paremeter bits</param>
      /// <param name="compact">compact true if this is a compact Aztec code</param>
      /// <returns></returns>
      private static bool correctParameterData(bool[] parameterData, bool compact)
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

         const int codewordSize = 4;
         for (int i = 0; i < numCodewords; i++)
         {
            int flag = 1;
            for (int j = 1; j <= codewordSize; j++)
            {
               if (parameterData[codewordSize * i + codewordSize - j])
               {
                  parameterWords[i] += flag;
               }
               flag <<= 1;
            }
         }

         var rsDecoder = new ReedSolomonDecoder(GenericGF.AZTEC_PARAM);
         if (!rsDecoder.decode(parameterWords, numECCodewords))
            return false;

         for (int i = 0; i < numDataCodewords; i++)
         {
            int flag = 1;
            for (int j = 1; j <= codewordSize; j++)
            {
               parameterData[i * codewordSize + codewordSize - j] = (parameterWords[i] & flag) == flag;
               flag <<= 1;
            }
         }

         return true;
      }

      /// <summary>
      /// Finds the corners of a bull-eye centered on the passed point
      /// </summary>
      /// <param name="pCenter">Center point</param>
      /// <returns>The corners of the bull-eye</returns>
      private Point[] getBullEyeCornerPoints(Point pCenter)
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
               float q = distance(poutd, pouta) * nbCenterLayers / (distance(pind, pina) * (nbCenterLayers + 2));
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

         float ratio = 0.75f * 2 / (2 * nbCenterLayers - 3);

         int dx = pina.x - pinc.x;
         int dy = pina.y - pinc.y;
         int targetcx = MathUtils.round(pinc.x - ratio * dx);
         int targetcy = MathUtils.round(pinc.y - ratio * dy);
         int targetax = MathUtils.round(pina.x + ratio * dx);
         int targetay = MathUtils.round(pina.y + ratio * dy);

         dx = pinb.x - pind.x;
         dy = pinb.y - pind.y;

         int targetdx = MathUtils.round(pind.x - ratio * dx);
         int targetdy = MathUtils.round(pind.y - ratio * dy);
         int targetbx = MathUtils.round(pinb.x + ratio * dx);
         int targetby = MathUtils.round(pinb.y + ratio * dy);

         if (!isValid(targetax, targetay) || !isValid(targetbx, targetby)
             || !isValid(targetcx, targetcy) || !isValid(targetdx, targetdy))
         {
            return null;
         }

         Point pa = new Point(targetax, targetay);
         Point pb = new Point(targetbx, targetby);
         Point pc = new Point(targetcx, targetcy);
         Point pd = new Point(targetdx, targetdy);

         return new[] { pa, pb, pc, pd };
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
            pointA = getFirstDifferent(new Point(cx + 15/2, cy - 15/2), false, 1, -1).toResultPoint();
            pointB = getFirstDifferent(new Point(cx + 15/2, cy + 15/2), false, 1, 1).toResultPoint();
            pointC = getFirstDifferent(new Point(cx - 15/2, cy + 15/2), false, -1, 1).toResultPoint();
            pointD = getFirstDifferent(new Point(cx - 15/2, cy - 15/2), false, -1, -1).toResultPoint();
         }

         //Compute the center of the rectangle
         cx = MathUtils.round((pointA.X + pointD.X + pointB.X + pointC.X) / 4);
         cy = MathUtils.round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4);

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
            pointA = getFirstDifferent(new Point(cx + 15/2, cy - 15/2), false, 1, -1).toResultPoint();
            pointB = getFirstDifferent(new Point(cx + 15/2, cy + 15/2), false, 1, 1).toResultPoint();
            pointC = getFirstDifferent(new Point(cx - 15/2, cy + 15/2), false, -1, 1).toResultPoint();
            pointD = getFirstDifferent(new Point(cx - 15/2, cy - 15/2), false, -1, -1).toResultPoint();
         }

         // Recompute the center of the rectangle
         cx = MathUtils.round((pointA.X + pointD.X + pointB.X + pointC.X) / 4);
         cy = MathUtils.round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4);

         return new Point(cx, cy);
      }

      /// <summary>
      /// Samples an Aztec matrix from an image
      /// </summary>
      /// <param name="image">The image.</param>
      /// <param name="topLeft">The top left.</param>
      /// <param name="bottomLeft">The bottom left.</param>
      /// <param name="bottomRight">The bottom right.</param>
      /// <param name="topRight">The top right.</param>
      /// <returns></returns>
      private BitMatrix sampleGrid(BitMatrix image,
                                   ResultPoint topLeft,
                                   ResultPoint bottomLeft,
                                   ResultPoint bottomRight,
                                   ResultPoint topRight)
      {
         int dimension;
         if (compact)
         {
            dimension = 4 * nbLayers + 11;
         }
         else
         {
            if (nbLayers <= 4)
            {
               dimension = 4 * nbLayers + 15;
            }
            else
            {
               dimension = 4 * nbLayers + 2 * ((nbLayers - 4) / 8 + 1) + 15;
            }
         }

         GridSampler sampler = GridSampler.Instance;

         return sampler.sampleGrid(image,
           dimension,
           dimension,
           0.5f,
           0.5f,
           dimension - 0.5f,
           0.5f,
           dimension - 0.5f,
           dimension - 0.5f,
           0.5f,
           dimension - 0.5f,
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
      /// Sets number of layers and number of datablocks from parameter bits
      /// </summary>
      /// <param name="parameterData">The parameter data.</param>
      private void getParameters(bool[] parameterData)
      {

         int nbBitsForNbLayers;
         int nbBitsForNbDatablocks;

         if (compact)
         {
            nbBitsForNbLayers = 2;
            nbBitsForNbDatablocks = 6;
         }
         else
         {
            nbBitsForNbLayers = 5;
            nbBitsForNbDatablocks = 11;
         }

         for (int i = 0; i < nbBitsForNbLayers; i++)
         {
            nbLayers <<= 1;
            if (parameterData[i])
            {
               nbLayers += 1;
            }
         }

         for (int i = nbBitsForNbLayers; i < nbBitsForNbLayers + nbBitsForNbDatablocks; i++)
         {
            nbDataBlocks <<= 1;
            if (parameterData[i])
            {
               nbDataBlocks += 1;
            }
         }

         nbLayers++;
         nbDataBlocks++;

      }

      /// <summary>
      /// Samples a line
      /// </summary>
      /// <param name="p1">first point</param>
      /// <param name="p2">second point</param>
      /// <param name="size">size number of bits</param>
      /// <returns>the array of bits</returns>
      private bool[] sampleLine(Point p1, Point p2, int size)
      {
         bool[] res = new bool[size];
         float d = distance(p1, p2);
         float moduleSize = d / (size - 1);
         float dx = moduleSize * (p2.x - p1.x) / d;
         float dy = moduleSize * (p2.y - p1.y) / d;

         float px = p1.x;
         float py = p1.y;

         for (int i = 0; i < size; i++)
         {
            res[i] = image[MathUtils.round(px), MathUtils.round(py)];
            px += dx;
            py += dy;
         }

         return res;
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

         p1 = new Point(p1.x - corr, p1.y + corr);
         p2 = new Point(p2.x - corr, p2.y - corr);
         p3 = new Point(p3.x + corr, p3.y - corr);
         p4 = new Point(p4.x + corr, p4.y + corr);

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
         float dx = (p2.x - p1.x) / d;
         float dy = (p2.y - p1.y) / d;
         int error = 0;

         float px = p1.x;
         float py = p1.y;

         bool colorModel = image[p1.x, p1.y];

         for (int i = 0; i < d; i++)
         {
            px += dx;
            py += dy;
            if (image[MathUtils.round(px), MathUtils.round(py)] != colorModel)
            {
               error++;
            }
         }

         float errRatio = error / d;

         if (errRatio > 0.1 && errRatio < 0.9)
         {
            return 0;
         }

         if (errRatio <= 0.1)
         {
            return colorModel ? 1 : -1;
         }
         return colorModel ? -1 : 1;
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
         int x = init.x + dx;
         int y = init.y + dy;

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

      internal sealed class Point
      {
         public int x;
         public int y;

         public ResultPoint toResultPoint()
         {
            return new ResultPoint(x, y);
         }

         internal Point(int x, int y)
         {
            this.x = x;
            this.y = y;
         }
      }

      private bool isValid(int x, int y)
      {
         return x >= 0 && x < image.Width && y > 0 && y < image.Height;
      }

      // L2 distance
      private static float distance(Point a, Point b)
      {
         return MathUtils.distance(a.x, a.y, b.x, b.y);
      }
   }
}