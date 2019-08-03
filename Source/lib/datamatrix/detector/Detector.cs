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

            ResultPoint[] points = detectSolid1(cornerPoints);
            points = detectSolid2(points);
            points[3] = correctTopRight(points);
            if (points[3] == null)
            {
                return null;
            }
            points = shiftToModuleCenter(points);

            ResultPoint topLeft = points[0];
            ResultPoint bottomLeft = points[1];
            ResultPoint bottomRight = points[2];
            ResultPoint topRight = points[3];

            int dimensionTop = transitionsBetween(topLeft, topRight) + 1;
            int dimensionRight = transitionsBetween(bottomRight, topRight) + 1;
            if ((dimensionTop & 0x01) == 1)
            {
                dimensionTop += 1;
            }
            if ((dimensionRight & 0x01) == 1)
            {
                dimensionRight += 1;
            }

            if (4 * dimensionTop < 7 * dimensionRight && 4 * dimensionRight < 7 * dimensionTop)
            {
                // The matrix is square
                dimensionTop = dimensionRight = Math.Max(dimensionTop, dimensionRight);
            }

            BitMatrix bits = sampleGrid(image,
                topLeft,
                bottomLeft,
                bottomRight,
                topRight,
                dimensionTop,
                dimensionRight);

            return new DetectorResult(bits, new ResultPoint[] { topLeft, bottomLeft, bottomRight, topRight });
        }

        private ResultPoint shiftPoint(ResultPoint point, ResultPoint to, int div)
        {
            float x = (to.X - point.X) / (div + 1);
            float y = (to.Y - point.Y) / (div + 1);
            return new ResultPoint(point.X + x, point.Y + y);
        }

        private ResultPoint moveAway(ResultPoint point, float fromX, float fromY)
        {
            float x = point.X;
            float y = point.Y;

            if (x < fromX)
            {
                x -= 1;
            }
            else
            {
                x += 1;
            }

            if (y < fromY)
            {
                y -= 1;
            }
            else
            {
                y += 1;
            }

            return new ResultPoint(x, y);
        }

        /// <summary>
        /// Detect a solid side which has minimum transition.
        /// </summary>
        /// <param name="cornerPoints"></param>
        /// <returns></returns>
        private ResultPoint[] detectSolid1(ResultPoint[] cornerPoints)
        {
            // 0  2
            // 1  3
            ResultPoint pointA = cornerPoints[0];
            ResultPoint pointB = cornerPoints[1];
            ResultPoint pointC = cornerPoints[3];
            ResultPoint pointD = cornerPoints[2];

            int trAB = transitionsBetween(pointA, pointB);
            int trBC = transitionsBetween(pointB, pointC);
            int trCD = transitionsBetween(pointC, pointD);
            int trDA = transitionsBetween(pointD, pointA);

            // 0..3
            // :  :
            // 1--2
            int min = trAB;
            ResultPoint[] points = { pointD, pointA, pointB, pointC };
            if (min > trBC)
            {
                min = trBC;
                points[0] = pointA;
                points[1] = pointB;
                points[2] = pointC;
                points[3] = pointD;
            }
            if (min > trCD)
            {
                min = trCD;
                points[0] = pointB;
                points[1] = pointC;
                points[2] = pointD;
                points[3] = pointA;
            }
            if (min > trDA)
            {
                points[0] = pointC;
                points[1] = pointD;
                points[2] = pointA;
                points[3] = pointB;
            }

            return points;
        }

        /// <summary>
        /// Detect a second solid side next to first solid side.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private ResultPoint[] detectSolid2(ResultPoint[] points)
        {
            // A..D
            // :  :
            // B--C
            ResultPoint pointA = points[0];
            ResultPoint pointB = points[1];
            ResultPoint pointC = points[2];
            ResultPoint pointD = points[3];

            // Transition detection on the edge is not stable.
            // To safely detect, shift the points to the module center.
            int tr = transitionsBetween(pointA, pointD);
            ResultPoint pointBs = shiftPoint(pointB, pointC, (tr + 1) * 4);
            ResultPoint pointCs = shiftPoint(pointC, pointB, (tr + 1) * 4);
            int trBA = transitionsBetween(pointBs, pointA);
            int trCD = transitionsBetween(pointCs, pointD);

            // 0..3
            // |  :
            // 1--2
            if (trBA < trCD)
            {
                // solid sides: A-B-C
                points[0] = pointA;
                points[1] = pointB;
                points[2] = pointC;
                points[3] = pointD;
            }
            else
            {
                // solid sides: B-C-D
                points[0] = pointB;
                points[1] = pointC;
                points[2] = pointD;
                points[3] = pointA;
            }

            return points;
        }

        /// <summary>
        /// Calculates the corner position of the white top right module.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private ResultPoint correctTopRight(ResultPoint[] points)
        {
            // A..D
            // |  :
            // B--C
            ResultPoint pointA = points[0];
            ResultPoint pointB = points[1];
            ResultPoint pointC = points[2];
            ResultPoint pointD = points[3];

            // shift points for safe transition detection.
            int trTop = transitionsBetween(pointA, pointD);
            int trRight = transitionsBetween(pointB, pointD);
            ResultPoint pointAs = shiftPoint(pointA, pointB, (trRight + 1) * 4);
            ResultPoint pointCs = shiftPoint(pointC, pointB, (trTop + 1) * 4);

            trTop = transitionsBetween(pointAs, pointD);
            trRight = transitionsBetween(pointCs, pointD);

            ResultPoint candidate1 = new ResultPoint(
                pointD.X + (pointC.X - pointB.X) / (trTop + 1),
                pointD.Y + (pointC.Y - pointB.Y) / (trTop + 1));
            ResultPoint candidate2 = new ResultPoint(
                pointD.X + (pointA.X - pointB.X) / (trRight + 1),
                pointD.Y + (pointA.Y - pointB.Y) / (trRight + 1));

            if (!isValid(candidate1))
            {
                if (isValid(candidate2))
                {
                    return candidate2;
                }
                return null;
            }
            if (!isValid(candidate2))
            {
                return candidate1;
            }

            int sumc1 = transitionsBetween(pointAs, candidate1) + transitionsBetween(pointCs, candidate1);
            int sumc2 = transitionsBetween(pointAs, candidate2) + transitionsBetween(pointCs, candidate2);

            if (sumc1 > sumc2)
            {
                return candidate1;
            }
            else
            {
                return candidate2;
            }
        }

        /// <summary>
        /// Shift the edge points to the module center.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private ResultPoint[] shiftToModuleCenter(ResultPoint[] points)
        {
            // A..D
            // |  :
            // B--C
            ResultPoint pointA = points[0];
            ResultPoint pointB = points[1];
            ResultPoint pointC = points[2];
            ResultPoint pointD = points[3];

            // calculate pseudo dimensions
            int dimH = transitionsBetween(pointA, pointD) + 1;
            int dimV = transitionsBetween(pointC, pointD) + 1;

            // shift points for safe dimension detection
            ResultPoint pointAs = shiftPoint(pointA, pointB, dimV * 4);
            ResultPoint pointCs = shiftPoint(pointC, pointB, dimH * 4);

            //  calculate more precise dimensions
            dimH = transitionsBetween(pointAs, pointD) + 1;
            dimV = transitionsBetween(pointCs, pointD) + 1;
            if ((dimH & 0x01) == 1)
            {
                dimH += 1;
            }
            if ((dimV & 0x01) == 1)
            {
                dimV += 1;
            }

            // WhiteRectangleDetector returns points inside of the rectangle.
            // I want points on the edges.
            float centerX = (pointA.X + pointB.X + pointC.X + pointD.X) / 4;
            float centerY = (pointA.Y + pointB.Y + pointC.Y + pointD.Y) / 4;
            pointA = moveAway(pointA, centerX, centerY);
            pointB = moveAway(pointB, centerX, centerY);
            pointC = moveAway(pointC, centerX, centerY);
            pointD = moveAway(pointD, centerX, centerY);

            ResultPoint pointBs;
            ResultPoint pointDs;

            // shift points to the center of each modules
            pointAs = shiftPoint(pointA, pointB, dimV * 4);
            pointAs = shiftPoint(pointAs, pointD, dimH * 4);
            pointBs = shiftPoint(pointB, pointA, dimV * 4);
            pointBs = shiftPoint(pointBs, pointC, dimH * 4);
            pointCs = shiftPoint(pointC, pointD, dimV * 4);
            pointCs = shiftPoint(pointCs, pointB, dimH * 4);
            pointDs = shiftPoint(pointD, pointC, dimV * 4);
            pointDs = shiftPoint(pointDs, pointA, dimH * 4);

            return new ResultPoint[] { pointAs, pointBs, pointCs, pointDs };
        }

        private bool isValid(ResultPoint p)
        {
            return p.X >= 0 && p.X < image.Width && p.Y > 0 && p.Y < image.Height;
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
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private int transitionsBetween(ResultPoint from, ResultPoint to)
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
            int error = -dx / 2;
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
            return transitions;
        }
    }
}
