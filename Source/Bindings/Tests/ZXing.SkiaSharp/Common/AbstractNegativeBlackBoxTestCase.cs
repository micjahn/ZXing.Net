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
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ZXing.SkiaSharp.Common.Test
{
    /// <summary>
    /// This abstract class looks for negative results, i.e. it only allows a certain number of false
    /// positives in images which should not decode. This helps ensure that we are not too lenient.
    ///
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    /// </summary>
    [TestFixture]
    public abstract class AbstractNegativeBlackBoxTestCase : SkiaBarcodeBlackBoxTestCase
    {
#if !SILVERLIGHT
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
      private static readonly DanielVaughan.Logging.ILog Log = DanielVaughan.Logging.LogManager.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

        private readonly List<TestResult> testResults;

        private class TestResult
        {
            private readonly int falsePositivesAllowed;
            private readonly float rotation;

            public TestResult(int falsePositivesAllowed, float rotation)
            {
                this.falsePositivesAllowed = falsePositivesAllowed;
                this.rotation = rotation;
            }

            public int getFalsePositivesAllowed()
            {
                return falsePositivesAllowed;
            }

            public float getRotation()
            {
                return rotation;
            }
        }

        // Use the multiformat reader to evaluate all decoders in the system.
        protected AbstractNegativeBlackBoxTestCase(String testBasePathSuffix)
           : base(testBasePathSuffix, null)
        {
            testResults = new List<TestResult>();
        }

        protected void addTest(int falsePositivesAllowed, float rotation)
        {
            testResults.Add(new TestResult(falsePositivesAllowed, rotation));
        }

        [Test]
        public new void testBlackBox()
        {
            Assert.IsFalse(testResults.Count == 0);

            var imageFiles = getImageFiles();
            int[] falsePositives = new int[testResults.Count];
            foreach (var testImage in imageFiles)
            {
                var absPath = Path.GetFullPath(testImage);
                Log.InfoFormat("Starting {0}", absPath);

                var image = openFromFile(testImage);

                for (int x = 0; x < testResults.Count; x++)
                {
                    TestResult testResult = testResults[x];
                    if (!checkForFalsePositives(image, testResult.getRotation()))
                    {
                        falsePositives[x]++;
                    }
                }
            }

            int totalFalsePositives = 0;
            int totalAllowed = 0;

            for (int x = 0; x < testResults.Count; x++)
            {
                TestResult testResult = testResults[x];
                totalFalsePositives += falsePositives[x];
                totalAllowed += testResult.getFalsePositivesAllowed();
            }

            if (totalFalsePositives < totalAllowed)
            {
                Log.InfoFormat("+++ Test too lax by {0} images", totalAllowed - totalFalsePositives);
            }
            else if (totalFalsePositives > totalAllowed)
            {
                Log.InfoFormat("--- Test failed by {0} images", totalFalsePositives - totalAllowed);
            }

            for (int x = 0; x < testResults.Count; x++)
            {
                TestResult testResult = testResults[x];
                Log.InfoFormat("Rotation {0} degrees: {1} of {2} images were false positives ({3} allowed)",
                               (int)testResult.getRotation(), falsePositives[x], imageFiles.Count(),
                               testResult.getFalsePositivesAllowed());
                Assert.IsTrue(falsePositives[x] <= testResult.getFalsePositivesAllowed(), "Rotation " + testResult.getRotation() + " degrees: Too many false positives found");
            }
        }
    }
}