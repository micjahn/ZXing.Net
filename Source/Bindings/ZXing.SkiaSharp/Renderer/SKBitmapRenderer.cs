/*
 * Copyright 2017 ZXing.Net authors
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
using System.Net.NetworkInformation;
using SkiaSharp;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.SkiaSharp.Rendering
{
    /// <summary>
    /// Renders a <see cref="BitMatrix" /> to a <see cref="SKBitmap" /> image
    /// </summary>
    public class SKBitmapRenderer : ZXing.Rendering.IBarcodeRenderer<SKBitmap>
    {
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public SKColor Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public SKColor Background { get; set; }

        /// <summary>
        /// Gets or sets the text font.
        /// </summary>
        /// <value>
        /// The text font.
        /// </value>
        public SKTypeface TextFont { get; set; }

        /// <summary>
        /// Gets or sets the height of the text
        /// </summary>
        public float TextSize { get; set; }

        private static readonly SKTypeface DefaultTextFont;

        static SKBitmapRenderer()
        {
            try
            {
                DefaultTextFont = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            catch (Exception)
            {
                // have to ignore, no better idea
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKBitmapRenderer"/> class.
        /// </summary>
        public SKBitmapRenderer()
        {
            Foreground = SKColors.Black;
            Background = SKColors.White;
            TextFont = DefaultTextFont;
            TextSize = 10;
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
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
        public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
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

            // calculating the scaling factor
            var pixelsizeWidth = width / matrix.Width;
            var pixelsizeHeight = height / matrix.Height;

            SKBitmap bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            IntPtr pixelsAddr = bitmap.GetPixels();

            unsafe
            {
                var ptr = (uint*) pixelsAddr.ToPointer();
                var forecolor = (uint) Foreground;
                var backcolor = (uint) Background;
                var textAreaHeight = (int) (TextSize < 1 ? 10 : TextSize);

                emptyArea = outputContent && height > textAreaHeight ? textAreaHeight : 0;

                for (int y = 0; y < matrix.Height - emptyArea; y++)
                {
                    // stretching the line by the scaling factor
                    for (var pixelsizeHeightProcessed = 0;
                        pixelsizeHeightProcessed < pixelsizeHeight;
                        pixelsizeHeightProcessed++)
                    {
                        // going through the columns of the current line
                        for (var x = 0; x < matrix.Width; x++)
                        {
                            var color = matrix[x, y] ? forecolor : backcolor;
                            // stretching the columns by the scaling factor
                            for (var pixelsizeWidthProcessed = 0;
                                pixelsizeWidthProcessed < pixelsizeWidth;
                                pixelsizeWidthProcessed++)
                            {
                                *ptr++ = color;
                            }
                        }
                        // fill up to the right if the barcode doesn't fully fit in
                        for (var x = pixelsizeWidth * matrix.Width; x < width; x++)
                        {
                            *ptr++ = backcolor;
                        }
                    }
                }
                // fill up to the bottom if the barcode doesn't fully fit in
                for (var y = pixelsizeHeight * matrix.Height; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        *ptr++ = backcolor;
                    }
                }
                // fill the bottom area with the background color if the content should be written below the barcode
                if (outputContent)
                {
                    if (emptyArea > 0)
                    {
                        for (int y = height - emptyArea; y < height; y++)
                        {
                            for (var x = 0; x < width; x++)
                            {
                                *ptr++ = backcolor;
                            }
                        }
                    }
                }
            }

            if (emptyArea > 0)
            {
                //using (var surface = SKSurface.Create(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul))
                using (SKCanvas myCanvas = new SKCanvas(bitmap))
                using (var paint = new SKPaint())
                using (var fontForPaint = new SKFont())
                {
                    paint.IsAntialias = true;
                    paint.Color = Foreground;
                    fontForPaint.Typeface = font;
                    fontForPaint.Size = TextSize < 1 ? 10 : TextSize;

                    // output content text below the barcode
                    if (emptyArea > 0)
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
#if SKIASHARP_V2
                        var textWidth = paint.MeasureText(content);
#else
                        var textWidth = fontForPaint.MeasureText(content, paint);
#endif
                        var x = (pixelsizeWidth * matrix.Width - textWidth) / 2;
                        var y = height - 1;
                        x = x < 0 ? 0 : x;
                        myCanvas.DrawText(content, x, y, fontForPaint, paint);
                    }
                    myCanvas.Flush();
                }
            }

            return bitmap;
        }
    }
}