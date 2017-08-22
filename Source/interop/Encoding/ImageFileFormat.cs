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

namespace ZXing.Interop.Encoding
{
   [ComVisible(true)]
   [Guid("91B0BED9-1238-4469-9C4F-6C06ECA006C6")]
   public enum ImageFileFormat
   {
      Bmp,
      Gif,
      Jpeg,
      Png,
      Tiff,
      Wmf
   }

   internal static class ImageFileFormatExtensions
   {
      public static System.Drawing.Imaging.ImageFormat ToDrawingFormat(this ImageFileFormat format)
      {
         switch (format)
         {
            case ImageFileFormat.Bmp:
               return System.Drawing.Imaging.ImageFormat.Bmp;
            case ImageFileFormat.Gif:
               return System.Drawing.Imaging.ImageFormat.Gif;
            case ImageFileFormat.Jpeg:
               return System.Drawing.Imaging.ImageFormat.Jpeg;
            case ImageFileFormat.Png:
               return System.Drawing.Imaging.ImageFormat.Png;
            case ImageFileFormat.Tiff:
               return System.Drawing.Imaging.ImageFormat.Tiff;
            case ImageFileFormat.Wmf:
               return System.Drawing.Imaging.ImageFormat.Wmf;
            default:
               return System.Drawing.Imaging.ImageFormat.Bmp;
         }
      }
   }
}
