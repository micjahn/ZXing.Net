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

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// Metadata about a PDF417 Barcode
    /// </summary>
    /// <author>Guenther Grau</author>
    public sealed class BarcodeMetadata
    {
        /// <summary>
        /// column count
        /// </summary>
        public int ColumnCount { get; private set; }
        /// <summary>
        /// error correction level
        /// </summary>
        public int ErrorCorrectionLevel { get; private set; }
        /// <summary>
        /// row count upper
        /// </summary>
        public int RowCountUpper { get; private set; }
        /// <summary>
        /// row count lower
        /// </summary>
        public int RowCountLower { get; private set; }
        /// <summary>
        /// row count
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="columnCount"></param>
        /// <param name="rowCountUpperPart"></param>
        /// <param name="rowCountLowerPart"></param>
        /// <param name="errorCorrectionLevel"></param>
        public BarcodeMetadata(int columnCount, int rowCountUpperPart, int rowCountLowerPart, int errorCorrectionLevel)
        {
            this.ColumnCount = columnCount;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
            this.RowCountUpper = rowCountUpperPart;
            this.RowCountLower = rowCountLowerPart;
            this.RowCount = rowCountLowerPart + rowCountUpperPart;
        }
    }
}