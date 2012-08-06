using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZXing
{
   public partial class RGBLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RGBLuminanceSource"/> class.
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      public RGBLuminanceSource(BitmapSource bitmap)
         : base(bitmap.PixelWidth, bitmap.PixelHeight)
      {
         switch (bitmap.Format.ToString())
         {
            case "Bgr24":
            case "Bgr32":
            case "Bgra32":
               CalculateLuminanceBGR(bitmap);
               break;
            case "Rgb24":
               CalculateLuminanceRGB(bitmap);
               break;
            case "Bgr565":
               CalculateLuminanceBGR565(bitmap);
               break;
            default:
               // there is no special conversion routine to luminance values
               // we have to convert the image to a supported format
               bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
               CalculateLuminanceBGR(bitmap);
               break;
         }
      }

      private void CalculateLuminanceRGB(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         luminances = new byte[width * height];

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var r = buffer[curX];
               var g = buffer[curX + 1];
               var b = buffer[curX + 2];
               luminances[luminanceIndex] = (byte)
                  (0.3 * r + 0.59 * g + 0.11 * b + 0.01);
               luminanceIndex++;
            }
            rect.Y++;
         }
      }

      private void CalculateLuminanceBGR(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         luminances = new byte[width * height];

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var b = buffer[curX];
               var g = buffer[curX + 1];
               var r = buffer[curX + 2];
               luminances[luminanceIndex] = (byte)
                  (0.3 * r + 0.59 * g + 0.11 * b + 0.01);
               luminanceIndex++;
            }
            rect.Y++;
         }
      }

      private void CalculateLuminanceBGR565(BitmapSource bitmap)
      {
         var width = bitmap.PixelWidth;
         var height = bitmap.PixelHeight;
         var stepX = (bitmap.Format.BitsPerPixel + 7) / 8;
         var bufferSize = width * stepX;
         var buffer = new byte[bufferSize];
         var rect = new Int32Rect(0, 0, width, 1);
         var luminanceIndex = 0;

         luminances = new byte[width * height];

         for (var curY = 0; curY < height; curY++)
         {
            bitmap.CopyPixels(rect, buffer, bufferSize, 0);
            for (var curX = 0; curX < bufferSize; curX += stepX)
            {
               var byte1 = buffer[curX];
               var byte2 = buffer[curX + 1];
               // cheap, not fully accurate conversion
               var b5 = byte1 & 0x1F;
               var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
               var r5 = (byte2 >> 2) & 0x1F;
               var r8 = (r5 * 527 + 23) >> 6;
               var g8 = (g5 * 527 + 23) >> 6;
               var b8 = (b5 * 527 + 23) >> 6;

               luminances[luminanceIndex] = (byte)
                  (0.3 * r8 + 0.59 * g8 + 0.11 * b8 + 0.01);
               luminanceIndex++;
            }
            rect.Y++;
         }
      }
   }
}
