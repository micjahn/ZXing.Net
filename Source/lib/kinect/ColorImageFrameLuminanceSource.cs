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

using Microsoft.Kinect;

namespace ZXing.Kinect
{
   /// <summary>
   /// special luminance class which supports ColorImageFrame directly
   /// </summary>
   public class ColorImageFrameLuminanceSource : RGBLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ColorImageFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected ColorImageFrameLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ColorImageFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      public ColorImageFrameLuminanceSource(ColorImageFrame bitmap)
         : this(bitmap, true)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ColorImageFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      /// <param name="flipTheImage">if set to <c>true</c> [flip the image].</param>
      public ColorImageFrameLuminanceSource(ColorImageFrame bitmap, bool flipTheImage)
         : base(bitmap.Width, bitmap.Height)
      {
         var pixelData = new byte[bitmap.PixelDataLength];
         var bitmapFormat = BitmapFormat.Unknown;
         switch (bitmap.Format)
         {
            case ColorImageFormat.InfraredResolution640x480Fps30:
               bitmapFormat = BitmapFormat.Gray16;
               break;
            case ColorImageFormat.RawBayerResolution1280x960Fps12: // 32-bit, linear X8R8G8B8-formatted
            case ColorImageFormat.RawBayerResolution640x480Fps30: // 32-bit, linear X8R8G8B8-formatted
            case ColorImageFormat.RgbResolution1280x960Fps12: // 32-bit, linear X8R8G8B8-formatted
            case ColorImageFormat.RgbResolution640x480Fps30: // 32-bit, linear X8R8G8B8-formatted
            case ColorImageFormat.YuvResolution640x480Fps15: // 32-bit, linear X8R8G8B8-formatted
               bitmapFormat = BitmapFormat.BGR32;
               break;
            case ColorImageFormat.RawYuvResolution640x480Fps15: // 16-bit, gamma-corrected linear UYVY-formatted
               bitmapFormat = BitmapFormat.UYVY;
               break;
            default:
               break;
         }
         bitmap.CopyPixelDataTo(pixelData);
         CalculateLuminance(pixelData, bitmapFormat);
         if (flipTheImage)
         {
            // flip the luminance values because the kinect has it flipped before
            FlipLuminanceValues();
         }
      }

      private void FlipLuminanceValues()
      {
         var width = Width;
         var height = Height;
         var halfWidth = width/2;

         for (var y = 0; y < height; y++)
         {
            for (var x = 0; x < halfWidth; x++)
            {
               var posLeft = y*width + x;
               var posRight = y * width + width - x - 1;
               var tmp = luminances[posLeft];
               luminances[posLeft] = luminances[posRight];
               luminances[posRight] = tmp;
            }
         }
      }

      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new ColorImageFrameLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}
