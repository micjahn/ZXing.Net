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
using System.Globalization;
using System.IO;
using System.Linq;
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif
using NUnit.Framework;
using ZXing.Multi;

namespace ZXing.Common.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public abstract class AbstractBlackBoxTestCase
   {
#if !SILVERLIGHT
      private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
      private static readonly DanielVaughan.Logging.ILog Log = DanielVaughan.Logging.LogManager.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

      public bool accept(String dir, String name)
      {
         String lowerCase = name.ToLower(CultureInfo.InvariantCulture);
         return lowerCase.EndsWith(".jpg") || lowerCase.EndsWith(".jpeg") ||
                lowerCase.EndsWith(".gif") || lowerCase.EndsWith(".png");
      }

      private readonly string testBase;
      private readonly Reader barcodeReader;
      private readonly BarcodeFormat? expectedFormat;
      private readonly List<TestResult> testResults;

      public static String buildTestBase(String testBasePathSuffix)
      {
         // A little workaround to prevent aggravation in my IDE
         if (!Directory.Exists(testBasePathSuffix))
         {
            // try starting with 'core' since the test base is often given as the project root
            return Path.Combine("..\\..\\..\\Source", testBasePathSuffix);
         }
         return testBasePathSuffix;
      }

      protected AbstractBlackBoxTestCase(String testBasePathSuffix,
                                         Reader barcodeReader,
                                         BarcodeFormat? expectedFormat)
      {
         this.testBase = buildTestBase(testBasePathSuffix);
         this.barcodeReader = barcodeReader;
         this.expectedFormat = expectedFormat;
         testResults = new List<TestResult>();
      }

      protected void addTest(int mustPassCount, int tryHarderCount, float rotation)
      {
         addTest(mustPassCount, tryHarderCount, 0, 0, rotation);
      }

      /// <summary>
      /// Adds a new test for the current directory of images.
      ///
      /// <param name="mustPassCount">The number of images which must decode for the test to pass.</param>
      /// <param name="tryHarderCount">The number of images which must pass using the try harder flag.</param>
      /// <param name="maxMisreads">Maximum number of images which can fail due to successfully reading the wrong contents</param>
      /// <param name="maxTryHarderMisreads">Maximum number of images which can fail due to successfully</param>
      ///                             reading the wrong contents using the try harder flag
      /// <param name="rotation">The rotation in degrees clockwise to use for this test.</param>
      /// </summary>
      protected void addTest(int mustPassCount,
                             int tryHarderCount,
                             int maxMisreads,
                             int maxTryHarderMisreads,
                             float rotation)
      {
         testResults.Add(new TestResult(mustPassCount, tryHarderCount, maxMisreads, maxTryHarderMisreads, rotation));
      }

      protected IEnumerable<string> getImageFiles()
      {
         Log.Info(testBase);
         Log.Info(Environment.CurrentDirectory);
         Assert.IsTrue(Directory.Exists(testBase), "Please run from the 'core' directory");
         return Directory.EnumerateFiles(testBase).Where(p => accept(testBase, p));
      }

      protected Reader getReader()
      {
         return barcodeReader;
      }

      // This workaround is used because AbstractNegativeBlackBoxTestCase overrides this method but does
      // not return SummaryResults.
      [Test]
      virtual public void testBlackBox()
      {
         testBlackBoxCountingResults(true);
      }

      private void testBlackBoxCountingResults(bool assertOnFailure)
      {
         Assert.IsFalse(testResults.Count == 0);

         IEnumerable<string> imageFiles = getImageFiles();
         int testCount = testResults.Count;

         int[] passedCounts = new int[testCount];
         int[] misreadCounts = new int[testCount];
         int[] tryHarderCounts = new int[testCount];
         int[] tryHarderMisreadCounts = new int[testCount];

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

            String expectedText;
            String expectedTextFile = Path.Combine(Path.GetDirectoryName(absPath), Path.GetFileNameWithoutExtension(absPath) + ".txt");
            if (File.Exists(expectedTextFile))
            {
               expectedText = File.ReadAllText(expectedTextFile, System.Text.Encoding.UTF8);
            }
            else
            {
               String expectedBinFile = Path.Combine(Path.GetDirectoryName(absPath), Path.GetFileNameWithoutExtension(absPath) + ".bin");
               if (File.Exists(expectedBinFile))
               {
                  // it is only a dirty workaround for some special cases
                  expectedText = File.ReadAllText(expectedBinFile, System.Text.Encoding.GetEncoding("ISO8859-1"));
               }
               else
               {
                  throw new InvalidOperationException("Missing expected result file: " + expectedTextFile);
               }
            }

            String expectedMetadataFile = Path.Combine(Path.GetDirectoryName(absPath), Path.GetFileNameWithoutExtension(absPath) + ".metadata.txt");
            var expectedMetadata = new Dictionary<string, string>();
            if (File.Exists(expectedMetadataFile))
            {
               foreach (var row in File.ReadLines(expectedMetadataFile))
                  expectedMetadata.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }

            for (int x = 0; x < testCount; x++)
            {
               var testResult = testResults[x];
               float rotation = testResult.Rotation;
               var rotatedImage = rotateImage(image, rotation);
               LuminanceSource source = new BitmapLuminanceSource(rotatedImage);
               BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
               try
               {
                  if (decode(bitmap, rotation, expectedText, expectedMetadata, false))
                  {
                     passedCounts[x]++;
                     Log.Info("   without try-hard ... ok.");
                  }
                  else
                  {
                     misreadCounts[x]++;
                     Log.Info("   without try-hard ... fail.");
                  }
               }
               catch (ReaderException )
               {
                  // continue
                  Log.Info("   without try-hard ... fail (exc).");
               }
               try
               {
                  if (decode(bitmap, rotation, expectedText, expectedMetadata, true))
                  {
                     tryHarderCounts[x]++;
                     Log.Info("   with try-hard ... ok.");
                  }
                  else
                  {
                     tryHarderMisreadCounts[x]++;
                     Log.Info("   with try-hard ... fail.");
                  }
               }
               catch (ReaderException )
               {
                  // continue
                  Log.Info("   with try-hard ... fail (exc).");
               }
            }
         }

         // Print the results of all tests first
         int totalFound = 0;
         int totalMustPass = 0;
         int totalMisread = 0;
         int totalMaxMisread = 0;
         var imageFilesCount = imageFiles.Count();
         for (int x = 0; x < testResults.Count; x++)
         {
            TestResult testResult = testResults[x];
            Log.InfoFormat("Rotation {0} degrees:", (int)testResult.Rotation);
            Log.InfoFormat(" {0} of {1} images passed ({2} required)",
                              passedCounts[x], imageFilesCount, testResult.MustPassCount);
            int failed = imageFilesCount - passedCounts[x];
            Log.InfoFormat(" {0} failed due to misreads, {1} not detected",
                              misreadCounts[x], failed - misreadCounts[x]);
            Log.InfoFormat(" {0} of {1} images passed with try harder ({2} required)",
                              tryHarderCounts[x], imageFilesCount, testResult.TryHarderCount);
            failed = imageFilesCount - tryHarderCounts[x];
            Log.InfoFormat(" {0} failed due to misreads, {1} not detected",
                              tryHarderMisreadCounts[x], failed - tryHarderMisreadCounts[x]);
            totalFound += passedCounts[x] + tryHarderCounts[x];
            totalMustPass += testResult.MustPassCount + testResult.TryHarderCount;
            totalMisread += misreadCounts[x] + tryHarderMisreadCounts[x];
            totalMaxMisread += testResult.MaxMisreads + testResult.MaxTryHarderMisreads;
         }

         int totalTests = imageFilesCount * testCount * 2;
         Log.InfoFormat("Decoded {0} images out of {1} ({2}%, {3} required)",
                           totalFound, totalTests, totalFound * 100 / totalTests, totalMustPass);
         if (totalFound > totalMustPass)
         {
            Log.WarnFormat("+++ Test too lax by {0} images", totalFound - totalMustPass);
         }
         else if (totalFound < totalMustPass)
         {
            Log.WarnFormat("--- Test failed by {0} images", totalMustPass - totalFound);
         }

         if (totalMisread < totalMaxMisread)
         {
            Log.WarnFormat("+++ Test expects too many misreads by {0} images", totalMaxMisread - totalMisread);
         }
         else if (totalMisread > totalMaxMisread)
         {
            Log.WarnFormat("--- Test had too many misreads by {0} images", totalMisread - totalMaxMisread);
         }

         // Then run through again and assert if any failed
         if (assertOnFailure)
         {
            for (int x = 0; x < testCount; x++)
            {
               TestResult testResult = testResults[x];
               String label = "Rotation " + testResult.Rotation + " degrees: Too many images failed";
               Assert.IsTrue(passedCounts[x] >= testResult.MustPassCount, label);
               Assert.IsTrue(tryHarderCounts[x] >= testResult.TryHarderCount, "Try harder, " + label);
               label = "Rotation " + testResult.Rotation + " degrees: Too many images misread";
               Assert.IsTrue(misreadCounts[x] <= testResult.MaxMisreads, label);
               Assert.IsTrue(tryHarderMisreadCounts[x] <= testResult.MaxTryHarderMisreads, "Try harder, " + label);
            }
         }
      }

      private bool decode(BinaryBitmap source,
                             float rotation,
                             String expectedText,
                             IDictionary<string, string> expectedMetadata,
                             bool tryHarder)
      {

         String suffix = String.Format(" ({0}rotation: {1})", tryHarder ? "try harder, " : "", (int)rotation);

         IDictionary<DecodeHintType, Object> hints = new Dictionary<DecodeHintType, Object>();
         if (tryHarder)
         {
            hints[DecodeHintType.TRY_HARDER] = true;
         }

         // Try in 'pure' mode mostly to exercise PURE_BARCODE code paths for exceptions;
         // not expected to pass, generally
         Result result = null;
         try
         {
            var pureHints = new Dictionary<DecodeHintType, object>();
            pureHints[DecodeHintType.PURE_BARCODE] = true;
            result = barcodeReader.decode(source, pureHints);
         }
         catch (ReaderException re)
         {
            // continue
         }

         var multiReader = barcodeReader as MultipleBarcodeReader;
         if (multiReader != null)
         {
            var expectedResults = expectedText.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var results = multiReader.decodeMultiple(source, hints);
            if (results == null)
               throw new ReaderException();

            if (expectedResults.Length != results.Length)
            {
               Log.InfoFormat("Count mismatch: expected '{0}' results but got '{1}'",
                  expectedResults.Length, results.Length);
               throw new ReaderException();
            }
            foreach (var oneResult in results)
            {
               if (expectedFormat != oneResult.BarcodeFormat)
               {
                  Log.InfoFormat("Format mismatch: expected '{0}' but got '{1}'{2}",
                     expectedFormat, oneResult.BarcodeFormat, suffix);
                  return false;
               }
               String resultText = oneResult.Text;
               bool found = false;
               foreach (var expectedResult in expectedResults)
               {
                  if (expectedResult.Equals(resultText))
                  {
                     found = true;
                     break;
                  }
               }
               if (!found)
               {
                  Log.InfoFormat("Content was not expected: '{0}'", resultText);
                  return false;
               }
            }
            foreach (var expectedResult in expectedResults)
            {
               bool found = false;
               foreach (var oneResult in results)
               {
                  String resultText = oneResult.Text;
                  if (expectedResult.Equals(resultText))
                  {
                     found = true;
                     break;
                  }
               }
               if (!found)
               {
                  Log.InfoFormat("Content was expected but not found: '{0}'", expectedResult);
                  return false;
               }
            }
         }
         else
         {
            if (result == null)
               result = barcodeReader.decode(source, hints);
            if (result == null)
               throw new ReaderException();

            if (expectedFormat != result.BarcodeFormat)
            {
               Log.InfoFormat("Format mismatch: expected '{0}' but got '{1}'{2}",
                  expectedFormat, result.BarcodeFormat, suffix);
               return false;
            }

            String resultText = result.Text;
            if (!expectedText.Equals(resultText))
            {
               Log.InfoFormat("Content mismatch: expected '{0}' but got '{1}'{2}",
                  expectedText, resultText, suffix);
               return false;
            }

            IDictionary<ResultMetadataType, object> resultMetadata = result.ResultMetadata;
            foreach (var metadatum in expectedMetadata)
            {
               ResultMetadataType key;
               ResultMetadataType.TryParse(metadatum.Key, out key);
               Object expectedValue = metadatum.Value;
               Object actualValue = resultMetadata == null ? null : resultMetadata[key];
               if (!expectedValue.Equals(actualValue))
               {
                  Log.InfoFormat("Metadata mismatch for key '{0}': expected '{1}' but got '{2}'",
                     key, expectedValue, actualValue);
                  return false;
               }
            }
         }

         return true;
      }

