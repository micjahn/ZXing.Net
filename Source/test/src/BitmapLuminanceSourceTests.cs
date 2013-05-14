/*
* Copyright 2013 ZXing.Net authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using NUnit.Framework;

namespace ZXing.Test
{
   [TestFixture]
   public class BitmapLuminanceSourceTests
   {
#if !SILVERLIGHT
      private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
      private static readonly DanielVaughan.Logging.ILog Log = DanielVaughan.Logging.LogManager.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

      private const string samplePicRelPath = @"../../../Source/test/data/luminance/01.jpg";

      [Test]
      [Explicit]
      public void Measure_Performance_of_different_Grayscale_Algorithms()
      {
         const int roundTripsPerAlgo = 1000;

         var bitmap = (Bitmap) Bitmap.FromFile(samplePicRelPath);

         var startAlgo1 = DateTime.Now;
         for (var count = 0; count < roundTripsPerAlgo; count++)
         {
            var luminanceSource = new GrayScalingAlgorithm1(bitmap);
            Assert.IsNotNull(luminanceSource);
         }
         var endAlgo1 = DateTime.Now;
         Log.InfoFormat("GrayScalingAlgorithm1: RoundTrips: {0}; Timespan: {1:c}", roundTripsPerAlgo, endAlgo1 - startAlgo1);

         var startAlgo2 = DateTime.Now;
         for (var count = 0; count < roundTripsPerAlgo; count++)
         {
            var luminanceSource = new GrayScalingAlgorithm2(bitmap);
            Assert.IsNotNull(luminanceSource);
         }
         var endAlgo2 = DateTime.Now;
         Log.InfoFormat("GrayScalingAlgorithm2: RoundTrips: {0}; Timespan: {1:c}", roundTripsPerAlgo, endAlgo2 - startAlgo2);

         var startAlgo3 = DateTime.Now;
         for (var count = 0; count < roundTripsPerAlgo; count++)
         {
            var luminanceSource = new GrayScalingAlgorithm3(bitmap);
            Assert.IsNotNull(luminanceSource);
         }
         var endAlgo3 = DateTime.Now;
         Log.InfoFormat("GrayScalingAlgorithm3: RoundTrips: {0}; Timespan: {1:c}", roundTripsPerAlgo, endAlgo3 - startAlgo3);

         var startAlgo4 = DateTime.Now;
         for (var count = 0; count < roundTripsPerAlgo; count++)
         {
            var luminanceSource = new GrayScalingAlgorithm4(bitmap);
            Assert.IsNotNull(luminanceSource);
         }
         var endAlgo4 = DateTime.Now;
         Log.InfoFormat("GrayScalingAlgorithm4: RoundTrips: {0}; Timespan: {1:c}", roundTripsPerAlgo, endAlgo4 - startAlgo4);
      }

      internal sealed class GrayScalingAlgorithm1 : BaseLuminanceSource
      {
         protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
         {
            throw new NotImplementedException();
         }

         public GrayScalingAlgorithm1(Bitmap bitmap)
            : base(bitmap.Width, bitmap.Height)
         {
            var height = bitmap.Height;
            var width = bitmap.Width;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            luminances = new byte[width*height];

            // The underlying raster of image consists of bytes with the luminance values
#if WindowsCE
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
#else
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
#endif
            try
            {
               var stride = Math.Abs(data.Stride);
               var pixelWidth = stride/width;

               if (pixelWidth > 4)
               {
                  // old slow way for unsupported bit depth
                  Color c;
                  for (int y = 0; y < height; y++)
                  {
                     int offset = y*width;
                     for (int x = 0; x < width; x++)
                     {
                        c = bitmap.GetPixel(x, y);
                        luminances[offset + x] = (byte) (0.3*c.R + 0.59*c.G + 0.11*c.B + 0.01);
                     }
                  }
               }
               else
               {
                  var strideStep = data.Stride;
                  var buffer = new byte[stride];
                  var ptrInBitmap = data.Scan0;

#if !WindowsCE
                  // prepare palette for 1 and 8 bit indexed bitmaps
                  var luminancePalette = new byte[bitmap.Palette.Entries.Length];
                  for (var index = 0; index < bitmap.Palette.Entries.Length; index++)
                  {
                     var color = bitmap.Palette.Entries[index];
                     luminancePalette[index] = (byte) (0.3*color.R +
                                                       0.59*color.G +
                                                       0.11*color.B + 0.01);
                  }
                  if (bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                      bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                  {
                     pixelWidth = 40;
                  }
#endif

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
#if !WindowsCE
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
#endif
                        case 2:
                           // should be RGB565 or RGB555, assume RGB565
                           {
                              for (int index = 0, x = 0; index < 2*width; index += 2, x++)
                              {
                                 var byte1 = buffer[index];
                                 var byte2 = buffer[index + 1];

                                 var b5 = byte1 & 0x1F;
                                 var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                                 var r5 = (byte2 >> 2) & 0x1F;
                                 var r8 = (r5*527 + 23) >> 6;
                                 var g8 = (g5*527 + 23) >> 6;
                                 var b8 = (b5*527 + 23) >> 6;

                                 luminances[offset + x] = (byte) (0.3*r8 + 0.59*g8 + 0.11*b8 + 0.01);
                              }
                           }
                           break;
                        case 3:
                           for (int x = 0; x < width; x++)
                           {
                              var luminance = (byte) (0.3*buffer[x*3] +
                                                      0.59*buffer[x*3 + 1] +
                                                      0.11*buffer[x*3 + 2] + 0.01);
                              luminances[offset + x] = luminance;
                           }
                           break;
                        case 4:
                           // 4 bytes without alpha channel value
                           for (int x = 0; x < width; x++)
                           {
                              var luminance = (byte) (0.30*buffer[x*4] +
                                                      0.59*buffer[x*4 + 1] +
                                                      0.11*buffer[x*4 + 2] + 0.01);

                              luminances[offset + x] = luminance;
                           }
                           break;
                        case 40:
                           // with alpha channel; some barcodes are completely black if you
                           // only look at the r, g and b channel but the alpha channel controls
                           // the view
                           for (int x = 0; x < width; x++)
                           {
                              var luminance = (byte) (0.30*buffer[x*4] +
                                                      0.59*buffer[x*4 + 1] +
                                                      0.11*buffer[x*4 + 2] + 0.01);

                              // calculating the resulting luminance based upon a white background
                              // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                              // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                              var alpha = buffer[x*4 + 3];
                              luminance = (byte) (((luminance*alpha) >> 8) + (255*(255 - alpha) >> 8));
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

      internal sealed class GrayScalingAlgorithm2 : BaseLuminanceSource
      {
         protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
         {
            throw new NotImplementedException();
         }

         public GrayScalingAlgorithm2(Bitmap bitmap)
            : base(bitmap.Width, bitmap.Height)
         {
            var height = bitmap.Height;
            var width = bitmap.Width;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // The underlying raster of image consists of bytes with the luminance values
#if WindowsCE
         var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
#else
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
#endif
            try
            {
               var stride = Math.Abs(data.Stride);
               var pixelWidth = stride/width;

               if (pixelWidth > 4)
               {
                  // old slow way for unsupported bit depth
                  Color c;
                  for (int y = 0; y < height; y++)
                  {
                     int offset = y*width;
                     for (int x = 0; x < width; x++)
                     {
                        c = bitmap.GetPixel(x, y);
                        luminances[offset + x] = (byte) ((RChannelWeight*c.R + GChannelWeight*c.G + BChannelWeight*c.B) >> ChannelWeight);
                     }
                  }
               }
               else
               {
                  var strideStep = data.Stride;
                  var buffer = new byte[stride];
                  var ptrInBitmap = data.Scan0;

#if !WindowsCE
                  // prepare palette for 1 and 8 bit indexed bitmaps
                  var luminancePalette = new byte[bitmap.Palette.Entries.Length];
                  for (var index = 0; index < bitmap.Palette.Entries.Length; index++)
                  {
                     var color = bitmap.Palette.Entries[index];
                     luminancePalette[index] = (byte) ((RChannelWeight*color.R +
                                                        RChannelWeight*color.G +
                                                        RChannelWeight*color.B) >> ChannelWeight);
                  }
                  if (bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                      bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                  {
                     pixelWidth = 40;
                  }
#endif

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
#if !WindowsCE
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
#endif
                        case 2:
                           // should be RGB565 or RGB555, assume RGB565
                           {
                              var maxIndex = 2*width;
                              for (int index = 0; index < maxIndex; index += 2)
                              {
                                 var byte1 = buffer[index];
                                 var byte2 = buffer[index + 1];

                                 var b5 = byte1 & 0x1F;
                                 var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                                 var r5 = (byte2 >> 2) & 0x1F;
                                 var r8 = (r5*527 + 23) >> 6;
                                 var g8 = (g5*527 + 23) >> 6;
                                 var b8 = (b5*527 + 23) >> 6;

                                 luminances[offset] = (byte) ((RChannelWeight*r8 + GChannelWeight*g8 + BChannelWeight*b8) >> ChannelWeight);
                                 offset++;
                              }
                           }
                           break;
                        case 3:
                           {
                              var maxIndex = width*3;
                              for (int x = 0; x < maxIndex; x += 3)
                              {
                                 var luminance = (byte) ((RChannelWeight*buffer[x] +
                                                          GChannelWeight*buffer[x + 1] +
                                                          BChannelWeight*buffer[x + 2]) >> ChannelWeight);
                                 luminances[offset] = luminance;
                                 offset++;
                              }
                           }
                           break;
                        case 4:
                           // 4 bytes without alpha channel value
                           {
                              var maxIndex = 4*width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 var luminance = (byte) ((RChannelWeight*buffer[x] +
                                                          GChannelWeight*buffer[x + 1] +
                                                          BChannelWeight*buffer[x + 2]) >> ChannelWeight);

                                 luminances[offset] = luminance;
                                 offset++;
                              }
                           }
                           break;
                        case 40:
                           // with alpha channel; some barcodes are completely black if you
                           // only look at the r, g and b channel but the alpha channel controls
                           // the view
                           {
                              var maxIndex = 4*width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 var luminance = (byte) ((BChannelWeight*buffer[x] +
                                                          GChannelWeight*buffer[x + 1] +
                                                          RChannelWeight*buffer[x + 2]) >> ChannelWeight);

                                 // calculating the resulting luminance based upon a white background
                                 // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                                 // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                                 var alpha = buffer[x + 3];
                                 luminance = (byte) (((luminance*alpha) >> 8) + (255*(255 - alpha) >> 8) + 1);
                                 luminances[offset] = luminance;
                                 offset++;
                              }
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

      internal sealed class GrayScalingAlgorithm3 : BaseLuminanceSource
      {
         protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
         {
            throw new NotImplementedException();
         }

         public GrayScalingAlgorithm3(Bitmap bitmap)
            : base(bitmap.Width, bitmap.Height)
         {
            var height = bitmap.Height;
            var width = bitmap.Width;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // The underlying raster of image consists of bytes with the luminance values
#if WindowsCE
         var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
#else
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
#endif
            try
            {
               var stride = Math.Abs(data.Stride);
               var pixelWidth = stride/width;

               if (pixelWidth > 4)
               {
                  // old slow way for unsupported bit depth
                  Color c;
                  for (int y = 0; y < height; y++)
                  {
                     int offset = y*width;
                     for (int x = 0; x < width; x++)
                     {
                        c = bitmap.GetPixel(x, y);
                        luminances[offset + x] = (byte) ((RChannelWeight*c.R + GChannelWeight*c.G + BChannelWeight*c.B) >> ChannelWeight);
                     }
                  }
               }
               else
               {
                  var strideBlockCount = (int) ((4096*16)/stride);
                  if (strideBlockCount < 1)
                     strideBlockCount = 1;
                  var strideBlockSize = strideBlockCount*stride;
                  var strideStep = data.Stride*strideBlockCount;
                  var linePadding = stride - width*pixelWidth;
                  var buffer = new byte[strideBlockSize];
                  var ptrInBitmap = data.Scan0;

#if !WindowsCE
                  // prepare palette for 1 and 8 bit indexed bitmaps
                  var luminancePalette = new byte[bitmap.Palette.Entries.Length];
                  for (var index = 0; index < bitmap.Palette.Entries.Length; index++)
                  {
                     var color = bitmap.Palette.Entries[index];
                     luminancePalette[index] = (byte) ((RChannelWeight*color.R +
                                                        RChannelWeight*color.G +
                                                        RChannelWeight*color.B) >> ChannelWeight);
                  }
                  if (bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                      bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                  {
                     pixelWidth = 40;
                  }
#endif

                  for (int y = 0; y < height; y += strideBlockCount)
                  {
                     // copy a scanline not the whole bitmap because of memory usage
                     if (y + strideBlockCount > height)
                     {
                        strideBlockCount = (height - y);
                        strideBlockSize = strideBlockCount*stride;
                     }
                     Marshal.Copy(ptrInBitmap, buffer, 0, strideBlockSize);
#if NET40
                     ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                     ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                     var offset = y*width;
                     switch (pixelWidth)
                     {
#if !WindowsCE
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
#endif
                        case 2:
                           // should be RGB565 or RGB555, assume RGB565
                           {
                              var maxIndex = 2*width;
                              for (int index = 0; index < maxIndex; index += 2)
                              {
                                 var byte1 = buffer[index];
                                 var byte2 = buffer[index + 1];

                                 var b5 = byte1 & 0x1F;
                                 var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                                 var r5 = (byte2 >> 2) & 0x1F;
                                 var r8 = (r5*527 + 23) >> 6;
                                 var g8 = (g5*527 + 23) >> 6;
                                 var b8 = (b5*527 + 23) >> 6;

                                 luminances[offset] = (byte) ((RChannelWeight*r8 + GChannelWeight*g8 + BChannelWeight*b8) >> ChannelWeight);
                                 offset++;
                              }
                           }
                           break;
                        case 3:
                           {
                              var x = 0;
                              for (var strideBlockIndex = 0; strideBlockIndex < strideBlockCount; strideBlockIndex++)
                              {
                                 var maxIndex = width*3*(strideBlockIndex + 1);
                                 for (; x < maxIndex; x += 3)
                                 {
                                    var luminance = (byte) ((RChannelWeight*buffer[x] +
                                                             GChannelWeight*buffer[x + 1] +
                                                             BChannelWeight*buffer[x + 2]) >> ChannelWeight);
                                    luminances[offset] = luminance;
                                    offset++;
                                 }
                                 x += linePadding;
                              }
                           }
                           break;
                        case 4:
                           // 4 bytes without alpha channel value
                           {
                              var maxIndex = 4*width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 var luminance = (byte) ((RChannelWeight*buffer[x] +
                                                          GChannelWeight*buffer[x + 1] +
                                                          BChannelWeight*buffer[x + 2]) >> ChannelWeight);

                                 luminances[offset] = luminance;
                                 offset++;
                              }
                           }
                           break;
                        case 40:
                           // with alpha channel; some barcodes are completely black if you
                           // only look at the r, g and b channel but the alpha channel controls
                           // the view
                           {
                              var maxIndex = 4*width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 var luminance = (byte) ((BChannelWeight*buffer[x] +
                                                          GChannelWeight*buffer[x + 1] +
                                                          RChannelWeight*buffer[x + 2]) >> ChannelWeight);

                                 // calculating the resulting luminance based upon a white background
                                 // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                                 // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                                 var alpha = buffer[x + 3];
                                 luminance = (byte) (((luminance*alpha) >> 8) + (255*(255 - alpha) >> 8) + 1);
                                 luminances[offset] = luminance;
                                 offset++;
                              }
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

      internal sealed class GrayScalingAlgorithm4 : BaseLuminanceSource
      {
         protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
         {
            throw new NotImplementedException();
         }

         public GrayScalingAlgorithm4(Bitmap bitmap)
            : base(bitmap.Width, bitmap.Height)
         {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppRgb);
            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);
            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });
            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();
            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);
            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
            //dispose the Graphics object
            g.Dispose();

            var height = bitmap.Height;
            var width = bitmap.Width;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // The underlying raster of image consists of bytes with the luminance values
#if WindowsCE
            var data = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
#else
            var data = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, newBitmap.PixelFormat);
#endif
            try
            {
               var stride = Math.Abs(data.Stride);
               var pixelWidth = stride / width;

               if (pixelWidth > 4)
               {
                  // old slow way for unsupported bit depth
                  Color c;
                  for (int y = 0; y < height; y++)
                  {
                     int offset = y * width;
                     for (int x = 0; x < width; x++)
                     {
                        c = newBitmap.GetPixel(x, y);
                        luminances[offset + x] = c.R;
                     }
                  }
               }
               else
               {
                  var strideStep = data.Stride;
                  var buffer = new byte[stride];
                  var ptrInBitmap = data.Scan0;

#if !WindowsCE
                  // prepare palette for 1 and 8 bit indexed bitmaps
                  var luminancePalette = new byte[newBitmap.Palette.Entries.Length];
                  for (var index = 0; index < newBitmap.Palette.Entries.Length; index++)
                  {
                     var color = newBitmap.Palette.Entries[index];
                     luminancePalette[index] = (byte) color.R;
                  }
                  if (newBitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                      newBitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                  {
                     pixelWidth = 40;
                  }
#endif

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
#if !WindowsCE
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
#endif
                        case 2:
                           // should be RGB565 or RGB555, assume RGB565
                           {
                              var maxIndex = 2 * width;
                              for (int index = 0; index < maxIndex; index += 2)
                              {
                                 var byte1 = buffer[index];
                                 var byte2 = buffer[index + 1];

                                 var b5 = byte1 & 0x1F;
                                 var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                                 var r5 = (byte2 >> 2) & 0x1F;
                                 var r8 = (r5 * 527 + 23) >> 6;
                                 var g8 = (g5 * 527 + 23) >> 6;
                                 var b8 = (b5 * 527 + 23) >> 6;

                                 luminances[offset] = (byte)((RChannelWeight * r8 + GChannelWeight * g8 + BChannelWeight * b8) >> ChannelWeight);
                                 offset++;
                              }
                           }
                           break;
                        case 3:
                           {
                              var maxIndex = width * 3;
                              for (int x = 0; x < maxIndex; x += 3)
                              {
                                 luminances[offset] = buffer[x];
                                 offset++;
                              }
                           }
                           break;
                        case 4:
                           // 4 bytes without alpha channel value
                           {
                              var maxIndex = 4 * width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 luminances[offset] = buffer[x];
                                 offset++;
                              }
                           }
                           break;
                        case 40:
                           // with alpha channel; some barcodes are completely black if you
                           // only look at the r, g and b channel but the alpha channel controls
                           // the view
                           {
                              var maxIndex = 4 * width;
                              for (int x = 0; x < maxIndex; x += 4)
                              {
                                 var luminance = buffer[x];

                                 // calculating the resulting luminance based upon a white background
                                 // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                                 // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                                 var alpha = buffer[x + 3];
                                 luminance = (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8) + 1);
                                 luminances[offset] = luminance;
                                 offset++;
                              }
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
               newBitmap.UnlockBits(data);
            }
         }
      }
   }
}