/*
 * Copyright 2012 ZXing authors
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

namespace ZXing.PDF417.Internal.EC
{
   /// <summary>
   /// <p>Incomplete implementation of PDF417 error correction. For now, only detects errors.</p>
   /// @see com.google.zxing.common.reedsolomon.ReedSolomonDecoder
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class ErrorCorrection
   {
      private readonly ModulusGF field;

      public ErrorCorrection()
      {
         field = ModulusGF.PDF417_GF;
      }

      public bool decode(int[] received, int numECCodewords)
      {
         var poly = new ModulusPoly(field, received);
         var syndromeCoefficients = new int[numECCodewords];
         var noError = true;
         for (var i = 0; i < numECCodewords; i++)
         {
            var eval = poly.evaluateAt(field.exp(i + 1));
            syndromeCoefficients[syndromeCoefficients.Length - 1 - i] = eval;
            if (eval != 0)
            {
               noError = false;
            }
         }
         if (!noError)
         {
            return false;
         }
         // TODO actually correct errors!

         return true;
      }
   }
}