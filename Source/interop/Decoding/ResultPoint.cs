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

using System;
using System.Runtime.InteropServices;

namespace ZXing.Interop.Decoding
{
   [ComVisible(true)]
   [Guid("16ADD35B-1F8B-41C3-BFE0-DA4D7E60B3FD")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class ResultPoint
   {
      private readonly int hashCode;
      private readonly String toString;

      /// <summary>
      /// Initializes a new instance of the <see cref="ResultPoint"/> class.
      /// </summary>
      public ResultPoint(ZXing.ResultPoint other)
      {
         X = other.X;
         Y = other.Y;
         hashCode = other.GetHashCode();
         toString = other.ToString();
      }

      /// <summary>
      /// Gets the X.
      /// </summary>
      public float X { get; private set; }

      /// <summary>
      /// Gets the Y.
      /// </summary>
      public float Y { get; private set; }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
      /// </summary>
      /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
      /// <returns>
      ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      public override bool Equals(Object other)
      {
         var otherPoint = other as ResultPoint;
         if (otherPoint == null)
            return false;
         return X == otherPoint.X && Y == otherPoint.Y;
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
      /// </returns>
      public override int GetHashCode()
      {
         return hashCode;
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override String ToString()
      {
         return toString;
      }
   }

   internal static class ResultPointExtensions
   {
      public static ResultPoint[] ToInteropResultPoints(this ZXing.ResultPoint[] resultPoints)
      {
         if (resultPoints == null)
            return null;

         var result = new ResultPoint[resultPoints.Length];
         for (var index = 0; index < resultPoints.Length; index++)
         {
            result[index] = new ResultPoint(resultPoints[index]);
         }

         return result;
      }
   }
}
