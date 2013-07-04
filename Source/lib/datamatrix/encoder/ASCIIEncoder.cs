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

namespace ZXing.Datamatrix.Encoder
{
   internal sealed class ASCIIEncoder : Encoder
   {
      public int EncodingMode
      {
         get { return Encodation.ASCII; }
      }

      public void encode(EncoderContext context)
      {
         //step B
         int n = HighLevelEncoder.determineConsecutiveDigitCount(context.Message, context.Pos);
         if (n >= 2)
         {
            context.writeCodeword(encodeASCIIDigits(context.Message[context.Pos],
                                                    context.Message[context.Pos + 1]));
            context.Pos += 2;
         }
         else
         {
            char c = context.CurrentChar;
            int newMode = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, EncodingMode);
            if (newMode != EncodingMode)
            {
               switch (newMode)
               {
                  case Encodation.BASE256:
                     context.writeCodeword(HighLevelEncoder.LATCH_TO_BASE256);
                     context.signalEncoderChange(Encodation.BASE256);
                     return;
                  case Encodation.C40:
                     context.writeCodeword(HighLevelEncoder.LATCH_TO_C40);
                     context.signalEncoderChange(Encodation.C40);
                     return;
                  case Encodation.X12:
                     context.writeCodeword(HighLevelEncoder.LATCH_TO_ANSIX12);
                     context.signalEncoderChange(Encodation.X12);
                     break;
                  case Encodation.TEXT:
                     context.writeCodeword(HighLevelEncoder.LATCH_TO_TEXT);
                     context.signalEncoderChange(Encodation.TEXT);
                     break;
                  case Encodation.EDIFACT:
                     context.writeCodeword(HighLevelEncoder.LATCH_TO_EDIFACT);
                     context.signalEncoderChange(Encodation.EDIFACT);
                     break;
                  default:
                     throw new InvalidOperationException("Illegal mode: " + newMode);
               }
            }
            else if (HighLevelEncoder.isExtendedASCII(c))
            {
               context.writeCodeword(HighLevelEncoder.UPPER_SHIFT);
               context.writeCodeword((char)(c - 128 + 1));
               context.Pos++;
            }
            else
            {
               context.writeCodeword((char)(c + 1));
               context.Pos++;
            }

         }
      }

      private static char encodeASCIIDigits(char digit1, char digit2)
      {
         if (HighLevelEncoder.isDigit(digit1) && HighLevelEncoder.isDigit(digit2))
         {
            int num = (digit1 - 48) * 10 + (digit2 - 48);
            return (char)(num + 130);
         }
         throw new ArgumentException("not digits: " + digit1 + digit2);
      }
   }
}