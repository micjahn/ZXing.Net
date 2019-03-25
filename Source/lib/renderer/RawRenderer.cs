﻿/*
 * Copyright 2014 ZXing.Net authors
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

using ZXing.Common;

namespace ZXing.Rendering
{
    /// <summary>
    /// Renders a <see cref="BitMatrix" /> to a byte array with ARGB 32bit data
    /// </summary>
    [Obsolete("please use PixelDataRenderer instead")]
    public class RawRenderer : IBarcodeRenderer<byte[]>
    {
        public struct Color
        {
            public static Color Black = new Color(0);
            public static Color White = new Color(0x00FFFFFF);

            public byte A;
            public byte R;
            public byte G;
            public byte B;

            public Color(int color)
            {
                A = (byte)((color & 0xFF000000) >> 24);
                R = (byte)((color & 0x00FF0000) >> 16);
                G = (byte)((color & 0x0000FF00) >> 8);
                B = (byte)((color & 0x000000FF));
            }
        }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>
        /// The foreground color.
        /// </value>
        public Color Foreground { get; set; }
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>
        /// The background color.
        /// </value>
        public Color Background { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawRenderer"/> class.
        /// </summary>
        public RawRenderer()
        {
            Foreground = Color.Black;
            Background = Color.White;
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public byte[] Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, null);
        }

        /// <summary>
        /// Renders the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        virtual public byte[] Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            bool outputContent = (options == null || !options.PureBarcode) &&
                                 !String.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 ||
                                                                    format == BarcodeFormat.CODE_128 ||
                                                                    format == BarcodeFormat.EAN_13 ||
                                                                    format == BarcodeFormat.EAN_8 ||
                                                                    format == BarcodeFormat.CODABAR ||
                                                                    format == BarcodeFormat.ITF ||
                                                                    format == BarcodeFormat.UPC_A ||
                                                                    format == BarcodeFormat.MSI ||
                                                                    format == BarcodeFormat.PLESSEY);
            int emptyArea = outputContent ? 16 : 0;
            int pixelsize = 1;

            if (options != null)
            {
                if (options.Width > width)
                {
                    width = options.Width;
                }
                if (options.Height > height)
                {
                    height = options.Height;
                }
                // calculating the scaling factor
                pixelsize = width / matrix.Width;
                if (pixelsize > height / matrix.Height)
                {
                    pixelsize = height / matrix.Height;
                }
            }

            var pixels = new byte[width * height * 4];
            var index = 0;

            for (int y = 0; y < matrix.Height - emptyArea; y++)
            {
                for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
                {
                    for (var x = 0; x < matrix.Width; x++)
                    {
                        var color = matrix[x, y] ? Foreground : Background;
                        for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                        {
                            pixels[index++] = color.A;
                            pixels[index++] = color.R;
                            pixels[index++] = color.G;
                            pixels[index++] = color.B;
                        }
                    }
                    for (var x = pixelsize * matrix.Width; x < width; x++)
                    {
                        pixels[index++] = Background.A;
                        pixels[index++] = Background.R;
                        pixels[index++] = Background.G;
                        pixels[index++] = Background.B;
                    }
                }
            }
            for (int y = matrix.Height * pixelsize - emptyArea; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[index++] = Background.A;
                    pixels[index++] = Background.R;
                    pixels[index++] = Background.G;
                    pixels[index++] = Background.B;
                }
            }

            return pixels;
        }
    }
}
