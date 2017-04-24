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


using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace ZXing
{
   /// <summary>
   /// class which represents the luminance values for a bitmap object of a WriteableBitmap class
   /// </summary>
   public partial class BitmapLuminanceSource : BaseLuminanceSource
   {
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
      /// initializing constructor
      /// </summary>
      /// <param name="writeableBitmap"></param>
      public BitmapLuminanceSource(WriteableBitmap writeableBitmap)
         : base(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight)
      {
         var height = writeableBitmap.PixelHeight;
         var width = writeableBitmap.PixelWidth;

         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // luminance array is initialized with new byte[width * height]; in base class

         var data = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(writeableBitmap.PixelBuffer, 0, (int)writeableBitmap.PixelBuffer.Length);
         var luminanceIndex = 0;
         var maxSourceIndex = width*height*4;
         for (var sourceIndex = 0; sourceIndex < maxSourceIndex; sourceIndex+=4)
         {
            var c = Color.FromArgb(
               data[sourceIndex], 
               data[sourceIndex + 1], 
               data[sourceIndex + 2], 
               data[sourceIndex + 3]);
            luminances[luminanceIndex] = (byte)((RChannelWeight * c.R + GChannelWeight * c.G + BChannelWeight * c.B) >> ChannelWeight);
            luminanceIndex++;
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
         return new BitmapLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}