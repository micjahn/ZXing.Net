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

namespace ZXing.OneD.RSS.Expanded.Decoders
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   internal sealed class DecodedNumeric : DecodedObject
   {
      private readonly int firstDigit;
      private readonly int secondDigit;

      internal static int FNC1 = 10;

      internal DecodedNumeric(int newPosition, int firstDigit, int secondDigit)
         : base(newPosition)
      {
         if (firstDigit < 0 || firstDigit > 10 || secondDigit < 0 || secondDigit > 10)
         {
            throw new FormatException("digits has to be between 0 and 10");
         }
         
         this.firstDigit = firstDigit;
         this.secondDigit = secondDigit;
      }

      internal int getFirstDigit()
      {
         return this.firstDigit;
      }

      internal int getSecondDigit()
      {
         return this.secondDigit;
      }

      internal int getValue()
      {
         return this.firstDigit * 10 + this.secondDigit;
      }

      internal bool isFirstDigitFNC1()
      {
         return this.firstDigit == FNC1;
      }

      internal bool isSecondDigitFNC1()
      {
         return this.secondDigit == FNC1;
      }

      internal bool isAnyFNC1()
      {
         return this.firstDigit == FNC1 || this.secondDigit == FNC1;
      }
   }
}
