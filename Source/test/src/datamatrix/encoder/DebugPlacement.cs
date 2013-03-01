/*
 * Copyright 2006 Jeremias Maerki
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

using ZXing.Datamatrix.Encoder;

namespace ZXing.Datamatrix.Test
{
   public sealed class DebugPlacement : DefaultPlacement
   {
      internal DebugPlacement(String codewords, int numcols, int numrows)
         : base(codewords, numcols, numrows)
      {
      }

      public String toBitFieldString()
      {
         var bits = Bits;
         var sb = new StringBuilder(bits.Length);
         foreach (byte bit in bits)
         {
            sb.Append(bit == 1 ? '1' : '0');
         }
         return sb.ToString();
      }

      internal String[] toBitFieldStringArray()
      {
         var bits = Bits;
         var numrows = Numrows;
         var numcols = Numcols;
         var array = new String[numrows];
         var startpos = 0;
         for (var row = 0; row < numrows; row++)
         {
            var sb = new StringBuilder(bits.Length);
            for (var i = 0; i < numcols; i++)
            {
               sb.Append(bits[startpos + i] == 1 ? '1' : '0');
            }
            array[row] = sb.ToString();
            startpos += numcols;
         }
         return array;
      }
   }
}