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

using System;
using System.Collections.Generic;

using ZXing.Common;
using ZXing.Datamatrix.Encoder;
using ZXing.QrCode.Internal;

namespace ZXing.Datamatrix
{
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
            }


            //1. step: Data encodation
            String encoded = HighLevelEncoder.encodeHighLevel(contents, shape, minSize, maxSize, defaultEncodation);

            SymbolInfo symbolInfo = SymbolInfo.lookup(encoded.Length, shape, minSize, maxSize, true);

            //2. step: ECC generation
            String codewords = ErrorCorrection.encodeECC200(encoded, symbolInfo);

            //3. step: Module placement in Matrix
            var placement = new DefaultPlacement(codewords, symbolInfo.getSymbolDataWidth(), symbolInfo.getSymbolDataHeight());
            placement.place();

            //4. step: low-level encoding
            return encodeLowLevel(placement, symbolInfo, width, height);
        }

        /// <summary>
        /// Encode the given symbol info to a bit matrix.
        /// </summary>
        /// <param name="placement">The DataMatrix placement.</param>
        /// <param name="symbolInfo">The symbol info to encode.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>The bit matrix generated.</returns>
        private static BitMatrix encodeLowLevel(DefaultPlacement placement, SymbolInfo symbolInfo, int width, int height)
        {
            int symbolWidth = symbolInfo.getSymbolDataWidth();
            int symbolHeight = symbolInfo.getSymbolDataHeight();

            var matrix = new ByteMatrix(symbolInfo.getSymbolWidth(), symbolInfo.getSymbolHeight());

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

            return convertByteMatrixToBitMatrix(matrix, width, height);
        }

        /// <summary>
        /// Convert the ByteMatrix to BitMatrix.
        /// </summary>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="reqWidth">The requested width of the image (in pixels) with the Datamatrix code</param>
        /// <param name="reqHeight">The requested height of the image (in pixels) with the Datamatrix code</param>
        /// <returns>The output matrix.</returns>
        private static BitMatrix convertByteMatrixToBitMatrix(ByteMatrix matrix, int reqWidth, int reqHeight)
        {
            var matrixWidth = matrix.Width;
            var matrixHeight = matrix.Height;
            var outputWidth = Math.Max(reqWidth, matrixWidth);
            var outputHeight = Math.Max(reqHeight, matrixHeight);

            int multiple = Math.Min(outputWidth / matrixWidth, outputHeight / matrixHeight);

            int leftPadding = (outputWidth - (matrixWidth * multiple)) / 2;
            int topPadding = (outputHeight - (matrixHeight * multiple)) / 2;

            BitMatrix output;

            // remove padding if requested width and height are too small
            if (reqHeight < matrixHeight || reqWidth < matrixWidth)
            {
                leftPadding = 0;
                topPadding = 0;
                output = new BitMatrix(matrixWidth, matrixHeight);
            }
            else
            {
                output = new BitMatrix(reqWidth, reqHeight);
            }

            output.clear();
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