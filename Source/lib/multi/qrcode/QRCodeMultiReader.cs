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

using System.Collections.Generic;

using ZXing.Common;
using ZXing.Multi.QrCode.Internal;
using ZXing.QrCode;

namespace ZXing.Multi.QrCode
{
   /// <summary>
   /// This implementation can detect and decode multiple QR Codes in an image.
   /// </summary>
   public sealed class QRCodeMultiReader : QRCodeReader, MultipleBarcodeReader
   {
      private static readonly Result[] EMPTY_RESULT_ARRAY = new Result[0];

      /// <summary>
      /// Decodes the multiple.
      /// </summary>
      /// <param name="image">The image.</param>
      /// <returns></returns>
      public Result[] decodeMultiple(BinaryBitmap image)
      {
         return decodeMultiple(image, null);
      }

      /// <summary>
      /// Decodes the multiple.
      /// </summary>
      /// <param name="image">The image.</param>
      /// <param name="hints">The hints.</param>
      /// <returns></returns>
      public Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         var results = new List<Result>();
         var detectorResults = new MultiDetector(image.BlackMatrix).detectMulti(hints);
         foreach (DetectorResult detectorResult in detectorResults)
         {
            var decoderResult = getDecoder().decode(detectorResult.Bits, hints);
            if (decoderResult == null)
               continue;

            var points = detectorResult.Points;
            var result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.QR_CODE);
            var byteSegments = decoderResult.ByteSegments;
            if (byteSegments != null)
            {
               result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
            }
            var ecLevel = decoderResult.ECLevel;
            if (ecLevel != null)
            {
               result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, ecLevel);
            }
            results.Add(result);
         }
         return results.Count == 0 ? EMPTY_RESULT_ARRAY : results.ToArray();
      }
   }
}