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

using com.google.zxing.common;
using com.google.zxing.multi.qrcode.detector;
using com.google.zxing.qrcode;

namespace com.google.zxing.multi.qrcode
{
   /// <summary>
   /// This implementation can detect and decode multiple QR Codes in an image.
   ///
   /// <author>Sean Owen</author>
   /// <author>Hannes Erven</author>
   /// </summary>
   public sealed class QRCodeMultiReader : QRCodeReader, MultipleBarcodeReader
   {
      private static readonly Result[] EMPTY_RESULT_ARRAY = new Result[0];

      public Result[] decodeMultiple(BinaryBitmap image)
      {
         return decodeMultiple(image, null);
      }

      public Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         List<Result> results = new List<Result>();
         DetectorResult[] detectorResults = new MultiDetector(image.BlackMatrix).detectMulti(hints);
         foreach (DetectorResult detectorResult in detectorResults)
         {
            DecoderResult decoderResult = getDecoder().decode(detectorResult.Bits, hints);
            if (decoderResult == null)
               continue;

            ResultPoint[] points = detectorResult.Points;
            Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points,
                                       BarcodeFormat.QR_CODE);
            IList<sbyte[]> byteSegments = decoderResult.ByteSegments;
            if (byteSegments != null)
            {
               result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
            }
            String ecLevel = decoderResult.ECLevel;
            if (ecLevel != null)
            {
               result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, ecLevel);
            }
            results.Add(result);
         }
         if (results.Count == 0)
         {
            return EMPTY_RESULT_ARRAY;
         }
         else
         {
            return results.ToArray();
         }
      }
   }
}