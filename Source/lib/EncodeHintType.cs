/*
* Copyright 2008 ZXing authors
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

namespace ZXing
{
    /// <summary>
    /// These are a set of hints that you may pass to Writers to specify their behavior.
    /// </summary>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    public enum EncodeHintType
    {
        /// <summary>
        /// Specifies the width of the barcode image
        /// type: <see cref="System.Int32" />
        /// </summary>
        WIDTH,

        /// <summary>
        /// Specifies the height of the barcode image
        /// type: <see cref="System.Int32" />
        /// </summary>
        HEIGHT,

        /// <summary>
        /// Don't put the content string into the output image.
        /// type: <see cref="System.Boolean" />
        /// </summary>
        PURE_BARCODE,

        /// <summary>
        /// Specifies what degree of error correction to use, for example in QR Codes.
        /// Type depends on the encoder. For example for QR codes it's type
        /// <see cref="ZXing.QrCode.Internal.ErrorCorrectionLevel" />
        /// For Aztec it is of type <see cref="System.Int32" />, representing the minimal percentage of error correction words. 
        /// In all cases, it can also be a <see cref="System.String" /> representation of the desired value as well.
        /// Note: an Aztec symbol should have a minimum of 25% EC words.
        /// For PDF417 it is of type <see cref="ZXing.PDF417.Internal.PDF417ErrorCorrectionLevel"/> or <see cref="System.Int32" /> (between 0 and 8),
        /// </summary>
        ERROR_CORRECTION,

        /// <summary>
        /// Specifies what character encoding to use where applicable.
        /// type: <see cref="System.String" />
        /// </summary>
        CHARACTER_SET,

        /// <summary>
        /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary
        /// by format; for example it controls margin before and after the barcode horizontally for
        /// most 1D formats.
        /// type: <see cref="System.Int32" />, or <see cref="System.String" /> representation of the integer value
        /// </summary>
        MARGIN,

        /// <summary>
        /// Specifies the aspect ratio to use.  Default is 4.
        /// type: <see cref="ZXing.PDF417.Internal.PDF417AspectRatio" />, or 1-4.
        /// </summary>
        PDF417_ASPECT_RATIO,

        /// <summary>
        /// Specifies the desired aspect ratio (number of columns / number of rows) of the output image.  Default is 3.
        /// type: <see cref="System.Single" />.
        /// </summary>
        PDF417_IMAGE_ASPECT_RATIO,

        /// <summary>
        /// Specifies whether to use compact mode for PDF417
        /// type: <see cref="System.Boolean" />, or "true" or "false"
        /// <see cref="System.String" /> value
        /// </summary>
        PDF417_COMPACT,

        /// <summary>
        /// Specifies what compaction mode to use for PDF417.
        /// type: <see cref="ZXing.PDF417.Internal.Compaction" /> or <see cref="System.String" /> value of one of its
        /// enum values
        /// </summary>
        PDF417_COMPACTION,

        /// <summary>
        /// Specifies the minimum and maximum number of rows and columns for PDF417.
        /// type: <see cref="ZXing.PDF417.Internal.Dimensions" />
        /// </summary>
        PDF417_DIMENSIONS,

        /// <summary>
        /// The Specifies that the PDF417 will contain macro metadata.
        /// type: <see cref="ZXing.PDF417.PDF417MacroMetadata"/>
        /// </summary>
        PDF417_MACRO_META_DATA,

        /// <summary>
        /// Don't append ECI segment.
        /// That is against the specification of QR Code but some
        /// readers have problems if the charset is switched from
        /// ISO-8859-1 (default) to UTF-8 with the necessary ECI segment.
        /// If you set the property to true you can use UTF-8 encoding
        /// and the ECI segment is omitted.
        /// type: <see cref="System.Boolean" />
        /// </summary>
        DISABLE_ECI,

        /// <summary>
        /// Specifies the matrix shape for Data Matrix (type <see cref="ZXing.Datamatrix.Encoder.SymbolShapeHint"/>)
        /// </summary>
        DATA_MATRIX_SHAPE,

        /// <summary>
        /// Specifies a minimum barcode size (type <see cref="ZXing.Dimension"/>). Only applicable to Data Matrix now.
        /// </summary>
        MIN_SIZE,

        /// <summary>
        /// Specifies a maximum barcode size (type <see cref="ZXing.Dimension"/>). Only applicable to Data Matrix now.
        /// </summary>
        MAX_SIZE,

        /// <summary>
        /// if true, don't switch to codeset C for numbers
        /// </summary>
        CODE128_FORCE_CODESET_B,

        /// <summary>
        /// Specifies the default encodation for Data Matrix (type <see cref="ZXing.Datamatrix.Encoder.Encodation"/>)
        /// Make sure that the content fits into the encodation value, otherwise there will be an exception thrown.
        /// standard value: Encodation.ASCII
        /// </summary>
        DATA_MATRIX_DEFAULT_ENCODATION,

        /// <summary>
        /// Specifies the required number of layers for an Aztec code.
        /// A negative number (-1, -2, -3, -4) specifies a compact Aztec code
        /// 0 indicates to use the minimum number of layers (the default)
        /// A positive number (1, 2, .. 32) specifies a normal (non-compact) Aztec code
        /// type: <see cref="System.Int32" />, or <see cref="System.String" /> representation of the integer value
        /// </summary>
        AZTEC_LAYERS,

        /// <summary>
        /// Specifies the exact version of QR code to be encoded.
        /// (Type <see cref="System.Int32" />, or <see cref="System.String" /> representation of the integer value).
        /// </summary>
        QR_VERSION,

        /// <summary>
        /// Specifies whether the data should be encoded to the GS1 standard
        /// type: <see cref="System.Boolean" />, or "true" or "false"
        /// <see cref="System.String" /> value
        /// </summary>
        GS1_FORMAT,

        /// <summary>
        ///  Specifies the QR code mask pattern to be used. Allowed values are
        /// 0..QRCode.NUM_MASK_PATTERNS-1. By default the code will automatically select
        /// the optimal mask pattern.
        /// (Type <see cref="System.Int32" />, or <see cref="System.String" /> representation of the integer value).
        /// </summary>
        QR_MASK_PATTERN,

        /// <summary>
        /// Forces which encoding will be used. Currently only used for Code-128 code sets (Type <see cref="System.String" />). Valid values are "A", "B", "C".
        /// see also CODE128_FORCE_CODESET_B
        /// </summary>
        FORCE_CODE_SET,

        /// <summary>
        /// Specifies whether to use compact mode for QR code (type <see cref="System.Boolean" />, or "true" or "false"
        /// When compaction is performed the value for <see cref="CHARACTER_SET"/> is ignored.
        /// <see cref="System.String" /> value).
        /// </summary>
        QR_COMPACT,
    }
}