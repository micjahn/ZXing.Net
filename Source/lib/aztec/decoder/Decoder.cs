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
using System.Text;
using com.google.zxing.common;
using com.google.zxing.common.reedsolomon;

namespace com.google.zxing.aztec.decoder
{
   /**
    * <p>The main class which implements Aztec Code decoding -- as opposed to locating and extracting
    * the Aztec Code from an image.</p>
    *
    * @author David Olivier
    */
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

      private static int[] NB_BITS_COMPACT = {
      0, 104, 240, 408, 608
  };

      private static int[] NB_BITS = {
      0, 128, 288, 480, 704, 960, 1248, 1568, 1920, 2304, 2720, 3168, 3648, 4160, 4704, 5280, 5888, 6528,
      7200, 7904, 8640, 9408, 10208, 11040, 11904, 12800, 13728, 14688, 15680, 16704, 17760, 18848, 19968
  };

      private static int[] NB_DATABLOCK_COMPACT = {
      0, 17, 40, 51, 76
  };

      private static int[] NB_DATABLOCK = {
      0, 21, 48, 60, 88, 120, 156, 196, 240, 230, 272, 316, 364, 416, 470, 528, 588, 652, 720, 790, 864,
      940, 1020, 920, 992, 1066, 1144, 1224, 1306, 1392, 1480, 1570, 1664
  };

      private static String[] UPPER_TABLE = {
      "CTRL_PS", " ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
      "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "CTRL_LL", "CTRL_ML", "CTRL_DL", "CTRL_BS"
  };

      private static String[] LOWER_TABLE = {
      "CTRL_PS", " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
      "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "CTRL_US", "CTRL_ML", "CTRL_DL", "CTRL_BS"
  };

      private static String[] MIXED_TABLE = {
      "CTRL_PS", " ", "\x1", "\x2", "\x3", "\x4", "\x5", "\x6", "\x7", "\b", "\t", "\n",
      "\xD", "\f", "\r", "\x21", "\x22", "\x23", "\x24", "\x25", "@", "\\", "^", "_",
      "`", "|", "~", "\xB1", "CTRL_LL", "CTRL_UL", "CTRL_PL", "CTRL_BS"
  };

      private static String[] PUNCT_TABLE = {
      "", "\r", "\r\n", ". ", ", ", ": ", "!", "\"", "#", "$", "%", "&", "'", "(", ")",
      "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "[", "]", "{", "}", "CTRL_UL"
  };

      private static String[] DIGIT_TABLE = {
    "CTRL_PS", " ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", ".", "CTRL_UL", "CTRL_US"
  };

      private int numCodewords;
      private int codewordSize;
      private AztecDetectorResult ddata;
      private int invertedBitCount;

      public DecoderResult decode(AztecDetectorResult detectorResult)
      {
         ddata = detectorResult;
         BitMatrix matrix = detectorResult.Bits;

         if (!ddata.isCompact())
         {
            matrix = removeDashedLines(ddata.Bits);
         }

         bool[] rawbits = extractBits(matrix);

         bool[] correctedBits = correctBits(rawbits);

         String result = getEncodedData(correctedBits);

         return new DecoderResult(null, result, null, null);
      }


      /**
       *
       * Gets the string encoded in the aztec code bits
       *
       * @return the decoded string
       * @throws FormatException if the input is not valid
       */
      private String getEncodedData(bool[] correctedBits)
      {

         int endIndex = codewordSize * ddata.getNbDatablocks() - invertedBitCount;
         if (endIndex > correctedBits.Length)
         {
            throw FormatException.Instance;
         }

         Table lastTable = Table.UPPER;
         Table table = Table.UPPER;
         int startIndex = 0;
         StringBuilder result = new StringBuilder(20);
         bool end = false;
         bool shift = false;
         bool switchShift = false;

         while (!end)
         {

            if (shift)
            {
               // the table is for the next character only
               switchShift = true;
            }
            else
            {
               // save the current table in case next one is a shift
               lastTable = table;
            }

            int code;
            switch (table)
            {
               case Table.BINARY:
                  if (endIndex - startIndex < 8)
                  {
                     end = true;
                     break;
                  }
                  code = readCode(correctedBits, startIndex, 8);
                  startIndex += 8;

                  result.Append((char)code);
                  break;

               default:
                  int size = 5;

                  if (table == Table.DIGIT)
                  {
                     size = 4;
                  }

                  if (endIndex - startIndex < size)
                  {
                     end = true;
                     break;
                  }

                  code = readCode(correctedBits, startIndex, size);
                  startIndex += size;

                  String str = getCharacter(table, code);
                  if (str.StartsWith("CTRL_"))
                  {
                     // Table changes
                     table = getTable(str[5]);

                     if (str[6] == 'S')
                     {
                        shift = true;
                     }
                  }
                  else
                  {
                     result.Append(str);
                  }

                  break;
            }

            if (switchShift)
            {
               table = lastTable;
               shift = false;
               switchShift = false;
            }

         }
         return result.ToString();
      }


