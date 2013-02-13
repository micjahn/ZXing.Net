/*
 * Copyright 2006 Jeremias Maerki.
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

namespace ZXing.Datamatrix.Encoder
{
   /// <summary>
   /// Symbol Character Placement Program. Adapted from Annex M.1 in ISO/IEC 16022:2000(E).
   /// </summary>
   public class DefaultPlacement
   {
      private readonly String codewords;
      private readonly int numrows;
      private readonly int numcols;
      private readonly byte[] bits;

      /// <summary>
      /// Main constructor
      /// </summary>
      /// <param name="codewords">the codewords to place</param>
      /// <param name="numcols">the number of columns</param>
      /// <param name="numrows">the number of rows</param>
      public DefaultPlacement(String codewords, int numcols, int numrows)
      {
         this.codewords = codewords;
         this.numcols = numcols;
         this.numrows = numrows;
         this.bits = new byte[numcols * numrows];
         SupportClass.Fill(this.bits, (byte)2); //Initialize with "not set" value
      }

      public int Numrows
      {
         get { return numrows; }
      }

      public int Numcols
      {
         get { return numcols; }
      }

      public byte[] Bits
      {
         get { return bits; }
      }

      public bool getBit(int col, int row)
      {
         return bits[row * numcols + col] == 1;
      }

      public void setBit(int col, int row, bool bit)
      {
         bits[row * numcols + col] = bit ? (byte)1 : (byte)0;
      }

      public bool hasBit(int col, int row)
      {
         return bits[row * numcols + col] < 2;
      }

      public void place()
      {
         int pos = 0;
         int row = 4;
         int col = 0;

         do
         {
            /* repeatedly first check for one of the special corner cases, then... */
            if ((row == numrows) && (col == 0))
            {
               corner1(pos++);
            }
            if ((row == numrows - 2) && (col == 0) && ((numcols % 4) != 0))
            {
               corner2(pos++);
            }
            if ((row == numrows - 2) && (col == 0) && (numcols % 8 == 4))
            {
               corner3(pos++);
            }
            if ((row == numrows + 4) && (col == 2) && ((numcols % 8) == 0))
            {
               corner4(pos++);
            }
            /* sweep upward diagonally, inserting successive characters... */
            do
            {
               if ((row < numrows) && (col >= 0) && !hasBit(col, row))
               {
                  utah(row, col, pos++);
               }
               row -= 2;
               col += 2;
            } while (row >= 0 && (col < numcols));
            row++;
            col += 3;

            /* and then sweep downward diagonally, inserting successive characters, ... */
            do
            {
               if ((row >= 0) && (col < numcols) && !hasBit(col, row))
               {
                  utah(row, col, pos++);
               }
               row += 2;
               col -= 2;
            } while ((row < numrows) && (col >= 0));
            row += 3;
            col++;

            /* ...until the entire array is scanned */
         } while ((row < numrows) || (col < numcols));

         /* Lastly, if the lower righthand corner is untouched, fill in fixed pattern */
         if (!hasBit(numcols - 1, numrows - 1))
         {
            setBit(numcols - 1, numrows - 1, true);
            setBit(numcols - 2, numrows - 2, true);
         }
      }

      private void module(int row, int col, int pos, int bit)
      {
         if (row < 0)
         {
            row += numrows;
            col += 4 - ((numrows + 4) % 8);
         }
         if (col < 0)
         {
            col += numcols;
            row += 4 - ((numcols + 4) % 8);
         }
         // Note the conversion:
         int v = codewords[pos];
         v &= 1 << (8 - bit);
         setBit(col, row, v != 0);
      }

      /// <summary>
      /// Places the 8 bits of a utah-shaped symbol character in ECC200.
      /// </summary>
      /// <param name="row">The row.</param>
      /// <param name="col">The col.</param>
      /// <param name="pos">character position</param>
      private void utah(int row, int col, int pos)
      {
         module(row - 2, col - 2, pos, 1);
         module(row - 2, col - 1, pos, 2);
         module(row - 1, col - 2, pos, 3);
         module(row - 1, col - 1, pos, 4);
         module(row - 1, col, pos, 5);
         module(row, col - 2, pos, 6);
         module(row, col - 1, pos, 7);
         module(row, col, pos, 8);
      }

      private void corner1(int pos)
      {
         module(numrows - 1, 0, pos, 1);
         module(numrows - 1, 1, pos, 2);
         module(numrows - 1, 2, pos, 3);
         module(0, numcols - 2, pos, 4);
         module(0, numcols - 1, pos, 5);
         module(1, numcols - 1, pos, 6);
         module(2, numcols - 1, pos, 7);
         module(3, numcols - 1, pos, 8);
      }

      private void corner2(int pos)
      {
         module(numrows - 3, 0, pos, 1);
         module(numrows - 2, 0, pos, 2);
         module(numrows - 1, 0, pos, 3);
         module(0, numcols - 4, pos, 4);
         module(0, numcols - 3, pos, 5);
         module(0, numcols - 2, pos, 6);
         module(0, numcols - 1, pos, 7);
         module(1, numcols - 1, pos, 8);
      }

      private void corner3(int pos)
      {
         module(numrows - 3, 0, pos, 1);
         module(numrows - 2, 0, pos, 2);
         module(numrows - 1, 0, pos, 3);
         module(0, numcols - 2, pos, 4);
         module(0, numcols - 1, pos, 5);
         module(1, numcols - 1, pos, 6);
         module(2, numcols - 1, pos, 7);
         module(3, numcols - 1, pos, 8);
      }

      private void corner4(int pos)
      {
         module(numrows - 1, 0, pos, 1);
         module(numrows - 1, numcols - 1, pos, 2);
         module(0, numcols - 3, pos, 3);
         module(0, numcols - 2, pos, 4);
         module(0, numcols - 1, pos, 5);
         module(1, numcols - 3, pos, 6);
         module(1, numcols - 2, pos, 7);
         module(1, numcols - 1, pos, 8);
      }
   }
}