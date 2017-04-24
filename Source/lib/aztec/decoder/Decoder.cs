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
using System.Text;

using ZXing.Common;
using ZXing.Common.ReedSolomon;

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// The main class which implements Aztec Code decoding -- as opposed to locating and extracting
   /// the Aztec Code from an image.
   /// </summary>
   /// <author>David Olivier</author>
   public sealed class Decoder
   {
      private enum Table
      {
         UPPER,
         LOWER,
         MIXED,
         DIGIT,
         PUNCT,
         BINARY
      }

      private static readonly String[] UPPER_TABLE =
      {
         "CTRL_PS", " ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
         "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "CTRL_LL", "CTRL_ML", "CTRL_DL", "CTRL_BS"
      };

      private static readonly String[] LOWER_TABLE =
      {
         "CTRL_PS", " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
         "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "CTRL_US", "CTRL_ML", "CTRL_DL", "CTRL_BS"
      };

      private static readonly String[] MIXED_TABLE =
      {
         "CTRL_PS", " ", "\x1", "\x2", "\x3", "\x4", "\x5", "\x6", "\x7", "\b", "\t", "\n",
         "\xD", "\f", "\r", "\x21", "\x22", "\x23", "\x24", "\x25", "@", "\\", "^", "_",
         "`", "|", "~", "\xB1", "CTRL_LL", "CTRL_UL", "CTRL_PL", "CTRL_BS"
      };

      private static readonly String[] PUNCT_TABLE =
      {
         "", "\r", "\r\n", ". ", ", ", ": ", "!", "\"", "#", "$", "%", "&", "'", "(", ")",
         "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "[", "]", "{", "}", "CTRL_UL"
      };

      private static readonly String[] DIGIT_TABLE =
      {
         "CTRL_PS", " ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", ".", "CTRL_UL", "CTRL_US"
      };

      private static readonly IDictionary<Table, String[]> codeTables = new Dictionary<Table, String[]>
      {
         {Table.UPPER, UPPER_TABLE},
         {Table.LOWER, LOWER_TABLE},
         {Table.MIXED, MIXED_TABLE},
         {Table.PUNCT, PUNCT_TABLE},
         {Table.DIGIT, DIGIT_TABLE},
         {Table.BINARY, null}
      };

      private static readonly IDictionary<char, Table> codeTableMap = new Dictionary<char, Table>
      {
         {'U', Table.UPPER},
         {'L', Table.LOWER},
         {'M', Table.MIXED},
         {'P', Table.PUNCT},
         {'D', Table.DIGIT},
         {'B', Table.BINARY}
      };

      private AztecDetectorResult ddata;

      /// <summary>
      /// Decodes the specified detector result.
      /// </summary>
      /// <param name="detectorResult">The detector result.</param>
      /// <returns></returns>
      public DecoderResult decode(AztecDetectorResult detectorResult)
      {
         ddata = detectorResult;
         var matrix = detectorResult.Bits;
         var rawbits = extractBits(matrix);
         if (rawbits == null)
            return null;

         var correctedBits = correctBits(rawbits);
         if (correctedBits == null)
            return null;

         var result = getEncodedData(correctedBits);
         if (result == null)
            return null;

         var rawBytes = convertBoolArrayToByteArray(correctedBits);

         return new DecoderResult(rawBytes, correctedBits.Length, result, null, null);
      }

      /// <summary>
      /// This method is used for testing the high-level encoder
      /// </summary>
      /// <param name="correctedBits"></param>
      /// <returns></returns>
      public static String highLevelDecode(bool[] correctedBits)
      {
         return getEncodedData(correctedBits);
      }

      /// <summary>
      /// Gets the string encoded in the aztec code bits
      /// </summary>
      /// <param name="correctedBits">The corrected bits.</param>
      /// <returns>the decoded string</returns>
      private static String getEncodedData(bool[] correctedBits)
      {
         var endIndex = correctedBits.Length;
         var latchTable = Table.UPPER; // table most recently latched to
         var shiftTable = Table.UPPER; // table to use for the next read
         var strTable = UPPER_TABLE;
         var result = new StringBuilder(20);
         var index = 0;

         while (index < endIndex)
         {
            if (shiftTable == Table.BINARY)
            {
               if (endIndex - index < 5)
               {
                  break;
               }
               int length = readCode(correctedBits, index, 5);
               index += 5;
               if (length == 0)
               {
                  if (endIndex - index < 11)
                  {
                     break;
                  }
                  length = readCode(correctedBits, index, 11) + 31;
                  index += 11;
               }
               for (int charCount = 0; charCount < length; charCount++)
               {
                  if (endIndex - index < 8)
                  {
                     index = endIndex; // Force outer loop to exit
                     break;
                  }
                  int code = readCode(correctedBits, index, 8);
                  result.Append((char) code);
                  index += 8;
               }
               // Go back to whatever mode we had been in
               shiftTable = latchTable;
               strTable = codeTables[shiftTable];
            }
            else
            {
               int size = shiftTable == Table.DIGIT ? 4 : 5;
               if (endIndex - index < size)
               {
                  break;
               }
               int code = readCode(correctedBits, index, size);
               index += size;
               String str = getCharacter(strTable, code);
               if (str.StartsWith("CTRL_"))
               {
                  // Table changes
                  // ISO/IEC 24778:2008 prescribes ending a shift sequence in the mode from which it was invoked.
                  // That's including when that mode is a shift.
                  // Our test case dlusbs.png for issue #642 exercises that.
                  latchTable = shiftTable;  // Latch the current mode, so as to return to Upper after U/S B/S
                  shiftTable = getTable(str[5]);
                  strTable = codeTables[shiftTable];
                  if (str[6] == 'L')
                  {
                     latchTable = shiftTable;
                  }
               }
               else
               {
                  result.Append(str);
                  // Go back to whatever mode we had been in
                  shiftTable = latchTable;
                  strTable = codeTables[shiftTable];
               }
            }
         }
         return result.ToString();
      }

      /// <summary>
      /// gets the table corresponding to the char passed
      /// </summary>
      /// <param name="t">The t.</param>
      /// <returns></returns>
      private static Table getTable(char t)
      {
         if (!codeTableMap.ContainsKey(t))
            return codeTableMap['U'];
         return codeTableMap[t];
      }

      /// <summary>
      /// Gets the character (or string) corresponding to the passed code in the given table
      /// </summary>
      /// <param name="table">the table used</param>
      /// <param name="code">the code of the character</param>
      /// <returns></returns>
      private static String getCharacter(String[] table, int code)
      {
         return table[code];
      }

      /// <summary>
      ///Performs RS error correction on an array of bits.
      /// </summary>
      /// <param name="rawbits">The rawbits.</param>
      /// <returns>the corrected array</returns>
      private bool[] correctBits(bool[] rawbits)
      {
         GenericGF gf;
         int codewordSize;

         if (ddata.NbLayers <= 2)
         {
            codewordSize = 6;
            gf = GenericGF.AZTEC_DATA_6;
         }
         else if (ddata.NbLayers <= 8)
         {
            codewordSize = 8;
            gf = GenericGF.AZTEC_DATA_8;
         }
         else if (ddata.NbLayers <= 22)
         {
            codewordSize = 10;
            gf = GenericGF.AZTEC_DATA_10;
         }
         else
         {
            codewordSize = 12;
            gf = GenericGF.AZTEC_DATA_12;
         }

         int numDataCodewords = ddata.NbDatablocks;
         int numCodewords = rawbits.Length/codewordSize;
         if (numCodewords < numDataCodewords)
            return null;

         int offset = rawbits.Length%codewordSize;
         int numECCodewords = numCodewords - numDataCodewords;

         int[] dataWords = new int[numCodewords];
         for (int i = 0; i < numCodewords; i++, offset += codewordSize)
         {
            dataWords[i] = readCode(rawbits, offset, codewordSize);
         }

         var rsDecoder = new ReedSolomonDecoder(gf);
         if (!rsDecoder.decode(dataWords, numECCodewords))
            return null;

         // Now perform the unstuffing operation.
         // First, count how many bits are going to be thrown out as stuffing
         int mask = (1 << codewordSize) - 1;
         int stuffedBits = 0;
         for (int i = 0; i < numDataCodewords; i++)
         {
            int dataWord = dataWords[i];
            if (dataWord == 0 || dataWord == mask)
            {
               return null;
            }
            else if (dataWord == 1 || dataWord == mask - 1)
            {
               stuffedBits++;
            }
         }
         // Now, actually unpack the bits and remove the stuffing
         bool[] correctedBits = new bool[numDataCodewords*codewordSize - stuffedBits];
         int index = 0;
         for (int i = 0; i < numDataCodewords; i++)
         {
            int dataWord = dataWords[i];
            if (dataWord == 1 || dataWord == mask - 1)
            {
               // next codewordSize-1 bits are all zeros or all ones
               SupportClass.Fill(correctedBits, index, index + codewordSize - 1, dataWord > 1);
               index += codewordSize - 1;
            }
            else
            {
               for (int bit = codewordSize - 1; bit >= 0; --bit)
               {
                  correctedBits[index++] = (dataWord & (1 << bit)) != 0;
               }
            }
         }

         if (index != correctedBits.Length)
            return null;

         return correctedBits;
      }

      /// <summary>
      /// Gets the array of bits from an Aztec Code matrix
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <returns>the array of bits</returns>
      private bool[] extractBits(BitMatrix matrix)
      {
         bool compact = ddata.Compact;
         int layers = ddata.NbLayers;
         int baseMatrixSize = (compact ? 11 : 14) + layers*4; // not including alignment lines
         int[] alignmentMap = new int[baseMatrixSize];
         bool[] rawbits = new bool[totalBitsInLayer(layers, compact)];

         if (compact)
         {
            for (int i = 0; i < alignmentMap.Length; i++)
            {
               alignmentMap[i] = i;
            }
         }
         else
         {
            int matrixSize = baseMatrixSize + 1 + 2*((baseMatrixSize/2 - 1)/15);
            int origCenter = baseMatrixSize/2;
            int center = matrixSize/2;
            for (int i = 0; i < origCenter; i++)
            {
               int newOffset = i + i/15;
               alignmentMap[origCenter - i - 1] = center - newOffset - 1;
               alignmentMap[origCenter + i] = center + newOffset + 1;
            }
         }
         for (int i = 0, rowOffset = 0; i < layers; i++)
         {
            int rowSize = (layers - i)*4 + (compact ? 9 : 12);
            // The top-left most point of this layer is <low, low> (not including alignment lines)
            int low = i*2;
            // The bottom-right most point of this layer is <high, high> (not including alignment lines)
            int high = baseMatrixSize - 1 - low;
            // We pull bits from the two 2 x rowSize columns and two rowSize x 2 rows
            for (int j = 0; j < rowSize; j++)
            {
               int columnOffset = j*2;
               for (int k = 0; k < 2; k++)
               {
                  // left column
                  rawbits[rowOffset + columnOffset + k] =
                     matrix[alignmentMap[low + k], alignmentMap[low + j]];
                  // bottom row
                  rawbits[rowOffset + 2*rowSize + columnOffset + k] =
                     matrix[alignmentMap[low + j], alignmentMap[high - k]];
                  // right column
                  rawbits[rowOffset + 4*rowSize + columnOffset + k] =
                     matrix[alignmentMap[high - k], alignmentMap[high - j]];
                  // top row
                  rawbits[rowOffset + 6*rowSize + columnOffset + k] =
                     matrix[alignmentMap[high - j], alignmentMap[low + k]];
               }
            }
            rowOffset += rowSize*8;
         }
         return rawbits;
      }

      /// <summary>
      /// Reads a code of given length and at given index in an array of bits
      /// </summary>
      /// <param name="rawbits">The rawbits.</param>
      /// <param name="startIndex">The start index.</param>
      /// <param name="length">The length.</param>
      /// <returns></returns>
      private static int readCode(bool[] rawbits, int startIndex, int length)
      {
         int res = 0;
         for (int i = startIndex; i < startIndex + length; i++)
         {
            res <<= 1;
            if (rawbits[i])
            {
               res++;
            }
         }
         return res;
      }

      /// <summary>
      /// Reads a code of length 8 in an array of bits, padding with zeros
      /// </summary>
      /// <param name="rawbits"></param>
      /// <param name="startIndex"></param>
      /// <returns></returns>
      private static byte readByte(bool[] rawbits, int startIndex)
      {
         int n = rawbits.Length - startIndex;
         if (n >= 8)
         {
            return (byte) readCode(rawbits, startIndex, 8);
         }
         return (byte) (readCode(rawbits, startIndex, n) << (8 - n));
      }

      /// <summary>
      /// Packs a bit array into bytes, most significant bit first
      /// </summary>
      /// <param name="boolArr"></param>
      /// <returns></returns>
      internal static byte[] convertBoolArrayToByteArray(bool[] boolArr)
      {
         byte[] byteArr = new byte[(boolArr.Length + 7)/8];
         for (int i = 0; i < byteArr.Length; i++)
         {
            byteArr[i] = readByte(boolArr, 8*i);
         }
         return byteArr;
      }

      private static int totalBitsInLayer(int layers, bool compact)
      {
         return ((compact ? 88 : 112) + 16*layers)*layers;
      }
   }
}