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

using System.Runtime.InteropServices;

namespace ZXing.Interop.Common
{
   [ComVisible(true)]
   [Guid("4B7E9244-FECF-4D85-BE7F-0572058DD5AE")]
   public enum BitmapFormat
   {
      /// <summary>
      /// format of the byte[] isn't known. RGBLuminanceSource tries to determine the best possible value
      /// </summary>
      Unknown,
      /// <summary>
      /// grayscale array, the byte array is a luminance array with 1 byte per pixel
      /// </summary>
      Gray8,
      /// <summary>
      /// 3 bytes per pixel with the channels red, green and blue
      /// </summary>
      RGB24,
      /// <summary>
      /// 4 bytes per pixel with the channels red, green and blue
      /// </summary>
      RGB32,
      /// <summary>
      /// 4 bytes per pixel with the channels alpha, red, green and blue
      /// </summary>
      ARGB32,
      /// <summary>
      /// 3 bytes per pixel with the channels blue, green and red
      /// </summary>
      BGR24,
      /// <summary>
      /// 4 bytes per pixel with the channels blue, green and red
      /// </summary>
      BGR32,
      /// <summary>
      /// 4 bytes per pixel with the channels blue, green, red and alpha
      /// </summary>
      BGRA32,
      /// <summary>
      /// 2 bytes per pixel, 5 bit red, 6 bits green and 5 bits blue
      /// </summary>
      RGB565,
      /// <summary>
      /// 4 bytes per pixel with the channels red, green, blue and alpha
      /// </summary>
      RGBA32,
   }

   internal static class BitmapFormatExtensions
   {
      public static RGBLuminanceSource.BitmapFormat ToZXing(this BitmapFormat format)
      {
         switch (format)
         {
            case BitmapFormat.ARGB32:
               return RGBLuminanceSource.BitmapFormat.ARGB32;
            case BitmapFormat.BGR24:
               return RGBLuminanceSource.BitmapFormat.BGR24;
            case BitmapFormat.BGR32:
               return RGBLuminanceSource.BitmapFormat.BGR32;
            case BitmapFormat.BGRA32:
               return RGBLuminanceSource.BitmapFormat.BGRA32;
            case BitmapFormat.Gray8:
               return RGBLuminanceSource.BitmapFormat.Gray8;
            case BitmapFormat.RGB24:
               return RGBLuminanceSource.BitmapFormat.RGB24;
            case BitmapFormat.RGB32:
               return RGBLuminanceSource.BitmapFormat.RGB32;
            case BitmapFormat.RGBA32:
               return RGBLuminanceSource.BitmapFormat.RGBA32;
            case BitmapFormat.RGB565:
               return RGBLuminanceSource.BitmapFormat.RGB565;
            default:
               return RGBLuminanceSource.BitmapFormat.Unknown;
         }
      }
   }
}
