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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

using NUnit.Framework;
using ZXing.Test;

namespace ZXing.Common.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public abstract class AbstractBlackBoxTestCase
   {
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

      protected AbstractBlackBoxTestCase(String testBasePathSuffix,
                                         Reader barcodeReader,
                                         BarcodeFormat? expectedFormat)
      {
         // A little workaround to prevent aggravation in my IDE
         if (!Directory.Exists(testBasePathSuffix))
         {
            // try starting with 'core' since the test base is often given as the project root
            testBasePathSuffix = Path.Combine("..\\..\\..\\Source", testBasePathSuffix);
         }
         this.testBase = testBasePathSuffix;
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
         Console.WriteLine(testBase);
         Console.WriteLine(Environment.CurrentDirectory);
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
      public void testBlackBox()
      {
         testBlackBoxCountingResults(true);
      }

      public SummaryResults testBlackBoxCountingResults(bool assertOnFailure)
      {
         Assert.IsFalse(testResults.Count == 0);

         IEnumerable<string> imageFiles = getImageFiles();
         int testCount = testResults.Count;

         int[] passedCounts = new int[testCount];
         int[] misreadCounts = new int[testCount];
         int[] tryHarderCounts = new int[testCount];
         int[] tryHaderMisreadCounts = new int[testCount];

         foreach (var testImage in imageFiles)
         {
            var absPath = Path.GetFullPath(testImage);
            Console.WriteLine("Starting {0}\n", absPath);

            var image = new Bitmap(Image.FromFile(testImage));

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
                  expectedText = File.ReadAllText(expectedTextFile, System.Text.Encoding.UTF7);
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
               foreach (var row in File.ReadAllLines(expectedMetadataFile))
                  expectedMetadata.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }

            for (int x = 0; x < testCount; x++)
            {
               var testResult = testResults[x];
               float rotation = testResult.Rotation;
               Bitmap rotatedImage = rotateImage(image, rotation);
               LuminanceSource source = new BufferedImageLuminanceSource(rotatedImage);
               BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
               try
               {
                  if (decode(bitmap, rotation, expectedText, expectedMetadata, false))
                  {
                     passedCounts[x]++;
                  }
                  else
                  {
                     misreadCounts[x]++;
                  }
               }
               catch (ReaderException re)
               {
                  // continue
               }
               try
               {
                  if (decode(bitmap, rotation, expectedText, expectedMetadata, true))
                  {
                     tryHarderCounts[x]++;
                  }
                  else
                  {
                     tryHaderMisreadCounts[x]++;
                  }
               }
               catch (ReaderException re)
               {
                  // continue
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
            Console.WriteLine("Rotation {0} degrees:\n", (int)testResult.Rotation);
            Console.WriteLine("  {0} of {1} images passed ({2} required)\n",
                              passedCounts[x], imageFilesCount, testResult.MustPassCount);
            int failed = imageFilesCount - passedCounts[x];
            Console.WriteLine("    {0} failed due to misreads, {1} not detected\n",
                              misreadCounts[x], failed - misreadCounts[x]);
            Console.WriteLine("  {0} of {1} images passed with try harder ({2} required)\n",
                              tryHarderCounts[x], imageFilesCount, testResult.TryHarderCount);
            failed = imageFilesCount - tryHarderCounts[x];
            Console.WriteLine("    {0} failed due to misreads, {1} not detected\n",
                              tryHaderMisreadCounts[x], failed - tryHaderMisreadCounts[x]);
            totalFound += passedCounts[x] + tryHarderCounts[x];
            totalMustPass += testResult.MustPassCount + testResult.TryHarderCount;
            totalMisread += misreadCounts[x] + tryHaderMisreadCounts[x];
            totalMaxMisread += testResult.MaxMisreads + testResult.MaxTryHarderMisreads;
         }

         int totalTests = imageFilesCount * testCount * 2;
         Console.WriteLine("TOTALS:\nDecoded {0} images out of {1} ({2}%, {3} required)\n",
                           totalFound, totalTests, totalFound * 100 / totalTests, totalMustPass);
         if (totalFound > totalMustPass)
         {
            Console.WriteLine("  +++ Test too lax by {0} images\n", totalFound - totalMustPass);
         }
         else if (totalFound < totalMustPass)
         {
            Console.WriteLine("  --- Test failed by {0} images\n", totalMustPass - totalFound);
         }

         if (totalMisread < totalMaxMisread)
         {
            Console.WriteLine("  +++ Test expects too many misreads by {0} images\n", totalMaxMisread - totalMisread);
         }
         else if (totalMisread > totalMaxMisread)
         {
            Console.WriteLine("  --- Test had too many misreads by {0} images\n", totalMisread - totalMaxMisread);
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
               Assert.IsTrue(tryHaderMisreadCounts[x] <= testResult.MaxTryHarderMisreads, "Try harder, " + label);
            }
         }
         return new SummaryResults(totalFound, totalMustPass, totalTests);
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

         Result result = barcodeReader.decode(source, hints);
         if (result == null)
            throw ReaderException.Instance;

         if (expectedFormat != result.BarcodeFormat)
         {
            Console.WriteLine("Format mismatch: expected '{0}' but got '{1}'{2}\n",
                              expectedFormat, result.BarcodeFormat, suffix);
            return false;
         }

         String resultText = result.Text;
         if (!expectedText.Equals(resultText))
         {
            Console.WriteLine("Content mismatch: expected '{0}' but got '{1}'{2}\n",
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
               Console.WriteLine("Metadata mismatch for key '{0}': expected '{1}' but got '{2}'\n",
                                key, expectedValue, actualValue);
               return false;
            }
         }

         return true;
      }

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

         //double radians = Math.toRadians(degrees);

         //// Transform simply to find out the new bounding box (don't actually run the image through it)
         //AffineTransform at = new AffineTransform();
         //at.rotate(radians, original.getWidth() / 2.0, original.getHeight() / 2.0);
         //BufferedImageOp op = new AffineTransformOp(at, AffineTransformOp.TYPE_BICUBIC);

         //Rectangle2D r = op.getBounds2D(original);
         //int width = (int)Math.ceil(r.getWidth());
         //int height = (int)Math.ceil(r.getHeight());

         //// Real transform, now that we know the size of the new image and how to translate after we rotate
         //// to keep it centered
         //at = new AffineTransform();
         //at.rotate(radians, width / 2.0, height / 2.0);
         //at.translate((width - original.getWidth()) / 2.0,
         //             (height - original.getHeight()) / 2.0);
         //op = new AffineTransformOp(at, AffineTransformOp.TYPE_BICUBIC);

         //return op.filter(original, null);
      }
   }
}
