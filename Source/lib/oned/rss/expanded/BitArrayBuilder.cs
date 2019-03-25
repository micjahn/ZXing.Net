﻿/*
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

using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded
{
    /// <summary>
    /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
    /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
    /// </summary>
    static class BitArrayBuilder
    {
        internal static BitArray buildBitArray(List<ExpandedPair> pairs)
        {
            int charNumber = (pairs.Count << 1) - 1;
            if (pairs[pairs.Count - 1].RightChar == null)
            {
                charNumber -= 1;
            }

            int size = 12 * charNumber;

            BitArray binary = new BitArray(size);
            int accPos = 0;

            ExpandedPair firstPair = pairs[0];
            int firstValue = firstPair.RightChar.Value;
            for (int i = 11; i >= 0; --i)
            {
                if ((firstValue & (1 << i)) != 0)
                {
                    binary[accPos] = true;
                }
                accPos++;
            }

            for (int i = 1; i < pairs.Count; ++i)
            {
                ExpandedPair currentPair = pairs[i];

                int leftValue = currentPair.LeftChar.Value;
                for (int j = 11; j >= 0; --j)
                {
                    if ((leftValue & (1 << j)) != 0)
                    {
                        binary[accPos] = true;
                    }
                    accPos++;
                }

                if (currentPair.RightChar != null)
                {
                    int rightValue = currentPair.RightChar.Value;
                    for (int j = 11; j >= 0; --j)
                    {
                        if ((rightValue & (1 << j)) != 0)
                        {
                            binary[accPos] = true;
                        }
                        accPos++;
                    }
                }
            }
            return binary;
        }
    }
}
