/*
 * Copyright 2012 ZXing.Net authors
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
    /// <summary> Renders a <see cref="BitMatrix" /> to a <see cref="string" /> </summary>
    /// <inheritdoc />
    public class StringRenderer : IBarcodeRenderer<string>
    {
        /// <summary> Foreground/filled Char. </summary>
        public Char Foreground { get; set; } = '█';

        /// <summary> Background/blank Char. </summary>
        public Char Background { get; set; } = ' ';

        [System.CLSCompliant(false)]
        public string Render(BitMatrix matrix, BarcodeFormat format, string content) => Render(matrix, format, content, null);

        [System.CLSCompliant(false)]
        public string Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            int height = matrix.Height;
            int width = matrix.Width + LineFeed.Length;
            char[] matrixTxt = new char[width * height];
            for (int i, y = i = 0; y < matrix.Height; y++)
            {
                for (var x = 0; x < matrix.Width; x++)
                {
                    matrixTxt[i++] = matrix[x, y] ? Foreground : Background;
                }
                foreach (var lf in LineFeed)
                {
                    matrixTxt[i++] = lf;
                }
            }

            return new string(matrixTxt);
        }

        public string LineFeed { get; set; } = "\n";
    }
}
