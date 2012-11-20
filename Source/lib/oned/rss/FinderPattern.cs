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

namespace ZXing.OneD.RSS
{
   /// <summary>
   /// 
   /// </summary>
   public sealed class FinderPattern
   {
      /// <summary>
      /// Gets the value.
      /// </summary>
      public int Value { get; private set; }
      /// <summary>
      /// Gets the start end.
      /// </summary>
      public int[] StartEnd { get; private set; }
      /// <summary>
      /// Gets the result points.
      /// </summary>
      public ResultPoint[] ResultPoints { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="FinderPattern"/> class.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="startEnd">The start end.</param>
      /// <param name="start">The start.</param>
      /// <param name="end">The end.</param>
      /// <param name="rowNumber">The row number.</param>
      public FinderPattern(int value, int[] startEnd, int start, int end, int rowNumber)
      {
         Value = value;
         StartEnd = startEnd;
         ResultPoints = new ResultPoint[]
                                {
                                   new ResultPoint(start, rowNumber),
                                   new ResultPoint(end, rowNumber),
                                };
      }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
      /// </summary>
      /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
      /// <returns>
      ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      override public bool Equals(Object o)
      {
         if (!(o is FinderPattern))
         {
            return false;
         }
         FinderPattern that = (FinderPattern)o;
         return Value == that.Value;
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
      /// </returns>
      override public int GetHashCode()
      {
         return Value;
      }
   }
}