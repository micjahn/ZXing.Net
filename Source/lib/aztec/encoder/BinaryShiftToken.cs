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

using System;

using ZXing.Common;

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// represents a token for a binary shift
   /// </summary>
   public sealed class BinaryShiftToken : Token
   {
      private readonly short binaryShiftStart;
      private readonly short binaryShiftByteCount;

      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="previous"></param>
      /// <param name="binaryShiftStart"></param>
      /// <param name="binaryShiftByteCount"></param>
      public BinaryShiftToken(Token previous,
                              int binaryShiftStart,
                              int binaryShiftByteCount)
         : base(previous)
      {
         this.binaryShiftStart = (short) binaryShiftStart;
         this.binaryShiftByteCount = (short) binaryShiftByteCount;
      }

      /// <summary>
      /// appends the byte array to the BitArray
      /// </summary>
      /// <param name="bitArray"></param>
      /// <param name="text"></param>
      public override void appendTo(BitArray bitArray, byte[] text)
      {
         for (int i = 0; i < binaryShiftByteCount; i++)
         {
            if (i == 0 || (i == 31 && binaryShiftByteCount <= 62))
            {
               // We need a header before the first character, and before
               // character 31 when the total byte code is <= 62
               bitArray.appendBits(31, 5);  // BINARY_SHIFT
               if (binaryShiftByteCount > 62)
               {
                  bitArray.appendBits(binaryShiftByteCount - 31, 16);
               }
               else if (i == 0)
               {
                  // 1 <= binaryShiftByteCode <= 62
                  bitArray.appendBits(Math.Min(binaryShiftByteCount, (short) 31), 5);
               }
               else
               {
                  // 32 <= binaryShiftCount <= 62 and i == 31
                  bitArray.appendBits(binaryShiftByteCount - 31, 5);
               }
            }
            bitArray.appendBits(text[binaryShiftStart + i], 8);
         }
      }

      /// <summary>
      /// string representation
      /// </summary>
      /// <returns></returns>
      public override String ToString()
      {
         return "<" + binaryShiftStart + "::" + (binaryShiftStart + binaryShiftByteCount - 1) + '>';
      }
   }
}