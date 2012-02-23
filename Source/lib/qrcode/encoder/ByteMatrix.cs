/*
 * Copyright 2008 ZXing authors
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
using System.Text;

namespace ZXing.QrCode.Internal
{
   /**
    * A class which wraps a 2D array of bytes. The default usage is signed. If you want to use it as a
    * unsigned container, it's up to you to do byteValue & 0xff at each location.
    *
    * JAVAPORT: The original code was a 2D array of ints, but since it only ever gets assigned
    * -1, 0, and 1, I'm going to use less memory and go with bytes.
    *
    * @author dswitkin@google.com (Daniel Switkin)
    */
   public sealed class ByteMatrix
   {
      private sbyte[][] bytes;
      private int width;
      private int height;

      public ByteMatrix(int width, int height)
      {
         bytes = new sbyte[height][];
         for (var i = 0; i < height; i++)
            bytes[i] = new sbyte[width];
         this.width = width;
         this.height = height;
      }

      public int Height
      {
         get { return height; }
      }

      public int Width
      {
         get { return width; }
      }

      public int this[int x, int y]
      {
         get { return bytes[y][x]; }
         set { bytes[y][x] = (sbyte)value; }
      }

      public sbyte[][] Array
      {
         get { return bytes; }
      }

      public void set(int x, int y, sbyte value)
      {
         bytes[y][x] = value;
      }

      public void set(int x, int y, bool value)
      {
         bytes[y][x] = (sbyte)(value ? 1 : 0);
      }

      public void clear(sbyte value)
      {
         for (int y = 0; y < height; ++y)
         {
            for (int x = 0; x < width; ++x)
            {
               bytes[y][x] = value;
            }
         }
      }

      override public String ToString()
      {
         var result = new StringBuilder(2 * width * height + 2);
         for (int y = 0; y < height; ++y)
         {
            for (int x = 0; x < width; ++x)
            {
               switch (bytes[y][x])
               {
                  case 0:
                     result.Append(" 0");
                     break;
                  case 1:
                     result.Append(" 1");
                     break;
                  default:
                     result.Append("  ");
                     break;
               }
            }
            result.Append('\n');
         }
         return result.ToString();
      }
   }
}