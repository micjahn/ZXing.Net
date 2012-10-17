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
using System.Text;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Decoders
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   sealed class AI013x0x1xDecoder : AI01weightDecoder
   {
      private static int HEADER_SIZE = 7 + 1;
      private static int WEIGHT_SIZE = 20;
      private static int DATE_SIZE = 16;

      private String dateCode;
      private String firstAIdigits;

      internal AI013x0x1xDecoder(BitArray information, String firstAIdigits, String dateCode)
         : base(information)
      {
         this.dateCode = dateCode;
         this.firstAIdigits = firstAIdigits;
      }

      override public String parseInformation()
      {
         if (this.getInformation().Size != HEADER_SIZE + GTIN_SIZE + WEIGHT_SIZE + DATE_SIZE)
         {
            return null;
         }

         StringBuilder buf = new StringBuilder();

         encodeCompressedGtin(buf, HEADER_SIZE);
         encodeCompressedWeight(buf, HEADER_SIZE + GTIN_SIZE, WEIGHT_SIZE);
         encodeCompressedDate(buf, HEADER_SIZE + GTIN_SIZE + WEIGHT_SIZE);

         return buf.ToString();
      }

      private void encodeCompressedDate(StringBuilder buf, int currentPos)
      {
         int numericDate = this.getGeneralDecoder().extractNumericValueFromBitArray(currentPos, DATE_SIZE);
         if (numericDate == 38400)
         {
            return;
         }

         buf.Append('(');
         buf.Append(this.dateCode);
         buf.Append(')');

         int day = numericDate % 32;
         numericDate /= 32;
         int month = numericDate % 12 + 1;
         numericDate /= 12;
         int year = numericDate;

         if (year / 10 == 0)
         {
            buf.Append('0');
         }
         buf.Append(year);
         if (month / 10 == 0)
         {
            buf.Append('0');
         }
         buf.Append(month);
         if (day / 10 == 0)
         {
            buf.Append('0');
         }
         buf.Append(day);
      }

      override protected void addWeightCode(StringBuilder buf, int weight)
      {
         int lastAI = weight / 100000;
         buf.Append('(');
         buf.Append(this.firstAIdigits);
         buf.Append(lastAI);
         buf.Append(')');
      }

      override protected int checkWeight(int weight)
      {
         return weight % 100000;
      }
   }
}
