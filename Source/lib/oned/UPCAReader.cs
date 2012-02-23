/*
 * Copyright 2008 ZXing authors
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
using System.Text;
using ZXing.Common;

namespace ZXing.OneD
{
   /// <summary>
   /// <p>Implements decoding of the UPC-A format.</p>
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class UPCAReader : UPCEANReader
   {
      private readonly UPCEANReader ean13Reader = new EAN13Reader();

      override public Result decodeRow(int rowNumber,
                              BitArray row,
                              int[] startGuardRange,
                              IDictionary<DecodeHintType, object> hints)
      {
         return maybeReturnResult(ean13Reader.decodeRow(rowNumber, row, startGuardRange, hints));
      }

      override public Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
      {
         return maybeReturnResult(ean13Reader.decodeRow(rowNumber, row, hints));
      }

      override public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         return maybeReturnResult(ean13Reader.decode(image, hints));
      }

      override internal BarcodeFormat BarcodeFormat
      {
         get { return BarcodeFormat.UPC_A; }
      }

      override protected internal int decodeMiddle(BitArray row, int[] startRange, StringBuilder resultString)
      {
         return ean13Reader.decodeMiddle(row, startRange, resultString);
      }

      private static Result maybeReturnResult(Result result)
      {
         if (result == null)
            return null;

         String text = result.Text;
         if (text[0] == '0')
         {
            return new Result(text.Substring(1), null, result.ResultPoints, BarcodeFormat.UPC_A);
         }
         else
         {
            return null;
         }
      }
   }
}