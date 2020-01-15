/*
 * Copyright 2020 ZXing authors
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

using SkiaSharp;
using System;

namespace ZXing.SkiaSharp.Common.Test
{
    public abstract class SkiaBarcodeBlackBoxTestCase : AbstractBlackBoxTestCase<SKBitmap>
    {
        public SkiaBarcodeBlackBoxTestCase(string testBasePathSuffix, BarcodeFormat? expectedFormat)
        : base(testBasePathSuffix, new BarcodeReader(), expectedFormat)
        {

        }

        protected override SKBitmap openFromFile(string filePath)
        {
            return SKBitmap.Decode(filePath);
        }

        protected override SKBitmap rotateImage(SKBitmap original, float degrees)
        {
            double radians = Math.PI * degrees / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2.0f, rotatedHeight / 2.0f);
                surface.RotateDegrees(degrees);
                surface.Translate(-originalWidth / 2.0f, -originalHeight / 2.0f);
                surface.DrawBitmap(original, 0, 0);
            }

            return rotatedBitmap;
        }

        protected override LuminanceSource getLuminanceSource(SKBitmap image)
        {
            return new SKBitmapLuminanceSource(image);
        }
    }
}
