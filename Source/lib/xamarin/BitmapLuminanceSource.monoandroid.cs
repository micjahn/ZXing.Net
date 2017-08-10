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

using System;

using Android.Graphics;

namespace ZXing
{
   /// <summary>
   /// A class which calculates the luminance values of an android bitmap object
   /// </summary>
   public class BitmapLuminanceSource : RGBLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      [CLSCompliant(false)]
      public BitmapLuminanceSource(Bitmap bitmap)
         : base(bitmap.Width, bitmap.Height)
      {
         // get all pixels at once from the bitmap (should be one of the fastest ways to analyze the whole picture)
         var pixels = new int[bitmap.Width * bitmap.Height];
         bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);
         // convert the pixel array to a byte array because the underlying method of 
         // RGBLuminanceSource doesn't support an int array
         var pixelBytes = new byte[pixels.Length * 4];
         Buffer.BlockCopy(pixels, 0, pixelBytes, 0, pixelBytes.Length);
         // calculating the luminance values the same way as RGBLuminanceSource
         if (bitmap.HasAlpha)
         {
            CalculateLuminance(pixelBytes, BitmapFormat.RGBA32);
         }
         else
         {
            CalculateLuminance(pixelBytes, BitmapFormat.RGB32);
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected BitmapLuminanceSource(int width, int height)
         : base(width, height)
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
         return new BitmapLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}