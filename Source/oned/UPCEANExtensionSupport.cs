/*
 * Copyright (C) 2010 ZXing authors
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

using com.google.zxing.common;

namespace com.google.zxing.oned
{
sealed class UPCEANExtensionSupport {

  private static int[] EXTENSION_START_PATTERN = {1,1,2};
  private static int[] CHECK_DIGIT_ENCODINGS = {
      0x18, 0x14, 0x12, 0x11, 0x0C, 0x06, 0x03, 0x0A, 0x09, 0x05
  };

  private int[] decodeMiddleCounters = new int[4];
  private StringBuilder decodeRowStringBuffer = new StringBuilder();

  internal Result decodeRow(int rowNumber, BitArray row, int rowOffset) {

    int[] extensionStartRange = UPCEANReader.findGuardPattern(row, rowOffset, false, EXTENSION_START_PATTERN);

    StringBuilder result = decodeRowStringBuffer;
    result.Length = 0;
    int end = decodeMiddle(row, extensionStartRange, result);

    String resultString = result.ToString();
    IDictionary<ResultMetadataType,Object> extensionData = parseExtensionString(resultString);

    Result extensionResult =
        new Result(resultString,
                   null,
                   new ResultPoint[] {
                       new ResultPoint((extensionStartRange[0] + extensionStartRange[1]) / 2.0f, rowNumber),
                       new ResultPoint(end, rowNumber),
                   },
                   BarcodeFormat.UPC_EAN_EXTENSION);
    if (extensionData != null) {
      extensionResult.putAllMetadata(extensionData);
    }
    return extensionResult;
  }

  int decodeMiddle(BitArray row,
                   int[] startRange,
                   StringBuilder resultString) {
    int[] counters = decodeMiddleCounters;
    counters[0] = 0;
    counters[1] = 0;
    counters[2] = 0;
    counters[3] = 0;
    int end = row.Size;
    int rowOffset = startRange[1];

    int lgPatternFound = 0;

    for (int x = 0; x < 5 && rowOffset < end; x++) {
      int bestMatch = UPCEANReader.decodeDigit(row, counters, rowOffset, UPCEANReader.L_AND_G_PATTERNS);
      resultString.Append((char) ('0' + bestMatch % 10));
      foreach (int counter in counters) {
        rowOffset += counter;
      }
      if (bestMatch >= 10) {
        lgPatternFound |= 1 << (4 - x);
      }
      if (x != 4) {
        // Read off separator if not last
        rowOffset = row.getNextSet(rowOffset);
        rowOffset = row.getNextUnset(rowOffset);
      }
    }

    if (resultString.Length != 5) {
      throw NotFoundException.Instance;
    }

    int checkDigit = determineCheckDigit(lgPatternFound);
    if (extensionChecksum(resultString.ToString()) != checkDigit) {
      throw NotFoundException.Instance;
    }
    
    return rowOffset;
  }

  private static int extensionChecksum(String s) {
    int length = s.Length;
    int sum = 0;
    for (int i = length - 2; i >= 0; i -= 2) {
      sum += (int) s[i] - (int) '0';
    }
    sum *= 3;
    for (int i = length - 1; i >= 0; i -= 2) {
      sum += (int) s[i] - (int) '0';
    }
    sum *= 3;
    return sum % 10;
  }

  private static int determineCheckDigit(int lgPatternFound)
 {
    for (int d = 0; d < 10; d++) {
      if (lgPatternFound == CHECK_DIGIT_ENCODINGS[d]) {
        return d;
      }
    }
    throw NotFoundException.Instance;
  }

  /// <summary>
  /// <param name="raw">raw content of extension</param>
  /// <returns>formatted interpretation of raw content as a <see cref="Map" />mapping</returns>
  ///  one <see cref="ResultMetadataType" />to appropriate value, or {@code null} if not known
  /// </summary>
  private static IDictionary<ResultMetadataType,Object> parseExtensionString(String raw) {
    ResultMetadataType type;
    Object value;
    switch (raw.Length) {
      case 2:
        type = ResultMetadataType.ISSUE_NUMBER;
        value = parseExtension2String(raw);
        break;
      case 5:
        type = ResultMetadataType.SUGGESTED_PRICE;
        value = parseExtension5String(raw);
        break;
      default:
        return null;
    }
    if (value == null) {
      return null;
    }
    IDictionary<ResultMetadataType,Object> result = new Dictionary<ResultMetadataType,Object>();
    result[type] = value;
    return result;
  }

  private static int parseExtension2String(String raw) {
    return Convert.ToInt32(raw);
  }

  private static String parseExtension5String(String raw) {
    String currency;
    switch (raw[0]) {
      case '0':
        currency = "£";
        break;
      case '5':
        currency = "$";
        break;
      case '9':
        // Reference: http://www.jollytech.com
        if ("90000".Equals(raw))
        {
          // No suggested retail price
          return null;
        }
        if ("99991".Equals(raw))
        {
          // Complementary
          return "0.00";
        }
        if ("99990".Equals(raw))
        {
          return "Used";
        }
        // Otherwise... unknown currency?
        currency = "";
        break;
      default:
        currency = "";
        break;
    }
    int rawAmount = Int32.Parse(raw.Substring(1));
    String unitsString = (rawAmount / 100).ToString();
    int hundredths = rawAmount % 100;
    String hundredthsString = hundredths < 10 ? "0" + hundredths : hundredths.ToString();
    return currency + unitsString + '.' + hundredthsString;
  }

}
}
