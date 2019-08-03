/*
* Copyright 2007 ZXing authors
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

namespace ZXing.QrCode.Internal
{
    /// <summary>
    /// <p>This class attempts to find finder patterns in a QR Code. Finder patterns are the square
    /// markers at three corners of a QR Code.</p>
    /// 
    /// <p>This class is thread-safe but not reentrant. Each thread must allocate its own object.</p>
    /// </summary>
    /// <author>Sean Owen</author>
    public class FinderPatternFinder
    {
        private const int CENTER_QUORUM = 2;
        private static EstimatedModuleComparator moduleComparator = new EstimatedModuleComparator();
        /// <summary>
        /// 1 pixel/module times 3 modules/center
        /// </summary>
        protected internal const int MIN_SKIP = 3;

        /// <summary>
        /// support up to version 20 for mobile clients
        /// </summary>
        protected internal const int MAX_MODULES = 97;

        private const int INTEGER_MATH_SHIFT = 8;

        private readonly BitMatrix image;
        private readonly List<FinderPattern> possibleCenters;
        private bool hasSkipped;
        private readonly int[] crossCheckStateCount;
        private readonly ResultPointCallback resultPointCallback;

        /// <summary>
        /// <p>Creates a finder that will search the image for three finder patterns.</p>
        /// </summary>
        /// <param name="image">image to search</param>
        public FinderPatternFinder(BitMatrix image)
            : this(image, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinderPatternFinder"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="resultPointCallback">The result point callback.</param>
        public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
        {
            this.image = image;
            this.possibleCenters = new List<FinderPattern>();
            this.crossCheckStateCount = new int[5];
            this.resultPointCallback = resultPointCallback;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        protected internal virtual BitMatrix Image
        {
            get { return image; }
        }

        /// <summary>
        /// Gets the possible centers.
        /// </summary>
        protected internal virtual List<FinderPattern> PossibleCenters
        {
            get { return possibleCenters; }
        }

        internal virtual FinderPatternInfo find(IDictionary<DecodeHintType, object> hints)
        {
            bool tryHarder = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
            int maxI = image.Height;
            int maxJ = image.Width;
            // We are looking for black/white/black/white/black modules in
            // 1:1:3:1:1 ratio; this tracks the number of such modules seen so far

            // Let's assume that the maximum version QR Code we support takes up 1/4 the height of the
            // image, and then account for the center being 3 modules in size. This gives the smallest
            // number of pixels the center could be, so skip this often. When trying harder, look for all
            // QR versions regardless of how dense they are.
            int iSkip = (3 * maxI) / (4 * MAX_MODULES);
            if (iSkip < MIN_SKIP || tryHarder)
            {
                iSkip = MIN_SKIP;
            }

            bool done = false;
            int[] stateCount = new int[5];
            for (int i = iSkip - 1; i < maxI && !done; i += iSkip)
            {
                // Get a row of black/white values
                clearCounts(stateCount);
                int currentState = 0;
                for (int j = 0; j < maxJ; j++)
                {
                    if (image[j, i])
                    {
                        // Black pixel
                        if ((currentState & 1) == 1)
                        {
                            // Counting white pixels
                            currentState++;
                        }
                        stateCount[currentState]++;
                    }
                    else
                    {
                        // White pixel
                        if ((currentState & 1) == 0)
                        {
                            // Counting black pixels
                            if (currentState == 4)
                            {
                                // A winner?
                                if (foundPatternCross(stateCount))
                                {
                                    // Yes
                                    bool confirmed = handlePossibleCenter(stateCount, i, j);
                                    if (confirmed)
                                    {
                                        // Start examining every other line. Checking each line turned out to be too
                                        // expensive and didn't improve performance.
                                        iSkip = 2;
                                        if (hasSkipped)
                                        {
                                            done = haveMultiplyConfirmedCenters();
                                        }
                                        else
                                        {
                                            int rowSkip = findRowSkip();
                                            if (rowSkip > stateCount[2])
                                            {
                                                // Skip rows between row of lower confirmed center
                                                // and top of presumed third confirmed center
                                                // but back up a bit to get a full chance of detecting
                                                // it, entire width of center of finder pattern

                                                // Skip by rowSkip, but back off by stateCount[2] (size of last center
                                                // of pattern we saw) to be conservative, and also back off by iSkip which
                                                // is about to be re-added
                                                i += rowSkip - stateCount[2] - iSkip;
                                                j = maxJ - 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        shiftCounts2(stateCount);
                                        currentState = 3;
                                        continue;
                                    }
                                    // Clear state to start looking again
                                    currentState = 0;
                                    clearCounts(stateCount);
                                }
                                else
                                {
                                    // No, shift counts back by two
                                    shiftCounts2(stateCount);
                                    currentState = 3;
                                }
                            }
                            else
                            {
                                stateCount[++currentState]++;
                            }
                        }
                        else
                        {
                            // Counting white pixels
                            stateCount[currentState]++;
                        }
                    }
                }
                if (foundPatternCross(stateCount))
                {
                    bool confirmed = handlePossibleCenter(stateCount, i, maxJ);
                    if (confirmed)
                    {
                        iSkip = stateCount[0];
                        if (hasSkipped)
                        {
                            // Found a third one
                            done = haveMultiplyConfirmedCenters();
                        }
                    }
                }
            }

            FinderPattern[] patternInfo = selectBestPatterns();
            if (patternInfo == null)
                return null;

            ResultPoint.orderBestPatterns(patternInfo);

            return new FinderPatternInfo(patternInfo);
        }

        /// <summary> Given a count of black/white/black/white/black pixels just seen and an end position,
        /// figures the location of the center of this run.
        /// </summary>
        private static float? centerFromEnd(int[] stateCount, int end)
        {
            var result = (end - stateCount[4] - stateCount[3]) - stateCount[2] / 2.0f;
            if (Single.IsNaN(result))
                return null;
            return result;
        }

        /// <param name="stateCount">count of black/white/black/white/black pixels just read
        /// </param>
        /// <returns> true iff the proportions of the counts is close enough to the 1/1/3/1/1 ratios
        /// used by finder patterns to be considered a match
        /// </returns>
        protected internal static bool foundPatternCross(int[] stateCount)
        {
            int totalModuleSize = 0;
            for (int i = 0; i < 5; i++)
            {
                int count = stateCount[i];
                if (count == 0)
                {
                    return false;
                }
                totalModuleSize += count;
            }
            if (totalModuleSize < 7)
            {
                return false;
            }
            int moduleSize = (totalModuleSize << INTEGER_MATH_SHIFT) / 7;
            int maxVariance = moduleSize / 2;
            // Allow less than 50% variance from 1-1-3-1-1 proportions
            return Math.Abs(moduleSize - (stateCount[0] << INTEGER_MATH_SHIFT)) < maxVariance &&
                   Math.Abs(moduleSize - (stateCount[1] << INTEGER_MATH_SHIFT)) < maxVariance &&
                   Math.Abs(3 * moduleSize - (stateCount[2] << INTEGER_MATH_SHIFT)) < 3 * maxVariance &&
                   Math.Abs(moduleSize - (stateCount[3] << INTEGER_MATH_SHIFT)) < maxVariance &&
                   Math.Abs(moduleSize - (stateCount[4] << INTEGER_MATH_SHIFT)) < maxVariance;
        }

        /// <summary>
        /// </summary>
        /// <param name="stateCount">count of black/white/black/white/black pixels just read</param>
        /// <returns>true if the proportions of the counts is close enough to the 1/1/3/1/1 ratios
        /// by finder patterns to be considered a match</returns>
        protected static bool foundPatternDiagonal(int[] stateCount)
        {
            int totalModuleSize = 0;
            for (int i = 0; i < 5; i++)
            {
                int count = stateCount[i];
                if (count == 0)
                {
                    return false;
                }
                totalModuleSize += count;
            }
            if (totalModuleSize < 7)
            {
                return false;
            }
            float moduleSize = totalModuleSize / 7.0f;
            float maxVariance = moduleSize / 1.333f;
            // Allow less than 75% variance from 1-1-3-1-1 proportions
            return
                Math.Abs(moduleSize - stateCount[0]) < maxVariance &&
                Math.Abs(moduleSize - stateCount[1]) < maxVariance &&
                Math.Abs(3.0f * moduleSize - stateCount[2]) < 3 * maxVariance &&
                Math.Abs(moduleSize - stateCount[3]) < maxVariance &&
                Math.Abs(moduleSize - stateCount[4]) < maxVariance;
        }

        private int[] CrossCheckStateCount
        {
            get
            {
                clearCounts(crossCheckStateCount);
                return crossCheckStateCount;
            }
        }

        protected void clearCounts(int[] counts)
        {
            for (int x = 0; x < counts.Length; x++)
            {
                counts[x] = 0;
            }
        }

        protected void shiftCounts2(int[] stateCount)
        {
            stateCount[0] = stateCount[2];
            stateCount[1] = stateCount[3];
            stateCount[2] = stateCount[4];
            stateCount[3] = 1;
            stateCount[4] = 0;
        }

        /// <summary>
        /// After a vertical and horizontal scan finds a potential finder pattern, this method
        /// "cross-cross-cross-checks" by scanning down diagonally through the center of the possible
        /// finder pattern to see if the same proportion is detected.
        /// </summary>
        /// <param name="centerI">row where a finder pattern was detected</param>
        /// <param name="centerJ">center of the section that appears to cross a finder pattern</param>
        /// <returns>true if proportions are withing expected limits</returns>
        private bool crossCheckDiagonal(int centerI, int centerJ)
        {
            int[] stateCount = CrossCheckStateCount;

            // Start counting up, left from center finding black center mass
            int i = 0;
            while (centerI >= i && centerJ >= i && image[centerJ - i, centerI - i])
            {
                stateCount[2]++;
                i++;
            }
            if (stateCount[2] == 0)
            {
                return false;
            }

            // Continue up, left finding white space
            while (centerI >= i && centerJ >= i && !image[centerJ - i, centerI - i])
            {
                stateCount[1]++;
                i++;
            }
            if (stateCount[1] == 0)
            {
                return false;
            }

            // Continue up, left finding black border
            while (centerI >= i && centerJ >= i && image[centerJ - i, centerI - i])
            {
                stateCount[0]++;
                i++;
            }
            if (stateCount[0] == 0)
            {
                return false;
            }

            int maxI = image.Height;
            int maxJ = image.Width;

            // Now also count down, right from center
            i = 1;
            while (centerI + i < maxI && centerJ + i < maxJ && image[centerJ + i, centerI + i])
            {
                stateCount[2]++;
                i++;
            }

            while (centerI + i < maxI && centerJ + i < maxJ && !image[centerJ + i, centerI + i])
            {
                stateCount[3]++;
                i++;
            }
            if (stateCount[3] == 0)
            {
                return false;
            }

            while (centerI + i < maxI && centerJ + i < maxJ && image[centerJ + i, centerI + i])
            {
                stateCount[4]++;
                i++;
            }
            if (stateCount[4] == 0)
            {
                return false;
            }

            return foundPatternDiagonal(stateCount);
        }

        /// <summary>
        ///   <p>After a horizontal scan finds a potential finder pattern, this method
        /// "cross-checks" by scanning down vertically through the center of the possible
        /// finder pattern to see if the same proportion is detected.</p>
        /// </summary>
        /// <param name="startI">row where a finder pattern was detected</param>
        /// <param name="centerJ">center of the section that appears to cross a finder pattern</param>
        /// <param name="maxCount">maximum reasonable number of modules that should be
        /// observed in any reading state, based on the results of the horizontal scan</param>
        /// <param name="originalStateCountTotal">The original state count total.</param>
        /// <returns>
        /// vertical center of finder pattern, or null if not found
        /// </returns>
        private float? crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            int maxI = image.Height;
            int[] stateCount = CrossCheckStateCount;

            // Start counting up from center
            int i = startI;
            while (i >= 0 && image[centerJ, i])
            {
                stateCount[2]++;
                i--;
            }
            if (i < 0)
            {
                return null;
            }
            while (i >= 0 && !image[centerJ, i] && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                i--;
            }
            // If already too many modules in this state or ran off the edge:
            if (i < 0 || stateCount[1] > maxCount)
            {
                return null;
            }
            while (i >= 0 && image[centerJ, i] && stateCount[0] <= maxCount)
            {
                stateCount[0]++;
                i--;
            }
            if (stateCount[0] > maxCount)
            {
                return null;
            }

            // Now also count down from center
            i = startI + 1;
            while (i < maxI && image[centerJ, i])
            {
                stateCount[2]++;
                i++;
            }
            if (i == maxI)
            {
                return null;
            }
            while (i < maxI && !image[centerJ, i] && stateCount[3] < maxCount)
            {
                stateCount[3]++;
                i++;
            }
            if (i == maxI || stateCount[3] >= maxCount)
            {
                return null;
            }
            while (i < maxI && image[centerJ, i] && stateCount[4] < maxCount)
            {
                stateCount[4]++;
                i++;
            }
            if (stateCount[4] >= maxCount)
            {
                return null;
            }

            // If we found a finder-pattern-like section, but its size is more than 40% different than
            // the original, assume it's a false positive
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
            if (5 * Math.Abs(stateCountTotal - originalStateCountTotal) >= 2 * originalStateCountTotal)
            {
                return null;
            }

            return foundPatternCross(stateCount) ? centerFromEnd(stateCount, i) : null;
        }

        /// <summary> <p>Like {@link #crossCheckVertical(int, int, int, int)}, and in fact is basically identical,
        /// except it reads horizontally instead of vertically. This is used to cross-cross
        /// check a vertical cross check and locate the real center of the alignment pattern.</p>
        /// </summary>
        private float? crossCheckHorizontal(int startJ, int centerI, int maxCount, int originalStateCountTotal)
        {
            int maxJ = image.Width;
            int[] stateCount = CrossCheckStateCount;

            int j = startJ;
            while (j >= 0 && image[j, centerI])
            {
                stateCount[2]++;
                j--;
            }
            if (j < 0)
            {
                return null;
            }
            while (j >= 0 && !image[j, centerI] && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                j--;
            }
            if (j < 0 || stateCount[1] > maxCount)
            {
                return null;
            }
            while (j >= 0 && image[j, centerI] && stateCount[0] <= maxCount)
            {
                stateCount[0]++;
                j--;
            }
            if (stateCount[0] > maxCount)
            {
                return null;
            }

            j = startJ + 1;
            while (j < maxJ && image[j, centerI])
            {
                stateCount[2]++;
                j++;
            }
            if (j == maxJ)
            {
                return null;
            }
            while (j < maxJ && !image[j, centerI] && stateCount[3] < maxCount)
            {
                stateCount[3]++;
                j++;
            }
            if (j == maxJ || stateCount[3] >= maxCount)
            {
                return null;
            }
            while (j < maxJ && image[j, centerI] && stateCount[4] < maxCount)
            {
                stateCount[4]++;
                j++;
            }
            if (stateCount[4] >= maxCount)
            {
                return null;
            }

            // If we found a finder-pattern-like section, but its size is significantly different than
            // the original, assume it's a false positive
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
            if (5 * Math.Abs(stateCountTotal - originalStateCountTotal) >= originalStateCountTotal)
            {
                return null;
            }

            return foundPatternCross(stateCount) ? centerFromEnd(stateCount, j) : null;
        }

        /// <summary>
        /// @see #handlePossibleCenter(int[], int, int)
        /// </summary>
        /// <param name="stateCount">reading state module counts from horizontal scan</param>
        /// <param name="i">row where finder pattern may be found</param>
        /// <param name="j">end of possible finder pattern in row</param>
        /// <param name="pureBarcode">ignored</param>
        /// <returns>true if a finder pattern candidate was found this time</returns>
        [Obsolete("only exists for backwards compatibility")]
        protected bool handlePossibleCenter(int[] stateCount, int i, int j, bool pureBarcode)
        {
            return handlePossibleCenter(stateCount, i, j);
        }

        /// <summary>
        ///   <p>This is called when a horizontal scan finds a possible alignment pattern. It will
        /// cross check with a vertical scan, and if successful, will, ah, cross-cross-check
        /// with another horizontal scan. This is needed primarily to locate the real horizontal
        /// center of the pattern in cases of extreme skew.
        /// And then we cross-cross-cross check with another diagonal scan.</p>
        /// If that succeeds the finder pattern location is added to a list that tracks
        /// the number of times each location has been nearly-matched as a finder pattern.
        /// Each additional find is more evidence that the location is in fact a finder
        /// pattern center
        /// </summary>
        /// <param name="stateCount">reading state module counts from horizontal scan</param>
        /// <param name="i">row where finder pattern may be found</param>
        /// <param name="j">end of possible finder pattern in row</param>
        /// <returns>
        /// true if a finder pattern candidate was found this time
        /// </returns>
        protected bool handlePossibleCenter(int[] stateCount, int i, int j)
        {
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] +
                                  stateCount[4];
            float? centerJ = centerFromEnd(stateCount, j);
            if (centerJ == null)
                return false;
            float? centerI = crossCheckVertical(i, (int) centerJ.Value, stateCount[2], stateCountTotal);
            if (centerI != null)
            {
                // Re-cross check
                centerJ = crossCheckHorizontal((int) centerJ.Value, (int) centerI.Value, stateCount[2], stateCountTotal);
                if (centerJ != null && crossCheckDiagonal((int) centerI, (int) centerJ))
                {
                    float estimatedModuleSize = stateCountTotal / 7.0f;
                    bool found = false;
                    for (int index = 0; index < possibleCenters.Count; index++)
                    {
                        var center = possibleCenters[index];
                        // Look for about the same center and module size:
                        if (center.aboutEquals(estimatedModuleSize, centerI.Value, centerJ.Value))
                        {
                            possibleCenters.RemoveAt(index);
                            possibleCenters.Insert(index, center.combineEstimate(centerI.Value, centerJ.Value, estimatedModuleSize));

                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        var point = new FinderPattern(centerJ.Value, centerI.Value, estimatedModuleSize);

                        possibleCenters.Add(point);
                        if (resultPointCallback != null)
                        {

                            resultPointCallback(point);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <returns> number of rows we could safely skip during scanning, based on the first
        /// two finder patterns that have been located. In some cases their position will
        /// allow us to infer that the third pattern must lie below a certain point farther
        /// down in the image.
        /// </returns>
        private int findRowSkip()
        {
            int max = possibleCenters.Count;
            if (max <= 1)
            {
                return 0;
            }
            ResultPoint firstConfirmedCenter = null;
            foreach (var center in possibleCenters)
            {
                if (center.Count >= CENTER_QUORUM)
                {
                    if (firstConfirmedCenter == null)
                    {
                        firstConfirmedCenter = center;
                    }
                    else
                    {
                        // We have two confirmed centers
                        // How far down can we skip before resuming looking for the next
                        // pattern? In the worst case, only the difference between the
                        // difference in the x / y coordinates of the two centers.
                        // This is the case where you find top left last.
                        hasSkipped = true;
                        //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                        return (int) (Math.Abs(firstConfirmedCenter.X - center.X) - Math.Abs(firstConfirmedCenter.Y - center.Y)) / 2;
                    }
                }
            }
            return 0;
        }

        /// <returns> true iff we have found at least 3 finder patterns that have been detected
        /// at least {@link #CENTER_QUORUM} times each, and, the estimated module size of the
        /// candidates is "pretty similar"
        /// </returns>
        private bool haveMultiplyConfirmedCenters()
        {
            int confirmedCount = 0;
            float totalModuleSize = 0.0f;
            int max = possibleCenters.Count;
            foreach (var pattern in possibleCenters)
            {
                if (pattern.Count >= CENTER_QUORUM)
                {
                    confirmedCount++;
                    totalModuleSize += pattern.EstimatedModuleSize;
                }
            }
            if (confirmedCount < 3)
            {
                return false;
            }
            // OK, we have at least 3 confirmed centers, but, it's possible that one is a "false positive"
            // and that we need to keep looking. We detect this by asking if the estimated module sizes
            // vary too much. We arbitrarily say that when the total deviation from average exceeds
            // 5% of the total module size estimates, it's too much.
            float average = totalModuleSize / max;
            float totalDeviation = 0.0f;
            for (int i = 0; i < max; i++)
            {
                var pattern = possibleCenters[i];
                totalDeviation += Math.Abs(pattern.EstimatedModuleSize - average);
            }
            return totalDeviation <= 0.05f * totalModuleSize;
        }

        /// <summary>
        /// Get square of distance between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double squaredDistance(FinderPattern a, FinderPattern b)
        {
            double x = a.X - b.X;
            double y = a.Y - b.Y;
            return x * x + y * y;
        }

        /// <returns> the 3 best {@link FinderPattern}s from our list of candidates. The "best" are
        /// those have similar module size and form a shape closer to a isosceles right triangle.
        /// </returns>
        private FinderPattern[] selectBestPatterns()
        {
            int startSize = possibleCenters.Count;
            if (startSize < 3)
            {
                // Couldn't find enough finder patterns
                return null;
            }

            possibleCenters.Sort(moduleComparator);

            double distortion = Double.MaxValue;
            double[] squares = new double[3];
            FinderPattern[] bestPatterns = new FinderPattern[3];

            for (int i = 0; i < possibleCenters.Count - 2; i++)
            {
                FinderPattern fpi = possibleCenters[i];
                float minModuleSize = fpi.EstimatedModuleSize;

                for (int j = i + 1; j < possibleCenters.Count - 1; j++)
                {
                    FinderPattern fpj = possibleCenters[j];
                    double squares0 = squaredDistance(fpi, fpj);

                    for (int k = j + 1; k < possibleCenters.Count; k++)
                    {
                        FinderPattern fpk = possibleCenters[k];
                        float maxModuleSize = fpk.EstimatedModuleSize;
                        if (maxModuleSize > minModuleSize * 1.4f)
                        {
                            // module size is not similar
                            continue;
                        }

                        squares[0] = squares0;
                        squares[1] = squaredDistance(fpj, fpk);
                        squares[2] = squaredDistance(fpi, fpk);
                        Array.Sort(squares);

                        // a^2 + b^2 = c^2 (Pythagorean theorem), and a = b (isosceles triangle).
                        // Since any right triangle satisfies the formula c^2 - b^2 - a^2 = 0,
                        // we need to check both two equal sides separately.
                        // The value of |c^2 - 2 * b^2| + |c^2 - 2 * a^2| increases as dissimilarity
                        // from isosceles right triangle.
                        double d = Math.Abs(squares[2] - 2 * squares[1]) + Math.Abs(squares[2] - 2 * squares[0]);
                        if (d < distortion)
                        {
                            distortion = d;
                            bestPatterns[0] = fpi;
                            bestPatterns[1] = fpj;
                            bestPatterns[2] = fpk;
                        }
                    }
                }
            }

            if (distortion == Double.MaxValue)
            {
                return null;
            }

            return bestPatterns;
        }

        /// <summary>
        /// Orders by {@link FinderPatternFinder#getEstimatedModuleSize()}
        /// </summary>
        private sealed class EstimatedModuleComparator : IComparer<FinderPattern>
        {
            public int Compare(FinderPattern center1, FinderPattern center2)
            {
                if (center1.EstimatedModuleSize == center2.EstimatedModuleSize)
                    return 0;
                if (center1.EstimatedModuleSize < center2.EstimatedModuleSize)
                    return -1;
                return 1;
            }
        }
    }
}