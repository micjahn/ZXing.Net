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

using System.IO;

namespace ZXing.Rendering
{
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

#if (NET45 || NET40 || NET35 || NET20 || WindowsCE) && !UNITY
      public System.Drawing.Bitmap ToBitmap()
      {
#if WindowsCE
         var bmp = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
         var bmpData = bmp.LockBits(
            new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            System.Drawing.Imaging.ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb);
#else
         var bmp = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         bmp.SetResolution(96, 96);
         var bmpData = bmp.LockBits(
            new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 
            System.Drawing.Imaging.ImageLockMode.WriteOnly, 
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
#endif
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
#endif

#if UNITY
      // Unity3D
#endif

#if NETFX_CORE
      public Windows.UI.Xaml.Media.Imaging.WriteableBitmap ToBitmap()
      {
         var bmp = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(Width, Height);
         using (var stream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(bmp.PixelBuffer))
         {
            stream.Write(Pixels, 0, Pixels.Length);
         }
         bmp.Invalidate();
         return bmp;
      }
#endif

#if SILVERLIGHT
      public System.Windows.Media.Imaging.WriteableBitmap ToBitmap()
      {
         var bmp = new System.Windows.Media.Imaging.WriteableBitmap(Width, Height);
         bmp.SetSource(new MemoryStream(Pixels));
         bmp.Invalidate();
         return bmp;
      }
#endif

#if MONOANDROID
      public Android.Graphics.Bitmap ToBitmap()
      {
         var pixels = Pixels;
         var colors = new int[Width*Height];
         for (var index = 0; index < Width*Height; index++)
         {
            colors[index] =
               pixels[index*4] << 24 |
               pixels[index*4 + 1] << 16 |
               pixels[index*4 + 2] << 8 |
               pixels[index*4 + 3];
         }
         return Android.Graphics.Bitmap.CreateBitmap(colors, Width, Height, Android.Graphics.Bitmap.Config.Argb8888);
      }
#endif

#if MONOTOUCH
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif
#endif

   }
}
