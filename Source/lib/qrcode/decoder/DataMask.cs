/*
* Copyright 2007 ZXing authors
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

namespace ZXing.QrCode.Internal
{
   /// <summary> <p>Encapsulates data masks for the data bits in a QR code, per ISO 18004:2006 6.8. Implementations
   /// of this class can un-mask a raw BitMatrix. For simplicity, they will unmask the entire BitMatrix,
   /// including areas used for finder patterns, timing patterns, etc. These areas should be unused
   /// after the point they are unmasked anyway.</p>
   /// 
   /// <p>Note that the diagram in section 6.8.1 is misleading since it indicates that i is column position
   /// and j is row position. In fact, as the text says, i is row position and j is column position.</p>
   /// 
   /// </summary>
   /// <author>Sean Owen</author>
   internal static class DataMask
   {
      /// <summary> See ISO 18004:2006 6.8.1</summary>
      private static readonly Func<int, int, bool>[] DATA_MASKS = new Func<int, int, bool>[]
                                                         {
                                                            /// <summary> 000: mask bits for which (x + y) mod 2 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => ((i + j) & 0x01) == 0),
                                                            /// <summary> 001: mask bits for which x mod 2 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => (i & 0x01) == 0),
                                                            /// <summary> 010: mask bits for which y mod 3 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => j % 3 == 0),
                                                            /// <summary> 011: mask bits for which (x + y) mod 3 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => (i + j) % 3 == 0),
                                                            /// <summary> 100: mask bits for which (x/2 + y/3) mod 2 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => ((((int)((uint)i >> 1)) + (j / 3)) & 0x01) == 0),
                                                            /// <summary> 101: mask bits for which xy mod 2 + xy mod 3 == 0, equivalently, such that xy mod 6 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => (i * j) % 6 == 0),
                                                            /// <summary> 110: mask bits for which (xy mod 2 + xy mod 3) mod 2 == 0, equivalently, such that xy mod 6 < 3</summary>
                                                            new Func<int, int, bool>((i, j) => ((i * j) % 6) < 3),
                                                            /// <summary> 111: mask bits for which ((x+y)mod 2 + xy mod 3) mod 2 == 0, equivalently, such that (x + y + xy mod 3) mod 2 == 0</summary>
                                                            new Func<int, int, bool>((i, j) => ((i + j + ((i * j) % 3)) & 0x01) == 0),
                                                         };

      /// <summary> <p>Implementations of this method reverse the data masking process applied to a QR Code and
      /// make its bits ready to read.</p>
      /// </summary>
      /// <param name="reference"></param>
      /// <param name="bits">representation of QR Code bits</param>
      /// <param name="dimension">dimension of QR Code, represented by bits, being unmasked</param>
      internal static void unmaskBitMatrix(int reference, BitMatrix bits, int dimension)
      {
         if (reference < 0 || reference > 7)
         {
            throw new System.ArgumentException();
         }

         var isMasked = DATA_MASKS[reference];

         bits.flipWhen(isMasked);
      }
   }
}