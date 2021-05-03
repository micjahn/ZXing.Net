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

extern alias Android;

namespace ZXing.Android.Rendering
{
    using ZXing.Rendering;

    /// <summary>
    /// extension method converting pixeldata to Android.Graphics.Bitmap
    /// </summary>
    public static class PixelDataExtensions
    {
        /// <summary>
        /// converts the pixel data to a bitmap object
        /// </summary>
        /// <returns></returns>
        [System.CLSCompliant(false)]
        public static Android::Android.Graphics.Bitmap ToBitmap(this PixelData pixelData)
        {
            if (pixelData == null)
                return null;

            var pixels = pixelData.Pixels;
            var colors = new int[pixelData.Width * pixelData.Height];
            for (var index = 0; index < pixelData.Width * pixelData.Height; index++)
            {
                colors[index] =
                    pixels[index * 4] << 24 |
                    pixels[index * 4 + 1] << 16 |
                    pixels[index * 4 + 2] << 8 |
                    pixels[index * 4 + 3];
            }
            return Android::Android.Graphics.Bitmap.CreateBitmap(colors, pixelData.Width, pixelData.Height, Android::Android.Graphics.Bitmap.Config.Argb8888);
        }
    }
}
