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

namespace ZXing.Rendering
{
   /// <summary>
   /// represents the generated code as a byte array with pixel data (4 byte per pixel, BGRA)
   /// </summary>
   public sealed partial class PixelData
   {
      internal PixelData(int width, int height, byte[] pixels)
      {
         Height = height;
         Width = width;
         Pixels = pixels;
      }

      /// <summary>
      /// the generated code as byte array of BGRA pixels
      /// </summary>
      public byte[] Pixels { get; private set; }
      /// <summary>
      /// the width of the image
      /// </summary>
      public int Width { get; private set; }
      /// <summary>
      /// the height of the image
      /// </summary>
      public int Height { get; private set; }
   }
}
