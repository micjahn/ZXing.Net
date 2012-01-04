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

using System;
using System.Collections.Generic;
using com.google.zxing.aztec.decoder;
using com.google.zxing.common;

namespace com.google.zxing.aztec
{
   /**
 * This implementation can detect and decode Aztec codes in an image.
 *
 * @author David Olivier
 */
   public class AztecReader : Reader
   {

      /**
       * Locates and decodes a Data Matrix code in an image.
       *
       * @return a String representing the content encoded by the Data Matrix code
       * @throws NotFoundException if a Data Matrix code cannot be found
       * @throws FormatException if a Data Matrix code cannot be decoded
       * @throws ChecksumException if error correction fails
       */
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {

         AztecDetectorResult detectorResult = new Detector(image.BlackMatrix).detect();
         ResultPoint[] points = detectorResult.Points;

         if (hints != null)
         {
            var rpcb = (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
            if (rpcb != null)
            {
               foreach (var point in points)
               {
                  rpcb.foundPossibleResultPoint(point);
               }
            }
         }

         DecoderResult decoderResult = new Decoder().decode(detectorResult);

         Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.AZTEC);

         IList<byte[]> byteSegments = decoderResult.ByteSegments;
         if (byteSegments != null)
         {
            result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
         }
         String ecLevel = decoderResult.ECLevel.ToString();
         if (ecLevel != null)
         {
            result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, ecLevel);
         }

         return result;
      }

      public void reset()
      {
         // do nothing
      }
   }
}