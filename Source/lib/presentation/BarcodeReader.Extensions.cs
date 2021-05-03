﻿/*
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

namespace ZXing
{
    using System.Windows.Media.Imaging;

    /// <summary>
    /// extensions methods which are working directly on any IBarcodeReaderGeneric implementation
    /// </summary>
    public static class BarcodeReaderExtensions
    {
        /// <summary>
        /// uses the IBarcodeReaderGeneric implementation and the <see cref="BitmapSourceLuminanceSource"/> class for decoding
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Result Decode(this IBarcodeReaderGeneric reader, BitmapSource image)
        {
            var luminanceSource = new BitmapSourceLuminanceSource(image);
            return reader.Decode(luminanceSource);
        }

        /// <summary>
        /// uses the IBarcodeReaderGeneric implementation and the <see cref="BitmapSourceLuminanceSource"/> class for decoding
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Result[] DecodeMultiple(this IBarcodeReaderGeneric reader, BitmapSource image)
        {
            var luminanceSource = new BitmapSourceLuminanceSource(image);
            return reader.DecodeMultiple(luminanceSource);
        }
    }
}
