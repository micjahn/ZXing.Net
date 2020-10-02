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
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ZXing.Windows.Compatibility
{
    /// <summary>
    /// class which represents the luminance values for a bitmap object
    /// </summary>
    public class BitmapLuminanceSource : BaseLuminanceSource
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
            CalculateLuminanceValues(bitmap, luminances);
        }

        /// <summary>
        /// calculates the luminance values for bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="luminances"></param>
        protected static void CalculateLuminanceValues(Bitmap bitmap, byte[] luminances)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // The underlying raster of image consists of bytes with the luminance values
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                var stride = Math.Abs(data.Stride);
                var pixelWidth = stride / width;

                if (pixelWidth > 4)
                {
                    // old slow way for unsupported bit depth
                    CalculateLuminanceValuesSlow(bitmap, luminances);
                }
                else
                {
                    if (bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                        bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                    {
                        pixelWidth = 40;
                    }
                    if ((int) bitmap.PixelFormat == 8207 ||
                        (bitmap.Flags & (int) ImageFlags.ColorSpaceCmyk) == (int) ImageFlags.ColorSpaceCmyk)
                    {
                        pixelWidth = 41;
                    }
                    switch (pixelWidth)
                    {
                        case 0:
                            if (bitmap.PixelFormat == PixelFormat.Format4bppIndexed)
                                CalculateLuminanceValuesForIndexed4Bit(bitmap, data, luminances);
                            else
                                CalculateLuminanceValuesForIndexed1Bit(bitmap, data, luminances);
                            break;
                        case 1:
                            CalculateLuminanceValuesForIndexed8Bit(bitmap, data, luminances);
                            break;
                        case 2:
                            // should be RGB565 or RGB555, assume RGB565
                            CalculateLuminanceValues565(bitmap, data, luminances);
                            break;
                        case 3:
                            CalculateLuminanceValues24Bit(bitmap, data, luminances);
                            break;
                        case 4:
                            CalculateLuminanceValues32BitWithoutAlpha(bitmap, data, luminances);
                            break;
                        case 40:
                            CalculateLuminanceValues32BitWithAlpha(bitmap, data, luminances);
                            break;
                        case 41:
                            CalculateLuminanceValues32BitCMYK(bitmap, data, luminances);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        /// <summary>
        /// old slow way for unsupported bit depth
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="luminances"></param>
        protected static void CalculateLuminanceValuesSlow(Bitmap bitmap, byte[] luminances)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;

            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    var c = bitmap.GetPixel(x, y);
                    luminances[offset + x] = (byte) ((RChannelWeight * c.R + GChannelWeight * c.G + BChannelWeight * c.B) >> ChannelWeight);
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 1-bit indexed bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        protected static void CalculateLuminanceValuesForIndexed1Bit(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;

            if (pixelWidth != 0)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            // prepare palette for 1, 4 and 8 bit indexed bitmaps
            var luminancePalette = new byte[256];
            var luminancePaletteLength = Math.Min(bitmap.Palette.Entries.Length, luminancePalette.Length);
            for (var index = 0; index < luminancePaletteLength; index++)
            {
                var color = bitmap.Palette.Entries[index];
                luminancePalette[index] = (byte) ((RChannelWeight * color.R +
                                                   GChannelWeight * color.G +
                                                   BChannelWeight * color.B) >> ChannelWeight);
            }

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                for (int x = 0; x * 8 < width; x++)
                {
                    var x8 = 8 * x;
                    var offset8 = offset + x8;
                    for (int subX = 0; subX < 8 && x8 + subX < width; subX++)
                    {
                        var index = (buffer[x] >> (7 - subX)) & 1;
                        luminances[offset8 + subX] = luminancePalette[index];
                    }
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 4-bit indexed bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        protected static void CalculateLuminanceValuesForIndexed4Bit(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;
            var evenWidth = (width / 2) * 2;

            if (pixelWidth != 0)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            // prepare palette for 1, 4 and 8 bit indexed bitmaps
            var luminancePalette = new byte[256];
            var luminancePaletteLength = Math.Min(bitmap.Palette.Entries.Length, luminancePalette.Length);
            for (var index = 0; index < luminancePaletteLength; index++)
            {
                var color = bitmap.Palette.Entries[index];
                luminancePalette[index] = (byte) ((RChannelWeight * color.R +
                                                   GChannelWeight * color.G +
                                                   BChannelWeight * color.B) >> ChannelWeight);
            }

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                int sourceX = 0;
                int destX = 0;
                byte sourceValue = 0;
                for (; destX < evenWidth; sourceX++, destX += 2)
                {
                    sourceValue = buffer[sourceX];
                    var index = sourceValue & 15;
                    luminances[offset + destX + 1] = luminancePalette[index];
                    index = (sourceValue >> 4) & 15;
                    luminances[offset + destX] = luminancePalette[index];
                }
                if (width > evenWidth)
                {
                    var index = (sourceValue >> 4) & 15;
                    luminances[offset + destX] = luminancePalette[index];
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 8-bit indexed bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        protected static void CalculateLuminanceValuesForIndexed8Bit(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;

            if (pixelWidth != 1)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            // prepare palette for 1, 4 and 8 bit indexed bitmaps
            var luminancePalette = new byte[256];
            var luminancePaletteLength = Math.Min(bitmap.Palette.Entries.Length, luminancePalette.Length);
            for (var index = 0; index < luminancePaletteLength; index++)
            {
                var color = bitmap.Palette.Entries[index];
                luminancePalette[index] = (byte) ((RChannelWeight * color.R +
                                                   GChannelWeight * color.G +
                                                   BChannelWeight * color.B) >> ChannelWeight);
            }

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    luminances[offset + x] = luminancePalette[buffer[x]];
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 565 encoded bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        private static void CalculateLuminanceValues565(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;

            if (pixelWidth != 2)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                var maxIndex = 2 * width;
                for (int index = 0; index < maxIndex; index += 2)
                {
                    var byte1 = buffer[index];
                    var byte2 = buffer[index + 1];

                    var b5 = byte1 & 0x1F;
                    var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                    var r5 = (byte2 >> 2) & 0x1F;
                    var r8 = (r5 * 527 + 23) >> 6;
                    var g8 = (g5 * 527 + 23) >> 6;
                    var b8 = (b5 * 527 + 23) >> 6;

                    luminances[offset] = (byte) ((RChannelWeight * r8 + GChannelWeight * g8 + BChannelWeight * b8) >> ChannelWeight);
                    offset++;
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 24-bit encoded bitmaps
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        private static void CalculateLuminanceValues24Bit(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;

            if (pixelWidth != 3)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                var maxIndex = width * 3;
                for (int x = 0; x < maxIndex; x += 3)
                {
                    var luminance = (byte) ((BChannelWeight * buffer[x] +
                                             GChannelWeight * buffer[x + 1] +
                                             RChannelWeight * buffer[x + 2]) >> ChannelWeight);
                    luminances[offset] = luminance;
                    offset++;
                }
            }
        }

        /// <summary>
        /// calculates the luminance values for 32-bit encoded bitmaps without respecting the alpha channel
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="data"></param>
        /// <param name="luminances"></param>
        private static void CalculateLuminanceValues32BitWithoutAlpha(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;
            var maxIndex = 4 * width;

            if (pixelWidth != 4)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                // 4 bytes without alpha channel value
                for (int x = 0; x < maxIndex; x += 4)
                {
                    var luminance = (byte) ((BChannelWeight * buffer[x] +
                                             GChannelWeight * buffer[x + 1] +
                                             RChannelWeight * buffer[x + 2]) >> ChannelWeight);

                    luminances[offset] = luminance;
                    offset++;
                }
            }
        }

        /// calculates the luminance values for 32-bit encoded bitmaps with alpha channel
        private static void CalculateLuminanceValues32BitWithAlpha(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;
            var maxIndex = 4 * width;

            if (pixelWidth != 4)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                // with alpha channel; some barcodes are completely black if you
                // only look at the r, g and b channel but the alpha channel controls
                // the view
                for (int x = 0; x < maxIndex; x += 4)
                {
                    var luminance = (byte) ((BChannelWeight * buffer[x] +
                                             GChannelWeight * buffer[x + 1] +
                                             RChannelWeight * buffer[x + 2]) >> ChannelWeight);

                    // calculating the resulting luminance based upon a white background
                    // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                    // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                    var alpha = buffer[x + 3];
                    luminance = (byte) (((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8) + 1);
                    luminances[offset] = luminance;
                    offset++;
                }
            }
        }

        /// calculates the luminance values for 32-bit CMYK encoded bitmaps (k is ignored at the momen)
        private static void CalculateLuminanceValues32BitCMYK(Bitmap bitmap, BitmapData data, byte[] luminances)
        {
            var height = data.Height;
            var width = data.Width;
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;
            var strideStep = data.Stride;
            var buffer = new byte[stride];
            var ptrInBitmap = data.Scan0;
            var maxIndex = 4 * width;

            if (pixelWidth != 4)
                throw new InvalidOperationException("Unsupported pixel format: " + bitmap.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                // copy a scanline not the whole bitmap because of memory usage
                Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40 || NET45 || NET46 || NET47 || NET48
                ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                var offset = y * width;
                for (int x = 0; x < maxIndex; x += 4)
                {
                    var luminance = (byte) (255 - ((BChannelWeight * buffer[x] +
                                                    GChannelWeight * buffer[x + 1] +
                                                    RChannelWeight * buffer[x + 2]) >> ChannelWeight));
                    // Ignore value of k at the moment
                    luminances[offset] = luminance;
                    offset++;
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