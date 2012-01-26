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

using System;
using System.Collections.Generic;

using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;
using com.google.zxing.qrcode.detector;

namespace com.google.zxing.qrcode
{
   /// <summary>
   /// This implementation can detect and decode QR Codes in an image.
   ///
   /// <author>Sean Owen</author>
   /// </summary>
   public class QRCodeReader : Reader
   {
      private static ResultPoint[] NO_POINTS = new ResultPoint[0];

      private Decoder decoder = new Decoder();

      protected Decoder getDecoder()
      {
         return decoder;
      }

      /// <summary>
      /// Locates and decodes a QR code in an image.
      ///
      /// <returns>a String representing the content encoded by the QR code</returns>
      /// <exception cref="NotFoundException">if a QR code cannot be found</exception>
      /// <exception cref="FormatException">if a QR code cannot be decoded</exception>
      /// <exception cref="ChecksumException">if error correction fails</exception>
      /// </summary>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         DecoderResult decoderResult;
         ResultPoint[] points;
         if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
         {
            BitMatrix bits = extractPureBits(image.BlackMatrix);
            decoderResult = decoder.decode(bits, hints);
            points = NO_POINTS;
         }
         else
         {
            DetectorResult detectorResult = new Detector(image.BlackMatrix).detect(hints);
            decoderResult = decoder.decode(detectorResult.Bits, hints);
            points = detectorResult.Points;
         }

         Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.QR_CODE);
         IList<sbyte[]> byteSegments = decoderResult.ByteSegments;
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
      /// @see com.google.zxing.pdf417.PDF417Reader#extractPureBits(BitMatrix)
      /// @see com.google.zxing.datamatrix.DataMatrixReader#extractPureBits(BitMatrix)
      /// </summary>
      private static BitMatrix extractPureBits(BitMatrix image)
      {

         int[] leftTopBlack = image.getTopLeftOnBit();
         int[] rightBottomBlack = image.getBottomRightOnBit();
         if (leftTopBlack == null || rightBottomBlack == null)
         {
            throw NotFoundException.Instance;
         }

         float moduleSize = QRCodeReader.moduleSize(leftTopBlack, image);

         int top = leftTopBlack[1];
         int bottom = rightBottomBlack[1];
         int left = leftTopBlack[0];
         int right = rightBottomBlack[0];

         if (bottom - top != right - left)
         {
            // Special case, where bottom-right module wasn't black so we found something else in the last row
            // Assume it's a square, so use height as the width
            right = left + (bottom - top);
         }

         int matrixWidth = (int)Math.Round((right - left + 1) / moduleSize);
         int matrixHeight = (int)Math.Round((bottom - top + 1) / moduleSize);
         if (matrixWidth <= 0 || matrixHeight <= 0)
         {
            throw NotFoundException.Instance;
         }
         if (matrixHeight != matrixWidth)
         {
            // Only possibly decode square regions
            throw NotFoundException.Instance;
         }

         // Push in the "border" by half the module width so that we start
         // sampling in the middle of the module. Just in case the image is a
         // little off, this will help recover.
         int nudge = (int)Math.Round(moduleSize / 2.0f);
         top += nudge;
         left += nudge;

         // Now just read off the bits
         BitMatrix bits = new BitMatrix(matrixWidth, matrixHeight);
         for (int y = 0; y < matrixHeight; y++)
         {
            int iOffset = top + (int)(y * moduleSize);
            for (int x = 0; x < matrixWidth; x++)
            {
               if (image[left + (int)(x * moduleSize), iOffset])
               {
                  bits[x, y] = true;
               }
            }
         }
         return bits;
      }

      private static float moduleSize(int[] leftTopBlack, BitMatrix image)
      {
         int height = image.Height;
         int width = image.Width;
         int x = leftTopBlack[0];
         int y = leftTopBlack[1];
         bool inBlack = true;
         int transitions = 0;
         while (x < width && y < height)
         {
            if (inBlack != image[x, y])
            {
               if (++transitions == 5)
               {
                  break;
               }
               inBlack = !inBlack;
            }
            x++;
            y++;
         }
         if (x == width || y == height)
         {
            throw NotFoundException.Instance;
         }
         return (x - leftTopBlack[0]) / 7.0f;
      }
   }
}