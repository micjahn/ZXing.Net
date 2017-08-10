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
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif
using NUnit.Framework;

namespace ZXing.Common.Test
{
   /// <summary>
   /// This abstract class looks for negative results, i.e. it only allows a certain number of false
   /// positives in images which should not decode. This helps ensure that we are not too lenient.
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   [TestFixture]
   public abstract class AbstractNegativeBlackBoxTestCase : AbstractBlackBoxTestCase
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
         : base(testBasePathSuffix, new MultiFormatReader(), null)
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

#if !SILVERLIGHT
            var image = new Bitmap(Image.FromFile(testImage));
#else
            var image = new WriteableBitmap(0, 0);
            image.SetSource(File.OpenRead(testImage));
#endif
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
                           (int) testResult.getRotation(), falsePositives[x], imageFiles.Count(),
                           testResult.getFalsePositivesAllowed());
            Assert.IsTrue(falsePositives[x] <= testResult.getFalsePositivesAllowed(), "Rotation " + testResult.getRotation() + " degrees: Too many false positives found");
         }
      }

      /// <summary>
      /// Make sure ZXing does NOT find a barcode in the image.
      ///
      /// <param name="image">The image to test</param>
      /// <param name="rotationInDegrees">The amount of rotation to apply</param>
      /// <returns>true if nothing found, false if a non-existent barcode was detected</returns>
      /// </summary>
#if !SILVERLIGHT
      private bool checkForFalsePositives(Bitmap image, float rotationInDegrees)
#else
      private bool checkForFalsePositives(WriteableBitmap image, float rotationInDegrees)
#endif
      {
         var rotatedImage = rotateImage(image, rotationInDegrees);
         var source = new BitmapLuminanceSource(rotatedImage);
         var bitmap = new BinaryBitmap(new HybridBinarizer(source));
         var result = getReader().decode(bitmap);
         if (result != null)
         {
            Log.InfoFormat("Found false positive: '{0}' with format '{1}' (rotation: {2})",
                           result.Text, result.BarcodeFormat, (int) rotationInDegrees);
            return false;
         }

         // Try "try harder" getMode
         var hints = new Dictionary<DecodeHintType, Object>();
         hints[DecodeHintType.TRY_HARDER] = true;
         result = getReader().decode(bitmap, hints);
         if (result != null)
         {
            Log.InfoFormat("Try harder found false positive: '{0}' with format '{1}' (rotation: {2})",
                           result.Text, result.BarcodeFormat, (int) rotationInDegrees);
            return false;
         }
         return true;
      }
   }
}