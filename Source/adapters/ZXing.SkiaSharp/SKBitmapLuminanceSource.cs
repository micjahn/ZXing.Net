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

using SkiaSharp;

namespace ZXing.SkiaSharp
{
   /// <summary>
   /// A luminance source class which consumes a Mat image from SkiaSharp and calculates the luminance values based on the bytes of the image
   /// </summary>
   internal class SKBitmapLuminanceSource : BaseLuminanceSource
   {
      public SKBitmapLuminanceSource(SKBitmap image)
         : base(image.Width, image.Height)
      {
         CalculateLuminance(image);
      }

      protected SKBitmapLuminanceSource(byte[] luminances, int width, int height)
         : base(luminances, width, height)
      {
      }

      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new SKBitmapLuminanceSource(newLuminances, width, height);
      }

      private void CalculateLuminance(SKBitmap src)
      {
         if (src == null)
            throw new ArgumentNullException("src");

         src.LockPixels();
         try
         {
            for (var index = 0; index < src.Width*src.Height; index++)
            {
               var pixel = src.Pixels[index];
               // Calculate luminance cheaply, favoring green.
               var luminance = (byte)((RChannelWeight * pixel.Red + GChannelWeight * pixel.Green + BChannelWeight * pixel.Blue) >> ChannelWeight);
               luminances[index] = (byte)(((luminance * pixel.Alpha) >> 8) + (255 * (255 - pixel.Alpha) >> 8));
            }
         }
         finally
         {
            src.UnlockPixels();
         }
      }
   }
}
