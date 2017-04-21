/*
 * Copyright 2012 ZXing authors
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
   /// Simply encapsulates a width and height.
   /// </summary>
   public sealed class Dimension
   {
      private readonly int width;
      private readonly int height;

      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="width"></param>
      /// <param name="height"></param>
      public Dimension(int width, int height)
      {
         if (width < 0 || height < 0)
         {
            throw new ArgumentException();
         }
         this.width = width;
         this.height = height;
      }

      /// <summary>
      /// the width
      /// </summary>
      public int Width
      {
         get { return width; }
      }

      /// <summary>
      /// the height
      /// </summary>
      public int Height
      {
         get { return height; }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public override bool Equals(Object other)
      {
         if (other is Dimension)
         {
            var d = (Dimension)other;
            return width == d.width && height == d.height;
         }
         return false;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode()
      {
         return width * 32713 + height;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public override String ToString()
      {
         return width + "x" + height;
      }
   }
}