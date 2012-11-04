//using System.Drawing.Imaging;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#else
using System.Drawing;
#endif
using System;

namespace ZXing
{
public class RGBLuminanceSource : LuminanceSource
{
    public enum BitmapFormat
      {
         /// <summary>
         /// format of the byte[] isn't known. RGBLuminanceSource tries to determine the best possible value
         /// </summary>
         Unknown,
         /// <summary>
         /// grayscale array, the byte array is a luminance array with 1 byte per pixel
         /// </summary>
         Gray8,
         /// <summary>
         /// 3 byte per pixel with the channels red, green and blue
         /// </summary>
         RGB24,
         /// <summary>
         /// 4 byte per pixel with the channels red, green and blue
         /// </summary>
         RGB32,
         /// <summary>
         /// 4 byte per pixel with the channels alpha, red, green and blue
         /// </summary>
         ARGB32,
         /// <summary>
         /// 3 byte per pixel with the channels blue, green and red
         /// </summary>
         BGR24,
         /// <summary>
         /// 4 byte per pixel with the channels blue, green and red
         /// </summary>
         BGR32,
         /// <summary>
         /// 4 byte per pixel with the channels blue, green, red and alpha
         /// </summary>
         BGRA32,
         /// <summary>
         /// 2 byte per pixel, 5 bit red, 6 bits green and 5 bits blue
         /// </summary>
         RGB565
      }
      
    private byte[] luminances;
    private bool isRotated = false;
    private bool __isRegionSelect = false;
    private Rectangle __Region;

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
                int b = d[offset * 4 + x * 4];
                int g = d[offset * 4 + x * 4 + 1];
                int r = d[offset * 4 + x * 4 + 2];
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
    public RGBLuminanceSource(byte[] d, int W, int H,bool Is8Bit)
        : base(W, H)
    {
        __width = W;
        __height = H;
        luminances = new byte[W * H];
        Buffer.BlockCopy(d,0, luminances,0, W * H);
    }
    
    public RGBLuminanceSource(byte[] d, int W, int H, bool Is8Bit,Rectangle Region)
        : base(W, H)
    {
        __width = (int)Region.Width;
        __height = (int)Region.Height;
        __Region = Region;
        __isRegionSelect = true;
        //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
    }

#if !WINDOWS_PHONE
    public RGBLuminanceSource(Bitmap d, int W, int H)
#else
    public RGBLuminanceSource(WriteableBitmap d, int W, int H)
#endif
        : base(W, H)
    {
        int width = __width = W;
        int height = __height = H;
        // In order to measure pure decoding speed, we convert the entire image to a greyscale array
        // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
        luminances = new byte[width * height];
        //if (format == PixelFormat.Format8bppIndexed)
        {
            Color c;
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    c = d.GetPixel(x, y);
                    luminances[offset + x] = (byte)(((int)c.R) << 16 | ((int)c.G) << 8 | ((int)c.B));
                }
            }
        }
    }
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
                row[i] = luminances[y * width + i];
            //System.arraycopy(luminances, y * width, row, 0, width);
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
                row[i] = luminances[i * width + y];
            //System.arraycopy(luminances, y * width, row, 0, width);
            return row;
        }
    }
    public override byte[] Matrix
    {
        get { return luminances; }
    }

    public override LuminanceSource crop(int left, int top, int width, int height)
    {
        return base.crop(left, top, width, height);
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