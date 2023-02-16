/*
 * Copyright 2008 ZXing authors
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

namespace ZXing.Datamatrix
{
    using System;
    using System.Collections.Generic;

    using ZXing.Common;
    using ZXing.Datamatrix.Encoder;

    /// <summary>
    /// This object renders a Data Matrix code as a BitMatrix 2D array of greyscale values.
    /// </summary>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    /// <author>Guillaume Le Biller Added to zxing lib.</author>
    public sealed class DataMatrixWriter : Writer
    {
        /// <summary>
        /// encodes the content to a BitMatrix
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public BitMatrix encode(String contents, BarcodeFormat format, int width, int height)
        {
            return encode(contents, format, width, height, null);
        }

        /// <summary>
        /// encodes the content to a BitMatrix
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public BitMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
        {
            if (String.IsNullOrEmpty(contents))
            {
                throw new ArgumentException("Found empty contents", contents);
            }

            if (format != BarcodeFormat.DATA_MATRIX)
            {
                throw new ArgumentException("Can only encode DATA_MATRIX, but got " + format);
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Requested dimensions can't be negative: " + width + 'x' + height);
            }

            // Try to get force shape & min / max size
            var shape = SymbolShapeHint.FORCE_NONE;
            var defaultEncodation = Encodation.ASCII;
            Dimension minSize = null;
            Dimension maxSize = null;
            var margin = 0;
            var noPadding = false;
            System.Text.Encoding encoding = null;
            var disableEci = false;
            if (hints != null)
            {
                if (hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
                {
                    var requestedShape = hints[EncodeHintType.DATA_MATRIX_SHAPE];
                    if (requestedShape is SymbolShapeHint)
                    {
                        shape = (SymbolShapeHint)requestedShape;
                    }
                    else
                    {
                        if (Enum.IsDefined(typeof(SymbolShapeHint), requestedShape.ToString()))
                        {
                            shape = (SymbolShapeHint)Enum.Parse(typeof(SymbolShapeHint), requestedShape.ToString(), true);
                        }
                    }
                }
                var requestedMinSize = hints.ContainsKey(EncodeHintType.MIN_SIZE) ? hints[EncodeHintType.MIN_SIZE] as Dimension : null;
                if (requestedMinSize != null)
                {
                    minSize = requestedMinSize;
                }
                var requestedMaxSize = hints.ContainsKey(EncodeHintType.MAX_SIZE) ? hints[EncodeHintType.MAX_SIZE] as Dimension : null;
                if (requestedMaxSize != null)
                {
                    maxSize = requestedMaxSize;
                }
                if (hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
                {
                    var requestedDefaultEncodation = hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION];
                    if (requestedDefaultEncodation != null)
                    {
                        defaultEncodation = Convert.ToInt32(requestedDefaultEncodation.ToString());
                    }
                }
                if (hints.ContainsKey(EncodeHintType.MARGIN))
                {
                    var marginInt = hints[EncodeHintType.MARGIN];
                    if (marginInt != null)
                    {
                        margin = Convert.ToInt32(marginInt.ToString());
                    }
                }
                if (hints.ContainsKey(EncodeHintType.NO_PADDING))
                {
                    var noPaddingObj = hints[EncodeHintType.NO_PADDING];
                    if (noPaddingObj != null)
                    {
                        bool.TryParse(noPaddingObj.ToString(), out noPadding);
                    }
                }
                encoding = IDictionaryExtensions.GetEncoding(hints, null);
                disableEci = IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.DISABLE_ECI, disableEci);
            }

            //1. step: Data encodation
            String encoded;

            if (IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.DATA_MATRIX_COMPACT))
            {
                var hasGS1FormatHint = IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.GS1_FORMAT);

                encoded = MinimalEncoder.encodeHighLevel(contents, encoding, hasGS1FormatHint ? 0x1D : -1, shape);
            }
            else
            {
                var hasForceC40Hint = IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.FORCE_C40);
                encoded = HighLevelEncoder.encodeHighLevel(contents, shape, minSize, maxSize, defaultEncodation, hasForceC40Hint, encoding, disableEci);
            }

            SymbolInfo symbolInfo = SymbolInfo.lookup(encoded.Length, shape, minSize, maxSize, true);

            //2. step: ECC generation
            String codewords = ErrorCorrection.encodeECC200(encoded, symbolInfo);

            //3. step: Module placement in Matrix
            var placement = new DefaultPlacement(codewords, symbolInfo.getSymbolDataWidth(), symbolInfo.getSymbolDataHeight());
            placement.place();

            //4. step: low-level encoding
            return encodeLowLevel(placement, symbolInfo, width, height, margin, noPadding);
        }

        /// <summary>
        /// Encode the given symbol info to a bit matrix.
        /// </summary>
        /// <param name="placement">The DataMatrix placement.</param>
        /// <param name="symbolInfo">The symbol info to encode.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="margin"></param>
        /// <returns>The bit matrix generated.</returns>
        private static BitMatrix encodeLowLevel(DefaultPlacement placement, SymbolInfo symbolInfo, int width, int height, int margin, bool noPadding)
        {
            int symbolWidth = symbolInfo.getSymbolDataWidth();
            int symbolHeight = symbolInfo.getSymbolDataHeight();

            var matrix = new QrCode.Internal.ByteMatrix(symbolInfo.getSymbolWidth(), symbolInfo.getSymbolHeight());

            int matrixY = 0;

            for (int y = 0; y < symbolHeight; y++)
            {
                // Fill the top edge with alternate 0 / 1
                int matrixX;
                if ((y % symbolInfo.matrixHeight) == 0)
                {
                    matrixX = 0;
                    for (int x = 0; x < symbolInfo.getSymbolWidth(); x++)
                    {
                        matrix.set(matrixX, matrixY, (x % 2) == 0);
                        matrixX++;
                    }
                    matrixY++;
                }
                matrixX = 0;
                for (int x = 0; x < symbolWidth; x++)
                {
                    // Fill the right edge with full 1
                    if ((x % symbolInfo.matrixWidth) == 0)
                    {
                        matrix.set(matrixX, matrixY, true);
                        matrixX++;
                    }
                    matrix.set(matrixX, matrixY, placement.getBit(x, y));
                    matrixX++;
                    // Fill the right edge with alternate 0 / 1
                    if ((x % symbolInfo.matrixWidth) == symbolInfo.matrixWidth - 1)
                    {
                        matrix.set(matrixX, matrixY, (y % 2) == 0);
                        matrixX++;
                    }
                }
                matrixY++;
                // Fill the bottom edge with full 1
                if ((y % symbolInfo.matrixHeight) == symbolInfo.matrixHeight - 1)
                {
                    matrixX = 0;
                    for (int x = 0; x < symbolInfo.getSymbolWidth(); x++)
                    {
                        matrix.set(matrixX, matrixY, true);
                        matrixX++;
                    }
                    matrixY++;
                }
            }

            return convertByteMatrixToBitMatrix(matrix, width, height, margin, noPadding);
        }

        /// <summary>
        /// Convert the ByteMatrix to BitMatrix.
        /// </summary>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="reqWidth">The requested width of the image (in pixels) with the Datamatrix code</param>
        /// <param name="reqHeight">The requested height of the image (in pixels) with the Datamatrix code</param>
        /// <param name="margin"></param>
        /// <param name="noPadding"></param>
        /// <returns>The output matrix.</returns>
        private static BitMatrix convertByteMatrixToBitMatrix(QrCode.Internal.ByteMatrix matrix, int reqWidth, int reqHeight, int margin, bool noPadding)
        {
            var matrixWidth = matrix.Width;
            var matrixHeight = matrix.Height;
            int datamatrixWidth = matrixWidth + (margin << 1);
            int datamatrixHeight = matrixHeight + (margin << 1);
            var outputWidth = Math.Max(reqWidth, datamatrixWidth);
            var outputHeight = Math.Max(reqHeight, datamatrixHeight);

            int multiple = Math.Min(outputWidth / datamatrixWidth, outputHeight / datamatrixHeight);
            int leftPadding = (outputWidth - (matrixWidth * multiple)) / 2;
            int topPadding = (outputHeight - (matrixHeight * multiple)) / 2;

            if (noPadding)
            {
                outputHeight -= (topPadding - margin) * 2;
                outputWidth -= (leftPadding - margin) * 2;
                leftPadding = margin;
                topPadding = margin;
            }

            var output = new BitMatrix(outputWidth, outputHeight);

            for (int inputY = 0, outputY = topPadding; inputY < matrixHeight; inputY++, outputY += multiple)
            {
                // Write the contents of this row of the bytematrix
                for (int inputX = 0, outputX = leftPadding; inputX < matrixWidth; inputX++, outputX += multiple)
                {
                    if (matrix[inputX, inputY] == 1)
                    {
                        output.setRegion(outputX, outputY, multiple, multiple);
                    }
                }
            }

            return output;
        }
    }
}