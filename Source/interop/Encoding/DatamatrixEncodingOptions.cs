/*
 * Copyright 2017 ZXing.Net authors
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

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ZXing.Interop.Encoding
{
    [ComVisible(true)]
    [Guid("117490D1-C212-4DEB-8F9A-041F69520F49")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class DatamatrixEncodingOptions : EncodingOptions
    {
        internal readonly Datamatrix.DatamatrixEncodingOptions wrappedDatamatrixEncodingOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AztecEncodingOptions"/> class.
        /// </summary>
        public DatamatrixEncodingOptions()
           : this(new Datamatrix.DatamatrixEncodingOptions())
        {
        }

        internal DatamatrixEncodingOptions(Datamatrix.DatamatrixEncodingOptions other)
           : base(other)
        {
            wrappedDatamatrixEncodingOptions = other;
        }

        /// <summary>
        /// Specifies the matrix shape for Data Matrix
        /// </summary>
        public SymbolShapeHint SymbolShape
        {
            get { return wrappedDatamatrixEncodingOptions.SymbolShape.ToInterop(); }
            set { wrappedDatamatrixEncodingOptions.SymbolShape = value.ToZXing(); }
        }

        /// <summary>
        /// Specifies a minimum barcode size
        /// </summary>
        public Dimension MinSize
        {
            get
            {
                var wrappedValue = wrappedDatamatrixEncodingOptions.MinSize;
                if (wrappedValue == null)
                    return null;
                return new Dimension(wrappedValue.Width, wrappedValue.Height);
            }
            set
            {
                wrappedDatamatrixEncodingOptions.MinSize = value == null ? null : new ZXing.Dimension(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Specifies a maximum barcode size
        /// </summary>
        public Dimension MaxSize
        {
            get
            {
                var wrappedValue = wrappedDatamatrixEncodingOptions.MaxSize;
                if (wrappedValue == null)
                    return null;
                return new Dimension(wrappedValue.Width, wrappedValue.Height);
            }
            set
            {
                wrappedDatamatrixEncodingOptions.MaxSize = value == null ? null : new ZXing.Dimension(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Specifies the default encodation
        /// Make sure that the content fits into the encodation value, otherwise there will be an exception thrown.
        /// standard value: Encodation.ASCII
        /// </summary>
        public int DefaultEncodation
        {
            get { return wrappedDatamatrixEncodingOptions.DefaultEncodation.GetValueOrDefault(Datamatrix.Encoder.Encodation.ASCII); }
            set { wrappedDatamatrixEncodingOptions.DefaultEncodation = value; }
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
        public bool CompactEncoding
        {
            get { return wrappedDatamatrixEncodingOptions.CompactEncoding; }
            set { wrappedDatamatrixEncodingOptions.CompactEncoding = value; }
        }

        /// <summary>
        /// Forces C40 encoding for data-matrix (type {@link Boolean}, or "true" or "false") {@link String } value). This 
        /// option and {@link #DATA_MATRIX_COMPACT} are mutually exclusive.
        /// </summary>
        public bool ForceC40
        {
            get { return wrappedDatamatrixEncodingOptions.ForceC40; }
            set { wrappedDatamatrixEncodingOptions.ForceC40 = value; }
        }

        /// <summary>
        /// Specifies what character encoding to use where applicable (type {@link String})
        /// </summary>
        public string CharacterSet
        {
            get { return wrappedDatamatrixEncodingOptions.CharacterSet; }
            set { wrappedDatamatrixEncodingOptions.CharacterSet = value; }
        }
    }

    [ComVisible(true)]
    [Guid("2D8B812C-B9CE-4003-82F8-E0F1563AD47F")]
    public enum SymbolShapeHint
    {
        FORCE_NONE,
        FORCE_SQUARE,
        FORCE_RECTANGLE,
    }

    internal static class SymbolShapeHintExtensions
    {
        public static SymbolShapeHint ToInterop(this Datamatrix.Encoder.SymbolShapeHint? other)
        {
            switch (other)
            {
                case Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE:
                    return SymbolShapeHint.FORCE_SQUARE;
                case Datamatrix.Encoder.SymbolShapeHint.FORCE_RECTANGLE:
                    return SymbolShapeHint.FORCE_RECTANGLE;
                case Datamatrix.Encoder.SymbolShapeHint.FORCE_NONE:
                case null:
                default:
                    return SymbolShapeHint.FORCE_NONE;
            }
        }
        public static Datamatrix.Encoder.SymbolShapeHint? ToZXing(this SymbolShapeHint other)
        {
            switch (other)
            {
                case SymbolShapeHint.FORCE_SQUARE:
                    return Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE;
                case SymbolShapeHint.FORCE_RECTANGLE:
                    return Datamatrix.Encoder.SymbolShapeHint.FORCE_RECTANGLE;
                case SymbolShapeHint.FORCE_NONE:
                default:
                    return Datamatrix.Encoder.SymbolShapeHint.FORCE_NONE;
            }
        }
    }

    [ComVisible(true)]
    [Guid("B89D3B64-7E44-486A-9160-E45EBB59F36D")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class Dimension
    {
        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Dimension(int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new System.ArgumentException();
            }
            Width = width;
            Height = height;
        }

        /// <summary>
        /// the width
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// the height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(System.Object other)
        {
            if (other is Dimension)
            {
                var d = (Dimension)other;
                return Width == d.Width && Height == d.Height;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Width * 32713 + Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.String ToString()
        {
            return Width + "x" + Height;
        }
    }
}
