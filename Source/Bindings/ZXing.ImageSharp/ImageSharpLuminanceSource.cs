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

namespace ZXing.ImageSharp
{
	/// <summary>
	/// specific implementation of a luminance source which can be used with ImageSharp Image objects
	/// </summary>
	public class ImageSharpLuminanceSource<TPixel> : BaseLuminanceSource where TPixel : struct, IPixel<TPixel>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSharpLuminanceSource{TPixel}"/> class.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		protected ImageSharpLuminanceSource(int width, int height)
			: base(width, height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSharpLuminanceSource{TPixel}"/> class
		/// with the image of a Bitmap instance
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public ImageSharpLuminanceSource(Image<TPixel> bitmap)
			: base(bitmap.Width, bitmap.Height)
		{
			var height = bitmap.Height;
			var width = bitmap.Width;
			var pixelRgba32 = new Rgba32();

			// In order to measure pure decoding speed, we convert the entire image to a greyscale array
			// The underlying raster of image consists of bytes with the luminance values
			var luminanceIndex = 0;

			for (int y = 0; y < height; y++)
			{
				// with alpha channel; some barcodes are completely black if you
				// only look at the r, g and b channel but the alpha channel controls
				// the view
				for (int x = 0; x < width; x++)
				{
					bitmap[x, y].ToRgba32(ref pixelRgba32);
					var luminance = (byte) ((BChannelWeight*pixelRgba32.B +
					                         GChannelWeight*pixelRgba32.G +
					                         RChannelWeight*pixelRgba32.R) >> ChannelWeight);

					// calculating the resulting luminance based upon a white background
					var alpha = pixelRgba32.A;
					luminance = (byte) (((luminance*alpha) >> 8) + (255*(255 - alpha) >> 8) + 1);
					luminances[luminanceIndex] = luminance;
					luminanceIndex++;
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
			return new ImageSharpLuminanceSource<TPixel>(width, height) {luminances = newLuminances};
		}
	}
}
