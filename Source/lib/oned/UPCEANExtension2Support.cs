/*
 * Copyright (C) 2012 ZXing authors
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
   /// @see UPCEANExtension5Support
   /// </summary>
   sealed class UPCEANExtension2Support
   {
      private readonly int[] decodeMiddleCounters = new int[4];
      private readonly StringBuilder decodeRowStringBuffer = new StringBuilder();

      internal Result decodeRow(int rowNumber, BitArray row, int[] extensionStartRange)
      {
         StringBuilder result = decodeRowStringBuffer;
         result.Length = 0;
         int end = decodeMiddle(row, extensionStartRange, result);
         if (end < 0)
            return null;

         String resultString = result.ToString();
         IDictionary<ResultMetadataType, Object> extensionData = parseExtensionString(resultString);

         Result extensionResult =
             new Result(resultString,
                        null,
                        new ResultPoint[] {
                       new ResultPoint((extensionStartRange[0] + extensionStartRange[1]) / 2.0f, (float) rowNumber),
                       new ResultPoint((float) end, (float) rowNumber),
                   },
                        BarcodeFormat.UPC_EAN_EXTENSION);
         if (extensionData != null)
         {
            extensionResult.putAllMetadata(extensionData);
         }
         return extensionResult;
      }

      int decodeMiddle(BitArray row, int[] startRange, StringBuilder resultString)
      {
         int[] counters = decodeMiddleCounters;
         counters[0] = 0;
         counters[1] = 0;
         counters[2] = 0;
         counters[3] = 0;
         int end = row.Size;
         int rowOffset = startRange[1];

         int checkParity = 0;

         for (int x = 0; x < 2 && rowOffset < end; x++)
         {
            int bestMatch;
            if (!UPCEANReader.decodeDigit(row, counters, rowOffset, UPCEANReader.L_AND_G_PATTERNS, out bestMatch))
               return -1;
            resultString.Append((char)('0' + bestMatch % 10));
            foreach (int counter in counters)
            {
               rowOffset += counter;
            }
            if (bestMatch >= 10)
            {
               checkParity |= 1 << (1 - x);
            }
            if (x != 1)
            {
               // Read off separator if not last
               rowOffset = row.getNextSet(rowOffset);
               rowOffset = row.getNextUnset(rowOffset);
            }
         }

         if (resultString.Length != 2)
         {
            return -1;
         }

         if (int.Parse(resultString.ToString()) % 4 != checkParity)
         {
            return -1;
         }

         return rowOffset;
      }

      /// <summary>
      /// Parses the extension string.
      /// </summary>
      /// <param name="raw">raw content of extension</param>
      /// <returns>formatted interpretation of raw content as a {@link Map} mapping
      private static IDictionary<ResultMetadataType, Object> parseExtensionString(String raw)
      {
         if (raw.Length != 2) 
         {
            return null;
         }
         IDictionary<ResultMetadataType,Object> result = new Dictionary<ResultMetadataType,Object>();
         result[ResultMetadataType.ISSUE_NUMBER] = Convert.ToInt32(raw);
         return result;
      }
   }
}