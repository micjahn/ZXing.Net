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

using NUnit.Framework;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public sealed class BitArrayBuilderTest
   {
      [Test]
      public void testBuildBitArray1()
      {
         int[][] pairValues = {
                                 new int[] { 19 },
                                 new int[] { 673, 16 }
                              };

         String expected = " .......X ..XX..X. X.X....X .......X ....";

         checkBinary(pairValues, expected);
      }

      private static void checkBinary(int[][] pairValues, String expected)
      {
         BitArray binary = buildBitArray(pairValues);
         Assert.AreEqual(expected, binary.ToString());
      }

      private static BitArray buildBitArray(int[][] pairValues)
      {
         List<ExpandedPair> pairs = new List<ExpandedPair>();
         for (int i = 0; i < pairValues.Length; ++i)
         {
            int[] pair = pairValues[i];

            DataCharacter leftChar;
            if (i == 0)
            {
               leftChar = null;
            }
            else
            {
               leftChar = new DataCharacter(pair[0], 0);
            }

            DataCharacter rightChar;
            if (i == 0)
            {
               rightChar = new DataCharacter(pair[0], 0);
            }
            else if (pair.Length == 2)
            {
               rightChar = new DataCharacter(pair[1], 0);
            }
            else
            {
               rightChar = null;
            }

            ExpandedPair expandedPair = new ExpandedPair(leftChar, rightChar, null, true);
            pairs.Add(expandedPair);
         }

         return BitArrayBuilder.buildBitArray(pairs);
      }
   }
}