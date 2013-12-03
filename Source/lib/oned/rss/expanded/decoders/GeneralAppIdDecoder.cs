/*
 * Copyright (C) 2010 ZXing authors
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

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Decoders
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   internal sealed class GeneralAppIdDecoder
   {
      private BitArray information;
      private CurrentParsingState current = new CurrentParsingState();
      private StringBuilder buffer = new StringBuilder();

      internal GeneralAppIdDecoder(BitArray information)
      {
         this.information = information;
      }

      internal String decodeAllCodes(StringBuilder buff, int initialPosition)
      {
         int currentPosition = initialPosition;
         String remaining = null;
         do
         {
            DecodedInformation info = this.decodeGeneralPurposeField(currentPosition, remaining);
            String parsedFields = FieldParser.parseFieldsInGeneralPurpose(info.getNewString());
            if (parsedFields != null)
            {
               buff.Append(parsedFields);
            }
            if (info.isRemaining())
            {
               remaining = info.getRemainingValue().ToString();
            }
            else
            {
               remaining = null;
            }

            if (currentPosition == info.NewPosition)
            {// No step forward!
               break;
            }
            currentPosition = info.NewPosition;
         } while (true);

         return buff.ToString();
      }

      private bool isStillNumeric(int pos)
      {
         // It's numeric if it still has 7 positions
         // and one of the first 4 bits is "1".
         if (pos + 7 > this.information.Size)
         {
            return pos + 4 <= this.information.Size;
         }

         for (int i = pos; i < pos + 3; ++i)
         {
            if (this.information[i])
            {
               return true;
            }
         }

         return this.information[pos + 3];
      }

      private DecodedNumeric decodeNumeric(int pos)
      {
         int numeric;
         if (pos + 7 > this.information.Size)
         {
            numeric = extractNumericValueFromBitArray(pos, 4);
            if (numeric == 0)
            {
               return new DecodedNumeric(this.information.Size, DecodedNumeric.FNC1, DecodedNumeric.FNC1);
            }
            return new DecodedNumeric(this.information.Size, numeric - 1, DecodedNumeric.FNC1);
         }
         numeric = extractNumericValueFromBitArray(pos, 7);

         int digit1 = (numeric - 8) / 11;
         int digit2 = (numeric - 8) % 11;

         return new DecodedNumeric(pos + 7, digit1, digit2);
      }

      internal int extractNumericValueFromBitArray(int pos, int bits)
      {
         return extractNumericValueFromBitArray(this.information, pos, bits);
      }

      internal static int extractNumericValueFromBitArray(BitArray information, int pos, int bits)
      {
         int value = 0;
         for (int i = 0; i < bits; ++i)
         {
            if (information[pos + i])
            {
               value |= 1 << (bits - i - 1);
            }
         }

         return value;
      }

      internal DecodedInformation decodeGeneralPurposeField(int pos, String remaining)
      {
         this.buffer.Length = 0;

         if (remaining != null)
         {
            this.buffer.Append(remaining);
         }

         this.current.setPosition(pos);

         DecodedInformation lastDecoded = parseBlocks();
         if (lastDecoded != null && lastDecoded.isRemaining())
         {
            return new DecodedInformation(this.current.getPosition(), this.buffer.ToString(), lastDecoded.getRemainingValue());
         }
         return new DecodedInformation(this.current.getPosition(), this.buffer.ToString());
      }

      private DecodedInformation parseBlocks()
      {
         bool isFinished;
         BlockParsedResult result;
         do
         {
            int initialPosition = current.getPosition();

            if (current.isAlpha())
            {
               result = parseAlphaBlock();
               isFinished = result.isFinished();
            }
            else if (current.isIsoIec646())
            {
               result = parseIsoIec646Block();
               isFinished = result.isFinished();
            }
            else
            { // it must be numeric
               result = parseNumericBlock();
               isFinished = result.isFinished();
            }

            bool positionChanged = initialPosition != current.getPosition();
            if (!positionChanged && !isFinished)
            {
               break;
            }
         } while (!isFinished);

         return result.getDecodedInformation();
      }

      private BlockParsedResult parseNumericBlock()
      {
         while (isStillNumeric(current.getPosition()))
         {
            DecodedNumeric numeric = decodeNumeric(current.getPosition());
            current.setPosition(numeric.NewPosition);

            if (numeric.isFirstDigitFNC1())
            {
               DecodedInformation information;
               if (numeric.isSecondDigitFNC1())
               {
                  information = new DecodedInformation(current.getPosition(), buffer.ToString());
               }
               else
               {
                  information = new DecodedInformation(current.getPosition(), buffer.ToString(), numeric.getSecondDigit());
               }
               return new BlockParsedResult(information, true);
            }
            buffer.Append(numeric.getFirstDigit());

            if (numeric.isSecondDigitFNC1())
            {
               DecodedInformation information = new DecodedInformation(current.getPosition(), buffer.ToString());
               return new BlockParsedResult(information, true);
            }
            buffer.Append(numeric.getSecondDigit());
         }

         if (isNumericToAlphaNumericLatch(current.getPosition()))
         {
            current.setAlpha();
            current.incrementPosition(4);
         }
         return new BlockParsedResult(false);
      }

      private BlockParsedResult parseIsoIec646Block()
      {
         while (isStillIsoIec646(current.getPosition()))
         {
            DecodedChar iso = decodeIsoIec646(current.getPosition());
            current.setPosition(iso.NewPosition);

            if (iso.isFNC1())
            {
               DecodedInformation information = new DecodedInformation(current.getPosition(), buffer.ToString());
               return new BlockParsedResult(information, true);
            }
            buffer.Append(iso.getValue());
         }

         if (isAlphaOr646ToNumericLatch(current.getPosition()))
         {
            current.incrementPosition(3);
            current.setNumeric();
         }
         else if (isAlphaTo646ToAlphaLatch(current.getPosition()))
         {
            if (current.getPosition() + 5 < this.information.Size)
            {
               current.incrementPosition(5);
            }
            else
            {
               current.setPosition(this.information.Size);
            }

            current.setAlpha();
         }
         return new BlockParsedResult(false);
      }

      private BlockParsedResult parseAlphaBlock()
      {
         while (isStillAlpha(current.getPosition()))
         {
            DecodedChar alpha = decodeAlphanumeric(current.getPosition());
            current.setPosition(alpha.NewPosition);

            if (alpha.isFNC1())
            {
               DecodedInformation information = new DecodedInformation(current.getPosition(), buffer.ToString());
               return new BlockParsedResult(information, true); //end of the char block
            }

            buffer.Append(alpha.getValue());
         }

         if (isAlphaOr646ToNumericLatch(current.getPosition()))
         {
            current.incrementPosition(3);
            current.setNumeric();
         }
         else if (isAlphaTo646ToAlphaLatch(current.getPosition()))
         {
            if (current.getPosition() + 5 < this.information.Size)
            {
               current.incrementPosition(5);
            }
            else
            {
               current.setPosition(this.information.Size);
            }

            current.setIsoIec646();
         }
         return new BlockParsedResult(false);
      }

      private bool isStillIsoIec646(int pos)
      {
         if (pos + 5 > this.information.Size)
         {
            return false;
         }

         int fiveBitValue = extractNumericValueFromBitArray(pos, 5);
         if (fiveBitValue >= 5 && fiveBitValue < 16)
         {
            return true;
         }

         if (pos + 7 > this.information.Size)
         {
            return false;
         }

         int sevenBitValue = extractNumericValueFromBitArray(pos, 7);
         if (sevenBitValue >= 64 && sevenBitValue < 116)
         {
            return true;
         }

         if (pos + 8 > this.information.Size)
         {
            return false;
         }

         int eightBitValue = extractNumericValueFromBitArray(pos, 8);
         return eightBitValue >= 232 && eightBitValue < 253;

      }

      private DecodedChar decodeIsoIec646(int pos)
      {
         int fiveBitValue = extractNumericValueFromBitArray(pos, 5);
         if (fiveBitValue == 15)
         {
            return new DecodedChar(pos + 5, DecodedChar.FNC1);
         }

         if (fiveBitValue >= 5 && fiveBitValue < 15)
         {
            return new DecodedChar(pos + 5, (char)('0' + fiveBitValue - 5));
         }

         int sevenBitValue = extractNumericValueFromBitArray(pos, 7);

         if (sevenBitValue >= 64 && sevenBitValue < 90)
         {
            return new DecodedChar(pos + 7, (char)(sevenBitValue + 1));
         }

         if (sevenBitValue >= 90 && sevenBitValue < 116)
         {
            return new DecodedChar(pos + 7, (char)(sevenBitValue + 7));
         }

         int eightBitValue = extractNumericValueFromBitArray(pos, 8);
         char c;
         switch (eightBitValue)
         {
            case 232:
               c = '!';
               break;
            case 233:
               c = '"';
               break;
            case 234:
               c = '%';
               break;
            case 235:
               c = '&';
               break;
            case 236:
               c = '\'';
               break;
            case 237:
               c = '(';
               break;
            case 238:
               c = ')';
               break;
            case 239:
               c = '*';
               break;
            case 240:
               c = '+';
               break;
            case 241:
               c = ',';
               break;
            case 242:
               c = '-';
               break;
            case 243:
               c = '.';
               break;
            case 244:
               c = '/';
               break;
            case 245:
               c = ':';
               break;
            case 246:
               c = ';';
               break;
            case 247:
               c = '<';
               break;
            case 248:
               c = '=';
               break;
            case 249:
               c = '>';
               break;
            case 250:
               c = '?';
               break;
            case 251:
               c = '_';
               break;
            case 252:
               c = ' ';
               break;
            default:
               throw new ArgumentException("Decoding invalid ISO/IEC 646 value: " + eightBitValue);
         }
         return new DecodedChar(pos + 8, c);
      }

      private bool isStillAlpha(int pos)
      {
         if (pos + 5 > this.information.Size)
         {
            return false;
         }

         // We now check if it's a valid 5-bit value (0..9 and FNC1)
         int fiveBitValue = extractNumericValueFromBitArray(pos, 5);
         if (fiveBitValue >= 5 && fiveBitValue < 16)
         {
            return true;
         }

         if (pos + 6 > this.information.Size)
         {
            return false;
         }

         int sixBitValue = extractNumericValueFromBitArray(pos, 6);
         return sixBitValue >= 16 && sixBitValue < 63; // 63 not included
      }

      private DecodedChar decodeAlphanumeric(int pos)
      {
         int fiveBitValue = extractNumericValueFromBitArray(pos, 5);
         if (fiveBitValue == 15)
         {
            return new DecodedChar(pos + 5, DecodedChar.FNC1);
         }

         if (fiveBitValue >= 5 && fiveBitValue < 15)
         {
            return new DecodedChar(pos + 5, (char)('0' + fiveBitValue - 5));
         }

         int sixBitValue = extractNumericValueFromBitArray(pos, 6);

         if (sixBitValue >= 32 && sixBitValue < 58)
         {
            return new DecodedChar(pos + 6, (char)(sixBitValue + 33));
         }

         char c;
         switch (sixBitValue)
         {
            case 58:
               c = '*';
               break;
            case 59:
               c = ',';
               break;
            case 60:
               c = '-';
               break;
            case 61:
               c = '.';
               break;
            case 62:
               c = '/';
               break;
            default:
               throw new InvalidOperationException("Decoding invalid alphanumeric value: " + sixBitValue);
         }
         return new DecodedChar(pos + 6, c);
      }

      private bool isAlphaTo646ToAlphaLatch(int pos)
      {
         if (pos + 1 > this.information.Size)
         {
            return false;
         }

         for (int i = 0; i < 5 && i + pos < this.information.Size; ++i)
         {
            if (i == 2)
            {
               if (!this.information[pos + 2])
               {
                  return false;
               }
            }
            else if (this.information[pos + i])
            {
               return false;
            }
         }

         return true;
      }

      private bool isAlphaOr646ToNumericLatch(int pos)
      {
         // Next is alphanumeric if there are 3 positions and they are all zeros
         if (pos + 3 > this.information.Size)
         {
            return false;
         }

         for (int i = pos; i < pos + 3; ++i)
         {
            if (this.information[i])
            {
               return false;
            }
         }
         return true;
      }

      private bool isNumericToAlphaNumericLatch(int pos)
      {
         // Next is alphanumeric if there are 4 positions and they are all zeros, or
         // if there is a subset of this just before the end of the symbol
         if (pos + 1 > this.information.Size)
         {
            return false;
         }

         for (int i = 0; i < 4 && i + pos < this.information.Size; ++i)
         {
            if (this.information[pos + i])
            {
               return false;
            }
         }
         return true;
      }
   }
}
