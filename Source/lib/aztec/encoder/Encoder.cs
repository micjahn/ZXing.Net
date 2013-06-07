/*
 * Copyright 2013 ZXing authors
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

using ZXing.Common;
using ZXing.Common.ReedSolomon;

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// Generates Aztec 2D barcodes.
   /// </summary>
   /// <author>Rustam Abdullaev</author>
   public static class Encoder
   {
      public const int DEFAULT_EC_PERCENT = 33; // default minimal percentage of error check words

      private static readonly int[] NB_BITS; // total bits per compact symbol for a given number of layers
      private static readonly int[] NB_BITS_COMPACT; // total bits per full symbol for a given number of layers

      static Encoder()
      {
         NB_BITS_COMPACT = new int[5];
         for (int i = 1; i < NB_BITS_COMPACT.Length; i++)
         {
            NB_BITS_COMPACT[i] = (88 + 16 * i) * i;
         }
         NB_BITS = new int[33];
         for (int i = 1; i < NB_BITS.Length; i++)
         {
            NB_BITS[i] = (112 + 16 * i) * i;
         }
      }

      private static readonly int[] WORD_SIZE = {
                                                   4, 6, 6, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                                                   12, 12, 12, 12, 12, 12, 12, 12, 12, 12
                                                };

      /// <summary>
      /// Encodes the given binary content as an Aztec symbol
      /// </summary>
      /// <param name="data">input data string</param>
      /// <returns>Aztec symbol matrix with metadata</returns>

      public static AztecCode encode(byte[] data)
      {
         return encode(data, DEFAULT_EC_PERCENT);
      }

      /// <summary>
      /// Encodes the given binary content as an Aztec symbol
      /// </summary>
      /// <param name="data">input data string</param>
      /// <param name="minECCPercent">minimal percentange of error check words (According to ISO/IEC 24778:2008,
      /// a minimum of 23% + 3 words is recommended)</param>
      /// <returns>Aztec symbol matrix with metadata</returns>
      public static AztecCode encode(byte[] data, int minECCPercent)
      {
         // High-level encode
         var bits = new HighLevelEncoder(data).encode();

         // stuff bits and choose symbol size
         int eccBits = bits.Size * minECCPercent / 100 + 11;
         int totalSizeBits = bits.Size + eccBits;
         int layers;
         int wordSize = 0;
         int totalSymbolBits = 0;
         BitArray stuffedBits = null;
         for (layers = 1; layers < NB_BITS_COMPACT.Length; layers++)
         {
            if (NB_BITS_COMPACT[layers] >= totalSizeBits)
            {
               if (wordSize != WORD_SIZE[layers])
               {
                  wordSize = WORD_SIZE[layers];
                  stuffedBits = stuffBits(bits, wordSize);
               }
               totalSymbolBits = NB_BITS_COMPACT[layers];
               if (stuffedBits.Size + eccBits <= NB_BITS_COMPACT[layers])
               {
                  break;
               }
            }
         }
         bool compact = true;
         if (layers == NB_BITS_COMPACT.Length)
         {
            compact = false;
            for (layers = 1; layers < NB_BITS.Length; layers++)
            {
               if (NB_BITS[layers] >= totalSizeBits)
               {
                  if (wordSize != WORD_SIZE[layers])
                  {
                     wordSize = WORD_SIZE[layers];
                     stuffedBits = stuffBits(bits, wordSize);
                  }
                  totalSymbolBits = NB_BITS[layers];
                  if (stuffedBits.Size + eccBits <= NB_BITS[layers])
                  {
                     break;
                  }
               }
            }
         }
         if (layers == NB_BITS.Length)
         {
            throw new ArgumentException("Data too large for an Aztec code");
         }

         // pad the end
         int messageSizeInWords = (stuffedBits.Size + wordSize - 1) / wordSize;
         for (int i = messageSizeInWords * wordSize - stuffedBits.Size; i > 0; i--)
         {
            stuffedBits.appendBit(true);
         }

         // generate check words
         var rs = new ReedSolomonEncoder(getGF(wordSize));
         var totalSizeInFullWords = totalSymbolBits / wordSize;
         var messageWords = bitsToWords(stuffedBits, wordSize, totalSizeInFullWords);
         rs.encode(messageWords, totalSizeInFullWords - messageSizeInWords);

         // convert to bit array and pad in the beginning
         var startPad = totalSymbolBits % wordSize;
         var messageBits = new BitArray();
         messageBits.appendBits(0, startPad);
         foreach (var messageWord in messageWords)
         {
            messageBits.appendBits(messageWord, wordSize);
         }

         // generate mode message
         var modeMessage = generateModeMessage(compact, layers, messageSizeInWords);

         // allocate symbol
         var baseMatrixSize = compact ? 11 + layers * 4 : 14 + layers * 4; // not including alignment lines
         var alignmentMap = new int[baseMatrixSize];
         int matrixSize;
         if (compact)
         {
            // no alignment marks in compact mode, alignmentMap is a no-op
            matrixSize = baseMatrixSize;
            for (int i = 0; i < alignmentMap.Length; i++)
            {
               alignmentMap[i] = i;
            }
         }
         else
         {
            matrixSize = baseMatrixSize + 1 + 2 * ((baseMatrixSize / 2 - 1) / 15);
            int origCenter = baseMatrixSize / 2;
            int center = matrixSize / 2;
            for (int i = 0; i < origCenter; i++)
            {
               int newOffset = i + i / 15;
               alignmentMap[origCenter - i - 1] = center - newOffset - 1;
               alignmentMap[origCenter + i] = center + newOffset + 1;
            }
         }
         var matrix = new BitMatrix(matrixSize);

         // draw mode and data bits
         for (int i = 0, rowOffset = 0; i < layers; i++)
         {
            int rowSize = compact ? (layers - i) * 4 + 9 : (layers - i) * 4 + 12;
            for (int j = 0; j < rowSize; j++)
            {
               int columnOffset = j * 2;
               for (int k = 0; k < 2; k++)
               {
                  if (messageBits[rowOffset + columnOffset + k])
                  {
                     matrix[alignmentMap[i * 2 + k], alignmentMap[i * 2 + j]] = true;
                  }
                  if (messageBits[rowOffset + rowSize * 2 + columnOffset + k])
                  {
                     matrix[alignmentMap[i * 2 + j], alignmentMap[baseMatrixSize - 1 - i * 2 - k]] = true;
                  }
                  if (messageBits[rowOffset + rowSize * 4 + columnOffset + k])
                  {
                     matrix[alignmentMap[baseMatrixSize - 1 - i * 2 - k], alignmentMap[baseMatrixSize - 1 - i * 2 - j]] = true;
                  }
                  if (messageBits[rowOffset + rowSize * 6 + columnOffset + k])
                  {
                     matrix[alignmentMap[baseMatrixSize - 1 - i * 2 - j], alignmentMap[i * 2 + k]] = true;
                  }
               }
            }
            rowOffset += rowSize * 8;
         }
         drawModeMessage(matrix, compact, matrixSize, modeMessage);

         // draw alignment marks
         if (compact)
         {
            drawBullsEye(matrix, matrixSize / 2, 5);
         }
         else
         {
            drawBullsEye(matrix, matrixSize / 2, 7);
            for (int i = 0, j = 0; i < baseMatrixSize / 2 - 1; i += 15, j += 16)
            {
               for (int k = (matrixSize / 2) & 1; k < matrixSize; k += 2)
               {
                  matrix[matrixSize / 2 - j, k] = true;
                  matrix[matrixSize / 2 + j, k] = true;
                  matrix[k, matrixSize / 2 - j] = true;
                  matrix[k, matrixSize / 2 + j] = true;
               }
            }
         }

         return new AztecCode
                        {
                           isCompact = compact,
                           Size = matrixSize,
                           Layers = layers,
                           CodeWords = messageSizeInWords,
                           Matrix = matrix
                        };
      }

      private static void drawBullsEye(BitMatrix matrix, int center, int size)
      {
         for (var i = 0; i < size; i += 2)
         {
            for (var j = center - i; j <= center + i; j++)
            {
               matrix[j, center - i] = true;
               matrix[j, center + i] = true;
               matrix[center - i, j] = true;
               matrix[center + i, j] = true;
            }
         }
         matrix[center - size, center - size] = true;
         matrix[center - size + 1, center - size] = true;
         matrix[center - size, center - size + 1] = true;
         matrix[center + size, center - size] = true;
         matrix[center + size, center - size + 1] = true;
         matrix[center + size, center + size - 1] = true;
      }

      internal static BitArray generateModeMessage(bool compact, int layers, int messageSizeInWords)
      {
         var modeMessage = new BitArray();
         if (compact)
         {
            modeMessage.appendBits(layers - 1, 2);
            modeMessage.appendBits(messageSizeInWords - 1, 6);
            modeMessage = generateCheckWords(modeMessage, 28, 4);
         }
         else
         {
            modeMessage.appendBits(layers - 1, 5);
            modeMessage.appendBits(messageSizeInWords - 1, 11);
            modeMessage = generateCheckWords(modeMessage, 40, 4);
         }
         return modeMessage;
      }

      private static void drawModeMessage(BitMatrix matrix, bool compact, int matrixSize, BitArray modeMessage)
      {
         if (compact)
         {
            for (var i = 0; i < 7; i++)
            {
               if (modeMessage[i])
               {
                  matrix[matrixSize / 2 - 3 + i, matrixSize / 2 - 5] = true;
               }
               if (modeMessage[i + 7])
               {
                  matrix[matrixSize / 2 + 5, matrixSize / 2 - 3 + i] = true;
               }
               if (modeMessage[20 - i])
               {
                  matrix[matrixSize / 2 - 3 + i, matrixSize / 2 + 5] = true;
               }
               if (modeMessage[27 - i])
               {
                  matrix[matrixSize / 2 - 5, matrixSize / 2 - 3 + i] = true;
               }
            }
         }
         else
         {
            for (var i = 0; i < 10; i++)
            {
               if (modeMessage[i])
               {
                  matrix[matrixSize / 2 - 5 + i + i / 5, matrixSize / 2 - 7] = true;
               }
               if (modeMessage[i + 10])
               {
                  matrix[matrixSize / 2 + 7, matrixSize / 2 - 5 + i + i / 5] = true;
               }
               if (modeMessage[29 - i])
               {
                  matrix[matrixSize / 2 - 5 + i + i / 5, matrixSize / 2 + 7] = true;
               }
               if (modeMessage[39 - i])
               {
                  matrix[matrixSize / 2 - 7, matrixSize / 2 - 5 + i + i / 5] = true;
               }
            }
         }
      }

      private static BitArray generateCheckWords(BitArray stuffedBits, int totalSymbolBits, int wordSize)
      {
         var messageSizeInWords = (stuffedBits.Size + wordSize - 1) / wordSize;
         for (var i = messageSizeInWords * wordSize - stuffedBits.Size; i > 0; i--)
         {
            stuffedBits.appendBit(true);
         }

         var rs = new ReedSolomonEncoder(getGF(wordSize));
         var totalSizeInFullWords = totalSymbolBits / wordSize;
         var messageWords = bitsToWords(stuffedBits, wordSize, totalSizeInFullWords);
         rs.encode(messageWords, totalSizeInFullWords - messageSizeInWords);

         var startPad = totalSymbolBits % wordSize;
         var messageBits = new BitArray();
         messageBits.appendBits(0, startPad);
         foreach (var messageWord in messageWords)
         {
            messageBits.appendBits(messageWord, wordSize);
         }
         return messageBits;
      }

      private static int[] bitsToWords(BitArray stuffedBits, int wordSize, int totalWords)
      {
         var message = new int[totalWords];
         int i;
         int n;
         for (i = 0, n = stuffedBits.Size / wordSize; i < n; i++)
         {
            int value = 0;
            for (int j = 0; j < wordSize; j++)
            {
               value |= stuffedBits[i * wordSize + j] ? (1 << wordSize - j - 1) : 0;
            }
            message[i] = value;
         }
         return message;
      }

      private static GenericGF getGF(int wordSize)
      {
         switch (wordSize)
         {
            case 4:
               return GenericGF.AZTEC_PARAM;
            case 6:
               return GenericGF.AZTEC_DATA_6;
            case 8:
               return GenericGF.AZTEC_DATA_8;
            case 10:
               return GenericGF.AZTEC_DATA_10;
            case 12:
               return GenericGF.AZTEC_DATA_12;
            default:
               return null;
         }
      }

      internal static BitArray stuffBits(BitArray bits, int wordSize)
      {
         var @out = new BitArray();

         // 1. stuff the bits
         int n = bits.Size;
         int mask = (1 << wordSize) - 2;
         for (int i = 0; i < n; i += wordSize)
         {
            int word = 0;
            for (int j = 0; j < wordSize; j++)
            {
               if (i + j >= n || bits[i + j])
               {
                  word |= 1 << (wordSize - 1 - j);
               }
            }
            if ((word & mask) == mask)
            {
               @out.appendBits(word & mask, wordSize);
               i--;
            }
            else if ((word & mask) == 0)
            {
               @out.appendBits(word | 1, wordSize);
               i--;
            }
            else
            {
               @out.appendBits(word, wordSize);
            }
         }

         // 2. pad last word to wordSize
         n = @out.Size;
         int remainder = n % wordSize;
         if (remainder != 0)
         {
            int j = 1;
            for (int i = 0; i < remainder; i++)
            {
               if (!@out[n - 1 - i])
               {
                  j = 0;
               }
            }
            for (int i = remainder; i < wordSize - 1; i++)
            {
               @out.appendBit(true);
            }
            @out.appendBit(j == 0);
         }
         return @out;
      }
   }
}