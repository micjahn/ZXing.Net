/*
 * Copyright 2009 ZXing authors
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
using ZXing.Multi;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417
{
   /// <summary>
   /// This implementation can detect and decode PDF417 codes in an image.
   ///
   /// <author>SITA Lab (kevin.osullivan@sita.aero)</author>
   /// <author>Guenther Grau</author>
   /// </summary>
   public sealed class PDF417Reader : Reader, MultipleBarcodeReader
   {
      /// <summary>
      /// Locates and decodes a PDF417 code in an image.
      ///
      /// <returns>a String representing the content encoded by the PDF417 code</returns>
      /// <exception cref="FormatException">if a PDF417 cannot be decoded</exception>
      /// </summary>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      /// Locates and decodes a barcode in some format within an image. This method also accepts
      /// hints, each possibly associated to some data, which may help the implementation decode.
      /// **Note** this will return the FIRST barcode discovered if there are many.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <param name="hints">passed as a <see cref="IDictionary{TKey, TValue}"/> from <see cref="DecodeHintType"/>
      /// to arbitrary data. The
      /// meaning of the data depends upon the hint type. The implementation may or may not do
      /// anything with these hints.</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      public Result decode(BinaryBitmap image,
                           IDictionary<DecodeHintType, object> hints)
      {
         Result[] results = decode(image, hints, false);
         if (results.Length == 0)
         {
            return null;
         }
         else
         {
            return results[0]; // First barcode discovered.
         }
      }

      /// <summary>
      /// Locates and decodes Multiple PDF417 codes in an image.
      ///
      /// <returns>an array of Strings representing the content encoded by the PDF417 codes</returns>
      /// </summary>
      public Result[] decodeMultiple(BinaryBitmap image)
      {
         return decodeMultiple(image, null);
      }

      /// <summary>
      /// Locates and decodes multiple barcodes in some format within an image. This method also accepts
      /// hints, each possibly associated to some data, which may help the implementation decode.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <param name="hints">passed as a <see cref="IDictionary{TKey, TValue}"/> from <see cref="DecodeHintType"/>
      /// to arbitrary data. The
      /// meaning of the data depends upon the hint type. The implementation may or may not do
      /// anything with these hints.</param>
      /// <returns>
      /// String which the barcodes encode
      /// </returns>
      public Result[] decodeMultiple(BinaryBitmap image,
                                     IDictionary<DecodeHintType, object> hints)
      {
         return decode(image, hints, true);
      }

      /// <summary>
      /// Decode the specified image, with the hints and optionally multiple barcodes.
      /// Based on Owen's Comments in <see cref="ZXing.ReaderException"/>, this method has been modified to continue silently
      /// if a barcode was not decoded where it was detected instead of throwing a new exception object.
      /// </summary>
      /// <param name="image">Image.</param>
      /// <param name="hints">Hints.</param>
      /// <param name="multiple">If set to <c>true</c> multiple.</param>
      private static Result[] decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints, bool multiple)
      {
         var results = new List<Result>();
         var detectorResult = Detector.detect(image, hints, multiple);
         if (detectorResult != null)
         {
            foreach (var points in detectorResult.Points)
            {
               var decoderResult = PDF417ScanningDecoder.decode(detectorResult.Bits, points[4], points[5],
                                                                points[6], points[7], getMinCodewordWidth(points), getMaxCodewordWidth(points));
               if (decoderResult == null)
               {
                  continue;
               }
               var result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.PDF_417);
               result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, decoderResult.ECLevel);
               var pdf417ResultMetadata = (PDF417ResultMetadata) decoderResult.Other;
               if (pdf417ResultMetadata != null)
               {
                  result.putMetadata(ResultMetadataType.PDF417_EXTRA_METADATA, pdf417ResultMetadata);
               }
               results.Add(result);
            }
         }
         return results.ToArray();
      }

      /// <summary>
      /// Gets the maximum width of the barcode
      /// </summary>
      /// <returns>The max width.</returns>
      /// <param name="p1">P1.</param>
      /// <param name="p2">P2.</param>
      private static int getMaxWidth(ResultPoint p1, ResultPoint p2)
      {
         if (p1 == null || p2 == null)
         {
            return 0;
         }
         return (int) Math.Abs(p1.X - p2.X);
      }

      /// <summary>
      /// Gets the minimum width of the barcode
      /// </summary>
      /// <returns>The minimum width.</returns>
      /// <param name="p1">P1.</param>
      /// <param name="p2">P2.</param>
      private static int getMinWidth(ResultPoint p1, ResultPoint p2)
      {
         if (p1 == null || p2 == null)
         {
            return int.MaxValue;
         }
         return (int) Math.Abs(p1.X - p2.X);
      }

      /// <summary>
      /// Gets the maximum width of the codeword.
      /// </summary>
      /// <returns>The max codeword width.</returns>
      /// <param name="p">P.</param>
      private static int getMaxCodewordWidth(ResultPoint[] p)
      {
         return Math.Max(
            Math.Max(getMaxWidth(p[0], p[4]), getMaxWidth(p[6], p[2])*PDF417Common.MODULES_IN_CODEWORD/
                                              PDF417Common.MODULES_IN_STOP_PATTERN),
            Math.Max(getMaxWidth(p[1], p[5]), getMaxWidth(p[7], p[3])*PDF417Common.MODULES_IN_CODEWORD/
                                              PDF417Common.MODULES_IN_STOP_PATTERN));
      }

      /// <summary>
      /// Gets the minimum width of the codeword.
      /// </summary>
      /// <returns>The minimum codeword width.</returns>
      /// <param name="p">P.</param>
      private static int getMinCodewordWidth(ResultPoint[] p)
      {
         return Math.Min(
            Math.Min(getMinWidth(p[0], p[4]), getMinWidth(p[6], p[2])*PDF417Common.MODULES_IN_CODEWORD/
                                              PDF417Common.MODULES_IN_STOP_PATTERN),
            Math.Min(getMinWidth(p[1], p[5]), getMinWidth(p[7], p[3])*PDF417Common.MODULES_IN_CODEWORD/
                                              PDF417Common.MODULES_IN_STOP_PATTERN));
      }

      /// <summary>
      /// Resets any internal state the implementation has after a decode, to prepare it
      /// for reuse.
      /// </summary>
      public void reset()
      {
         // do nothing
      }
   }
}