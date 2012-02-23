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

using System;
using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.PDF417.Internal
{
   /// <summary>
   /// <author>Jacob Haynes</author>
   /// </summary>
   public sealed class PDF417Writer : Writer
   {

      public BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height,
                              IDictionary<EncodeHintType, object> hints)
      {
         return encode(contents, format, width, height);
      }

      public BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height)
      {
         PDF417 encoder = initializeEncoder(format, false);
         return bitMatrixFromEncoder(encoder, contents, width, height);
      }

      public BitMatrix encode(String contents,
                              BarcodeFormat format,
                              bool compact,
                              int width,
                              int height,
                              int minCols,
                              int maxCols,
                              int minRows,
                              int maxRows,
                              Compaction compaction)
      {
         PDF417 encoder = initializeEncoder(format, compact);

         // Set options: dimensions and byte compaction
         encoder.setDimensions(maxCols, minCols, maxRows, minRows);
         encoder.setCompaction(compaction);

         return bitMatrixFromEncoder(encoder, contents, width, height);
      }

      /// <summary>
      /// Initializes the encoder based on the format (whether it's compact or not)
      /// </summary>
      private static PDF417 initializeEncoder(BarcodeFormat format, bool compact)
      {
         if (format != BarcodeFormat.PDF_417)
         {
            throw new ArgumentException("Can only encode PDF_417, but got " + format);
         }

         PDF417 encoder = new PDF417();
         encoder.setCompact(compact);
         return encoder;
      }

      /// <summary>
      /// Takes encoder, accounts for width/height, and retrieves bit matrix
      /// </summary>
      private static BitMatrix bitMatrixFromEncoder(PDF417 encoder,
                                                    String contents,
                                                    int width,
                                                    int height)
      {
         int errorCorrectionLevel = 2;
         encoder.generateBarcodeLogic(contents, errorCorrectionLevel);

         int lineThickness = 2;
         int aspectRatio = 4;
         sbyte[][] originalScale = encoder.BarcodeMatrix.getScaledMatrix(lineThickness, aspectRatio * lineThickness);
         bool rotated = false;
         if ((height > width) ^ (originalScale[0].Length < originalScale.Length))
         {
            originalScale = rotateArray(originalScale);
            rotated = true;
         }

         int scaleX = width / originalScale[0].Length;
         int scaleY = height / originalScale.Length;

         int scale;
         if (scaleX < scaleY)
         {
            scale = scaleX;
         }
         else
         {
            scale = scaleY;
         }

         if (scale > 1)
         {
            sbyte[][] scaledMatrix =
                encoder.BarcodeMatrix.getScaledMatrix(scale * lineThickness, scale * aspectRatio * lineThickness);
            if (rotated)
            {
               scaledMatrix = rotateArray(scaledMatrix);
            }
            return bitMatrixFrombitArray(scaledMatrix);
         }
         return bitMatrixFrombitArray(originalScale);
      }

      /// <summary>
      /// This takes an array holding the values of the PDF 417
      ///
      /// <param name="input">a byte array of information with 0 is black, and 1 is white</param>
      /// <returns>BitMatrix of the input</returns>
      /// </summary>
      private static BitMatrix bitMatrixFrombitArray(sbyte[][] input)
      {
         //Creates a small whitespace boarder around the barcode
         int whiteSpace = 30;

         //Creates the bitmatrix with extra space for whtespace
         BitMatrix output = new BitMatrix(input.Length + 2 * whiteSpace, input[0].Length + 2 * whiteSpace);
         output.clear();
         for (int ii = 0; ii < input.Length; ii++)
         {
            for (int jj = 0; jj < input[0].Length; jj++)
            {
               // Zero is white in the bytematrix
               if (input[ii][jj] == 1)
               {
                  output[ii + whiteSpace, jj + whiteSpace] = true;
               }
            }
         }
         return output;
      }

      /// <summary>
      /// Takes and rotates the it 90 degrees
      /// </summary>
      private static sbyte[][] rotateArray(sbyte[][] bitarray)
      {
         sbyte[][] temp = new sbyte[bitarray[0].Length][];
         for (int idx = 0; idx < bitarray[0].Length; idx++)
            temp[idx] = new sbyte[bitarray.Length];
         for (int ii = 0; ii < bitarray.Length; ii++)
         {
            // This makes the direction consistent on screen when rotating the
            // screen;
            int inverseii = bitarray.Length - ii - 1;
            for (int jj = 0; jj < bitarray[0].Length; jj++)
            {
               temp[jj][inverseii] = bitarray[ii][jj];
            }
         }
         return temp;
      }

   }
}
