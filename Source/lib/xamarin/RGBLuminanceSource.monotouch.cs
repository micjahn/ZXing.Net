using System;
using System.Drawing;
using System.Runtime.InteropServices;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace ZXing
{
   public partial class RGBLuminanceSource
   {
      /// <summary>
      /// Only for compatibility to older version
      /// should be replace by BitmapLuminanceSource and the faster grayscale algorithm
      /// </summary>
      /// <param name="d"></param>
      public RGBLuminanceSource(UIImage d)
         : base(d.CGImage.Width, d.CGImage.Height)
      {
         CalculateLuminance(d);
      }

      private void CalculateLuminance(UIImage d)
      {
         var imageRef = d.CGImage;
         var width = imageRef.Width;
         var height = imageRef.Height;
         var colorSpace = CGColorSpace.CreateDeviceRGB();

         var rawData = Marshal.AllocHGlobal(height * width * 4);

         try
         {
			var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little; 
            var context = new CGBitmapContext(rawData, width, height, 8, 4 * width,
            colorSpace, (CGImageAlphaInfo)flags);

            context.DrawImage(new RectangleF(0.0f, 0.0f, (float)width, (float)height), imageRef);
            var pixelData = new byte[height * width * 4];
            Marshal.Copy(rawData, pixelData, 0, pixelData.Length);

            CalculateLuminance(pixelData, BitmapFormat.BGRA32);
         }
         finally
         {
            Marshal.FreeHGlobal(rawData);
         }
      }
   }
}