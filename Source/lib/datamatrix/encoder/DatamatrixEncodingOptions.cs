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
using ZXing.Datamatrix.Encoder;

namespace ZXing.Datamatrix
{
    /// <summary>
    /// The class holds the available options for the DatamatrixWriter
    /// </summary>
    [Serializable]
    public class DatamatrixEncodingOptions : EncodingOptions
    {
        /// <summary>
        /// Specifies the matrix shape for Data Matrix
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Output options"), DescriptionAttribute("Specifies the matrix shape for Data Matrix.")]
#endif
        public SymbolShapeHint? SymbolShape
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
                {
                    return (SymbolShapeHint)Hints[EncodeHintType.DATA_MATRIX_SHAPE];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
                        Hints.Remove(EncodeHintType.DATA_MATRIX_SHAPE);
                }
                else
                {
                    Hints[EncodeHintType.DATA_MATRIX_SHAPE] = value;
                }
            }
        }

        /// <summary>
        /// Specifies a minimum barcode size
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies a minimum barcode size.")]
        [TypeConverter(typeof(DimensionConverter))]
#endif
        public Dimension MinSize
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.MIN_SIZE))
                {
                    return (Dimension)Hints[EncodeHintType.MIN_SIZE];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.MIN_SIZE))
                        Hints.Remove(EncodeHintType.MIN_SIZE);
                }
                else
                {
                    Hints[EncodeHintType.MIN_SIZE] = value;
                }
            }
        }

        /// <summary>
        /// Specifies a maximum barcode size
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies a maximum barcode size.")]
        [TypeConverter(typeof(DimensionConverter))]
#endif
        public Dimension MaxSize
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.MAX_SIZE))
                {
                    return (Dimension)Hints[EncodeHintType.MAX_SIZE];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.MAX_SIZE))
                        Hints.Remove(EncodeHintType.MAX_SIZE);
                }
                else
                {
                    Hints[EncodeHintType.MAX_SIZE] = value;
                }
            }
        }

        /// <summary>
        /// Specifies the default encodation
        /// Make sure that the content fits into the encodation value, otherwise there will be an exception thrown.
        /// standard value: Encodation.ASCII
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies the default encodation." + 
			" Make sure that the content fits into the encodation value, otherwise there will be an exception thrown." +
			" Standard value: Encodation.ASCII")]
#endif
        public int? DefaultEncodation
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
                {
                    return (int)Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
                        Hints.Remove(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION);
                }
                else
                {
                    Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION] = value;
                }
            }
        }

        /// <summary>
        /// Specifies whether to use compact mode for Data Matrix (type {@link Boolean}, or "true" or "false"
        /// The compact encoding mode also supports the encoding of characters that are not in the ISO-8859-1
        /// character set via ECIs.
        /// Please note that in that case, the most compact character encoding is chosen for characters in
        /// the input that are not in the ISO-8859-1 character set. Based on experience, some scanners do not
        /// support encodings like cp-1256 (Arabic). In such cases the encoding can be forced to UTF-8 by
        /// means of the {@link #CHARACTER_SET} encoding hint.
        /// Compact encoding also provides GS1-FNC1 support when {@link #GS1_FORMAT} is selected. In this case
        /// group-separator character (ASCII 29 decimal) can be used to encode the positions of FNC1 codewords
        /// for the purpose of delimiting AIs.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies whether to use compact mode for Data Matrix."+
            " The compact encoding mode also supports the encoding of characters that are not in the ISO-8859-1" +
            " character set via ECIs." +
            " Please note that in that case, the most compact character encoding is chosen for characters in" +
            " the input that are not in the ISO-8859-1 character set. Based on experience, some scanners do not" +
            " support encodings like cp-1256 (Arabic). In such cases the encoding can be forced to UTF-8 by" +
            " means of the {@link #CHARACTER_SET} encoding hint." +
            " Compact encoding also provides GS1-FNC1 support when {@link #GS1_FORMAT} is selected. In this case" +
            " group-separator character (ASCII 29 decimal) can be used to encode the positions of FNC1 codewords" +
            " for the purpose of delimiting AIs.")]
#endif
        public bool CompactEncoding
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_COMPACT))
                {
                    var boolObj = Hints[EncodeHintType.DATA_MATRIX_COMPACT];
                    if (boolObj != null)
                        return (bool)boolObj;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[EncodeHintType.DATA_MATRIX_COMPACT] = value;
                }
                else
                {
                    if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_COMPACT))
                        Hints.Remove(EncodeHintType.DATA_MATRIX_COMPACT);
                }
            }
        }

        /// <summary>
        /// Forces C40 encoding for data-matrix (type {@link Boolean}, or "true" or "false") {@link String } value). This 
        /// option and {@link #DATA_MATRIX_COMPACT} are mutually exclusive.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Forces C40 encoding for data-matrix. This " +
            " option and CompactEncoding are mutually exclusive.")]
#endif
        public bool ForceC40
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.FORCE_C40))
                {
                    var boolObj = Hints[EncodeHintType.FORCE_C40];
                    if (boolObj != null)
                        return (bool)boolObj;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[EncodeHintType.FORCE_C40] = value;
                }
                else
                {
                    if (Hints.ContainsKey(EncodeHintType.FORCE_C40))
                        Hints.Remove(EncodeHintType.FORCE_C40);
                }
            }
        }
    }

#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
    internal class DimensionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(Dimension))
                return true;
            if (sourceType == typeof(String))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Dimension))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var dim = value as Dimension;
            if (dim != null)
            {
                return dim.Height + "x" + dim.Width;
            }
            if (value is String)
            {
                var valStr = value.ToString();
                var valStrParts = valStr.Split('x');
                if (valStrParts.Length > 1)
                {
                    int h;
                    int w;
                    if (int.TryParse(valStrParts[0], out h) &&
                        int.TryParse(valStrParts[1], out w))
                        return new Dimension(w, h);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;
            var dim = value as Dimension;
            if (dim != null)
            {
                return dim.Height + "x" + dim.Width;
            }
            if (destinationType == typeof(Dimension))
            {
                var valStr = value.ToString();
                var valStrParts = valStr.Split('x');
                if (valStrParts.Length > 1)
                {
                    int h;
                    int w;
                    if (int.TryParse(valStrParts[0], out h) &&
                        int.TryParse(valStrParts[1], out w))
                        return new Dimension(w, h);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
}
