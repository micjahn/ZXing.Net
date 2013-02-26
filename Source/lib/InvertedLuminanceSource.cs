/*
 * Copyright 2013 ZXing authors
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

namespace ZXing
{
   /// <summary>
   /// A wrapper implementation of {@link LuminanceSource} which inverts the luminances it returns -- black becomes
   /// white and vice versa, and each value becomes (255-value).
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class InvertedLuminanceSource : LuminanceSource
   {
      private readonly LuminanceSource @delegate;
      private byte[] invertedMatrix;

      /// <summary>
      /// Initializes a new instance of the <see cref="InvertedLuminanceSource"/> class.
      /// </summary>
      /// <param name="delegate">The @delegate.</param>
      public InvertedLuminanceSource(LuminanceSource @delegate)
         : base(@delegate.Width, @delegate.Height)
      {
         this.@delegate = @delegate;
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
         row = @delegate.getRow(y, row);
         int width = Width;
         for (int i = 0; i < width; i++)
         {
            row[i] = (byte)(255 - (row[i] & 0xFF));
         }
         return row;
      }

      /// <summary>
      /// Fetches luminance data for the underlying bitmap. Values should be fetched using:
      /// int luminance = array[y * width + x] &amp; 0xff;
      /// </summary>
      /// <returns> A row-major 2D array of luminance values. Do not use result.length as it may be
      /// larger than width * height bytes on some platforms. Do not modify the contents
      /// of the result.
      ///   </returns>
      override public byte[] Matrix
      {
         get
         {
            if (invertedMatrix == null)
            {
               byte[] matrix = @delegate.Matrix;
               int length = Width*Height;
               invertedMatrix = new byte[length];
               for (int i = 0; i < length; i++)
               {
                  invertedMatrix[i] = (byte) (255 - (matrix[i] & 0xFF));
               }
            }
            return invertedMatrix;
         }
      }

      /// <summary>
      /// </summary>
      /// <returns> Whether this subclass supports cropping.</returns>
      override public bool CropSupported
      {
         get { return @delegate.CropSupported; }
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
         return new InvertedLuminanceSource(@delegate.crop(left, top, width, height));
      }

      /// <summary>
      /// </summary>
      /// <returns> Whether this subclass supports counter-clockwise rotation.</returns>
      override public bool RotateSupported
      {
         get { return @delegate.RotateSupported; }
      }

      /// <summary>
      /// Inverts this instance.
      /// </summary>
      /// <returns>original delegate {@link LuminanceSource} since invert undoes itself</returns>
      override public LuminanceSource invert()
      {
         return @delegate;
      }

      /// <summary>
      /// Returns a new object with rotated image data by 90 degrees counterclockwise.
      /// Only callable if {@link #isRotateSupported()} is true.
      /// </summary>
      /// <returns>
      /// A rotated version of this object.
      /// </returns>
      override public LuminanceSource rotateCounterClockwise()
      {
         return new InvertedLuminanceSource(@delegate.rotateCounterClockwise());
      }

      /// <summary>
      /// Returns a new object with rotated image data by 45 degrees counterclockwise.
      /// Only callable if {@link #isRotateSupported()} is true.
      /// </summary>
      /// <returns>
      /// A rotated version of this object.
      /// </returns>
      override public LuminanceSource rotateCounterClockwise45()
      {
         return new InvertedLuminanceSource(@delegate.rotateCounterClockwise45());
      }
   }
}