using System;

namespace ZXing
{
   public partial class RGBLuminanceSource : LuminanceSource
   {
      private int __height;
      private int __width;
      private byte[] luminances;
      private bool __isRegionSelect = false;

      override public int Height
      {
         get
         {
            return __height;
         }

      }
      override public int Width
      {
         get
         {
            return __width;
         }

      }

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

      override public byte[] getRow(int y, byte[] row)
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

      public override byte[] Matrix
      {
         get { return luminances; }
      }

      public override LuminanceSource rotateCounterClockwise()
      {
         var rotatedLuminances = new byte[__width * __height];
         var newWidth = __height;
         var newHeight = __width;
         for (var yold = 0; yold < __height; yold++)
         {
            for (var xold = 0; xold < __width; xold++)
            {
               var ynew = xold;
               var xnew = newWidth - yold - 1;
               rotatedLuminances[ynew * newWidth + xnew] = luminances[yold * __width + xold];
            }
         }
         luminances = rotatedLuminances;
         __height = newHeight;
         __width = newWidth;
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