/*
 * Copyright 2006 Jeremias Maerki
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

namespace ZXing.Datamatrix.Encoder
{
   /// <summary>
   /// Symbol info table for DataMatrix.
   /// </summary>
   public class SymbolInfo
   {
      internal static readonly SymbolInfo[] PROD_SYMBOLS = {
                                                         new SymbolInfo(false, 3, 5, 8, 8, 1),
                                                         new SymbolInfo(false, 5, 7, 10, 10, 1),
                                                         /*rect*/new SymbolInfo(true, 5, 7, 16, 6, 1),
                                                         new SymbolInfo(false, 8, 10, 12, 12, 1),
                                                         /*rect*/new SymbolInfo(true, 10, 11, 14, 6, 2),
                                                         new SymbolInfo(false, 12, 12, 14, 14, 1),
                                                         /*rect*/new SymbolInfo(true, 16, 14, 24, 10, 1),

                                                         new SymbolInfo(false, 18, 14, 16, 16, 1),
                                                         new SymbolInfo(false, 22, 18, 18, 18, 1),
                                                         /*rect*/new SymbolInfo(true, 22, 18, 16, 10, 2),
                                                         new SymbolInfo(false, 30, 20, 20, 20, 1),
                                                         /*rect*/new SymbolInfo(true, 32, 24, 16, 14, 2),
                                                         new SymbolInfo(false, 36, 24, 22, 22, 1),
                                                         new SymbolInfo(false, 44, 28, 24, 24, 1),
                                                         /*rect*/new SymbolInfo(true, 49, 28, 22, 14, 2),

                                                         new SymbolInfo(false, 62, 36, 14, 14, 4),
                                                         new SymbolInfo(false, 86, 42, 16, 16, 4),
                                                         new SymbolInfo(false, 114, 48, 18, 18, 4),
                                                         new SymbolInfo(false, 144, 56, 20, 20, 4),
                                                         new SymbolInfo(false, 174, 68, 22, 22, 4),

                                                         new SymbolInfo(false, 204, 84, 24, 24, 4, 102, 42),
                                                         new SymbolInfo(false, 280, 112, 14, 14, 16, 140, 56),
                                                         new SymbolInfo(false, 368, 144, 16, 16, 16, 92, 36),
                                                         new SymbolInfo(false, 456, 192, 18, 18, 16, 114, 48),
                                                         new SymbolInfo(false, 576, 224, 20, 20, 16, 144, 56),
                                                         new SymbolInfo(false, 696, 272, 22, 22, 16, 174, 68),
                                                         new SymbolInfo(false, 816, 336, 24, 24, 16, 136, 56),
                                                         new SymbolInfo(false, 1050, 408, 18, 18, 36, 175, 68),
                                                         new SymbolInfo(false, 1304, 496, 20, 20, 36, 163, 62),
                                                         new DataMatrixSymbolInfo144(),
                                                      };

      private static SymbolInfo[] symbols = PROD_SYMBOLS;

      private readonly bool rectangular;
      internal readonly int dataCapacity;
      internal readonly int errorCodewords;
      public readonly int matrixWidth;
      public readonly int matrixHeight;
      private readonly int dataRegions;
      private readonly int rsBlockData;
      private readonly int rsBlockError;

      /**
       * Overrides the symbol info set used by this class. Used for testing purposes.
       *
       * @param override the symbol info set to use
       */
      public static void overrideSymbolSet(SymbolInfo[] @override)
      {
         symbols = @override;
      }

      public SymbolInfo(bool rectangular, int dataCapacity, int errorCodewords,
                        int matrixWidth, int matrixHeight, int dataRegions)
         : this(rectangular, dataCapacity, errorCodewords, matrixWidth, matrixHeight, dataRegions,
              dataCapacity, errorCodewords)
      {
      }

      internal SymbolInfo(bool rectangular, int dataCapacity, int errorCodewords,
                         int matrixWidth, int matrixHeight, int dataRegions,
                         int rsBlockData, int rsBlockError)
      {
         this.rectangular = rectangular;
         this.dataCapacity = dataCapacity;
         this.errorCodewords = errorCodewords;
         this.matrixWidth = matrixWidth;
         this.matrixHeight = matrixHeight;
         this.dataRegions = dataRegions;
         this.rsBlockData = rsBlockData;
         this.rsBlockError = rsBlockError;
      }

      public static SymbolInfo lookup(int dataCodewords)
      {
         return lookup(dataCodewords, SymbolShapeHint.FORCE_NONE, true);
      }

      public static SymbolInfo lookup(int dataCodewords, SymbolShapeHint shape)
      {
         return lookup(dataCodewords, shape, true);
      }

      public static SymbolInfo lookup(int dataCodewords, bool allowRectangular, bool fail)
      {
         SymbolShapeHint shape = allowRectangular
             ? SymbolShapeHint.FORCE_NONE : SymbolShapeHint.FORCE_SQUARE;
         return lookup(dataCodewords, shape, fail);
      }

      private static SymbolInfo lookup(int dataCodewords, SymbolShapeHint shape, bool fail)
      {
         return lookup(dataCodewords, shape, null, null, fail);
      }

      public static SymbolInfo lookup(int dataCodewords,
                                      SymbolShapeHint shape,
                                      Dimension minSize,
                                      Dimension maxSize,
                                      bool fail)
      {
         foreach (SymbolInfo symbol in symbols)
         {
            if (shape == SymbolShapeHint.FORCE_SQUARE && symbol.rectangular)
            {
               continue;
            }
            if (shape == SymbolShapeHint.FORCE_RECTANGLE && !symbol.rectangular)
            {
               continue;
            }
            if (minSize != null
                && (symbol.getSymbolWidth() < minSize.Width
                || symbol.getSymbolHeight() < minSize.Height))
            {
               continue;
            }
            if (maxSize != null
                && (symbol.getSymbolWidth() > maxSize.Width
                || symbol.getSymbolHeight() > maxSize.Height))
            {
               continue;
            }
            if (dataCodewords <= symbol.dataCapacity)
            {
               return symbol;
            }
         }
         if (fail)
         {
            throw new ArgumentException(
                "Can't find a symbol arrangement that matches the message. Data codewords: "
                    + dataCodewords);
         }
         return null;
      }

      private int getHorizontalDataRegions()
      {
         switch (dataRegions)
         {
            case 1:
               return 1;
            case 2:
            case 4:
               return 2;
            case 16:
               return 4;
            case 36:
               return 6;
            default:
               throw new InvalidOperationException("Cannot handle this number of data regions");
         }
      }

      private int getVerticalDataRegions()
      {
         switch (dataRegions)
         {
            case 1:
            case 2:
               return 1;
            case 4:
               return 2;
            case 16:
               return 4;
            case 36:
               return 6;
            default:
               throw new InvalidOperationException("Cannot handle this number of data regions");
         }
      }
      
      public int getSymbolDataWidth()
      {
         return getHorizontalDataRegions() * matrixWidth;
      }

      public int getSymbolDataHeight()
      {
         return getVerticalDataRegions() * matrixHeight;
      }

      public int getSymbolWidth()
      {
         return getSymbolDataWidth() + (getHorizontalDataRegions() * 2);
      }

      public int getSymbolHeight()
      {
         return getSymbolDataHeight() + (getVerticalDataRegions() * 2);
      }

      public int getCodewordCount()
      {
         return dataCapacity + errorCodewords;
      }

      virtual public int getInterleavedBlockCount()
      {
         return dataCapacity / rsBlockData;
      }

      virtual public int getDataLengthForInterleavedBlock(int index)
      {
         return rsBlockData;
      }

      public int getErrorLengthForInterleavedBlock(int index)
      {
         return rsBlockError;
      }

      public override String ToString()
      {
         var sb = new StringBuilder();
         sb.Append(rectangular ? "Rectangular Symbol:" : "Square Symbol:");
         sb.Append(" data region ").Append(matrixWidth).Append('x').Append(matrixHeight);
         sb.Append(", symbol size ").Append(getSymbolWidth()).Append('x').Append(getSymbolHeight());
         sb.Append(", symbol data size ").Append(getSymbolDataWidth()).Append('x').Append(getSymbolDataHeight());
         sb.Append(", codewords ").Append(dataCapacity).Append('+').Append(errorCodewords);
         return sb.ToString();
      }
   }
}