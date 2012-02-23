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
   abstract class AI01decoder : AbstractExpandedDecoder
   {
      protected static int GTIN_SIZE = 40;

      internal AI01decoder(BitArray information)
         : base(information)
      {
      }

      protected void encodeCompressedGtin(StringBuilder buf, int currentPos)
      {
         buf.Append("(01)");
         int initialPosition = buf.Length;
         buf.Append('9');

         encodeCompressedGtinWithoutAI(buf, currentPos, initialPosition);
      }

      protected void encodeCompressedGtinWithoutAI(StringBuilder buf, int currentPos, int initialBufferPosition)
      {
         for (int i = 0; i < 4; ++i)
         {
            int currentBlock = this.getGeneralDecoder().extractNumericValueFromBitArray(currentPos + 10 * i, 10);
            if (currentBlock / 100 == 0)
            {
               buf.Append('0');
            }
            if (currentBlock / 10 == 0)
            {
               buf.Append('0');
            }
            buf.Append(currentBlock);
         }

         appendCheckDigit(buf, initialBufferPosition);
      }

      private static void appendCheckDigit(StringBuilder buf, int currentPos)
      {
         int checkDigit = 0;
         for (int i = 0; i < 13; i++)
         {
            int digit = buf[i + currentPos] - '0';
            checkDigit += (i & 0x01) == 0 ? 3 * digit : digit;
         }

         checkDigit = 10 - (checkDigit % 10);
         if (checkDigit == 10)
         {
            checkDigit = 0;
         }

         buf.Append(checkDigit);
      }
   }
}
