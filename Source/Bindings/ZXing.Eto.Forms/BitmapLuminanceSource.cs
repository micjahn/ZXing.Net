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

using Eto.Drawing;

namespace ZXing.Eto.Forms
{
    /// <summary>
    /// class which represents the luminance values for a bitmap object
    /// </summary>
    public partial class BitmapLuminanceSource : BaseLuminanceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected BitmapLuminanceSource(int width, int height)
            : base(width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class
        /// with the image of a Bitmap instance
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public BitmapLuminanceSource(Bitmap bitmap)
            : base(bitmap.Width, bitmap.Height)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;
            var luminanceIndex = 0;

            using (var lockBits = bitmap.Lock())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color c = lockBits.GetPixel(x, y);
                        var lum = (byte) ((BChannelWeight * c.Bb +
                                           GChannelWeight * c.Gb +
                                           RChannelWeight * c.Rb) >> ChannelWeight);

                        var alpha = c.Ab;
                        lum = (byte) (((lum * alpha) >> 8) + 255 * (255 - alpha) + 1);
                        luminances[luminanceIndex] = lum;
                        luminanceIndex++;
                    }
                }
            }
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
            return new BitmapLuminanceSource(width, height) {luminances = newLuminances};
        }
    }
}