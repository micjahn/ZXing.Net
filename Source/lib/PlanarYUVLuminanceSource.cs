/*
 * Copyright 2009 ZXing authors
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

namespace ZXing
{
   /// <summary>
   /// This object extends LuminanceSource around an array of YUV data returned from the camera driver,
   /// with the option to crop to a rectangle within the full data. This can be used to exclude
   /// superfluous pixels around the perimeter and speed up decoding.
   /// It works for any pixel format where the Y channel is planar and appears first, including
   /// YCbCr_420_SP and YCbCr_422_SP.
   /// @author dswitkin@google.com (Daniel Switkin)
   /// </summary>
   public sealed class PlanarYUVLuminanceSource : BaseLuminanceSource
   {
      private const int THUMBNAIL_SCALE_FACTOR = 2;

      private readonly byte[] yuvData;
      private readonly int dataWidth;
      private readonly int dataHeight;
      private readonly int left;
      private readonly int top;

      /// <summary>
      /// Initializes a new instance of the <see cref="PlanarYUVLuminanceSource"/> class.
      /// </summary>
      /// <param name="yuvData">The yuv data.</param>
      /// <param name="dataWidth">Width of the data.</param>
      /// <param name="dataHeight">Height of the data.</param>
      /// <param name="left">The left.</param>
      /// <param name="top">The top.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="reverseHoriz">if set to <c>true</c> [reverse horiz].</param>
      public PlanarYUVLuminanceSource(byte[] yuvData,
                                      int dataWidth,
                                      int dataHeight,
                                      int left,
                                      int top,
                                      int width,
                                      int height,
                                      bool reverseHoriz)
         : base(width, height)
      {
         if (left + width > dataWidth || top + height > dataHeight)
         {
            throw new ArgumentException("Crop rectangle does not fit within image data.");
         }

         this.yuvData = yuvData;
         this.dataWidth = dataWidth;
         this.dataHeight = dataHeight;
         this.left = left;
         this.top = top;
         if (reverseHoriz)
         {
            reverseHorizontal(width, height);
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="PlanarYUVLuminanceSource"/> class.
      /// </summary>
      /// <param name="luminances">The luminances.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      private PlanarYUVLuminanceSource(byte[] luminances, int width, int height)
         : base(width, height)
      {
         yuvData = luminances;
         this.luminances = luminances;
         dataWidth = width;
         dataHeight = height;
         left = 0;
         top = 0;
      }

      /// <summary>
      /// Fetches one row of luminance data from the underlying platform's bitmap. Values range from
      /// 0 (black) to 255 (white). Because Java does not have an unsigned byte type, callers will have
      /// to bitwise and with 0xff for each value. It is preferable for implementations of this method
      /// to only fetch this row rather than the whole image, since no 2D Readers may be installed and
      /// getMatrix() may never be called.
      /// </summary>
      /// <param name="y">The row to fetch, 0 &lt;= y &lt; Height.</param>
      /// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
      /// Always use the returned object, and ignore the .length of the array.</param>
      /// <returns>
      /// An array containing the luminance data.
      /// </returns>
      override public byte[] getRow(int y, byte[] row)
      {
         if (y < 0 || y >= Height)
         {
            throw new ArgumentException("Requested row is outside the image: " + y);
         }
         int width = Width;
         if (row == null || row.Length < width)
         {
            row = new byte[width];
         }
         int offset = (y + top) * dataWidth + left;
         Array.Copy(yuvData, offset, row, 0, width);
         return row;
      }

      /// <summary>
      /// 
      /// </summary>
      override public byte[] Matrix
      {
         get
         {
            int width = Width;
            int height = Height;

            // If the caller asks for the entire underlying image, save the copy and give them the
            // original data. The docs specifically warn that result.length must be ignored.
            if (width == dataWidth && height == dataHeight)
            {
               return yuvData;
            }

            int area = width * height;
            byte[] matrix = new byte[area];
            int inputOffset = top * dataWidth + left;

            // If the width matches the full width of the underlying data, perform a single copy.
            if (width == dataWidth)
            {
               Array.Copy(yuvData, inputOffset, matrix, 0, area);
               return matrix;
            }

            // Otherwise copy one cropped row at a time.
            byte[] yuv = yuvData;
            for (int y = 0; y < height; y++)
            {
               int outputOffset = y * width;
               Array.Copy(yuvData, inputOffset, matrix, outputOffset, width);
               inputOffset += dataWidth;
            }
            return matrix;
         }
      }

      /// <summary>
      /// </summary>
      /// <returns> Whether this subclass supports cropping.</returns>
      override public bool CropSupported
      {
         get { return true; }
      }

      /// <summary>
      /// Returns a new object with cropped image data. Implementations may keep a reference to the
      /// original data rather than a copy. Only callable if CropSupported is true.
      /// </summary>
      /// <param name="left">The left coordinate, 0 &lt;= left &lt; Width.</param>
      /// <param name="top">The top coordinate, 0 &lt;= top &lt;= Height.</param>
      /// <param name="width">The width of the rectangle to crop.</param>
      /// <param name="height">The height of the rectangle to crop.</param>
      /// <returns>
      /// A cropped version of this object.
      /// </returns>
      override public LuminanceSource crop(int left, int top, int width, int height)
      {
         return new PlanarYUVLuminanceSource(yuvData,
                                             dataWidth,
                                             dataHeight,
                                             this.left + left,
                                             this.top + top,
                                             width,
                                             height,
                                             false);
      }

      /// <summary>
      /// Renders the cropped greyscale bitmap.
      /// </summary>
      /// <returns></returns>
      public int[] renderThumbnail()
      {
         int width = Width / THUMBNAIL_SCALE_FACTOR;
         int height = Height / THUMBNAIL_SCALE_FACTOR;
         int[] pixels = new int[width * height];
         byte[] yuv = yuvData;
         int inputOffset = top * dataWidth + left;

         for (int y = 0; y < height; y++)
         {
            int outputOffset = y * width;
            for (int x = 0; x < width; x++)
            {
               int grey = yuv[inputOffset + x * THUMBNAIL_SCALE_FACTOR] & 0xff;
               pixels[outputOffset + x] = ((0x00FF0000 << 8) | (grey * 0x00010101));
            }
            inputOffset += dataWidth * THUMBNAIL_SCALE_FACTOR;
         }
         return pixels;
      }

      /// <summary>
      /// width of image from {@link #renderThumbnail()}
      /// </summary>
      public int ThumbnailWidth
      {
         get
         {
            return Width / THUMBNAIL_SCALE_FACTOR;
         }
      }

      /// <summary>
      /// height of image from {@link #renderThumbnail()}
      /// </summary>
      public int ThumbnailHeight
      {
         get
         {
            return Height / THUMBNAIL_SCALE_FACTOR;
         }
      }

      private void reverseHorizontal(int width, int height)
      {
         byte[] yuvData = this.yuvData;
         for (int y = 0, rowStart = top * dataWidth + left; y < height; y++, rowStart += dataWidth)
         {
            int middle = rowStart + width / 2;
            for (int x1 = rowStart, x2 = rowStart + width - 1; x1 < middle; x1++, x2--)
            {
               byte temp = yuvData[x1];
               yuvData[x1] = yuvData[x2];
               yuvData[x2] = temp;
            }
         }
      }

      /// <summary>
      /// creates a new instance
      /// </summary>
      /// <param name="newLuminances"></param>
      /// <param name="width"></param>
      /// <param name="height"></param>
      /// <returns></returns>
      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new PlanarYUVLuminanceSource(newLuminances, width, height);
      }
   }
}