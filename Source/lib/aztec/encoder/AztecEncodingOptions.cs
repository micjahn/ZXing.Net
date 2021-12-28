/*
 * Copyright 2013 ZXing.Net authors
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

using System;
using System.ComponentModel;

using ZXing.Common;

namespace ZXing.Aztec
{
    /// <summary>
    /// The class holds the available options for the <see cref="AztecWriter" />
    /// </summary>
    [Serializable]
    public class AztecEncodingOptions : EncodingOptions
    {
        /// <summary>
        /// Representing the minimal percentage of error correction words. 
        /// Note: an Aztec symbol should have a minimum of 25% EC words.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("The minimal percentage of error correction words (> 25%).")]
#endif
        public int? ErrorCorrection
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
                {
                    return (int) Hints[EncodeHintType.ERROR_CORRECTION];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
                        Hints.Remove(EncodeHintType.ERROR_CORRECTION);
                }
                else
                {
                    Hints[EncodeHintType.ERROR_CORRECTION] = value;
                }
            }
        }

        /// <summary>
        /// Specifies the required number of layers for an Aztec code:
        /// a negative number (-1, -2, -3, -4) specifies a compact Aztec code
        /// 0 indicates to use the minimum number of layers (the default)
        /// a positive number (1, 2, .. 32) specifies a normal (non-compact) Aztec code
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("The required number of layers for an Aztec code" +
                                                             " -1 to -4 specify a compact code, 0 indicates to use the minimum number of layers and" +
                                                             " 1 to 32 specify a normal (non-compact) Aztec code.")]
#endif
        public int? Layers
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
                {
                    return (int) Hints[EncodeHintType.AZTEC_LAYERS];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
                        Hints.Remove(EncodeHintType.AZTEC_LAYERS);
                }
                else
                {
                    Hints[EncodeHintType.AZTEC_LAYERS] = value;
                }
            }
        }


        /// <summary>
        /// Specifies what character encoding to use where applicable (type <see cref="String"/>)
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies what character encoding to use where applicable.")]
#endif
        public string CharacterSet
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
                {
                    return (string) Hints[EncodeHintType.CHARACTER_SET];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
                        Hints.Remove(EncodeHintType.CHARACTER_SET);
                }
                else
                {
                    Hints[EncodeHintType.CHARACTER_SET] = value;
                }
            }
        }
    }
}
