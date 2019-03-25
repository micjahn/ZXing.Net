/*
 * Copyright 2011 ZXing authors
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

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// Holds all of the information for a barcode in a format where it can be easily accessible
    ///
    /// <author>Jacob Haynes</author>
    /// </summary>
    internal sealed class BarcodeMatrix
    {
        private readonly BarcodeRow[] matrix;
        private int currentRow;
        private readonly int height;
        private readonly int width;
        internal const int COLUMN_WIDTH = 17;

        /// <summary>
        /// <param name="height">the height of the matrix (Rows)</param>
        /// <param name="width">the width of the matrix (Cols)</param>
        /// </summary>
        internal BarcodeMatrix(int height, int width, bool compact)
        {
            matrix = new BarcodeRow[height];
            //Initializes the array to the correct width
            for (int i = 0, matrixLength = matrix.Length; i < matrixLength; i++)
            {
                //Matrix width is: Cols*COLUMN_WIDTH + <PDF417_START_COLS> + <PDF417_END_COLS> + 1
                //When compact=true, the END_COLS are omitted.
                matrix[i] = new BarcodeRow(width * COLUMN_WIDTH + (2 * COLUMN_WIDTH) + (compact ? 0 : 2) * COLUMN_WIDTH + 1);
            }
            this.width = width * COLUMN_WIDTH;
            this.height = height;
            this.currentRow = -1;
        }

        internal void set(int x, int y, sbyte value)
        {
            matrix[y][x] = value;
        }

        /*
        internal void setMatrix(int x, int y, bool black)
        {
           set(x, y, (sbyte) (black ? 1 : 0));
        }
        */

        internal void startRow()
        {
            ++currentRow;
        }

        internal BarcodeRow getCurrentRow()
        {
            return matrix[currentRow];
        }

        internal sbyte[][] getMatrix()
        {
            return getScaledMatrix(1, 1);
        }

        /*
        internal sbyte[][] getScaledMatrix(int Scale)
        {
           return getScaledMatrix(Scale, Scale);
        }
        */

        internal sbyte[][] getScaledMatrix(int xScale, int yScale)
        {
            sbyte[][] matrixOut = new sbyte[height * yScale][];
            for (int idx = 0; idx < height * yScale; idx++)
                matrixOut[idx] = new sbyte[width * xScale];
            int yMax = height * yScale;
            for (int ii = 0; ii < yMax; ii++)
            {
                matrixOut[yMax - ii - 1] = matrix[ii / yScale].getScaledRow(xScale);
            }
            return matrixOut;
        }
    }
}