/*
 * Copyright 2007 ZXing authors
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
using ZXing.Datamatrix.Internal;

namespace ZXing.Datamatrix
{
   /// <summary>
   /// This implementation can detect and decode Data Matrix codes in an image.
   ///
   /// <author>bbrown@google.com (Brian Brown)</author>
   /// </summary>
   public sealed class DataMatrixReader : Reader
   {
      private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

      private readonly Decoder decoder = new Decoder();

      /// <summary>
      /// Locates and decodes a Data Matrix code in an image.
      /// </summary>
      /// <param name="image"></param>
      /// <returns>a String representing the content encoded by the Data Matrix code</returns>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      /// Locates and decodes a Data Matrix code in an image.
      /// </summary>
      /// <param name="image"></param>
      /// <param name="hints"></param>
      /// <returns>a String representing the content encoded by the Data Matrix code</returns>
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         DecoderResult decoderResult;
         ResultPoint[] points;
         if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
         {
            BitMatrix bits = extractPureBits(image.BlackMatrix);
            if (bits == null)
               return null;
            decoderResult = decoder.decode(bits);
            points = NO_POINTS;
         }
         else
         {
            DetectorResult detectorResult = new Detector(image.BlackMatrix).detect();
            if (detectorResult == null)
               return null;
            decoderResult = decoder.decode(detectorResult.Bits);
            points = detectorResult.Points;
         }
         if (decoderResult == null)
            return null;

         Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points,
             BarcodeFormat.DATA_MATRIX);
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
         return result;
      }

      /// <summary>
      /// does nothing here
      /// </summary>
      public void reset()
      {
         // do nothing
      }

      /// <summary>
      /// This method detects a code in a "pure" image -- that is, pure monochrome image
      /// which contains only an unrotated, unskewed, image of a code, with some white border
      /// around it. This is a specialized method that works exceptionally fast in this special
      /// case.
      ///
      /// <seealso cref="ZXing.QrCode.QRCodeReader.extractPureBits(BitMatrix)" />
      /// </summary>
      private static BitMatrix extractPureBits(BitMatrix image)
      {
         int[] leftTopBlack = image.getTopLeftOnBit();
         int[] rightBottomBlack = image.getBottomRightOnBit();
         if (leftTopBlack == null || rightBottomBlack == null)
         {
            return null;
         }

         int moduleSize;
         if (!DataMatrixReader.moduleSize(leftTopBlack, image, out moduleSize))
            return null;

         int top = leftTopBlack[1];
         int bottom = rightBottomBlack[1];
         int left = leftTopBlack[0];
         int right = rightBottomBlack[0];

         int matrixWidth = (right - left + 1) / moduleSize;
         int matrixHeight = (bottom - top + 1) / moduleSize;
         if (matrixWidth <= 0 || matrixHeight <= 0)
         {
            return null;
         }

         // Push in the "border" by half the module width so that we start
         // sampling in the middle of the module. Just in case the image is a
         // little off, this will help recover.
         int nudge = moduleSize >> 1;
         top += nudge;
         left += nudge;

         // Now just read off the bits
         BitMatrix bits = new BitMatrix(matrixWidth, matrixHeight);
         for (int y = 0; y < matrixHeight; y++)
         {
            int iOffset = top + y * moduleSize;
            for (int x = 0; x < matrixWidth; x++)
            {
               if (image[left + x * moduleSize, iOffset])
               {
                  bits[x, y] = true;
               }
            }
         }
         return bits;
      }

      private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out int modulesize)
      {
         int width = image.Width;
         int x = leftTopBlack[0];
         int y = leftTopBlack[1];
         while (x < width && image[x, y])
         {
            x++;
         }
         if (x == width)
         {
            modulesize = 0;
            return false;
         }

         modulesize = x - leftTopBlack[0];
         if (modulesize == 0)
         {
            return false;
         }
         return true;
      }
   }
}
