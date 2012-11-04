using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Runtime.InteropServices;
namespace System.Drawing
{
	public class Bitmap : IDisposable
	{
		private UIImage _backingImage = null;
		byte[] pixelData = new byte[0];
		int width = 0;
		int height = 0;
		IntPtr rawData;
		
		public Bitmap (UIImage image)
		{
			_backingImage = image;
			CGImage imageRef = _backingImage.CGImage;
			width = imageRef.Width;
			height = imageRef.Height;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			
			rawData = Marshal.AllocHGlobal(height*width*4);
			CGContext context = new CGBitmapContext(
				rawData, width, height, 8, 4*width, colorSpace, CGImageAlphaInfo.PremultipliedLast
			);
			context.DrawImage(new System.Drawing.RectangleF(0.0f,0.0f,(float)width,(float)height),imageRef);
			
			pixelData = new byte[height*width*4];
			Marshal.Copy(rawData,pixelData,0,pixelData.Length);
		}

		public int Width { get { return width; } }
		public int Height { get { return height; } }
		
		public System.Drawing.Color GetPixel(int x, int y)
		{
			try {				
				byte bytesPerPixel = 4;
				int bytesPerRow = width * bytesPerPixel;
				int rowOffset = y * bytesPerRow;
				int colOffset = x * bytesPerPixel;
				int pixelDataLoc = rowOffset + colOffset;
				
				var ret = System.Drawing.Color.FromArgb(pixelData[pixelDataLoc+3],pixelData[pixelDataLoc+0],pixelData[pixelDataLoc+1],pixelData[pixelDataLoc+2]);
				return ret;
			} catch (Exception ex) {
				Console.WriteLine("Orig: {0}x{1}", _backingImage.Size.Width,_backingImage.Size.Height);
				Console.WriteLine("Req:  {0}x{1}", x, y);
				throw ex;
			}
		
			return System.Drawing.Color.FromArgb(0,0,0,0);
		}
	

		void IDisposable.Dispose ()
		{
			Marshal.FreeHGlobal(rawData);
		}
}
}

