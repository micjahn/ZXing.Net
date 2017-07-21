/*
 * Copyright 2006-2007 Jeremias Maerki.
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

namespace ZXing.Datamatrix.Encoder
{
   internal sealed class Base256Encoder : Encoder
   {
      public int EncodingMode
      {
         get { return Encodation.BASE256; }
      }

      public void encode(EncoderContext context)
      {
         var buffer = new StringBuilder();
         buffer.Append('\u0000'); //Initialize length field
         while (context.HasMoreCharacters)
         {
            char c = context.CurrentChar;
            buffer.Append(c);

            context.Pos++;

            int newMode = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, EncodingMode);
            if (newMode != EncodingMode)
            {
               // Return to ASCII encodation, which will actually handle latch to new mode
               context.signalEncoderChange(Encodation.ASCII);
               break;
            }
         }
         int dataCount = buffer.Length - 1;
         const int lengthFieldSize = 1;
         int currentSize = context.CodewordCount + dataCount + lengthFieldSize;
         context.updateSymbolInfo(currentSize);
         bool mustPad = (context.SymbolInfo.dataCapacity - currentSize) > 0;
         if (context.HasMoreCharacters || mustPad)
         {
            if (dataCount <= 249)
            {
               buffer[0] = (char)dataCount;
            }
            else if (dataCount <= 1555)
            {
               buffer[0] = (char)((dataCount / 250) + 249);
               buffer.Insert(1, new [] { (char)(dataCount % 250) });
            }
            else
            {
               throw new InvalidOperationException(
                   "Message length not in valid ranges: " + dataCount);
            }
         }
         {
            var c = buffer.Length;
            for (int i = 0; i < c; i++)
            {
               context.writeCodeword(randomize255State(
                  buffer[i], context.CodewordCount + 1));
            }
         }
      }

      private static char randomize255State(char ch, int codewordPosition)
      {
         int pseudoRandom = ((149 * codewordPosition) % 255) + 1;
         int tempVariable = ch + pseudoRandom;
         if (tempVariable <= 255)
         {
            return (char)tempVariable;
         }
         return (char)(tempVariable - 256);
      }
   }
}