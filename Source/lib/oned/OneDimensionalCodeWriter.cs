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

namespace ZXing.OneD
{
   /// <summary>
   /// <p>Encapsulates functionality and implementation that is common to one-dimensional barcodes.</p>
   ///
   /// <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
   /// </summary>
   public abstract class OneDimensionalCodeWriter : Writer
   {
      private int sidesMargin;

      protected OneDimensionalCodeWriter(int sidesMargin)
      {
         this.sidesMargin = sidesMargin;
      }

      public BitMatrix encode(String contents, BarcodeFormat format, int width, int height)
      {
         return encode(contents, format, width, height, null);
      }

      /// <summary>
      /// Encode the contents following specified format.
      /// {@code width} and {@code height} are required size. This method may return bigger size
      /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
      /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
      /// or {@code height}, {@code IllegalArgumentException} is thrown.
      /// </summary>
      public virtual BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height,
                              IDictionary<EncodeHintType, object> hints)
      {
         if (contents.Length == 0)
         {
            throw new ArgumentException("Found empty contents");
         }

         if (width < 0 || height < 0)
         {
            throw new ArgumentException("Negative size is not allowed. Input: "
                                                   + width + 'x' + height);
         }

         sbyte[] code = encode(contents);
         return renderResult(code, width, height);
      }

      /// <summary>
      /// <returns>a byte array of horizontal pixels (0 = white, 1 = black)</returns>
      /// </summary>
      private BitMatrix renderResult(sbyte[] code, int width, int height)
      {
         int inputWidth = code.Length;
         // Add quiet zone on both sides.
         int fullWidth = inputWidth + sidesMargin;
         int outputWidth = Math.Max(width, fullWidth);
         int outputHeight = Math.Max(1, height);

         int multiple = outputWidth / fullWidth;
         int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;

         BitMatrix output = new BitMatrix(outputWidth, outputHeight);
         for (int inputX = 0, outputX = leftPadding; inputX < inputWidth; inputX++, outputX += multiple)
         {
            if (code[inputX] == 1)
            {
               output.setRegion(outputX, 0, multiple, outputHeight);
            }
         }
         return output;
      }


      /// <summary>
      /// Appends the given pattern to the target array starting at pos.
      ///
      /// <param name="startColor">starting color - 0 for white, 1 for black</param>
      /// <returns>the number of elements added to target.</returns>
      /// </summary>
      protected static int appendPattern(sbyte[] target, int pos, int[] pattern, int startColor)
      {
         if (startColor != 0 && startColor != 1)
         {
            throw new ArgumentException(
                "startColor must be either 0 or 1, but got: " + startColor);
         }

         var color = (sbyte)startColor;
         int numAdded = 0;
         foreach (int len in pattern)
         {
            for (int j = 0; j < len; j++)
            {
               target[pos] = color;
               pos += 1;
               numAdded += 1;
            }
            color ^= 1; // flip color after each segment
         }
         return numAdded;
      }

      /// <summary>
      /// Encode the contents to byte array expression of one-dimensional barcode.
      /// Start code and end code should be included in result, and side margins should not be included.
      ///
      /// <returns>a byte array of horizontal pixels (0 = white, 1 = black)</returns>
      /// </summary>
      public abstract sbyte[] encode(String contents);
   }
}
