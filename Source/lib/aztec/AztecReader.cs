/*
 * Copyright 2010 ZXing authors
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
using ZXing.Aztec.Internal;

namespace ZXing.Aztec
{
   /// <summary>
   /// This implementation can detect and decode Aztec codes in an image.
   /// </summary>
   /// <author>David Olivier</author>
   public class AztecReader : Reader
   {
      /// <summary>
      /// Locates and decodes a barcode in some format within an image.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <returns>
      /// a String representing the content encoded by the Data Matrix code
      /// </returns>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      ///  Locates and decodes a Data Matrix code in an image.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <param name="hints">passed as a {@link java.util.Hashtable} from {@link com.google.zxing.DecodeHintType}
      /// to arbitrary data. The
      /// meaning of the data depends upon the hint type. The implementation may or may not do
      /// anything with these hints.</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         var blackmatrix = image.BlackMatrix;
         if (blackmatrix == null)
            return null;

         Detector detector = new Detector(blackmatrix);
         ResultPoint[] points = null;
         DecoderResult decoderResult = null;

         var detectorResult = detector.detect(false);
         if (detectorResult != null)
         {
            points = detectorResult.Points;

            decoderResult = new Decoder().decode(detectorResult);
         }
         if (decoderResult == null)
         {
            detectorResult = detector.detect(true);
            if (detectorResult == null)
               return null;
               
            points = detectorResult.Points;
            decoderResult = new Decoder().decode(detectorResult);
            if (decoderResult == null)
               return null;
         }

         if (hints != null &&
             hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
         {
            var rpcb = (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
            if (rpcb != null)
            {
               foreach (var point in points)
               {
                  rpcb(point);
               }
            }
         }

         var result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.AZTEC);

         IList<byte[]> byteSegments = decoderResult.ByteSegments;
         if (byteSegments != null)
         {
            result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
         }
         var ecLevel = decoderResult.ECLevel;
         if (ecLevel != null)
         {
            result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, ecLevel);
         }

         result.putMetadata(ResultMetadataType.AZTEC_EXTRA_METADATA,
                            new AztecResultMetadata(detectorResult.Compact, detectorResult.NbDatablocks, detectorResult.NbLayers));

         return result;
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