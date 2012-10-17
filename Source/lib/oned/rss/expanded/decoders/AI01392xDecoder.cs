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
   /// </summary>
   internal sealed class AI01392xDecoder : AI01decoder
   {
      private const int HEADER_SIZE = 5 + 1 + 2;
      private const int LAST_DIGIT_SIZE = 2;

      internal AI01392xDecoder(BitArray information)
         : base(information)
      {
      }

      override public String parseInformation()
      {
         if (this.getInformation().Size < HEADER_SIZE + GTIN_SIZE)
         {
            return null;
         }

         StringBuilder buf = new StringBuilder();

         encodeCompressedGtin(buf, HEADER_SIZE);

         int lastAIdigit =
             this.getGeneralDecoder().extractNumericValueFromBitArray(HEADER_SIZE + GTIN_SIZE, LAST_DIGIT_SIZE);
         buf.Append("(392");
         buf.Append(lastAIdigit);
         buf.Append(')');

         DecodedInformation decodedInformation =
             this.getGeneralDecoder().decodeGeneralPurposeField(HEADER_SIZE + GTIN_SIZE + LAST_DIGIT_SIZE, null);
         buf.Append(decodedInformation.getNewString());

         return buf.ToString();
      }
   }
}
