/*
* Copyright 2017 ZXing.Net authors
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


using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ZXing
{
   [ComImport]
   [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
   [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   unsafe interface IMemoryBufferByteAccess
   {
      void GetBuffer(out byte* buffer, out uint capacity);
   }

   /// <summary>
   /// class which represents the luminance values for a bitmap object of a SoftwareBitmap class
   /// </summary>
   public partial class SoftwareBitmapLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected SoftwareBitmapLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="softwareBitmap"></param>
      [System.CLSCompliant(false)]
      public SoftwareBitmapLuminanceSource(SoftwareBitmap softwareBitmap)
         : base(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight)
      {
         var height = softwareBitmap.PixelHeight;
         var width = softwareBitmap.PixelWidth;
         var luminanceIndex = 0;

         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         // luminance array is initialized with new byte[width * height]; in base class

         if (softwareBitmap.BitmapPixelFormat == BitmapPixelFormat.Gray8)
         {
            // shortcut
            var buffer = luminances.AsBuffer();
            softwareBitmap.CopyToBuffer(buffer);
         }
         else
         {
            using (var bitmapBuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
            using (var reference = bitmapBuffer.CreateReference())
            {
               unsafe
               {
                  byte* data;
                  uint capacity;
                  ((IMemoryBufferByteAccess) reference).GetBuffer(out data, out capacity);

                  // Get information about the BitmapBuffer
                  BitmapPlaneDescription desc = bitmapBuffer.GetPlaneDescription(0);

                  switch (softwareBitmap.BitmapPixelFormat)
                  {
                     case BitmapPixelFormat.Bgra8:
                     {
                        const int BYTES_PER_PIXEL = 4;
                        for (var row = 0; row < height; row++)
                        {
                           for (var col = 0; col < width; col++)
                           {
                              var currPixel = desc.StartIndex + desc.Stride*row + BYTES_PER_PIXEL*col;
                              var blue = data[currPixel];
                              var green = data[currPixel + 1];
                              var red = data[currPixel + 2];
                              var alpha = data[currPixel + 3];
                              var luminance =
                                 (byte)
                                 ((RChannelWeight*red + GChannelWeight*green + BChannelWeight*blue) >> ChannelWeight);
                              switch (softwareBitmap.BitmapAlphaMode)
                              {
                                 case BitmapAlphaMode.Straight:
                                    luminances[luminanceIndex] =
                                       (byte) (((luminance*alpha) >> 8) + (255*(255 - alpha) >> 8));
                                    break;
                                 default:
                                    luminances[luminanceIndex] = luminance;
                                    break;
                              }
                              luminanceIndex++;
                           }
                        }
                     }
                        break;
                     case BitmapPixelFormat.Rgba8:
                     {
                        const int BYTES_PER_PIXEL = 4;
                        for (var row = 0; row < height; row++)
                        {
                           for (var col = 0; col < width; col++)
                           {
                              var currPixel = desc.StartIndex + desc.Stride*row + BYTES_PER_PIXEL*col;
                              var red = data[currPixel];
                              var green = data[currPixel + 1];
                              var blue = data[currPixel + 2];
                              var alpha = data[currPixel + 3];
                              var luminance =
                                 (byte)
                                 ((RChannelWeight*red + GChannelWeight*green + BChannelWeight*blue) >> ChannelWeight);
                                 switch (softwareBitmap.BitmapAlphaMode)
                                 {
                                    case BitmapAlphaMode.Straight:
                                       luminances[luminanceIndex] =
                                          (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
                                       break;
                                    default:
                                       luminances[luminanceIndex] = luminance;
                                       break;
                                 }
                                 luminanceIndex++;
                           }
                        }
                     }
                        break;
                     case BitmapPixelFormat.Yuy2:
                        {
                           const int BYTES_PER_PIXEL = 2;
                           for (var row = 0; row < height; row++)
                           {
                              for (var col = 0; col < width; col++)
                              {
                                 var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;
                                 var y = data[currPixel];
                                 luminances[luminanceIndex] = y;
                                 luminanceIndex++;
                              }
                           }
                        }
                        break;
                     case BitmapPixelFormat.Rgba16:
                        {
                           const int SHORTS_PER_PIXEL = 4;
                           var uint16Data = (ushort*) data;
                           for (var row = 0; row < height; row++)
                           {
                              for (var col = 0; col < width; col++)
                              {
                                 var currPixel = desc.StartIndex + desc.Stride * row / 2 + SHORTS_PER_PIXEL * col;
                                 // scaling down from 16 to 8 bit because channel weights are related to 8 bit values
                                 var red = uint16Data[currPixel] >> 8;
                                 var green = uint16Data[currPixel+1] >> 8;
                                 var blue = uint16Data[currPixel+2] >> 8;
                                 var alpha = uint16Data[currPixel+3] >> 8;
                                 var luminance =
                                    (byte)
                                    ((RChannelWeight * red + GChannelWeight * green + BChannelWeight * blue) >> ChannelWeight);
                                 switch (softwareBitmap.BitmapAlphaMode)
                                 {
                                    case BitmapAlphaMode.Straight:
                                       luminances[luminanceIndex] =
                                          (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
                                       break;
                                    default:
                                       luminances[luminanceIndex] = luminance;
                                       break;
                                 }
                                 luminanceIndex++;
                              }
                           }
                        }
                        break;
                     case BitmapPixelFormat.Gray16:
                        {
                           const int SHORTS_PER_PIXEL = 1;
                           var uint16Data = (ushort*)data;
                           for (var row = 0; row < height; row++)
                           {
                              for (var col = 0; col < width; col++)
                              {
                                 var currPixel = desc.StartIndex + desc.Stride * row / 2 + SHORTS_PER_PIXEL * col;
                                 var gray16 = uint16Data[currPixel];
                                 luminances[luminanceIndex] = (byte)(gray16 >> 8);
                                 luminanceIndex++;
                              }
                           }
                        }
                        break;
                     case BitmapPixelFormat.Nv12:
                        {
                           for (var row = 0; row < height; row++)
                           {
                              for (var col = 0; col < width; col++)
                              {
                                 var currPixel = desc.StartIndex + desc.Stride * row + col;
                                 var y = data[currPixel];
                                 luminances[luminanceIndex] = y;
                                 luminanceIndex++;
                              }
                           }
                        }
                        break;
                     case BitmapPixelFormat.Gray8:
                        // done before
                        break;
                     default:
                        break;
                  }
               }
            }
         }
      }

      /// <summary>
      /// Should create a new luminance source with the right class type.
      /// The method is used in methods crop and rotate.
      /// </summary>
      /// <param name="newLuminances">The new luminances.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns></returns>
      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new SoftwareBitmapLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}