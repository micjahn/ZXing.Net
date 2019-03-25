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

using System;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Decoders
{
    /// <summary>
    /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
    /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
    /// </summary>
    public abstract class AbstractExpandedDecoder
    {
        private readonly BitArray information;
        private readonly GeneralAppIdDecoder generalDecoder;

        internal AbstractExpandedDecoder(BitArray information)
        {
            this.information = information;
            this.generalDecoder = new GeneralAppIdDecoder(information);
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <returns></returns>
        protected BitArray getInformation()
        {
            return information;
        }

        internal GeneralAppIdDecoder getGeneralDecoder()
        {
            return generalDecoder;
        }

        /// <summary>
        /// Parses the information.
        /// </summary>
        /// <returns></returns>
        public abstract String parseInformation();

        /// <summary>
        /// Creates the decoder.
        /// </summary>
        /// <param name="information">The information.</param>
        /// <returns></returns>
        public static AbstractExpandedDecoder createDecoder(BitArray information)
        {
            if (information[1])
            {
                return new AI01AndOtherAIs(information);
            }
            if (!information[2])
            {
                return new AnyAIDecoder(information);
            }

            int fourBitEncodationMethod = GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 4);

            switch (fourBitEncodationMethod)
            {
                case 4: return new AI013103decoder(information);
                case 5: return new AI01320xDecoder(information);
            }

            int fiveBitEncodationMethod = GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 5);
            switch (fiveBitEncodationMethod)
            {
                case 12: return new AI01392xDecoder(information);
                case 13: return new AI01393xDecoder(information);
            }

            int sevenBitEncodationMethod = GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 7);
            switch (sevenBitEncodationMethod)
            {
                case 56: return new AI013x0x1xDecoder(information, "310", "11");
                case 57: return new AI013x0x1xDecoder(information, "320", "11");
                case 58: return new AI013x0x1xDecoder(information, "310", "13");
                case 59: return new AI013x0x1xDecoder(information, "320", "13");
                case 60: return new AI013x0x1xDecoder(information, "310", "15");
                case 61: return new AI013x0x1xDecoder(information, "320", "15");
                case 62: return new AI013x0x1xDecoder(information, "310", "17");
                case 63: return new AI013x0x1xDecoder(information, "320", "17");
            }

            throw new InvalidOperationException("unknown decoder: " + information);
        }
    }
}
