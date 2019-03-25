/*
 * Copyright 2012 ZXing authors
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

namespace ZXing.Common.Detector
{
    /// <summary>
    /// General math-related and numeric utility functions.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Ends up being a bit faster than {@link Math#round(float)}. This merely rounds its
        /// argument to the nearest int, where x.5 rounds up to x+1. Semantics of this shortcut
        /// differ slightly from {@link Math#round(float)} in that half rounds down for negative
        /// values. -2.5 rounds to -3, not -2. For purposes here it makes no difference.
        /// </summary>
        /// <param name="d">real value to round</param>
        /// <returns>nearest <c>int</c></returns>
        public static int round(float d)
        {
            if (float.IsNaN(d))
                return 0;
            if (float.IsPositiveInfinity(d))
                return int.MaxValue;
            return (int)(d + (d < 0.0f ? -0.5f : 0.5f));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aX"></param>
        /// <param name="aY"></param>
        /// <param name="bX"></param>
        /// <param name="bY"></param>
        /// <returns>Euclidean distance between points A and B</returns>
        public static float distance(float aX, float aY, float bX, float bY)
        {
            double xDiff = aX - bX;
            double yDiff = aY - bY;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aX"></param>
        /// <param name="aY"></param>
        /// <param name="bX"></param>
        /// <param name="bY"></param>
        /// <returns>Euclidean distance between points A and B</returns>
        public static float distance(int aX, int aY, int bX, int bY)
        {
            double xDiff = aX - bX;
            double yDiff = aY - bY;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        /// <summary>
        /// </summary>
        /// <param name="array">values to sum</param>
        /// <returns>sum of values in array</returns>
        public static int sum(int[] array)
        {
            int count = 0;
            foreach (int a in array)
            {
                count += a;
            }
            return count;
        }
    }
}
