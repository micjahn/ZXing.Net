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
    /// 
    /// </summary>
    public sealed class SimpleToken : Token
    {
        // For normal words, indicates value and bitCount
        private readonly short value;
        private readonly short bitCount;

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="bitCount"></param>
        public SimpleToken(Token previous, int value, int bitCount)
           : base(previous)
        {
            this.value = (short)value;
            this.bitCount = (short)bitCount;
        }
        /// <summary>
        /// append token to bitarray
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="text"></param>
        public override void appendTo(BitArray bitArray, byte[] text)
        {
            bitArray.appendBits(value, bitCount);
        }
        /// <summary>
        /// string representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            int value = this.value & ((1 << bitCount) - 1);
            value |= 1 << bitCount;
            return '<' + SupportClass.ToBinaryString(value | (1 << bitCount)).Substring(1) + '>';
        }
    }
}