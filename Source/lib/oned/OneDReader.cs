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
using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// Encapsulates functionality and implementation that is common to all families
   /// of one-dimensional barcodes.
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// <author>Sean Owen</author>
   /// </summary>
   public abstract class OneDReader : Reader
   {
      /// <summary>
      /// 
      /// </summary>
      protected static int INTEGER_MATH_SHIFT = 8;
      /// <summary>
      /// 
      /// </summary>
      protected static int PATTERN_MATCH_RESULT_SCALE_FACTOR = 1 << INTEGER_MATH_SHIFT;

      /// <summary>
      /// Locates and decodes a barcode in some format within an image.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      /// Locates and decodes a barcode in some format within an image. This method also accepts
      /// hints, each possibly associated to some data, which may help the implementation decode.
      /// Note that we don't try rotation without the try harder flag, even if rotation was supported.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <param name="hints">passed as a <see cref="IDictionary{TKey, TValue}"/> from <see cref="DecodeHintType"/>
      /// to arbitrary data. The
      /// meaning of the data depends upon the hint type. The implementation may or may not do
      /// anything with these hints.</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      virtual public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         var result = doDecode(image, hints);
         if (result == null)
         {
            bool tryHarder = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
            bool tryHarderWithoutRotation = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER_WITHOUT_ROTATION);
            if (tryHarder && !tryHarderWithoutRotation && image.RotateSupported)
            {
               BinaryBitmap rotatedImage = image.rotateCounterClockwise();
               result = doDecode(rotatedImage, hints);
               if (result == null)
                  return null;
               // Record that we found it rotated 90 degrees CCW / 270 degrees CW
               IDictionary<ResultMetadataType, object> metadata = result.ResultMetadata;
               int orientation = 270;
               if (metadata != null && metadata.ContainsKey(ResultMetadataType.ORIENTATION))
               {
                  // But if we found it reversed in doDecode(), add in that result here:
                  orientation = (orientation +
                                 (int) metadata[ResultMetadataType.ORIENTATION])%360;
               }
               result.putMetadata(ResultMetadataType.ORIENTATION, orientation);
               // Update result points
               ResultPoint[] points = result.ResultPoints;
               if (points != null)
               {
                  int height = rotatedImage.Height;
                  for (int i = 0; i < points.Length; i++)
                  {
                     points[i] = new ResultPoint(height - points[i].Y - 1, points[i].X);
                  }
               }
            }
         }
         return result;
      }

      /// <summary>
      /// Resets any internal state the implementation has after a decode, to prepare it
      /// for reuse.
      /// </summary>
      virtual public void reset()
      {
         // do nothing
      }

      /// <summary>
      /// We're going to examine rows from the middle outward, searching alternately above and below the
      /// middle, and farther out each time. rowStep is the number of rows between each successive
      /// attempt above and below the middle. So we'd scan row middle, then middle - rowStep, then
      /// middle + rowStep, then middle - (2 * rowStep), etc.
      /// rowStep is bigger as the image is taller, but is always at least 1. We've somewhat arbitrarily
      /// decided that moving up and down by about 1/16 of the image is pretty good; we try more of the
      /// image if "trying harder".
      /// </summary>
      /// <param name="image">The image to decode</param>
      /// <param name="hints">Any hints that were requested</param>
      /// <returns>The contents of the decoded barcode</returns>
      virtual protected Result doDecode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         int width = image.Width;
         int height = image.Height;
         BitArray row = new BitArray(width);

         int middle = height >> 1;
         bool tryHarder = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
         int rowStep = Math.Max(1, height >> (tryHarder ? 8 : 5));
         int maxLines;
         if (tryHarder)
         {
            maxLines = height; // Look at the whole image, not just the center
         }
         else
         {
            maxLines = 15; // 15 rows spaced 1/32 apart is roughly the middle half of the image
         }

         for (int x = 0; x < maxLines; x++)
         {

            // Scanning from the middle out. Determine which row we're looking at next:
            int rowStepsAboveOrBelow = (x + 1) >> 1;
            bool isAbove = (x & 0x01) == 0; // i.e. is x even?
            int rowNumber = middle + rowStep * (isAbove ? rowStepsAboveOrBelow : -rowStepsAboveOrBelow);
            if (rowNumber < 0 || rowNumber >= height)
            {
               // Oops, if we run off the top or bottom, stop
               break;
            }

            // Estimate black point for this row and load it:
            row = image.getBlackRow(rowNumber, row);
            if (row == null)
               continue;

            // While we have the image data in a BitArray, it's fairly cheap to reverse it in place to
            // handle decoding upside down barcodes.
            for (int attempt = 0; attempt < 2; attempt++)
            {
               if (attempt == 1)
               { 
                  // trying again?
                  row.reverse(); // reverse the row and continue
                  // This means we will only ever draw result points *once* in the life of this method
                  // since we want to avoid drawing the wrong points after flipping the row, and,
                  // don't want to clutter with noise from every single row scan -- just the scans
                  // that start on the center line.
                  if (hints != null && hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
                  {
                     IDictionary<DecodeHintType, Object> newHints = new Dictionary<DecodeHintType, Object>();
                     foreach (var hint in hints)
                     {
                        if (hint.Key != DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                           newHints.Add(hint.Key, hint.Value);
                     }
                     hints = newHints;
                  }
               }
               // Look for a barcode
               Result result = decodeRow(rowNumber, row, hints);
               if (result == null)
                  continue;

               // We found our barcode
               if (attempt == 1)
               {
                  // But it was upside down, so note that
                  result.putMetadata(ResultMetadataType.ORIENTATION, 180);
                  // And remember to flip the result points horizontally.
                  ResultPoint[] points = result.ResultPoints;
                  if (points != null)
                  {
                     points[0] = new ResultPoint(width - points[0].X - 1, points[0].Y);
                     points[1] = new ResultPoint(width - points[1].X - 1, points[1].Y);
                  }
               }
               return result;
            }
         }

         return null;
      }

      /// <summary>
      /// Records the size of successive runs of white and black pixels in a row, starting at a given point.
      /// The values are recorded in the given array, and the number of runs recorded is equal to the size
      /// of the array. If the row starts on a white pixel at the given start point, then the first count
      /// recorded is the run of white pixels starting from that point; likewise it is the count of a run
      /// of black pixels if the row begin on a black pixels at that point.
      /// </summary>
      /// <param name="row">row to count from</param>
      /// <param name="start">offset into row to start at</param>
      /// <param name="counters">array into which to record counts</param>
      protected static bool recordPattern(BitArray row,
                                          int start,
                                          int[] counters)
      {
         return recordPattern(row, start, counters, counters.Length);
      }

      /// <summary>
      /// Records the size of successive runs of white and black pixels in a row, starting at a given point.
      /// The values are recorded in the given array, and the number of runs recorded is equal to the size
      /// of the array. If the row starts on a white pixel at the given start point, then the first count
      /// recorded is the run of white pixels starting from that point; likewise it is the count of a run
      /// of black pixels if the row begin on a black pixels at that point.
      /// </summary>
      /// <param name="row">row to count from</param>
      /// <param name="start">offset into row to start at</param>
      /// <param name="counters">array into which to record counts</param>
      protected static bool recordPattern(BitArray row,
                                          int start,
                                          int[] counters,
                                          int numCounters)
      {
         for (int idx = 0; idx < numCounters; idx++)
         {
            counters[idx] = 0;
         }
         int end = row.Size;
         if (start >= end)
         {
            return false;
         }
         bool isWhite = !row[start];
         int counterPosition = 0;
         int i = start;
         while (i < end)
         {
            if (row[i] ^ isWhite)
            { // that is, exactly one is true
               counters[counterPosition]++;
            }
            else
            {
               counterPosition++;
               if (counterPosition == numCounters)
               {
                  break;
               }
               else
               {
                  counters[counterPosition] = 1;
                  isWhite = !isWhite;
               }
            }
            i++;
         }
         // If we read fully the last section of pixels and filled up our counters -- or filled
         // the last counter but ran off the side of the image, OK. Otherwise, a problem.
         return (counterPosition == numCounters || (counterPosition == numCounters - 1 && i == end));
      }

      /// <summary>
      /// Records the pattern in reverse.
      /// </summary>
      /// <param name="row">The row.</param>
      /// <param name="start">The start.</param>
      /// <param name="counters">The counters.</param>
      /// <returns></returns>
      protected static bool recordPatternInReverse(BitArray row, int start, int[] counters)
      {
         // This could be more efficient I guess
         int numTransitionsLeft = counters.Length;
         bool last = row[start];
         while (start > 0 && numTransitionsLeft >= 0)
         {
            if (row[--start] != last)
            {
               numTransitionsLeft--;
               last = !last;
            }
         }
         if (numTransitionsLeft >= 0)
         {
            return false;
         }
         return recordPattern(row, start + 1, counters);
      }

      /// <summary>
      /// Determines how closely a set of observed counts of runs of black/white values matches a given
      /// target pattern. This is reported as the ratio of the total variance from the expected pattern
      /// proportions across all pattern elements, to the length of the pattern.
      /// </summary>
      /// <param name="counters">observed counters</param>
      /// <param name="pattern">expected pattern</param>
      /// <param name="maxIndividualVariance">The most any counter can differ before we give up</param>
      /// <returns>ratio of total variance between counters and pattern compared to total pattern size,
      ///  where the ratio has been multiplied by 256. So, 0 means no variance (perfect match); 256 means
      ///  the total variance between counters and patterns equals the pattern length, higher values mean
      ///  even more variance</returns>
      protected static int patternMatchVariance(int[] counters,
                                                int[] pattern,
                                                int maxIndividualVariance)
      {
         int numCounters = counters.Length;
         int total = 0;
         int patternLength = 0;
         for (int i = 0; i < numCounters; i++)
         {
            total += counters[i];
            patternLength += pattern[i];
         }
         if (total < patternLength)
         {
            // If we don't even have one pixel per unit of bar width, assume this is too small
            // to reliably match, so fail:
            return Int32.MaxValue;
         }
         // We're going to fake floating-point math in integers. We just need to use more bits.
         // Scale up patternLength so that intermediate values below like scaledCounter will have
         // more "significant digits"
         int unitBarWidth = (total << INTEGER_MATH_SHIFT) / patternLength;
         maxIndividualVariance = (maxIndividualVariance * unitBarWidth) >> INTEGER_MATH_SHIFT;

         int totalVariance = 0;
         for (int x = 0; x < numCounters; x++)
         {
            int counter = counters[x] << INTEGER_MATH_SHIFT;
            int scaledPattern = pattern[x] * unitBarWidth;
            int variance = counter > scaledPattern ? counter - scaledPattern : scaledPattern - counter;
            if (variance > maxIndividualVariance)
            {
               return Int32.MaxValue;
            }
            totalVariance += variance;
         }
         return totalVariance / total;
      }

      /// <summary>
      /// Attempts to decode a one-dimensional barcode format given a single row of
      /// an image.
      /// </summary>
      /// <param name="rowNumber">row number from top of the row</param>
      /// <param name="row">the black/white pixel data of the row</param>
      /// <param name="hints">decode hints</param>
      /// <returns>
      ///   <see cref="Result"/>containing encoded string and start/end of barcode
      /// </returns>
      public abstract Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints);
   }
}