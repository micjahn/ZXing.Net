/*
* Copyright 2014 ZXing.Net authors
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

#if WINDOWS_COMPATIBILITY
namespace ZXing.Windows.Compatibility
#else
namespace ZXing.Presentation
#endif
{
    using System.Windows.Media.Imaging;

#if WINDOWS_COMPATIBILITY
    using ZXing.Windows.Compatibility;
#else
    using ZXing.Rendering;
#endif

    /// <summary>
    /// A smart class to encode some content to a barcode image
    /// </summary>
#if WINDOWS_COMPATIBILITY
    public class BarcodeWriterWriteableBitmap : BarcodeWriter<WriteableBitmap>
#else
    public class BarcodeWriter : BarcodeWriter<WriteableBitmap>
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
        /// </summary>
#if WINDOWS_COMPATIBILITY
        public BarcodeWriterWriteableBitmap()
#else
        public BarcodeWriter()
#endif
        {
            Renderer = new WriteableBitmapRenderer();
        }
    }
}
