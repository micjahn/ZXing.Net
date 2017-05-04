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

using ImageMagick;

namespace ZXing.Magick
{
   /// <summary>
   /// A luminance source class which consumes a MagickImage image from ImageMagick and calculates the luminance values based on the bytes of the image
   /// </summary>
   public class MagickImageLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="image"></param>
      public MagickImageLuminanceSource(MagickImage image)
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
      protected MagickImageLuminanceSource(byte[] luminances, int width, int height)
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
         return new MagickImageLuminanceSource(newLuminances, width, height);
      }

      private void CalculateLuminance(MagickImage src)
      {
         if (src == null)
            throw new ArgumentNullException("src");

         var pixels = src.GetPixels();

         switch (src.Format)
         {
            case MagickFormat.Gray:
               CalculateLuminanceGray8(pixels, src.Width, src.Height);
               break;
            case MagickFormat.Bgr:
            case MagickFormat.Rgb:
               CalculateLuminanceBGR24(pixels, src.Width, src.Height);
               break;
            case MagickFormat.Bgra:
            case MagickFormat.Rgba:
               CalculateLuminanceBGRA32(pixels, src.Width, src.Height);
               break;
            default:
               throw new ArgumentOutOfRangeException(String.Format("MagickFormat {0} is not supported", src.Format));
         }
      }

      private unsafe void CalculateLuminanceGray8(PixelCollection pixels, int width, int height)
      {
         var luminanceIndex = 0;
         for (var y = 0; y < height; y++)
         {
            for (var x = 0; x < width; x++)
            {
               var pixel = pixels[x, y];
               luminances[luminanceIndex] = pixel.ToColor().R;
               luminanceIndex++;
            }
         }
      }

      private unsafe void CalculateLuminanceBGR24(PixelCollection pixels, int width, int height)
      {
         var luminanceIndex = 0;
         for (var y = 0; y < height; y++)
         {
            for (var x = 0; x < width; x++)
            {
               var pixelColor = pixels[x, y].ToColor();
               luminances[luminanceIndex] = (byte)((RChannelWeight * pixelColor.R + GChannelWeight * pixelColor.G + BChannelWeight * pixelColor.B) >> ChannelWeight);
               luminanceIndex++;
            }
         }
      }

      private unsafe void CalculateLuminanceBGRA32(PixelCollection pixels, int width, int height)
      {
         var luminanceIndex = 0;
         for (var y = 0; y < height; y++)
         {
            for (var x = 0; x < width; x++)
            {
               var pixelColor = pixels[x, y].ToColor();
               var luminance = (byte)((RChannelWeight * pixelColor.R + GChannelWeight * pixelColor.G + BChannelWeight * pixelColor.B) >> ChannelWeight);
               luminances[luminanceIndex] = (byte)(((luminance * pixelColor.A) >> 8) + (255 * (255 - pixelColor.A) >> 8));
               luminanceIndex++;
            }
         }
      }
   }
}
