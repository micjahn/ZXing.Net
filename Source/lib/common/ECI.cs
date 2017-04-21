/*
* Copyright 2008 ZXing authors
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

namespace ZXing.Common
{

   /// <summary>
   /// Superclass of classes encapsulating types ECIs, according to "Extended Channel Interpretations"
   /// 5.3 of ISO 18004.
   /// </summary>
   /// <author>Sean Owen</author>
   public abstract class ECI
   {
      /// <summary>
      /// the ECI value
      /// </summary>
      public virtual int Value { get; private set; }

      internal ECI(int val)
      {
         Value = val;
      }

      /// <param name="val">ECI value</param>
      /// <returns><see cref="ECI"/> representing ECI of given value, or null if it is legal but unsupported</returns>
      /// <throws>ArgumentException if ECI value is invalid </throws>
      public static ECI getECIByValue(int val)
      {
         if (val < 0 || val > 999999)
         {
            throw new System.ArgumentException("Bad ECI value: " + val);
         }
         if (val < 900)
         {
            // Character set ECIs use 000000 - 000899
            return CharacterSetECI.getCharacterSetECIByValue(val);
         }
         return null;
      }
   }
}