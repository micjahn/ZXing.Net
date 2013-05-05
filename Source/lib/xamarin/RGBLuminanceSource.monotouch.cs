using System.Drawing;
using System;

namespace ZXing
{
   public partial class RGBLuminanceSource
   {
      /// <summary>
      /// Only for compatibility to older version
      /// should be replace by BitmapLuminanceSource and the faster grayscale algorithm
      /// </summary>
      /// <param name="d"></param>
      /// <param name="W"></param>
      /// <param name="H"></param>
      public RGBLuminanceSource(Bitmap d, int W, int H)
         : base(W, H)
      {
         int width = W;
         int height = H;
         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
         //if (format == PixelFormat.Format8bppIndexed)
         {
            for (int y = 0; y < height; y++)
            {
               int offset = y * width;
               for (int x = 0; x < width; x++)
               {
                  var c = d.GetPixel(x, y);
                  luminances[offset + x] = (byte)((RChannelWeight * c.R + GChannelWeight * c.G + BChannelWeight * c.B) >> ChannelWeight);
               }
            }
         }
      }
   }
}