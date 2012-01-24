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
using System.Text.RegularExpressions;

using com.google.zxing.common;

namespace com.google.zxing.oned.rss.expanded
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// </summary>
   public sealed class BinaryUtil
   {
      private static string ONE = "1";
      private static string ZERO = "0";
      private static string SPACE = " ";

      private BinaryUtil()
      {
      }

      /// <summary>
      /// Constructs a BitArray from a String like the one returned from BitArray.toString()
      /// </summary>
      public static BitArray buildBitArrayFromString(String data)
      {
         String dotsAndXs = Regex.Replace(Regex.Replace(data, ONE, "X"), ZERO, ".");
         BitArray binary = new BitArray(Regex.Replace(dotsAndXs, SPACE, "").Length);
         int counter = 0;

         for (int i = 0; i < dotsAndXs.Length; ++i)
         {
            if (i % 9 == 0)
            { // spaces
               if (dotsAndXs[i] != ' ')
               {
                  throw new InvalidOperationException("space expected");
               }
               continue;
            }

            char currentChar = dotsAndXs[i];
            if (currentChar == 'X' || currentChar == 'x')
            {
               binary[counter] = true;
            }
            counter++;
         }
         return binary;
      }

      public static BitArray buildBitArrayFromStringWithoutSpaces(String data)
      {
         StringBuilder sb = new StringBuilder();
         String dotsAndXs = Regex.Replace(Regex.Replace(data, ONE, "X"), ZERO, ".");
         int current = 0;
         while (current < dotsAndXs.Length)
         {
            sb.Append(' ');
            for (int i = 0; i < 8 && current < dotsAndXs.Length; ++i)
            {
               sb.Append(dotsAndXs[current]);
               current++;
            }
         }
         return buildBitArrayFromString(sb.ToString());
      }
   }
}