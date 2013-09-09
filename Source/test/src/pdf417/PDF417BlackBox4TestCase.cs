/*
 * Copyright 2013 ZXing authors
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
using System.IO;
using System.Text;

using NUnit.Framework;

using ZXing.Common;
using ZXing.Common.Test;

namespace ZXing.PDF417.Test
{
   /// <summary>
   /// This class tests Macro PDF417 barcode specific functionality. It ensures that information, which is split into
   /// several barcodes can be properly combined again to yield the original data content.
   /// @author Guenther Grau
   /// </summary>
   [TestFixture]
   public sealed class PDF417BlackBox4TestCase : AbstractBlackBoxTestCase
   {
#if !SILVERLIGHT
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
      private static readonly DanielVaughan.Logging.ILog log = DanielVaughan.Logging.LogManager.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

      private static readonly Encoding UTF8 = Encoding.UTF8;
      private static readonly Encoding ISO88591 = Encoding.GetEncoding("ISO8859-1");
      private const String TEST_BASE_PATH_SUFFIX = "test/data/blackbox/pdf417-4";
      private readonly PDF417Reader barcodeReader = new PDF417Reader();

      private readonly List<TestResult> testResults = new List<TestResult>();
      private String testBase;

      public PDF417BlackBox4TestCase()
         : base(TEST_BASE_PATH_SUFFIX, null, BarcodeFormat.PDF_417)
      {
         // A little workaround to prevent aggravation in my IDE
         if (!Directory.Exists(TEST_BASE_PATH_SUFFIX))
         {
            // try starting with 'core' since the test base is often given as the project root
            testBase = Path.Combine("..\\..\\..\\Source", TEST_BASE_PATH_SUFFIX);
         }
         else
         {
            testBase = TEST_BASE_PATH_SUFFIX;
         }
         testResults.Add(new TestResult(2, 2, 0, 0, 0.0f));
      }

      [Test]
      public override void testBlackBox()
      {
         testPDF417BlackBoxCountingResults(true);
      }

      public SummaryResults testPDF417BlackBoxCountingResults(bool assertOnFailure)
      {
         Assert.IsFalse(testResults.Count == 0);

         IDictionary<String, List<String>> imageFiles = getImageFileLists();
         int testCount = testResults.Count;

         int[] passedCounts = new int[testCount];
         int[] misreadCounts = new int[testCount];
         int[] tryHarderCounts = new int[testCount];
         int[] tryHaderMisreadCounts = new int[testCount];

         foreach (KeyValuePair<String, List<String>> testImageGroup in imageFiles)
         {
            log.InfoFormat("Starting Image Group {0}", testImageGroup.Key);

            String fileBaseName = testImageGroup.Key;
            String expectedText;
            String expectedTextFile = fileBaseName + ".txt";
            if (File.Exists(expectedTextFile))
            {
               expectedText = File.ReadAllText(expectedTextFile, UTF8);
            }
            else
            {
               expectedTextFile = fileBaseName + ".bin";
               Assert.IsTrue(File.Exists(expectedTextFile));
               expectedText = File.ReadAllText(expectedTextFile, ISO88591);
            }

            for (int x = 0; x < testCount; x++)
            {
               List<Result> results = new List<Result>();
               foreach (var imageFile in testImageGroup.Value)
               {
#if !SILVERLIGHT
                  var image = new Bitmap(Image.FromFile(imageFile));
#else
            var image = new WriteableBitmap(0, 0);
            image.SetSource(File.OpenRead(testImage));
#endif
                  var rotation = testResults[x].Rotation;
                  var rotatedImage = rotateImage(image, rotation);
                  var source = new BitmapLuminanceSource(rotatedImage);
                  var bitmap = new BinaryBitmap(new HybridBinarizer(source));

                  try
                  {
                     results.AddRange(decode(bitmap, false));
                  }
                  catch (ReaderException ignored)
                  {
                     // ignore
                  }
               }
               results.Sort((arg0, arg1) =>
                  {
                     PDF417ResultMetadata resultMetadata = getMeta(arg0);
                     PDF417ResultMetadata otherResultMetadata = getMeta(arg1);
                     return resultMetadata.SegmentIndex - otherResultMetadata.SegmentIndex;
                  });
               var resultText = new StringBuilder();
               String fileId = null;
               foreach (Result result in results)
               {
                  PDF417ResultMetadata resultMetadata = getMeta(result);
                  Assert.NotNull(resultMetadata, "resultMetadata");
                  if (fileId == null)
                  {
                     fileId = resultMetadata.FileId;
                  }
                  Assert.AreEqual(fileId, resultMetadata.FileId, "FileId");
                  resultText.Append(result.Text);
               }
               Assert.AreEqual(expectedText, resultText.ToString(), "ExpectedText");
               passedCounts[x]++;
               tryHarderCounts[x]++;
            }
         }

         // Print the results of all tests first
         int totalFound = 0;
         int totalMustPass = 0;
         int totalMisread = 0;
         int totalMaxMisread = 0;

         int numberOfTests = imageFiles.Count;
         for (int x = 0; x < testResults.Count; x++)
         {
            TestResult testResult = testResults[x];
            log.InfoFormat("Rotation {0} degrees:", (int) testResult.Rotation);
            log.InfoFormat(" {0} of {1} images passed ({2} required)", passedCounts[x], numberOfTests,
                           testResult.MustPassCount);
            int failed = numberOfTests - passedCounts[x];
            log.InfoFormat(" {0} failed due to misreads, {1} not detected", misreadCounts[x], failed - misreadCounts[x]);
            log.InfoFormat(" {0} of {1} images passed with try harder ({2} required)", tryHarderCounts[x],
                           numberOfTests, testResult.TryHarderCount);
            failed = numberOfTests - tryHarderCounts[x];
            log.InfoFormat(" {0} failed due to misreads, {1} not detected", tryHaderMisreadCounts[x], failed -
                                                                                                      tryHaderMisreadCounts[x]);
            totalFound += passedCounts[x] + tryHarderCounts[x];
            totalMustPass += testResult.MustPassCount + testResult.TryHarderCount;
            totalMisread += misreadCounts[x] + tryHaderMisreadCounts[x];
            totalMaxMisread += testResult.MaxMisreads + testResult.MaxTryHarderMisreads;
         }

         int totalTests = numberOfTests*testCount*2;
         log.InfoFormat("Decoded {0} images out of {1} ({2}%, {3} required)", totalFound, totalTests, totalFound*
                                                                                                      100/
                                                                                                      totalTests, totalMustPass);
         if (totalFound > totalMustPass)
         {
            log.WarnFormat("+++ Test too lax by {0} images", totalFound - totalMustPass);
         }
         else if (totalFound < totalMustPass)
         {
            log.WarnFormat("--- Test failed by {0} images", totalMustPass - totalFound);
         }

         if (totalMisread < totalMaxMisread)
         {
            log.WarnFormat("+++ Test expects too many misreads by {0} images", totalMaxMisread - totalMisread);
         }
         else if (totalMisread > totalMaxMisread)
         {
            log.WarnFormat("--- Test had too many misreads by {0} images", totalMisread - totalMaxMisread);
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

      private static PDF417ResultMetadata getMeta(Result result)
      {
         return result.ResultMetadata == null ? null : (PDF417ResultMetadata) result.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA];
      }

      private Result[] decode(BinaryBitmap source, bool tryHarder)
      {
         IDictionary<DecodeHintType, Object> hints = new Dictionary<DecodeHintType, object>();
         if (tryHarder)
         {
            hints[DecodeHintType.TRY_HARDER] = true;
         }

         return barcodeReader.decodeMultiple(source, hints);
      }

      private IDictionary<String, List<String>> getImageFileLists()
      {
         IDictionary<String, List<String>> result = new Dictionary<string, List<String>>();
         foreach (string fileName in getImageFiles())
         {
            String testImageFileName = fileName;
            String fileBaseName = testImageFileName.Substring(0, testImageFileName.LastIndexOf('-'));
            List<String> files;
            if (!result.ContainsKey(fileBaseName))
            {
               files = new List<String>();
               result[fileBaseName] = files;
            }
            else
            {
               files = result[fileBaseName];
            }
            files.Add(fileName);
         }
         return result;
      }
   }
}
