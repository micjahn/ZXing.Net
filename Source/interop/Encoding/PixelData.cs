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
   [Guid("FA47BA12-9F41-4AA0-ADB4-F23C9A2D8999")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public sealed class PixelData
   {
      internal PixelData(int width, int height, byte[] pixels)
      {
         Height = height;
         Width = width;
         Pixels = pixels;
      }

      public byte[] Pixels { get; private set; }
      public int Width { get; private set; }
      public int Height { get; private set; }
   }

   internal static class PixelDataExtensions
   {
      public static PixelData ToInterop(this Rendering.PixelData other)
      {
         if (other == null)
            return null;

         return new PixelData(other.Width, other.Height, other.Pixels);
      }
   }
}