      /**
       * gets the table corresponding to the char passed
       */
      private static Table getTable(char t)
      {
         switch (t)
         {
            case 'L':
               return Table.LOWER;
            case 'P':
               return Table.PUNCT;
            case 'M':
               return Table.MIXED;
            case 'D':
               return Table.DIGIT;
            case 'B':
               return Table.BINARY;
            case 'U':
            default:
               return Table.UPPER;
         }
      }

      /**
       *
       * Gets the character (or string) corresponding to the passed code in the given table
       *
       * @param table the table used
       * @param code the code of the character
       */
      private static String getCharacter(Table table, int code)
      {
         switch (table)
         {
            case Table.UPPER:
               return UPPER_TABLE[code];
            case Table.LOWER:
               return LOWER_TABLE[code];
            case Table.MIXED:
               return MIXED_TABLE[code];
            case Table.PUNCT:
               return PUNCT_TABLE[code];
            case Table.DIGIT:
               return DIGIT_TABLE[code];
            default:
               return "";
         }
      }

      /**
       *
       * <p> performs RS error correction on an array of bits </p>
       *
       * @return the corrected array
       * @throws FormatException if the input contains too many errors
       */
      private bool[] correctBits(bool[] rawbits)
      {
         GenericGF gf;

         if (ddata.getNbLayers() <= 2)
         {
            codewordSize = 6;
            gf = GenericGF.AZTEC_DATA_6;
         }
         else if (ddata.getNbLayers() <= 8)
         {
            codewordSize = 8;
            gf = GenericGF.AZTEC_DATA_8;
         }
         else if (ddata.getNbLayers() <= 22)
         {
            codewordSize = 10;
            gf = GenericGF.AZTEC_DATA_10;
         }
         else
         {
            codewordSize = 12;
            gf = GenericGF.AZTEC_DATA_12;
         }

         int numDataCodewords = ddata.getNbDatablocks();
         int numECCodewords;
         int offset;

         if (ddata.isCompact())
         {
            offset = NB_BITS_COMPACT[ddata.getNbLayers()] - numCodewords * codewordSize;
            numECCodewords = NB_DATABLOCK_COMPACT[ddata.getNbLayers()] - numDataCodewords;
         }
         else
         {
            offset = NB_BITS[ddata.getNbLayers()] - numCodewords * codewordSize;
            numECCodewords = NB_DATABLOCK[ddata.getNbLayers()] - numDataCodewords;
         }

         int[] dataWords = new int[numCodewords];
         for (int i = 0; i < numCodewords; i++)
         {
            int flag = 1;
            for (int j = 1; j <= codewordSize; j++)
            {
               if (rawbits[codewordSize * i + codewordSize - j + offset])
               {
                  dataWords[i] += flag;
               }
               flag <<= 1;
            }

            //if (dataWords[i] >= flag) {
            //  flag++;
            //}
         }

         try
         {
            ReedSolomonDecoder rsDecoder = new ReedSolomonDecoder(gf);
            rsDecoder.decode(dataWords, numECCodewords);
         }
         catch (ReedSolomonException )
         {
            throw FormatException.Instance;
         }

         offset = 0;
         invertedBitCount = 0;

         bool[] correctedBits = new bool[numDataCodewords * codewordSize];
         for (int i = 0; i < numDataCodewords; i++)
         {

            bool seriesColor = false;
            int seriesCount = 0;
            int flag = 1 << (codewordSize - 1);

            for (int j = 0; j < codewordSize; j++)
            {

               bool color = (dataWords[i] & flag) == flag;

               if (seriesCount == codewordSize - 1)
               {

                  if (color == seriesColor)
                  {
                     //bit must be inverted
                     throw FormatException.Instance;
                  }

                  seriesColor = false;
                  seriesCount = 0;
                  offset++;
                  invertedBitCount++;
               }
               else
               {

                  if (seriesColor == color)
                  {
                     seriesCount++;
                  }
                  else
                  {
                     seriesCount = 1;
                     seriesColor = color;
                  }

                  correctedBits[i * codewordSize + j - offset] = color;

               }

               flag = (int)((uint)flag >> 1); // flag >>>= 1;
            }
         }

         return correctedBits;
      }

