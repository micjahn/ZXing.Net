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

namespace ZXing.EmguCV
{
    using Emgu.CV;

    using ZXing;

    /// <summary>
    /// A luminance source class which consumes a Image from EmguCV and calculates the luminance values based on the bytes of the image
    /// </summary>
    public class ImageLuminanceSource : BaseLuminanceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLuminanceSource"/> class
        /// with an image instance
        /// </summary>
        /// <param name="image"></param>
        public ImageLuminanceSource(Image<Emgu.CV.Structure.Bgr, byte> image)
           : base(image.Size.Width, image.Size.Height)
        {
            var bytes = image.Bytes;
            for (int indexB = 0, indexL = 0; indexB < bytes.Length; indexB += 3, indexL++)
            {
                var b = bytes[indexB];
                var g = bytes[indexB + 1];
                var r = bytes[indexB + 2];
                // Calculate luminance cheaply, favoring green.
                luminances[indexL] = (byte)((r + g + g + b) >> 2);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLuminanceSource"/> class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected ImageLuminanceSource(int width, int height)
           : base(width, height)
        {
        }

        /// <summary>
        /// Should create a new luminance source with the right class type.
        /// The method is used in methods crop and rotate.
        /// </summary>
        /// <param name="newLuminances">The new luminances.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            return new ImageLuminanceSource(width, height) { luminances = newLuminances };
        }
    }
}
