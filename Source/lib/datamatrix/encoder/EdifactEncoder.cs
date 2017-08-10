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
   internal sealed class EdifactEncoder : Encoder
   {
      public int EncodingMode
      {
         get { return Encodation.EDIFACT; }
      }

      public void encode(EncoderContext context)
      {
         //step F
         var buffer = new StringBuilder();
         while (context.HasMoreCharacters)
         {
            char c = context.CurrentChar;
            encodeChar(c, buffer);
            context.Pos++;

            int count = buffer.Length;
            if (count >= 4)
            {
               context.writeCodewords(encodeToCodewords(buffer, 0));
               buffer.Remove(0, 4);

               int newMode = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, EncodingMode);
               if (newMode != EncodingMode)
               {
                  // Return to ASCII encodation, which will actually handle latch to new mode
                  context.signalEncoderChange(Encodation.ASCII);
                  break;
               }
            }
         }
         buffer.Append((char)31); //Unlatch
         handleEOD(context, buffer);
      }

      /// <summary>
      /// Handle "end of data" situations
      /// </summary>
      /// <param name="context">the encoder context</param>
      /// <param name="buffer">the buffer with the remaining encoded characters</param>
      private static void handleEOD(EncoderContext context, StringBuilder buffer)
      {
         try
         {
            int count = buffer.Length;
            if (count == 0)
            {
               return; //Already finished
            }
            if (count == 1)
            {
               //Only an unlatch at the end
               context.updateSymbolInfo();
               int available = context.SymbolInfo.dataCapacity - context.CodewordCount;
               int remaining = context.RemainingCharacters;
               if (remaining == 0 && available <= 2)
               {
                  return; //No unlatch
               }
            }

            if (count > 4)
            {
               throw new InvalidOperationException("Count must not exceed 4");
            }
            int restChars = count - 1;
            String encoded = encodeToCodewords(buffer, 0);
            bool endOfSymbolReached = !context.HasMoreCharacters;
            bool restInAscii = endOfSymbolReached && restChars <= 2;

            if (restChars <= 2)
            {
               context.updateSymbolInfo(context.CodewordCount + restChars);
               int available = context.SymbolInfo.dataCapacity - context.CodewordCount;
               if (available >= 3)
               {
                  restInAscii = false;
                  context.updateSymbolInfo(context.CodewordCount + encoded.Length);
                  //available = context.symbolInfo.dataCapacity - context.getCodewordCount();
               }
            }

            if (restInAscii)
            {
               context.resetSymbolInfo();
               context.Pos -= restChars;
            }
            else
            {
               context.writeCodewords(encoded);
            }
         }
         finally
         {
            context.signalEncoderChange(Encodation.ASCII);
         }
      }

      private static void encodeChar(char c, StringBuilder sb)
      {
         if (c >= ' ' && c <= '?')
         {
            sb.Append(c);
         }
         else if (c >= '@' && c <= '^')
         {
            sb.Append((char)(c - 64));
         }
         else
         {
            HighLevelEncoder.illegalCharacter(c);
         }
      }

      private static String encodeToCodewords(StringBuilder sb, int startPos)
      {
         int len = sb.Length - startPos;
         if (len == 0)
         {
            throw new InvalidOperationException("StringBuilder must not be empty");
         }
         char c1 = sb[startPos];
         char c2 = len >= 2 ? sb[startPos + 1] : (char)0;
         char c3 = len >= 3 ? sb[startPos + 2] : (char)0;
         char c4 = len >= 4 ? sb[startPos + 3] : (char)0;

         int v = (c1 << 18) + (c2 << 12) + (c3 << 6) + c4;
         char cw1 = (char)((v >> 16) & 255);
         char cw2 = (char)((v >> 8) & 255);
         char cw3 = (char)(v & 255);
         var res = new StringBuilder(3);
         res.Append(cw1);
         if (len >= 2)
         {
            res.Append(cw2);
         }
         if (len >= 3)
         {
            res.Append(cw3);
         }
         return res.ToString();
      }
   }
}