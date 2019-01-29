/*
* Copyright 2012 ZXing.Net authors
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

namespace System.Numerics
{
   internal static class BigIntegerExtensions
   {
      internal static BigInteger Parse(string str)
      {
         if (String.IsNullOrEmpty(str))
            return BigInteger.Zero;

         var result = new BigInteger();
         var idx = 0;
         var sign = false;
         if (str[0] == '-')
         {
            idx++;
            sign = true;
         }
         if (str[0] == '+')
         {
            idx++;
         }

         int num = str.Length - idx;
         result = 0;
         while (--num >= 0)
         {
            result *= 10;
            if (str[idx] != '\0')
            {
               result += (str[idx++] - '0');
            }
         }

         if (sign)
         {
            result = -result;
         }

         return result;
      }
   }
}
