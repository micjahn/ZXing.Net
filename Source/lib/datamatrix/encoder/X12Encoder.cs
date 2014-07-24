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

using System.Text;

namespace ZXing.Datamatrix.Encoder
{
   internal sealed class X12Encoder : C40Encoder
   {
      override public int EncodingMode
      {
         get { return Encodation.X12; }
      }

      override public void encode(EncoderContext context)
      {
         //step C
         var buffer = new StringBuilder();
         int currentMode = EncodingMode;
         while (context.HasMoreCharacters)
         {
            char c = context.CurrentChar;
            context.Pos++;

            encodeChar(c, buffer);

            int count = buffer.Length;
            if ((count % 3) == 0)
            {
               writeNextTriplet(context, buffer);

               int newMode = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, currentMode);
               if (newMode != currentMode)
               {
                  handleEOD(context, buffer);
                  context.signalEncoderChange(newMode);
                  return;
               }
            }
         }
         handleEOD(context, buffer);
      }

      override protected int encodeChar(char c, StringBuilder sb)
      {
         if (c == '\r')
         {
            sb.Append('\u0000');
         }
         else if (c == '*')
         {
            sb.Append('\u0001');
         }
         else if (c == '>')
         {
            sb.Append('\u0002');
         }
         else if (c == ' ')
         {
            sb.Append('\u0003');
         }
         else if (c >= '0' && c <= '9')
         {
            sb.Append((char)(c - 48 + 4));
         }
         else if (c >= 'A' && c <= 'Z')
         {
            sb.Append((char)(c - 65 + 14));
         }
         else
         {
            HighLevelEncoder.illegalCharacter(c);
         }
         return 1;
      }

      override protected void handleEOD(EncoderContext context, StringBuilder buffer)
      {
         context.updateSymbolInfo();
         int available = context.SymbolInfo.dataCapacity - context.CodewordCount;
         int count = buffer.Length;
         if (count == 2)
         {
            context.writeCodeword(HighLevelEncoder.X12_UNLATCH);
            context.Pos -= 2;
            context.signalEncoderChange(Encodation.ASCII);
         }
         else if (count == 1)
         {
            context.Pos--;
            if (context.RemainingCharacters > 1)
            {
               context.writeCodeword(HighLevelEncoder.X12_UNLATCH);
               if (available < 1)
               {
                  context.updateSymbolInfo();
               }
            }
            context.signalEncoderChange(Encodation.ASCII);
         }
         else if (count == 0)
         {
            if (context.RemainingCharacters > 1)
            {
               context.writeCodeword(HighLevelEncoder.X12_UNLATCH);
               if (available < 1)
               {
                  context.updateSymbolInfo();
               }
            }
         }
      }
   }
}