using System;
#if !SILVERLIGHT
using System.Drawing.Imaging;
using System.Drawing;
#endif

namespace ZXing
{
   public class RGBLuminanceSource : LuminanceSource
   {

      private sbyte[] luminances;
      private bool isRotated = false;
      private bool __isRegionSelect = false;
#if SILVERLIGHT
   private System.Windows.Rect __Region;
#else
      private Rectangle __Region;
#endif

      override public int Height
      {
         get
         {
            if (!isRotated)
               return __height;
            else
               return __width;
         }

      }
      override public int Width
      {
         get
         {
            if (!isRotated)
               return __width;
            else
               return __height;
         }

      }
      private int __height;
      private int __width;

      public RGBLuminanceSource(byte[] d, int W, int H)
         : base(W, H)
      {
         __width = W;
         __height = H;
         int width = W;
         int height = H;
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         luminances = new sbyte[width * height];
         for (int y = 0; y < height; y++)
         {
            int offset = y * width;
            for (int x = 0; x < width; x++)
            {
               int r = d[offset * 3 + x * 3];
               int g = d[offset * 3 + x * 3 + 1];
               int b = d[offset * 3 + x * 3 + 2];
               if (r == g && g == b)
               {
                  // Image is already greyscale, so pick any channel.
                  luminances[offset + x] = (sbyte)r;
               }
               else
               {
                  // Calculate luminance cheaply, favoring green.
                  luminances[offset + x] = (sbyte)((r + g + g + b) >> 2);
               }
            }
         }
      }
      public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit)
         : base(W, H)
      {
         __width = W;
         __height = H;
         luminances = new sbyte[W * H];
         Buffer.BlockCopy(d, 0, luminances, 0, W * H);
      }

#if SILVERLIGHT

   public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit, System.Windows.Rect Region)
    : base(W, H)
   {
      __width = (int)Region.Width;
      __height = (int)Region.Height;
      __Region = Region;
      __isRegionSelect = true;
      //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
   }

   public RGBLuminanceSource(System.Windows.Media.Imaging.WriteableBitmap d, int W, int H)
        : base(W, H)
    {
        int width = __width = W;
        int height = __height = H;
        // In order to measure pure decoding speed, we convert the entire image to a greyscale array
        // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
        luminances = new sbyte[width * height];
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
                    luminances[offset + x] = (sbyte) (0.3*c.R + 0.59*c.G + 0.11*c.B);
                }
            }
        }
    }

#else

      public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit, Rectangle Region)
         : base(W, H)
      {
         __width = Region.Width;
         __height = Region.Height;
         __Region = Region;
         __isRegionSelect = true;
         //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
      }

      public RGBLuminanceSource(Bitmap d, int W, int H)
         : base(W, H)
      {
         int width = __width = W;
         int height = __height = H;
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         luminances = new sbyte[width * height];
         //if (format == PixelFormat.Format8bppIndexed)
         {
            Color c;
            for (int y = 0; y < height; y++)
            {
               int offset = y * width;
               for (int x = 0; x < width; x++)
               {
                  c = d.GetPixel(x, y);
                  luminances[offset + x] = (sbyte) (0.3*c.R + 0.59*c.G + 0.11*c.B);
               }
            }
         }
      }

#endif

      override public sbyte[] getRow(int y, sbyte[] row)
      {
         if (isRotated == false)
         {
            int width = Width;
            if (row == null || row.Length < width)
            {
               row = new sbyte[width];
            }
            for (int i = 0; i < width; i++)
               row[i] = (sbyte)(luminances[y * width + i] - 128);
            return row;
         }
         else
         {
            int width = __width;
            int height = __height;
            if (row == null || row.Length < height)
            {
               row = new sbyte[height];
            }
            for (int i = 0; i < height; i++)
               row[i] = (sbyte)(luminances[i * width + y] - 128);
            return row;
         }
      }
      public override sbyte[] Matrix
      {
         get { return luminances; }
      }

      public override LuminanceSource rotateCounterClockwise()
      {
         isRotated = true;
         return this;
      }
      public override bool RotateSupported
      {
         get
         {
            return true;
         }
      }
   }
}