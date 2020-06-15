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

using ZXing.Common;

namespace ZXing.Aztec.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// represents an empty token
        /// </summary>
        public static Token EMPTY = new SimpleToken(null, 0, 0);

        private readonly Token previous;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="previous"></param>
        protected Token(Token previous)
        {
            this.previous = previous;
        }

        /// <summary>
        /// previous token
        /// </summary>
        public Token Previous
        {
            get { return previous; }
        }
        /// <summary>
        /// adds a new simple token
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        public Token add(int value, int bitCount)
        {
            return new SimpleToken(this, value, bitCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public Token addBinaryShift(int start, int byteCount)
        {
            int bitCount = (byteCount * 8) + (byteCount <= 31 ? 10 : byteCount <= 62 ? 20 : 21);
            return new BinaryShiftToken(this, start, byteCount);
        }
        /// <summary>
        /// append to bitarray
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="text"></param>
        public abstract void appendTo(BitArray bitArray, byte[] text);
    }
}
