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
using System.Text;

using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   ///   <p>Encapsulates functionality and implementation that is common to UPC and EAN families
   /// of one-dimensional barcodes.</p>
   ///   <author>dswitkin@google.com (Daniel Switkin)</author>
   ///   <author>Sean Owen</author>
   ///   <author>alasdair@google.com (Alasdair Mackintosh)</author>
   /// </summary>
   public abstract class UPCEANReader : OneDReader
   {

      // These two values are critical for determining how permissive the decoding will be.
      // We've arrived at these values through a lot of trial and error. Setting them any higher
      // lets false positives creep in quickly.
      private static readonly int MAX_AVG_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.48f);
      private static readonly int MAX_INDIVIDUAL_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.7f);

      /// <summary>
      /// Start/end guard pattern.
      /// </summary>
      internal static int[] START_END_PATTERN = { 1, 1, 1, };

      /// <summary>
      /// Pattern marking the middle of a UPC/EAN pattern, separating the two halves.
      /// </summary>
      internal static int[] MIDDLE_PATTERN = { 1, 1, 1, 1, 1 };

      /// <summary>
      /// "Odd", or "L" patterns used to encode UPC/EAN digits.
      /// </summary>
      internal static int[][] L_PATTERNS = {
                                              new[] {3, 2, 1, 1}, // 0
                                              new[] {2, 2, 2, 1}, // 1
                                              new[] {2, 1, 2, 2}, // 2
                                              new[] {1, 4, 1, 1}, // 3
                                              new[] {1, 1, 3, 2}, // 4
                                              new[] {1, 2, 3, 1}, // 5
                                              new[] {1, 1, 1, 4}, // 6
                                              new[] {1, 3, 1, 2}, // 7
                                              new[] {1, 2, 1, 3}, // 8
                                              new[] {3, 1, 1, 2} // 9
                                           };

      /// <summary>
      /// As above but also including the "even", or "G" patterns used to encode UPC/EAN digits.
      /// </summary>
      internal static int[][] L_AND_G_PATTERNS;

      static UPCEANReader()
      {
         L_AND_G_PATTERNS = new int[20][];
         Array.Copy(L_PATTERNS, 0, L_AND_G_PATTERNS, 0, 10);
         for (int i = 10; i < 20; i++)
         {
            int[] widths = L_PATTERNS[i - 10];
            int[] reversedWidths = new int[widths.Length];
            for (int j = 0; j < widths.Length; j++)
            {
               reversedWidths[j] = widths[widths.Length - j - 1];
            }
            L_AND_G_PATTERNS[i] = reversedWidths;
         }
      }

      private readonly StringBuilder decodeRowStringBuffer;
      private readonly UPCEANExtensionSupport extensionReader;
      private readonly EANManufacturerOrgSupport eanManSupport;

      /// <summary>
      /// Initializes a new instance of the <see cref="UPCEANReader"/> class.
      /// </summary>
      protected UPCEANReader()
      {
         decodeRowStringBuffer = new StringBuilder(20);
         extensionReader = new UPCEANExtensionSupport();
         eanManSupport = new EANManufacturerOrgSupport();
      }

      internal static int[] findStartGuardPattern(BitArray row)
      {
         bool foundStart = false;
         int[] startRange = null;
         int nextStart = 0;
         int[] counters = new int[START_END_PATTERN.Length];
         while (!foundStart)
         {
            for (int idx = 0; idx < START_END_PATTERN.Length; idx++)
               counters[idx] = 0;
            startRange = findGuardPattern(row, nextStart, false, START_END_PATTERN, counters);
            if (startRange == null)
               return null;
            int start = startRange[0];
            nextStart = startRange[1];
            // Make sure there is a quiet zone at least as big as the start pattern before the barcode.
            // If this check would run off the left edge of the image, do not accept this barcode,
            // as it is very likely to be a false positive.
            int quietStart = start - (nextStart - start);
            if (quietStart >= 0)
            {
               foundStart = row.isRange(quietStart, start, false);
            }
         }
         return startRange;
      }

      /// <summary>
      ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
      /// an image.</p>
      /// </summary>
      /// <param name="rowNumber">row number from top of the row</param>
      /// <param name="row">the black/white pixel data of the row</param>
      /// <param name="hints">decode hints</param>
      /// <returns>
      ///   <see cref="Result"/>containing encoded string and start/end of barcode or null, if an error occurs or barcode cannot be found
      /// </returns>
      override public Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
      {
         return decodeRow(rowNumber, row, findStartGuardPattern(row), hints);
      }

      /// <summary>
      ///   <p>Like decodeRow(int, BitArray, java.util.Map), but
      /// allows caller to inform method about where the UPC/EAN start pattern is
      /// found. This allows this to be computed once and reused across many implementations.</p>
      /// </summary>
      /// <param name="rowNumber">row index into the image</param>
      /// <param name="row">encoding of the row of the barcode image</param>
      /// <param name="startGuardRange">start/end column where the opening start pattern was found</param>
      /// <param name="hints">optional hints that influence decoding</param>
      /// <returns><see cref="Result"/> encapsulating the result of decoding a barcode in the row</returns>
      virtual public Result decodeRow(int rowNumber,
                              BitArray row,
                              int[] startGuardRange,
                              IDictionary<DecodeHintType, object> hints)
      {
         var resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? null :
             (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];

         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(
                (startGuardRange[0] + startGuardRange[1]) / 2.0f, rowNumber
            ));
         }

         var result = decodeRowStringBuffer;
         result.Length = 0;
         var endStart = decodeMiddle(row, startGuardRange, result);
         if (endStart < 0)
            return null;

         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(
                endStart, rowNumber
            ));
         }

         var endRange = decodeEnd(row, endStart);
         if (endRange == null)
            return null;

         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(
                (endRange[0] + endRange[1]) / 2.0f, rowNumber
            ));
         }


         // Make sure there is a quiet zone at least as big as the end pattern after the barcode. The
         // spec might want more whitespace, but in practice this is the maximum we can count on.
         var end = endRange[1];
         var quietEnd = end + (end - endRange[0]);
         if (quietEnd >= row.Size || !row.isRange(end, quietEnd, false))
         {
            return null;
         }

         var resultString = result.ToString();
         // UPC/EAN should never be less than 8 chars anyway
         if (resultString.Length < 8)
         {
            return null;
         }
         if (!checkChecksum(resultString))
         {
            return null;
         }

         var left = (startGuardRange[1] + startGuardRange[0]) / 2.0f;
         var right = (endRange[1] + endRange[0]) / 2.0f;
         var format = BarcodeFormat;
         var decodeResult = new Result(resultString,
                                          null, // no natural byte representation for these barcodes
                                          new ResultPoint[]
                                             {
                                                new ResultPoint(left, rowNumber),
                                                new ResultPoint(right, rowNumber)
                                             },
                                          format);

         var extensionResult = extensionReader.decodeRow(rowNumber, row, endRange[1]);
         if (extensionResult != null)
         {
            decodeResult.putMetadata(ResultMetadataType.UPC_EAN_EXTENSION, extensionResult.Text);
            decodeResult.putAllMetadata(extensionResult.ResultMetadata);
            decodeResult.addResultPoints(extensionResult.ResultPoints);
            int extensionLength = extensionResult.Text.Length;
            int[] allowedExtensions = hints != null && hints.ContainsKey(DecodeHintType.ALLOWED_EAN_EXTENSIONS) ? 
               (int[]) hints[DecodeHintType.ALLOWED_EAN_EXTENSIONS] : null;
            if (allowedExtensions != null)
            {
               bool valid = false;
               foreach (int length in allowedExtensions)
               {
                  if (extensionLength == length)
                  {
                     valid = true;
                     break;
                  }
               }
               if (!valid)
               {
                  return null;
               }
            }
         }

         if (format == BarcodeFormat.EAN_13 || format == BarcodeFormat.UPC_A)
         {
            String countryID = eanManSupport.lookupCountryIdentifier(resultString);
            if (countryID != null)
            {
               decodeResult.putMetadata(ResultMetadataType.POSSIBLE_COUNTRY, countryID);
            }
         }

         return decodeResult;
      }

      /// <summary>
      /// </summary>
      /// <param name="s">string of digits to check</param>
      /// <returns>see <see cref="checkStandardUPCEANChecksum(String)"/></returns>
      virtual protected bool checkChecksum(String s)
      {
         return checkStandardUPCEANChecksum(s);
      }

      /// <summary>
      /// Computes the UPC/EAN checksum on a string of digits, and reports
      /// whether the checksum is correct or not.
      /// </summary>
      /// <param name="s">string of digits to check</param>
      /// <returns>true iff string of digits passes the UPC/EAN checksum algorithm</returns>
      internal static bool checkStandardUPCEANChecksum(String s)
      {
         int length = s.Length;
         if (length == 0)
         {
            return false;
         }

         int sum = 0;
         for (int i = length - 2; i >= 0; i -= 2)
         {
            int digit = (int)s[i] - (int)'0';
            if (digit < 0 || digit > 9)
            {
               return false;
            }
            sum += digit;
         }
         sum *= 3;
         for (int i = length - 1; i >= 0; i -= 2)
         {
            int digit = (int)s[i] - (int)'0';
            if (digit < 0 || digit > 9)
            {
               return false;
            }
            sum += digit;
         }
         return sum % 10 == 0;
      }

      /// <summary>
      /// Decodes the end.
      /// </summary>
      /// <param name="row">The row.</param>
      /// <param name="endStart">The end start.</param>
      /// <returns></returns>
      virtual protected int[] decodeEnd(BitArray row, int endStart)
      {
         return findGuardPattern(row, endStart, false, START_END_PATTERN);
      }

      internal static int[] findGuardPattern(BitArray row,
                                    int rowOffset,
                                    bool whiteFirst,
                                    int[] pattern)
      {
         return findGuardPattern(row, rowOffset, whiteFirst, pattern, new int[pattern.Length]);
      }

      /// <summary>
      /// </summary>
      /// <param name="row">row of black/white values to search</param>
      /// <param name="rowOffset">position to start search</param>
      /// <param name="whiteFirst">if true, indicates that the pattern specifies white/black/white/...</param>
      /// pixel counts, otherwise, it is interpreted as black/white/black/...
      /// <param name="pattern">pattern of counts of number of black and white pixels that are being</param>
      /// searched for as a pattern
      /// <param name="counters">array of counters, as long as pattern, to re-use</param>
      /// <returns>start/end horizontal offset of guard pattern, as an array of two ints</returns>
      internal static int[] findGuardPattern(BitArray row,
                                    int rowOffset,
                                    bool whiteFirst,
                                    int[] pattern,
                                    int[] counters)
      {
         int patternLength = pattern.Length;
         int width = row.Size;
         bool isWhite = whiteFirst;
         rowOffset = whiteFirst ? row.getNextUnset(rowOffset) : row.getNextSet(rowOffset);
         int counterPosition = 0;
         int patternStart = rowOffset;
         for (int x = rowOffset; x < width; x++)
         {
            if (row[x] ^ isWhite)
            {
               counters[counterPosition]++;
            }
            else
            {
               if (counterPosition == patternLength - 1)
               {
                  if (patternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE) < MAX_AVG_VARIANCE)
                  {
                     return new int[] { patternStart, x };
                  }
                  patternStart += counters[0] + counters[1];
                  Array.Copy(counters, 2, counters, 0, patternLength - 2);
                  counters[patternLength - 2] = 0;
                  counters[patternLength - 1] = 0;
                  counterPosition--;
               }
               else
               {
                  counterPosition++;
               }
               counters[counterPosition] = 1;
               isWhite = !isWhite;
            }
         }
         return null;
      }

      /// <summary>
      /// Attempts to decode a single UPC/EAN-encoded digit.
      /// </summary>
      /// <param name="row">row of black/white values to decode</param>
      /// <param name="counters">the counts of runs of observed black/white/black/... values</param>
      /// <param name="rowOffset">horizontal offset to start decoding from</param>
      /// <param name="patterns">the set of patterns to use to decode -- sometimes different encodings</param>
      /// for the digits 0-9 are used, and this indicates the encodings for 0 to 9 that should
      /// be used
      /// <returns>horizontal offset of first pixel beyond the decoded digit</returns>
      internal static bool decodeDigit(BitArray row, int[] counters, int rowOffset, int[][] patterns, out int digit)
      {
         digit = -1;

         if (!recordPattern(row, rowOffset, counters))
            return false;

         int bestVariance = MAX_AVG_VARIANCE; // worst variance we'll accept
         int max = patterns.Length;
         for (int i = 0; i < max; i++)
         {
            int[] pattern = patterns[i];
            int variance = patternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE);
            if (variance < bestVariance)
            {
               bestVariance = variance;
               digit = i;
            }
         }
         return digit >= 0;
      }

      /// <summary>
      /// Get the format of this decoder.
      /// </summary>
      /// <returns>The 1D format.</returns>
      internal abstract BarcodeFormat BarcodeFormat { get; }

      /// <summary>
      /// Subclasses override this to decode the portion of a barcode between the start
      /// and end guard patterns.
      /// </summary>
      /// <param name="row">row of black/white values to search</param>
      /// <param name="startRange">start/end offset of start guard pattern</param>
      /// <param name="resultString"><see cref="StringBuilder" />to append decoded chars to</param>
      /// <returns>horizontal offset of first pixel after the "middle" that was decoded or -1 if decoding could not complete successfully</returns>
      protected internal abstract int decodeMiddle(BitArray row,
                                                   int[] startRange,
                                                   StringBuilder resultString);
   }
}