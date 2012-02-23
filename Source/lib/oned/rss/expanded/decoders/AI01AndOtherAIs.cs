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
   internal sealed class AI01AndOtherAIs : AI01decoder
   {
      private static int HEADER_SIZE = 1 + 1 + 2; //first bit encodes the linkage flag,
      //the second one is the encodation method, and the other two are for the variable length

      internal AI01AndOtherAIs(BitArray information)
         : base(information)
      {
      }

      override public String parseInformation()
      {
         StringBuilder buff = new StringBuilder();

         buff.Append("(01)");
         int initialGtinPosition = buff.Length;
         int firstGtinDigit = this.getGeneralDecoder().extractNumericValueFromBitArray(HEADER_SIZE, 4);
         buff.Append(firstGtinDigit);

         this.encodeCompressedGtinWithoutAI(buff, HEADER_SIZE + 4, initialGtinPosition);

         return this.getGeneralDecoder().decodeAllCodes(buff, HEADER_SIZE + 44);
      }
   }
}