      /**
       *
       * Gets the array of bits from an Aztec Code matrix
       *
       * @return the array of bits
       * @throws FormatException if the matrix is not a valid aztec code
       */
      private bool[] extractBits(BitMatrix matrix)
      {

         bool[] rawbits;
         if (ddata.isCompact())
         {
            if (ddata.getNbLayers() > NB_BITS_COMPACT.Length)
            {
               throw FormatException.Instance;
            }
            rawbits = new bool[NB_BITS_COMPACT[ddata.getNbLayers()]];
            numCodewords = NB_DATABLOCK_COMPACT[ddata.getNbLayers()];
         }
         else
         {
            if (ddata.getNbLayers() > NB_BITS.Length)
            {
               throw FormatException.Instance;
            }
            rawbits = new bool[NB_BITS[ddata.getNbLayers()]];
            numCodewords = NB_DATABLOCK[ddata.getNbLayers()];
         }

         int layer = ddata.getNbLayers();
         int size = matrix.Height;
         int rawbitsOffset = 0;
         int matrixOffset = 0;

         while (layer != 0)
         {

            int flip = 0;
            for (int i = 0; i < 2 * size - 4; i++)
            {
               rawbits[rawbitsOffset + i] = matrix[matrixOffset + flip, matrixOffset + i / 2];
               rawbits[rawbitsOffset + 2 * size - 4 + i] = matrix[matrixOffset + i / 2, matrixOffset + size - 1 - flip];
               flip = (flip + 1) % 2;
            }

            flip = 0;
            for (int i = 2 * size + 1; i > 5; i--)
            {
               rawbits[rawbitsOffset + 4 * size - 8 + (2 * size - i) + 1] = matrix[matrixOffset + size - 1 - flip, matrixOffset + i / 2 - 1];
               rawbits[rawbitsOffset + 6 * size - 12 + (2 * size - i) + 1] = matrix[matrixOffset + i / 2 - 1, matrixOffset + flip];
               flip = (flip + 1) % 2;
            }

            matrixOffset += 2;
            rawbitsOffset += 8 * size - 16;
            layer--;
            size -= 4;
         }

         return rawbits;
      }


      /**
       * Transforms an Aztec code matrix by removing the control dashed lines
       */
      private static BitMatrix removeDashedLines(BitMatrix matrix)
      {
         int nbDashed = 1 + 2 * ((matrix.Width - 1) / 2 / 16);
         BitMatrix newMatrix = new BitMatrix(matrix.Width - nbDashed, matrix.Height - nbDashed);

         int nx = 0;

         for (int x = 0; x < matrix.Width; x++)
         {

            if ((matrix.Width / 2 - x) % 16 == 0)
            {
               continue;
            }

            int ny = 0;
            for (int y = 0; y < matrix.Height; y++)
            {

               if ((matrix.Width / 2 - y) % 16 == 0)
               {
                  continue;
               }

               newMatrix[nx, ny] = matrix[x, y];
               ny++;
            }
            nx++;
         }

         return newMatrix;
      }

      /**
       * Reads a code of given length and at given index in an array of bits
       */
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

   }
}