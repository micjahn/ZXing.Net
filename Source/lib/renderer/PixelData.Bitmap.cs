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
      /// <summary>
      /// converts the pixel data to a bitmap object
      /// </summary>
      /// <returns></returns>
      public System.Drawing.Bitmap ToBitmap()
      {
         var bmp = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         bmp.SetResolution(96, 96);
         var bmpData = bmp.LockBits(
            new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 
            System.Drawing.Imaging.ImageLockMode.WriteOnly, 
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

         try
         {
            //Copy the data from the byte array into BitmapData.Scan0
            System.Runtime.InteropServices.Marshal.Copy(Pixels, 0, bmpData.Scan0, Pixels.Length);
         }
         finally
         {
            //Unlock the pixels
            bmp.UnlockBits(bmpData);
         }

         return bmp;
      }
   }
}
