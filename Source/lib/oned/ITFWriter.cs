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
using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// This object renders a ITF code as a <see cref="BitMatrix" />.
   /// 
   /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
   /// </summary>
   public sealed class ITFWriter : UPCEANWriter
   {
      public override BitMatrix encode(String contents,
                              BarcodeFormat format,
                              int width,
                              int height,
                              IDictionary<EncodeHintType, object> hints)
      {
         if (format != BarcodeFormat.ITF)
         {
            throw new ArgumentException("Can only encode ITF, but got " + format);
         }

         return base.encode(contents, format, width, height, hints);
      }

      override public sbyte[] encode(String contents)
      {
         int length = contents.Length;
         if (length % 2 != 0)
         {
            throw new ArgumentException("The lenght of the input should be even");
         } 
         if (length > 80)
         {
            throw new ArgumentException(
                "Requested contents should be less than 80 digits long, but got " + length);
         }
         for (var i = 0; i < length; i++)
         {
            if (!Char.IsDigit(contents[i]))
               throw new ArgumentException("Requested contents should only contain digits, but got '" + contents[i] + "'");
         }

         sbyte[] result = new sbyte[9 + 9 * length];
         int[] start = { 1, 1, 1, 1 };
         int pos = appendPattern(result, 0, start, 1);
         for (int i = 0; i < length; i += 2)
         {
            int one = Convert.ToInt32(contents[i].ToString(), 10);
            int two = Convert.ToInt32(contents[i + 1].ToString(), 10);
            int[] encoding = new int[18];
            for (int j = 0; j < 5; j++)
            {
               encoding[(j << 1)] = ITFReader.PATTERNS[one][j];
               encoding[(j << 1) + 1] = ITFReader.PATTERNS[two][j];
            }
            pos += appendPattern(result, pos, encoding, 1);
         }
         int[] end = { 3, 1, 1 };
         pos += appendPattern(result, pos, end, 1);

         return result;
      }
   }
}