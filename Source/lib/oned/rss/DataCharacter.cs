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
   }
}
