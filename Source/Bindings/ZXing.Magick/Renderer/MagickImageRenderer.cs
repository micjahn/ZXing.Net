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

namespace ZXing.Magick.Rendering
{
    using ZXing.Common;
    using ZXing.Rendering;

    using ImageMagick;
    using ImageMagick.Factories;

    /// <summary>
    /// renderer class which generates a <see cref="IMagickImage{TQuantumType}"/> from a BitMatrix
    /// </summary>
    public class MagickImageRenderer<TQuantumType> : IBarcodeRenderer<IMagickImage<TQuantumType>>
        where TQuantumType : struct, System.IConvertible
    {
        private readonly IMagickImageFactory<TQuantumType> magickImageFactory;

        /// <summary>
        /// constructor, which can be used with a special implementation of <see cref="IMagickImageFactory{TQuantumType}"/>.
        /// </summary>
        /// <param name="magickImageFactory"></param>
        public MagickImageRenderer(IMagickImageFactory<TQuantumType> magickImageFactory)
        {
            this.magickImageFactory = magickImageFactory;
        }

        /// <summary>
        /// renders the BitMatrix as <see cref="IMagickImage{TQuantumType}"/>
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="format"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public IMagickImage<TQuantumType> Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }

        /// <summary>
        /// renders the BitMatrix as <see cref="IMagickImage{TQuantumType}"/>
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="format"></param>
        /// <param name="content"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        virtual public IMagickImage<TQuantumType> Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
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

            return this.magickImageFactory.Create(totalBuffer);
        }
    }
}
