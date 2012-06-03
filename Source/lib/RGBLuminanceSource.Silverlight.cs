namespace ZXing
{
   public partial class RGBLuminanceSource : LuminanceSource
   {
      private System.Windows.Rect __Region;

      public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit, System.Windows.Rect Region)
         : base(W, H)
      {
         __width = (int)Region.Width;
         __height = (int)Region.Height;
         __Region = Region;
         __isRegionSelect = true;
         //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
      }

      public RGBLuminanceSource(System.Windows.Media.Imaging.WriteableBitmap d)
         : this(d, d.PixelWidth, d.PixelHeight)
      {

      }
      public RGBLuminanceSource(System.Windows.Media.Imaging.WriteableBitmap d, int W, int H)
         : base(W, H)
      {
         int width = __width = W;
         int height = __height = H;
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         luminances = new byte[width * height];
         //if (format == PixelFormat.Format8bppIndexed)
         {
            System.Windows.Media.Color c;
            for (int y = 0; y < height; y++)
            {
               int offset = y * width;
               for (int x = 0; x < width; x++)
               {
                  int srcPixel = d.Pixels[x + offset];
                  c = System.Windows.Media.Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                        (byte)((srcPixel >> 0x10) & 0xff),
                        (byte)((srcPixel >> 8) & 0xff),
                        (byte)(srcPixel & 0xff));
                  luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
               }
            }
         }
      }
   }
}