#if !SILVERLIGHT
      protected static Bitmap rotateImage(Bitmap original, float degrees)
      {
         if (degrees == 0.0f)
         {
            return original;
         }

         RotateFlipType rotate;
         switch ((int)degrees)
         {
            case 90:
               rotate = RotateFlipType.Rotate90FlipNone;
               break;
            case 180:
               rotate = RotateFlipType.Rotate180FlipNone;
               break;
            case 270:
               rotate = RotateFlipType.Rotate270FlipNone;
               break;
            default:
               throw new NotSupportedException();

         }
         var newRotated = (Bitmap)original.Clone();
         newRotated.RotateFlip(rotate);
         return newRotated;
      }
#else
      protected static WriteableBitmap rotateImage(WriteableBitmap original, float degrees)
      {
         if (degrees == 0.0f)
         {
            return original;
         }

         int width = original.PixelWidth;
         int height = original.PixelHeight;
         int full = Math.Max(width, height);

         Image tempImage2 = new Image();
         tempImage2.Width = full;
         tempImage2.Height = full;
         tempImage2.Source = original;

         // New bitmap has swapped width/height
         WriteableBitmap newRotated = new WriteableBitmap(height, width);


         TransformGroup transformGroup = new TransformGroup();

         // Rotate around centre
         RotateTransform rotate = new RotateTransform();
         rotate.Angle = degrees;
         rotate.CenterX = full / 2;
         rotate.CenterY = full / 2;
         transformGroup.Children.Add(rotate);

         // and transform back to top left corner of new image
         TranslateTransform translate = new TranslateTransform();
         translate.X = -(full - height) / 2;
         translate.Y = -(full - width) / 2;
         transformGroup.Children.Add(translate);

         newRotated.Render(tempImage2, transformGroup);
         newRotated.Invalidate();

         return newRotated;
      }
#endif
   }
}
