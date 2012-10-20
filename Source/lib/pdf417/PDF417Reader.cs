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
using ZXing.PDF417.Internal;

namespace ZXing.PDF417
{
   /// <summary>
   /// This implementation can detect and decode PDF417 codes in an image.
   ///
   /// <author>SITA Lab (kevin.osullivan@sita.aero)</author>
   /// </summary>
   public sealed class PDF417Reader : Reader
   {
      private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

      private readonly Decoder decoder = new Decoder();

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
            DetectorResult detectorResult = new Detector(image).detect(hints);
            if (detectorResult == null || detectorResult.Bits == null)
               return null;
            decoderResult = decoder.decode(detectorResult.Bits);
            points = detectorResult.Points;
         }
         if (decoderResult == null)
            return null;

         return new Result(decoderResult.Text, decoderResult.RawBytes, points,
             BarcodeFormat.PDF_417);
      }

      /// <summary>
      /// Resets any internal state the implementation has after a decode, to prepare it
      /// for reuse.
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
      /// <see cref="QrCode.QRCodeReader.extractPureBits(BitMatrix)" />
      /// <see cref="Datamatrix.DataMatrixReader.extractPureBits(BitMatrix)" />
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
         if (!PDF417Reader.moduleSize(leftTopBlack, image, out moduleSize))
            return null;

         int top = leftTopBlack[1];
         int bottom = rightBottomBlack[1];
         int left;
         if (!findPatternStart(leftTopBlack[0], top, image, out left))
            return null;
         int right;
         if (!findPatternEnd(leftTopBlack[0], top, image, out right))
            return null;

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
         var bits = new BitMatrix(matrixWidth, matrixHeight);
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

      private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out int msize)
      {
         int x = leftTopBlack[0];
         int y = leftTopBlack[1];
         int width = image.Width;
         while (x < width && image[x, y])
         {
            x++;
         }
         if (x == width)
         {
            msize = 0;
            return false;
         }

         msize = (int)((uint)(x - leftTopBlack[0]) >> 3); // (x - leftTopBlack[0]) >>> 3// We've crossed left first bar, which is 8x
         if (msize == 0)
         {
            return false;
         }
         return true;
      }

      private static bool findPatternStart(int x, int y, BitMatrix image, out int start)
      {
         int width = image.Width;
         start = x;
         // start should be on black
         int transitions = 0;
         bool black = true;
         while (start < width - 1 && transitions < 8)
         {
            start++;
            bool newBlack = image[start, y];
            if (black != newBlack)
            {
               transitions++;
            }
            black = newBlack;
         }
         if (start == width - 1)
         {
            return false;
         }
         return true;
      }

      private static bool findPatternEnd(int x, int y, BitMatrix image, out int end)
      {
         int width = image.Width;
         end = width - 1;
         // end should be on black
         while (end > x && !image[end, y])
         {
            end--;
         }
         int transitions = 0;
         bool black = true;
         while (end > x && transitions < 9)
         {
            end--;
            bool newBlack = image[end, y];
            if (black != newBlack)
            {
               transitions++;
            }
            black = newBlack;
         }
         if (end == x)
         {
            return false;
         }
         return true;
      }
   }
}
