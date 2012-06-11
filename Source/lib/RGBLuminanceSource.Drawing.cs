using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ZXing
{
   public partial class RGBLuminanceSource
   {
      public RGBLuminanceSource(Bitmap bitmap)
         : this(bitmap, bitmap.Width, bitmap.Height)
      {
      }

      public RGBLuminanceSource(Bitmap bitmap, int width, int height)
         : base(width, height)
      {
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         luminances = new byte[width * height];

         // The underlying raster of image consists of bytes with the luminance values
         var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
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
                  int offset = y * width;
                  for (int x = 0; x < width; x++)
                  {
                     c = bitmap.GetPixel(x, y);
                     luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                  }
               }
            }
            else
            {
               var strideStep = data.Stride;
               var buffer = new byte[stride];
               var ptrInBitmap = data.Scan0;

               // prepare palette for 1 and 8 bit indexed bitmaps
               var luminancePalette = new byte[bitmap.Palette.Entries.Length];
               for (var index = 0; index < bitmap.Palette.Entries.Length; index++)
               {
                  var color = bitmap.Palette.Entries[index];
                  luminancePalette[index] = (byte)(0.3 * color.R +
                                                    0.59 * color.G +
                                                    0.11 * color.B + 0.01);
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
                  var offset = y * width;
                  switch (pixelWidth)
                  {
                     case 0:
                        for (int x = 0; x * 8 < width; x++)
                        {
                           for (int subX = 0; subX < 8 && 8 * x + subX < width; subX++)
                           {
                              var index = (buffer[x] >> (7 - subX)) & 1;
                              luminances[offset + 8 * x + subX] = luminancePalette[index];
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
                           var luminance = (byte)(0.3 * buffer[x * pixelWidth] +
                                                   0.59 * buffer[x * pixelWidth + 1] +
                                                   0.11 * buffer[x * pixelWidth + 2] + 0.01);
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
            bitmap.UnlockBits(data);
         }
      }
   }
}