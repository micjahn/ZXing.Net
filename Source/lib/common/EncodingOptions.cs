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

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ZXing.Common
{
    /// <summary>
    /// Defines an container for encoder options
    /// </summary>
    [Serializable]
    public class EncodingOptions
    {
        /// <summary>
        /// Gets the data container for all options
        /// </summary>
#if !UNITY
        [Browsable(false)]
#endif
        public IDictionary<EncodeHintType, object> Hints { get; private set; }

        /// <summary>
        /// Specifies the height of the barcode image
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Output dimensions"), DescriptionAttribute("Height in pixels.")]
#endif
        public int Height
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.HEIGHT))
                {
                    return (int)Hints[EncodeHintType.HEIGHT];
                }
                return 0;
            }
            set
            {
                Hints[EncodeHintType.HEIGHT] = value;
            }
        }

        /// <summary>
        /// Specifies the width of the barcode image
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Output dimensions"), DescriptionAttribute("Width in pixels.")]
#endif
        public int Width
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.WIDTH))
                {
                    return (int)Hints[EncodeHintType.WIDTH];
                }
                return 0;
            }
            set
            {
                Hints[EncodeHintType.WIDTH] = value;
            }
        }

        /// <summary>
        /// Don't put the content string into the output image.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Output options"), DescriptionAttribute("Output only barcode, no Human Readable Interpretation.")]
#endif
        public bool PureBarcode
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.PURE_BARCODE))
                {
                    return (bool)Hints[EncodeHintType.PURE_BARCODE];
                }
                return false;
            }
            set
            {
                Hints[EncodeHintType.PURE_BARCODE] = value;
            }
        }

        /// <summary>
        /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary
        /// by format; for example it controls margin before and after the barcode horizontally for
        /// most 1D formats.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Output dimensions"), DescriptionAttribute("Specifies margin, in pixels, to use " +
            "when generating the barcode. The meaning can vary by format; for example it controls margin " +
            "before and after the barcode horizontally for most 1D formats.")]
#endif
        public int Margin
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.MARGIN))
                {
                    return (int)Hints[EncodeHintType.MARGIN];
                }
                return 0;
            }
            set
            {
                Hints[EncodeHintType.MARGIN] = value;
            }
        }

        /// <summary>
        /// Specifies whether the data should be encoded to the GS1 standard;
        /// FNC1 character is added in front of the data
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies whether the data should be encoded " +
            "to the GS1 standard; if so a FNC1 character is added in front of the data.")]
#endif
        public bool GS1Format
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.GS1_FORMAT))
                {
                    return (bool)Hints[EncodeHintType.GS1_FORMAT];
                }
                return false;
            }
            set
            {
                Hints[EncodeHintType.GS1_FORMAT] = value;
            }
        }

        /// <summary>
        /// Don't put the content string into the output image.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Output options"), DescriptionAttribute("Don't add a white area around the generated barcode if the requested size is larger than then barcode.")]
#endif
        public bool NoPadding
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.NO_PADDING))
                {
                    return (bool)Hints[EncodeHintType.NO_PADDING];
                }
                return false;
            }
            set
            {
                Hints[EncodeHintType.NO_PADDING] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingOptions"/> class.
        /// </summary>
        public EncodingOptions()
        {
            Hints = new Dictionary<EncodeHintType, object>();
        }
    }
}
