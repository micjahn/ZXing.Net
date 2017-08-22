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
   public class ColorFrameLuminanceSource : RGBLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ColorFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected ColorFrameLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ColorFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      public ColorFrameLuminanceSource(ColorFrame bitmap)
         : this(bitmap, false)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ColorFrameLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      /// <param name="flipTheImage">if set to <c>true</c> [flip the image].</param>
      public ColorFrameLuminanceSource(ColorFrame bitmap, bool flipTheImage)
         : base(bitmap.FrameDescription.Width, bitmap.FrameDescription.Height)
      {
         //
         // micjahn: I'm really not sure if the following code works in every situation
         // I don't own a kinect device, I can't test it. The documentations are very preliminary
         // I really want to use the LockRawImageBuffer method, it should be faster then copying
         // the bytes arround. But the method CalculateLuminance doesn't support IntPtr.
         // I'm also not sure if image flipping is necessary with Kinect V2. I have disabled it
         // at the moment.
         //

         var pixelData = new byte[bitmap.FrameDescription.BytesPerPixel * bitmap.FrameDescription.Width * bitmap.FrameDescription.Height];
         var bitmapFormat = BitmapFormat.Unknown;
         switch (bitmap.RawColorImageFormat)
         {
            case ColorImageFormat.Bayer: // docu says 1 byte per pixel, not sure if this is real
            case ColorImageFormat.Rgba:  // 32-bit per pixel
               bitmapFormat = BitmapFormat.RGB32;
               break;
            case ColorImageFormat.Bgra:  // 32-bit per pixel
               bitmapFormat = BitmapFormat.BGR32;
               break;
            case ColorImageFormat.Yuy2:  // 16-bit per pixel
               bitmapFormat = BitmapFormat.YUYV;
               break;
            case ColorImageFormat.Yuv:   // 16-bit per pixel
               bitmapFormat = BitmapFormat.UYVY;
               break;
            default:
               break;
         }
         bitmap.CopyRawFrameDataToArray(pixelData);
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
         return new ColorFrameLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}
