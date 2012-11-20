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
   public class DataCharacter
   {
      /// <summary>
      /// Gets the value.
      /// </summary>
      public int Value { get; private set; }
      /// <summary>
      /// Gets the checksum portion.
      /// </summary>
      public int ChecksumPortion { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="DataCharacter"/> class.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="checksumPortion">The checksum portion.</param>
      public DataCharacter(int value, int checksumPortion)
      {
         Value = value;
         ChecksumPortion = checksumPortion;
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      override public String ToString()
      {
         return Value + "(" + ChecksumPortion + ')';
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
         if (!(o is DataCharacter))
         {
            return false;
         }
         DataCharacter that = (DataCharacter)o;
         return Value == that.Value && ChecksumPortion == that.ChecksumPortion;
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
      /// </returns>
      override public int GetHashCode()
      {
         return Value ^ ChecksumPortion;
      }
   }
}
