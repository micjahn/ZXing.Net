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

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing.ImageSharp;

namespace ZXing
{
	/// <summary>
	/// extensions methods which are working directly on any IBarcodeReaderGeneric implementation
	/// </summary>
	public static class BarcodeReaderExtensions
	{
		/// <summary>
		/// uses the IBarcodeReaderGeneric implementation and the <see cref="ImageSharpLuminanceSource{TPixel}"/> class for decoding
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public static Result Decode<TPixel>(this IBarcodeReaderGeneric reader, Image<TPixel> image) where TPixel : struct, IPixel<TPixel>
		{
			var luminanceSource = new ImageSharpLuminanceSource<TPixel>(image);
			return reader.Decode(luminanceSource);
		}

		/// <summary>
		/// uses the IBarcodeReaderGeneric implementation and the <see cref="ImageSharpLuminanceSource{TPixel}"/> class for decoding
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public static Result[] DecodeMultiple<TPixel>(this IBarcodeReaderGeneric reader, Image<TPixel> image) where TPixel : struct, IPixel<TPixel>
		{
			var luminanceSource = new ImageSharpLuminanceSource<TPixel>(image);
			return reader.DecodeMultiple(luminanceSource);
		}
	}
}
