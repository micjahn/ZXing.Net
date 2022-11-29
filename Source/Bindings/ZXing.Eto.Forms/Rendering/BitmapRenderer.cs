/*
 * Copyright 2018 ZXing.Net authors
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

using Eto.Drawing;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Eto.Forms
{
    /// <summary>
    /// Renders a <see cref="BitMatrix" /> to a <see cref="Bitmap" /> image
    /// </summary>
    public class BitmapRenderer : ZXing.Rendering.IBarcodeRenderer<Bitmap>
    {
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public Color Background { get; set; }

        /// <summary>
        /// Gets or sets the text font.
        /// </summary>
        /// <value>
        /// The text font.
        /// </value>
        public Font TextFont { get; set; }

        private static readonly Font DefaultTextFont;

        static BitmapRenderer()
        {
            try
            {
                DefaultTextFont = new Font("Arial", 10);
            }
            catch (Exception)
            {
                // have to ignore, no better idea
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
        /// </summary>
        public BitmapRenderer()
        {
            Foreground = Colors.Black;
            Background = Colors.White;
            TextFont = DefaultTextFont;
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            var width = matrix.Width;
            var height = matrix.Height;
            var font = TextFont ?? DefaultTextFont;
            var emptyArea = 0;
            var outputContent = font != null &&
                                (options == null || !options.PureBarcode) &&
                                !String.IsNullOrEmpty(content) &&
                                (format == BarcodeFormat.CODE_39 ||
                                 format == BarcodeFormat.CODE_93 ||
                                 format == BarcodeFormat.CODE_128 ||
                                 format == BarcodeFormat.EAN_13 ||
                                 format == BarcodeFormat.EAN_8 ||
                                 format == BarcodeFormat.CODABAR ||
                                 format == BarcodeFormat.ITF ||
                                 format == BarcodeFormat.UPC_A ||
                                 format == BarcodeFormat.UPC_E ||
                                 format == BarcodeFormat.MSI ||
                                 format == BarcodeFormat.PLESSEY);

            if (options != null && !options.NoPadding)
            {
                if (options.Width > width)
                {
                    width = options.Width;
                }
                if (options.Height > height)
                {
                    height = options.Height;
                }
            }

            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            // calculating the scaling factor
            var pixelsizeWidth = width / matrix.Width;

            using (var bmpData = bmp.Lock())
            { 
                bmpData.SetPixels(CreatePixels(matrix, width, height));
            }

            if (outputContent)
            {
                var textAreaHeight = (int)font.LineHeight;

                emptyArea = height > textAreaHeight ? textAreaHeight : 0;
            }

            // output content text below the barcode
            if (emptyArea > 0)
            {
                using (var g = new Graphics(bmp))
                {
                    switch (format)
                    {
                        case BarcodeFormat.UPC_E:
                        case BarcodeFormat.EAN_8:
                            if (content.Length < 8)
                                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                            if (content.Length > 4)
                                content = content.Insert(4, "   ");
                            break;
                        case BarcodeFormat.EAN_13:
                            if (content.Length < 13)
                                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                            if (content.Length > 7)
                                content = content.Insert(7, "   ");
                            if (content.Length > 1)
                                content = content.Insert(1, "   ");
                            break;
                        case BarcodeFormat.UPC_A:
                            if (content.Length < 12)
                                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
                            if (content.Length > 11)
                                content = content.Insert(11, "   ");
                            if (content.Length > 6)
                                content = content.Insert(6, "   ");
                            if (content.Length > 1)
                                content = content.Insert(1, "   ");
                            break;
                    }
                    g.FillRectangle(Background, 0, height - emptyArea, width, emptyArea);
                    var brush = new SolidBrush(Foreground);
                    g.DrawText(font, brush, pixelsizeWidth * matrix.Width / 2, height - emptyArea, content);
                }
            }

            return bmp;
        }

        private IEnumerable<Color> CreatePixels(BitMatrix matrix, int width, int height)
        {
            var color = Background;

            // calculating the scaling factor
            var pixelsizeWidth = width / matrix.Width;
            var pixelsizeHeight = height / matrix.Height;

            // going through the lines of the matrix
            for (int y = 0; y < matrix.Height; y++)
            {
                // stretching the line by the scaling factor
                for (var pixelsizeHeightProcessed = 0; pixelsizeHeightProcessed < pixelsizeHeight; pixelsizeHeightProcessed++)
                {
                    // going through the columns of the current line
                    for (var x = 0; x < matrix.Width; x++)
                    {
                        color = matrix[x, y] ? Foreground : Background;
                        // stretching the columns by the scaling factor
                        for (var pixelsizeWidthProcessed = 0;
                           pixelsizeWidthProcessed < pixelsizeWidth;
                           pixelsizeWidthProcessed++)
                        {
                            yield return color;
                        }
                    }
                    // fill up to the right if the barcode doesn't fully fit in 
                    for (var x = pixelsizeWidth * matrix.Width; x < width; x++)
                    {
                        yield return Background;
                    }
                }
            }
            // fill up to the bottom if the barcode doesn't fully fit in 
            for (var y = pixelsizeHeight * matrix.Height; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    yield return Background;
                }
            }
        }
    }
}