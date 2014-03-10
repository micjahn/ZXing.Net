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

namespace ZXing.PDF417.Internal
{
   /// <summary>
   /// Represents a Column in the Detection Result
   /// </summary>
   /// <author>Guenther Grau</author>
   public sealed class DetectionResultRowIndicatorColumn : DetectionResultColumn
   {
      /// <summary>
      /// Gets or sets a value indicating whether this instance is the left indicator
      /// </summary>
      /// <value><c>true</c> if this instance is left; otherwise, <c>false</c>.</value>
      public bool IsLeft { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn"/> class.
      /// </summary>
      /// <param name="box">Box.</param>
      /// <param name="isLeft">If set to <c>true</c> is left.</param>
      public DetectionResultRowIndicatorColumn(BoundingBox box, bool isLeft)
         : base(box)
      {
         this.IsLeft = isLeft;
      }

      /// <summary>
      /// Sets the Row Numbers as Inidicator Columns
      /// </summary>
      public void setRowNumbers()
      {
         foreach (var cw in Codewords)
         {
            if (cw != null)
            {
               cw.setRowNumberAsRowIndicatorColumn();
            }
         }
      }


      /// <summary>
      /// TODO implement properly
      /// TODO maybe we should add missing codewords to store the correct row number to make
      /// finding row numbers for other columns easier
      /// use row height count to make detection of invalid row numbers more reliable
      /// </summary>
      /// <returns>The indicator column row numbers.</returns>
      /// <param name="metadata">Metadata.</param>
      public int adjustCompleteIndicatorColumnRowNumbers(BarcodeMetadata metadata)
      {
         var codewords = Codewords;
         setRowNumbers(); // Assign this as an indicator column
         removeIncorrectCodewords(codewords, metadata);

         ResultPoint top = IsLeft ? Box.TopLeft : Box.TopRight;
         ResultPoint bottom = IsLeft ? Box.BottomLeft : Box.BottomRight;

         int firstRow = imageRowToCodewordIndex((int) top.Y);
         int lastRow = imageRowToCodewordIndex((int) bottom.Y);

         // We need to be careful using the average row height.  
         // Barcode could be skewed so that we have smaller and taller rows
         float averageRowHeight = (lastRow - firstRow)/(float) metadata.RowCount;

         // initialize loop
         int barcodeRow = -1;
         int maxRowHeight = 1;
         int currentRowHeight = 0;

         for (int codewordRow = firstRow; codewordRow < lastRow; codewordRow++)
         {
            var codeword = codewords[codewordRow];
            if (codeword == null)
            {
               continue;
            }

            //      float expectedRowNumber = (codewordsRow - firstRow) / averageRowHeight;
            //      if (Math.abs(codeword.getRowNumber() - expectedRowNumber) > 2) {
            //        SimpleLog.log(LEVEL.WARNING,
            //            "Removing codeword, rowNumberSkew too high, codeword[" + codewordsRow + "]: Expected Row: " +
            //                expectedRowNumber + ", RealRow: " + codeword.getRowNumber() + ", value: " + codeword.getValue());
            //        codewords[codewordsRow] = null;
            //      }

            int rowDifference = codeword.RowNumber - barcodeRow;

            // TODO improve handling with case where first row indicator doesn't start with 0

            if (rowDifference == 0)
            {
               currentRowHeight++;
            }
            else if (rowDifference == 1)
            {
               maxRowHeight = Math.Max(maxRowHeight, currentRowHeight);
               currentRowHeight = 1;
               barcodeRow = codeword.RowNumber;
            }
            else if (rowDifference < 0 ||
                     codeword.RowNumber >= metadata.RowCount ||
                     rowDifference > codewordRow)
            {
               codewords[codewordRow] = null;
            }
            else
            {
               int checkedRows;
               if (maxRowHeight > 2)
               {
                  checkedRows = (maxRowHeight - 2)*rowDifference;
               }
               else
               {
                  checkedRows = rowDifference;
               }
               bool closePreviousCodewordFound = checkedRows > codewordRow;
               for (int i = 1; i <= checkedRows && !closePreviousCodewordFound; i++)
               {
                  // there must be (height * rowDifference) number of codewords missing. For now we assume height = 1.
                  // This should hopefully get rid of most problems already.
                  closePreviousCodewordFound = codewords[codewordRow - i] != null;
               }
               if (closePreviousCodewordFound)
               {
                  codewords[codewordRow] = null;
               }
               else
               {
                  barcodeRow = codeword.RowNumber;
                  currentRowHeight = 1;
               }
            }

         }
         return (int) (averageRowHeight + 0.5);
      }

      /// <summary>
      /// Gets the row heights.
      /// </summary>
      /// <returns>The row heights.</returns>
      public int[] getRowHeights()
      {
         BarcodeMetadata barcodeMetadata = getBarcodeMetadata();
         if (barcodeMetadata == null)
         {
            return null;
         }
         adjustIncompleteIndicatorColumnRowNumbers(barcodeMetadata);
         int[] result = new int[barcodeMetadata.RowCount];
         foreach (var codeword in Codewords)
         {
            if (codeword != null)
            {
               int rowNumber = codeword.RowNumber;
               if (rowNumber >= result.Length)
               {
                  return null;
               }
               result[rowNumber]++;
            } // else throw exception? (or return null)
         }
         return result;
      }

      /// <summary>
      /// Adjusts the in omplete indicator column row numbers.
      /// </summary>
      /// <param name="metadata">Metadata.</param>
      public int adjustIncompleteIndicatorColumnRowNumbers(BarcodeMetadata metadata)
      {
         // TODO maybe we should add missing codewords to store the correct row number to make
         // finding row numbers for other columns easier
         // use row height count to make detection of invalid row numbers more reliable

         ResultPoint top = IsLeft ? Box.TopLeft : Box.TopRight;
         ResultPoint bottom = IsLeft ? Box.BottomLeft : Box.BottomRight;

         int firstRow = imageRowToCodewordIndex((int) top.Y);
         int lastRow = imageRowToCodewordIndex((int) bottom.Y);

         // We need to be careful using the average row height.  
         // Barcode could be skewed so that we have smaller and taller rows
         float averageRowHeight = (lastRow - firstRow)/(float) metadata.RowCount;
         var codewords = Codewords;

         // initialize loop
         int barcodeRow = -1;
         int maxRowHeight = 1;
         int currentRowHeight = 0;

         for (int codewordRow = firstRow; codewordRow < lastRow; codewordRow++)
         {
            var codeword = codewords[codewordRow];
            if (codeword == null)
            {
               continue;
            }

            codeword.setRowNumberAsRowIndicatorColumn();

            int rowDifference = codeword.RowNumber - barcodeRow;

            // TODO improve handling with case where first row indicator doesn't start with 0

            if (rowDifference == 0)
            {
               currentRowHeight++;
            }
            else if (rowDifference == 1)
            {
               maxRowHeight = Math.Max(maxRowHeight, currentRowHeight);
               currentRowHeight = 1;
               barcodeRow = codeword.RowNumber;
            }
            else if (codeword.RowNumber > metadata.RowCount)
            {
               Codewords[codewordRow] = null;
            }
            else
            {
               barcodeRow = codeword.RowNumber;
               currentRowHeight = 1;
            }

         }
         return (int) (averageRowHeight + 0.5);
      }

      /// <summary>
      /// Gets the barcode metadata.
      /// </summary>
      /// <returns>The barcode metadata.</returns>
      public BarcodeMetadata getBarcodeMetadata()
      {
         var codewords = Codewords;
         BarcodeValue barcodeColumnCount = new BarcodeValue();
         BarcodeValue barcodeRowCountUpperPart = new BarcodeValue();
         BarcodeValue barcodeRowCountLowerPart = new BarcodeValue();
         BarcodeValue barcodeECLevel = new BarcodeValue();
         foreach (Codeword codeword in codewords)
         {
            if (codeword == null)
            {
               continue;
            }
            codeword.setRowNumberAsRowIndicatorColumn();
            int rowIndicatorValue = codeword.Value%30;
            int codewordRowNumber = codeword.RowNumber;
            if (!IsLeft)
            {
               codewordRowNumber += 2;
            }
            switch (codewordRowNumber%3)
            {
               case 0:
                  barcodeRowCountUpperPart.setValue(rowIndicatorValue*3 + 1);
                  break;
               case 1:
                  barcodeECLevel.setValue(rowIndicatorValue/3);
                  barcodeRowCountLowerPart.setValue(rowIndicatorValue%3);
                  break;
               case 2:
                  barcodeColumnCount.setValue(rowIndicatorValue + 1);
                  break;
            }
         }
         // Maybe we should check if we have ambiguous values?
         var barcodeColumnCountValues = barcodeColumnCount.getValue();
         var barcodeRowCountUpperPartValues = barcodeRowCountUpperPart.getValue();
         var barcodeRowCountLowerPartValues = barcodeRowCountLowerPart.getValue();
         var barcodeECLevelValues = barcodeECLevel.getValue();
         if ((barcodeColumnCountValues.Length == 0) ||
             (barcodeRowCountUpperPartValues.Length == 0) ||
             (barcodeRowCountLowerPartValues.Length == 0) ||
             (barcodeECLevelValues.Length == 0) ||
              barcodeColumnCountValues[0] < 1 ||
              barcodeRowCountUpperPartValues[0] + barcodeRowCountLowerPartValues[0] < PDF417Common.MIN_ROWS_IN_BARCODE ||
              barcodeRowCountUpperPartValues[0] + barcodeRowCountLowerPartValues[0] > PDF417Common.MAX_ROWS_IN_BARCODE)
         {
            return null;
         }
         var barcodeMetadata = new BarcodeMetadata(barcodeColumnCountValues[0],
                                                   barcodeRowCountUpperPartValues[0],
                                                   barcodeRowCountLowerPartValues[0],
                                                   barcodeECLevelValues[0]);
         removeIncorrectCodewords(codewords, barcodeMetadata);
         return barcodeMetadata;
      }

      /// <summary>
      /// Prune the codewords which do not match the metadata
      /// TODO Maybe we should keep the incorrect codewords for the start and end positions?
      /// </summary>
      /// <param name="codewords">Codewords.</param>
      /// <param name="metadata">Metadata.</param>
      private void removeIncorrectCodewords(Codeword[] codewords, BarcodeMetadata metadata)
      {
         for (int row = 0; row < codewords.Length; row++)
         {
            var codeword = codewords[row];
            if (codeword == null)
               continue;

            int indicatorValue = codeword.Value%30;
            int rowNumber = codeword.RowNumber;

            // Row does not exist in the metadata
            if (rowNumber >= metadata.RowCount) // different to java rowNumber > metadata.RowCount
            {
               codewords[row] = null; // remove this.
               continue;
            }

            if (!IsLeft)
            {
               rowNumber += 2;
            }

            switch (rowNumber%3)
            {
               default:
               case 0:
                  if (indicatorValue*3 + 1 != metadata.RowCountUpper)
                  {
                     codewords[row] = null;
                  }
                  break;

               case 1:
                  if (indicatorValue%3 != metadata.RowCountLower ||
                      indicatorValue/3 != metadata.ErrorCorrectionLevel)
                  {
                     codewords[row] = null;
                  }
                  break;

               case 2:
                  if (indicatorValue + 1 != metadata.ColumnCount)
                  {
                     codewords[row] = null;
                  }
                  break;
            }

         }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents the current <see cref="ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn"/>.
      /// </summary>
      /// <returns>A <see cref="System.String"/> that represents the current <see cref="ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn"/>.</returns>
      public override string ToString()
      {
         return "Is Left: " + IsLeft + " \n" + base.ToString();
      }
   }
}