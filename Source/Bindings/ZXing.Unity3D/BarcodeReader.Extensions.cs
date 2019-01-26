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

using UnityEngine;
using ZXing.Unity;

namespace ZXing
{
    /// <summary>
    /// extensions methods which are working directly on any IBarcodeReaderGeneric implementation
    /// </summary>
    public static class BarcodeReaderExtensions
    {
        /// <summary>
        /// uses the IBarcodeReaderGeneric implementation and the <see cref="Color32LuminanceSource"/> class for decoding
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="rawColor32"></param>
        /// <returns></returns>
        public static Result Decode(this IBarcodeReaderGeneric reader, Color32[] rawColor32, int width, int height)
        {
            var luminanceSource = new Color32LuminanceSource(rawColor32, width, height);
            return reader.Decode(luminanceSource);
        }

        /// <summary>
        /// uses the IBarcodeReaderGeneric implementation and the <see cref="Color32LuminanceSource"/> class for decoding
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="rawColor32"></param>
        /// <returns></returns>
        public static Result[] DecodeMultiple(this IBarcodeReaderGeneric reader, Color32[] rawColor32, int width, int height)
        {
            var luminanceSource = new Color32LuminanceSource(rawColor32, width, height);
            return reader.DecodeMultiple(luminanceSource);
        }
    }
}
