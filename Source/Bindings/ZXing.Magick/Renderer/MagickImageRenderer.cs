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

using ZXing.Common;
using ZXing.Rendering;

using ImageMagick;

namespace ZXing.Magick.Rendering
{
    public class MagickImageRenderer : IBarcodeRenderer<MagickImage>
    {
        public MagickImage Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }

        public MagickImage Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            byte[] header = System.Text.Encoding.UTF8.GetBytes($"P4\n{matrix.Width} {matrix.Height}\n");

            int rowBytes = matrix.Width / 8;

            if ((matrix.Width % 8) != 0)
            {
                rowBytes++;
            }

            byte[] totalBuffer = new byte[header.Length + rowBytes * matrix.Height];

            header.CopyTo(totalBuffer, 0);

            int bufferOffset = header.Length;

            for (int y = 0; y < matrix.Height; y++)
            {
                for (int x = 0; x < matrix.Width; x++)
                {
                    if (matrix[x, y])
                    {
                        totalBuffer[bufferOffset] |= (byte)(((byte)1) << 7 - (x % 8));
                    }

                    if (x % 8 == 7)
                    {
                        bufferOffset++;
                    }
                }

                if ((matrix.Width % 8) != 0)
                {
                    bufferOffset++;
                }
            }

            return new MagickImage(totalBuffer);
        }
    }
}
