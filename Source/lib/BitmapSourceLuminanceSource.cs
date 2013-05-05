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

using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace ZXing
{
   public partial class BitmapSourceLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapSourceLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected BitmapSourceLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapSourceLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      public BitmapSourceLuminanceSource(BitmapSource bitmap)
         : base(bitmap.PixelWidth, bitmap.PixelHeight)
      {
         switch (bitmap.Format.ToString())
         {
            case "Bgr24":
            case "Bgr32":
               CalculateLuminanceBGR(bitmap);
               break;
            case "Bgra32":
               CalculateLuminanceBGRA(bitmap);
               break;
            case "Rgb24":
               CalculateLuminanceRGB(bitmap);
               break;
            case "Bgr565":
               CalculateLuminanceBGR565(bitmap);
               break;
            default:
               // there is no special conversion routine to luminance values
               // we have to convert the image to a supported format
               bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
               CalculateLuminanceBGR(bitmap);
               break;
         }
      }

      private void CalculateLuminanceRGB(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var r = buffer[curX];
               var g = buffer[curX + 1];
               var b = buffer[curX + 2];
               luminances[luminanceIndex] = (byte)((RChannelWeight * r + GChannelWeight * g + BChannelWeight * b) >> ChannelWeight);
               luminanceIndex++;
            }
            rect.Y++;
         }
      }

      private void CalculateLuminanceBGR(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var b = buffer[curX];
               var g = buffer[curX + 1];
               var r = buffer[curX + 2];
               luminances[luminanceIndex] = (byte)((RChannelWeight * r + GChannelWeight * g + BChannelWeight * b) >> ChannelWeight);
               luminanceIndex++;
            }
            rect.Y++;
         }
      }

      private void CalculateLuminanceBGRA(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var b = buffer[curX];
               var g = buffer[curX + 1];
               var r = buffer[curX + 2];
               var luminance = (byte)((RChannelWeight * r + GChannelWeight * g + BChannelWeight * b) >> ChannelWeight);
               var alpha = buffer[curX + 3];
               luminance = (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
               luminances[luminanceIndex] = luminance;
               luminanceIndex++;
            }
            rect.Y++;
         }
      }

      private void CalculateLuminanceBGR565(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var byte1 = buffer[curX];
               var byte2 = buffer[curX + 1];

               var b5 = byte1 & 0x1F;
               var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
               var r5 = (byte2 >> 2) & 0x1F;
               var r8 = (r5 * 527 + 23) >> 6;
               var g8 = (g5 * 527 + 23) >> 6;
               var b8 = (b5 * 527 + 23) >> 6;

               // cheap, not fully accurate conversion
               //var pixel = (byte2 << 8) | byte1;
               //b8 = (((pixel) & 0x001F) << 3);
               //g8 = (((pixel) & 0x07E0) >> 2) & 0xFF;
               //r8 = (((pixel) & 0xF800) >> 8);

               luminances[luminanceIndex] = (byte)((RChannelWeight * r8 + GChannelWeight * g8 + BChannelWeight * b8) >> ChannelWeight);
               luminanceIndex++;
            }
            rect.Y++;
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
         return new BitmapSourceLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}
