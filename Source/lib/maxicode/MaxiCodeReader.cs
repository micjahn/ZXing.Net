/*
 * Copyright 2011 ZXing authors
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
using ZXing.Maxicode.Internal;

namespace ZXing.Maxicode
{
   /// <summary>
   /// This implementation can detect and decode a MaxiCode in an image.
   /// </summary>
   public sealed class MaxiCodeReader : Reader
   {
      private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
      private const int MATRIX_WIDTH = 30;
      private const int MATRIX_HEIGHT = 33;

      private readonly Decoder decoder = new Decoder();

      /// <summary>
      /// Locates and decodes a MaxiCode in an image.
      ///
      /// <returns>a String representing the content encoded by the MaxiCode</returns>
      /// <exception cref="FormatException">if a MaxiCode cannot be decoded</exception>
      /// </summary>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      /// Locates and decodes a MaxiCode within an image. This method also accepts
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
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         DecoderResult decoderResult;
         if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
         {
            BitMatrix bits = extractPureBits(image.BlackMatrix);
            if (bits == null)
               return null;
            decoderResult = decoder.decode(bits, hints);
            if (decoderResult == null)
               return null;
         }
         else
         {
            return null;
         }

         ResultPoint[] points = NO_POINTS;
         Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.MAXICODE);

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
      /// <seealso cref="ZXing.Datamatrix.DataMatrixReader.extractPureBits(BitMatrix)" />
      /// <seealso cref="ZXing.QrCode.QRCodeReader.extractPureBits(BitMatrix)" />
      /// </summary>
      private static BitMatrix extractPureBits(BitMatrix image)
      {

         int[] enclosingRectangle = image.getEnclosingRectangle();
         if (enclosingRectangle == null)
         {
            return null;
         }

         int left = enclosingRectangle[0];
         int top = enclosingRectangle[1];
         int width = enclosingRectangle[2];
         int height = enclosingRectangle[3];

         // Now just read off the bits
         BitMatrix bits = new BitMatrix(MATRIX_WIDTH, MATRIX_HEIGHT);
         for (int y = 0; y < MATRIX_HEIGHT; y++)
         {
            int iy = top + (y * height + height / 2) / MATRIX_HEIGHT;
            for (int x = 0; x < MATRIX_WIDTH; x++)
            {
               int ix = left + (x * width + width / 2 + (y & 0x01) * width / 2) / MATRIX_WIDTH;
               if (image[ix, iy])
               {
                  bits[x, y] = true;
               }
            }
         }
         return bits;
      }
   }
}