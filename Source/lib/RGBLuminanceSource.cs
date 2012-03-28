using System;
#if !SILVERLIGHT
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

#endif

namespace ZXing
{
   public class RGBLuminanceSource : LuminanceSource
   {
      private byte[] luminances;
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
         luminances = new byte[width * height];
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
                  luminances[offset + x] = (byte)r;
               }
               else
               {
                  // Calculate luminance cheaply, favoring green.
                  luminances[offset + x] = (byte)((r + g + g + b) >> 2);
               }
            }
         }
      }
      public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit)
         : base(W, H)
      {
         __width = W;
         __height = H;
         luminances = new byte[W * H];
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
                    luminances[offset + x] = (byte) (0.3*c.R + 0.59*c.G + 0.11*c.B);
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

      public RGBLuminanceSource(Bitmap d)
         : this(d, d.Width, d.Height)
      {
      }

      public RGBLuminanceSource(Bitmap d, int W, int H)
         : base(W, H)
      {
         int width = __width = W;
         int height = __height = H;
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         luminances = new byte[width * height];
          
         // The underlying raster of image consists of bytes with the luminance values
         var data = d.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, d.PixelFormat);
         try
         {
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;

            if (pixelWidth == 2 || pixelWidth > 4)
            {
               // old slow way for unsupported bit depth
               Color c;
               for (int y = 0; y < height; y++)
               {
                  int offset = y*width;
                  for (int x = 0; x < width; x++)
                  {
                     c = d.GetPixel(x, y);
                     luminances[offset + x] = (byte)(0.3*c.R + 0.59*c.G + 0.11*c.B);
                  }
               }
            }
            else
            {
               var strideStep = data.Stride;
               var buffer = new byte[stride];
               var ptrInBitmap = data.Scan0;

               // prepare palette for 1 and 8 bit indexed bitmaps
               var luminancePalette = new byte[d.Palette.Entries.Length];
               for (var index = 0; index < d.Palette.Entries.Length; index++)
               {
                  var color = d.Palette.Entries[index];
                  luminancePalette[index] = (byte) (0.3*color.R +
                                                    0.59*color.G +
                                                    0.11*color.B);
               }

               for (int y = 0; y < height; y++)
               {
                  // copy a scanline not the whole bitmap because of memory usage
                  Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40
                  ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                  ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                  var offset = y*width;
                  switch (pixelWidth)
                  {
                     case 0:
                        for (int x = 0; x*8 < width; x++)
                        {
                           for (int subX = 0; subX < 8 && 8*x + subX < width; subX++)
                           {
                              var index = (buffer[x] >> (7 - subX)) & 1;
                              luminances[offset + 8*x + subX] = luminancePalette[index];
                           }
                        }
                        break;
                     case 1:
                        for (int x = 0; x < width; x++)
                        {
                           luminances[offset + x] = luminancePalette[buffer[x]];
                        }
                        break;
                     case 3:
                     case 4:
                        for (int x = 0; x < width; x++)
                        {
                           var luminance = (byte) (0.3*buffer[x*pixelWidth] +
                                                   0.59*buffer[x*pixelWidth + 1] +
                                                   0.11*buffer[x*pixelWidth + 2]);
                           luminances[offset + x] = luminance;
                        }
                        break;
                     default:
                        throw new NotSupportedException();
                  }
               }
            }
         }
         finally
         {
            d.UnlockBits(data);
         }
      }

#endif

      override public byte[] getRow(int y, byte[] row)
      {
         if (isRotated == false)
         {
            int width = Width;
            if (row == null || row.Length < width)
            {
               row = new byte[width];
            }
            for (int i = 0; i < width; i++)
               row[i] = (byte)(luminances[y * width + i] - 128);
            return row;
         }
         else
         {
            int width = __width;
            int height = __height;
            if (row == null || row.Length < height)
            {
               row = new byte[height];
            }
            for (int i = 0; i < height; i++)
               row[i] = (byte)(luminances[i * width + y] - 128);
            return row;
         }
      }
      public override byte[] Matrix
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