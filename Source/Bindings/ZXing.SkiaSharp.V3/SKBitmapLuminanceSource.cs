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
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace ZXing.SkiaSharp
{
    /// <summary>
    /// A luminance source class which consumes a Mat image from SkiaSharp and calculates the luminance values based on the bytes of the image
    /// </summary>
    public class SKBitmapLuminanceSource : BaseLuminanceSource
    {
        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="image"></param>
        public SKBitmapLuminanceSource(SKBitmap image)
           : base(image.Width, image.Height)
        {
            CalculateLuminance(image);
        }

        /// <summary>
        /// internal constructor used by CreateLuminanceSource
        /// </summary>
        /// <param name="luminances"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected SKBitmapLuminanceSource(byte[] luminances, int width, int height)
           : base(luminances, width, height)
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
            return new SKBitmapLuminanceSource(newLuminances, width, height);
        }

        private void CalculateLuminance(SKBitmap src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            var imageInfo = src.Info;
            var width = imageInfo.Width;
            var height = imageInfo.Height;
            var colorType = imageInfo.ColorType;

#if SKColorType_Index8_Support
            if (colorType == SKColorType.Index8)
            {
                // use the color table for indexed images
                for (int row = 0; row < height; row++)
                {
                    var index = width * row;
                    for (int col = 0; col < width; col++, index++)
                    {
                        var pixel = src.GetIndex8Color(col, row);

                        SetLuminance(index, pixel.Red, pixel.Green, pixel.Blue, pixel.Alpha);
                    }
                }
            }
            else
#endif
            if (colorType == SKColorType.Rgba8888
                    || colorType == SKColorType.RgbaF16
                    || colorType == SKColorType.Bgra8888)
            {
                // Read pixels from unmanaged memory to set luminance
                // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
                IntPtr pixelsAddr = src.GetPixels();

                unsafe
                {
                    uint* ptr = (uint*)pixelsAddr.ToPointer();

                    for (int row = 0; row < height; row++)
                    {
                        var index = width * row;
                        for (int col = 0; col < width; col++, index++)
                        {
                            uint colorVal = *ptr++;

                            var alpha = (byte)((colorVal >> 24) & 0xff);
                            var red = (byte)((colorVal >> 16) & 0xff);
                            var green = (byte)((colorVal >> 8) & 0xff);
                            var blue = (byte)((colorVal) & 0xff);

                            SetLuminance(index, red, green, blue, alpha);
                        }
                    }
                }
            }
            else
            {
                // unknown type or other color types, use the 'old' way allocating more managed memory
                // can potentially migrate this to the above case, but leaving here for safety
                var pixels = src.Pixels;
                var length = pixels.Length;
                for (var index = 0; index < length; index++)
                {
                    var pixel = pixels[index];

                    SetLuminance(index, pixel.Red, pixel.Green, pixel.Blue, pixel.Alpha);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetLuminance(int index, byte red, byte green, byte blue, byte alpha)
        {
            // Calculate luminance cheaply, favoring green.
            var luminance = (byte)((RChannelWeight * red + GChannelWeight * green + BChannelWeight * blue) >> ChannelWeight);
            luminances[index] = (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
        }
    }
}
