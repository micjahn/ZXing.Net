using System;
using System.Runtime.InteropServices;

#if __UNIFIED__
using UIKit;
using CoreGraphics;
#else
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using CGRect = global::System.Drawing.RectangleF;
using CGPoint = global::System.Drawing.PointF;
using CGSize = global::System.Drawing.SizeF;
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

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
          : base((int)d.CGImage.Width, (int)d.CGImage.Height)
      {
         CalculateLuminance(d);
      }

      private void CalculateLuminance(UIImage d)
      {
         var imageRef = d.CGImage;
         var width = (int)imageRef.Width;
         var height = (int)imageRef.Height;
         var colorSpace = CGColorSpace.CreateDeviceRGB();

         var rawData = Marshal.AllocHGlobal(height * width * 4);

         try
         {
			var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little; 
            var context = new CGBitmapContext(rawData, width, height, 8, 4 * width,
            colorSpace, (CGImageAlphaInfo)flags);

            context.DrawImage(new CGRect(0.0f, 0.0f, (float)width, (float)height), imageRef);
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