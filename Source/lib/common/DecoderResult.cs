/*
* Copyright 2007 ZXing authors
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

namespace ZXing.Common
{
    /// <summary>
    /// Encapsulates the result of decoding a matrix of bits. This typically
    /// applies to 2D barcode formats. For now it contains the raw bytes obtained,
    /// as well as a String interpretation of those bytes, if applicable.
    /// <author>Sean Owen</author>
    /// </summary>
    public sealed class DecoderResult
    {
        /// <summary>
        /// raw bytes representing the result, or null if not applicable
        /// </summary>
        public byte[] RawBytes { get; private set; }

        /// <summary>
        /// how many bits of<see cref="RawBytes"/> are valid; typically 8 times its length
        /// </summary>
        public int NumBits { get; private set; }

        /// <summary>
        /// text representation of the result
        /// </summary>
        public String Text { get; private set; }

        /// <summary>
        /// bytes representing the encoded text field
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// list of byte segments in the result, or null if not applicable
        /// </summary>
        public IList<byte[]> ByteSegments { get; private set; }

        /// <summary>
        /// name of error correction level used, or null if not applicable
        /// </summary>
        public String ECLevel { get; private set; }

        /// <summary>
        /// gets a value which describe if structure append data was found
        /// </summary>
        public bool StructuredAppend
        {
            get { return StructuredAppendParity >= 0 && StructuredAppendSequenceNumber >= 0; }
        }

        /// <summary>
        /// number of errors corrected, or null if not applicable
        /// </summary>
        public int ErrorsCorrected { get; set; }

        /// <summary>
        /// gives the sequence number of the result if structured append was found
        /// </summary>
        public int StructuredAppendSequenceNumber { get; private set; }

        /// <summary>
        /// number of erasures corrected, or null if not applicable
        /// </summary>
        public int Erasures { get; set; }

        /// <summary>
        /// gives the parity information if structured append was found
        /// </summary>
        public int StructuredAppendParity { get; private set; }

        /// <summary>
        /// Miscellanseous data value for the various decoders
        /// </summary>
        /// <value>The other.</value>
        public object Other { get; set; }

        /// <summary>
        /// gives the symbology identifier
        /// </summary>
        public int SymbologyModifier { get; set; }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        public DecoderResult(byte[] rawBytes, String text, List<byte[]> byteSegments, String ecLevel)
            : this(rawBytes, text, byteSegments, ecLevel, -1, -1, 0)
        {

        }

        /// <summary>
        /// initilizing constructor 
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        public DecoderResult(byte[] rawBytes, byte[] data, String text, List<byte[]> byteSegments, String ecLevel)
            : this(rawBytes, text, byteSegments, ecLevel)
        {
            Data = data;
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, String text, IList<byte[]> byteSegments, String ecLevel, int symbologyModifier)
           : this(rawBytes, text, byteSegments, ecLevel, -1, -1, symbologyModifier)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, byte[] data, String text, IList<byte[]> byteSegments, String ecLevel, int symbologyModifier)
           : this(rawBytes, text, byteSegments, ecLevel, -1, -1, symbologyModifier)
        {
            this.Data = data;
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="saSequence"></param>
        /// <param name="saParity"></param>
        public DecoderResult(byte[] rawBytes,
                     String text,
                     List<byte[]> byteSegments,
                     String ecLevel,
                     int saSequence,
                     int saParity)
             : this(rawBytes, text, byteSegments, ecLevel, saSequence, saParity, 0)
        {

        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="saSequence"></param>
        /// <param name="saParity"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, String text, IList<byte[]> byteSegments, String ecLevel, int saSequence, int saParity, int symbologyModifier)
           : this(rawBytes, rawBytes == null ? 0 : 8 * rawBytes.Length, text, byteSegments, ecLevel, saSequence, saParity, symbologyModifier)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="saSequence"></param>
        /// <param name="saParity"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, byte[] data, String text, IList<byte[]> byteSegments, String ecLevel, int saSequence, int saParity, int symbologyModifier)
            : this(rawBytes, text, byteSegments, ecLevel, saSequence, saParity, symbologyModifier)
        {
            Data = data;
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="numBits"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        public DecoderResult(byte[] rawBytes, int numBits, String text, IList<byte[]> byteSegments, String ecLevel)
           : this(rawBytes, numBits, text, byteSegments, ecLevel, -1, -1, 0)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="numBits"></param>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        public DecoderResult(byte[] rawBytes, int numBits, byte[] data, String text, IList<byte[]> byteSegments, String ecLevel)
            : this(rawBytes, numBits, text, byteSegments, ecLevel)
        {
            this.Data = data;
        }


        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="numBits"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="saSequence"></param>
        /// <param name="saParity"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, int numBits, String text, IList<byte[]> byteSegments, String ecLevel, int saSequence, int saParity, int symbologyModifier)
            : this(rawBytes, null, numBits, text, byteSegments, ecLevel, saSequence, saParity, symbologyModifier)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="numBits"></param>
        /// <param name="text"></param>
        /// <param name="byteSegments"></param>
        /// <param name="ecLevel"></param>
        /// <param name="saSequence"></param>
        /// <param name="saParity"></param>
        /// <param name="symbologyModifier"></param>
        public DecoderResult(byte[] rawBytes, byte[] data, int numBits, String text, IList<byte[]> byteSegments, String ecLevel, int saSequence, int saParity, int symbologyModifier)
        {
            if (rawBytes == null && text == null)
            {
                throw new ArgumentException();
            }
            RawBytes = rawBytes;
            Data = data;
            NumBits = numBits;
            Text = text;
            ByteSegments = byteSegments;
            ECLevel = ecLevel;
            StructuredAppendParity = saParity;
            StructuredAppendSequenceNumber = saSequence;
            SymbologyModifier = symbologyModifier;
        }
    }
}