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


using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ZXing
{
    /// <summary>
    /// class which represents the luminance values for a bitmap object of a SoftwareBitmap class
    /// </summary>
    public partial class SoftwareBitmapLuminanceSource : BaseLuminanceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected SoftwareBitmapLuminanceSource(int width, int height)
           : base(width, height)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="softwareBitmap"></param>
        public SoftwareBitmapLuminanceSource(SoftwareBitmap softwareBitmap)
           : base(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight)
        {
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Gray8)
            {
                using (SoftwareBitmap convertedSoftwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Gray8))
                {
                    convertedSoftwareBitmap.CopyToBuffer(luminances.AsBuffer());
                }
            }
            else
            {
                softwareBitmap.CopyToBuffer(luminances.AsBuffer());
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
            return new SoftwareBitmapLuminanceSource(width, height) { luminances = newLuminances };
        }
    }
}