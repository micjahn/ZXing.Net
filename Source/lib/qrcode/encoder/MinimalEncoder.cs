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

namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using ZXing.Common;

    /// <summary>
    /// * Encoder that encodes minimally
    /// *
    /// * Version selection:
    /// * The version can be preset in the constructor. If it isn't specified then the algorithm will compute three solutions
    /// * for the three different version classes 1-9, 10-26 and 27-40.
    /// *
    /// * It is not clear to me if ever a solution using for example Medium (Versions 10-26) could be smaller than a Small
    /// * solution (Versions 1-9) (proof for or against would be nice to have).
    /// * With hypothetical values for the number of length bits, the number of bits per mode and the number of bits per
    /// * encoded character it can be shown that it can happen at all as follows:
    /// * We hypothetically assume that a mode is encoded using 1 bit (instead of 4) and a character is encoded in BYTE mode
    /// * using 2 bit (instead of 8). Using these values we now attempt to encode the four characters "1234".
    /// * If we furthermore assume that in Version 1-9 the length field has 1 bit length so that it can encode up to 2
    /// * characters and that in Version 10-26 it has 2 bits length so that we can encode up to 2 characters then it is more
    /// * efficient to encode with Version 10-26 than with Version 1-9 as shown below:
    /// *
    /// * Number of length bits small version (1-9): 1
    /// * Number of length bits large version (10-26): 2
    /// * Number of bits per mode item: 1
    /// * Number of bits per character item: 2
    /// * BYTE(1,2),BYTE(3,4): 1+1+2+2,1+1+2+2=12 bits
    /// * BYTE(1,2,3,4): 1+2+2+2+2+2          =11 bits
    /// *
    /// * If we however change the capacity of the large encoding from 2 bit to 4 bit so that it potentially can encode 16
    /// * items, then it is more efficient to encode using the small encoding
    /// * as shown below:
    /// *
    /// * Number of length bits small version (1-9): 1
    /// * Number of length bits large version (10-26): 4
    /// * Number of bits per mode item: 1
    /// * Number of bits per character item: 2
    /// * BYTE(1,2),BYTE(3,4): 1+1+2+2,1+1+2+2=12 bits
    /// * BYTE(1,2,3,4): 1+4+2+2+2+2          =13 bits
    /// *
    /// * But as mentioned, it is not clear to me if this can ever happen with the actual values.
    /// *
    /// * ECI switching:
    /// *
    /// * In multi language content the algorithm selects the most compact representation using ECI modes. For example the
    /// * it is more compactly represented using one ECI to UTF-8 rather than two ECIs to ISO-8859-6 and ISO-8859-1 if the
    /// * text contains more ASCII characters (since they are represented as one byte sequence) as opposed to the case where
    /// * there are proportionally more Arabic characters that require two bytes in UTF-8 and only one in ISO-8859-6.
    /// *
    /// * @author Alex Geller
    /// </summary>
    internal class MinimalEncoder
    {
        internal enum VersionSize
        {
            SMALL,
            MEDIUM,
            LARGE
        }

        private String stringToEncode;
        private Version version;
        private bool isGS1;
        private Encoding[] encoders;

        private static bool canEncode(Encoding encoding, char c)
        {
            try
            {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3 && !WINDOWS_UWP && !PORTABLE
                var prevFallback = encoding.EncoderFallback;
#endif
                try
                {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3 && !WINDOWS_UWP && !PORTABLE
                    encoding.EncoderFallback = EncoderFallback.ExceptionFallback;
#endif
                    var result = encoding.GetByteCount(new char[] { c });
                    return result > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_3 && !WINDOWS_UWP && !PORTABLE
                    encoding.EncoderFallback = prevFallback;
#endif
                }
            }
            catch
            {
                return false;
            }
        }

        private static Encoding Clone(Encoding encoding)
        {
            // encodings have to be cloned to change the EncoderFallback property later

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !PORTABLE
            // Clone isn't supported by .net standard 1.0, 1.1 and portable
            return (Encoding)encoding.Clone();
#else
            return encoding;
#endif
        }

        /// <summary>
        /// Encoding is optional (default ISO-8859-1) and version is optional (minimal version is computed if not specified.
        /// </summary>
        /// <param name="stringToEncode"></param>
        /// <param name="version"></param>
        /// <param name="isGS1"></param>
        public MinimalEncoder(String stringToEncode, Version version, bool isGS1)
        {
            this.stringToEncode = stringToEncode;
            this.version = version;
            this.isGS1 = isGS1;

            // encodings have to be cloned to change the EncoderFallback property later
            var isoEncoders = new Encoding[15]; //room for the 15 ISO-8859 charsets 1 through 16.
            isoEncoders[0] = Clone(StringUtils.ISO88591_ENCODING);
            bool needUnicodeEncoder = false;

            for (int i = 0; i < stringToEncode.Length; i++)
            {
                int cnt = 0;
                int j;
                for (j = 0; j < 15; j++)
                {
                    if (isoEncoders[j] != null)
                    {
                        cnt++;
                        if (canEncode(isoEncoders[j], stringToEncode[i]))
                        {
                            break;
                        }
                    }
                }

                if (cnt == 14)
                {
                    //we need all. Can stop looking further.
                    break;
                }

                if (j >= 15)
                {
                    //no encoder found
                    for (j = 0; j < 15; j++)
                    {
                        if (j != 11 && isoEncoders[j] == null)
                        {
                            // ISO-8859-12 doesn't exist
                            try
                            {
                                var ce = Clone(Encoding.GetEncoding("ISO-8859-" + (j + 1)));
                                if (canEncode(ce, stringToEncode[i]))
                                {
                                    isoEncoders[j] = ce;
                                    break;
                                }
                            }
                            catch (Exception ) { }
                        }
                    }
                    if (j >= 15)
                    {
                        if (!canEncode(Clone(Encoding.BigEndianUnicode), stringToEncode[i]))
                        {
                            throw new WriterException("Can not encode character \\u" + String.Format("%04X",
                                (int)stringToEncode[i]) + " at position " + i + " in input \"" + stringToEncode + "\"");
                        }
                        needUnicodeEncoder = true;
                    }
                }
            }

            int numberOfEncoders = 0;
            for (int j = 0; j < 15; j++)
            {
                if (isoEncoders[j] != null && CharacterSetECI.getCharacterSetECI(isoEncoders[j]) != null)
                {
                    numberOfEncoders++;
                }
            }

            if (numberOfEncoders == 1 && !needUnicodeEncoder)
            {
                encoders = new Encoding[1];
                encoders[0] = isoEncoders[0];
            }
            else
            {
                encoders = new Encoding[numberOfEncoders + 2];
                int index = 0;
                for (int j = 0; j < 15; j++)
                {
                    if (isoEncoders[j] != null && CharacterSetECI.getCharacterSetECI(isoEncoders[j]) != null)
                    {
                        encoders[index++] = isoEncoders[j];
                    }
                }

                encoders[index] = Clone(Encoding.UTF8);
                encoders[index + 1] = Clone(Encoding.BigEndianUnicode);
            }
        }

        public static ResultList encode(String stringToEncode, Version version, bool isGS1)
        {
            return new MinimalEncoder(stringToEncode, version, isGS1).encode();
        }

        public ResultList encode()
        {
            if (version == null) { //compute minimal encoding trying the three version sizes.
                ResultList[] results =
                {
                    encode(getVersion(VersionSize.SMALL)),
                    encode(getVersion(VersionSize.MEDIUM)),
                    encode(getVersion(VersionSize.LARGE))
                };
                return postProcess(smallest(results));
            } else { //compute minimal encoding for a given version
                return postProcess(encode(version));
            }
        }

        public static VersionSize getVersionSize(Version version)
        {
            return version.VersionNumber <= 9 ? VersionSize.SMALL : version.VersionNumber <= 26 ?
              VersionSize.MEDIUM : VersionSize.LARGE;
        }

        public static Version getVersion(VersionSize versionSize)
        {
            switch (versionSize)
            {
                case VersionSize.SMALL:
                    return Version.getVersionForNumber(9);
                case VersionSize.MEDIUM:
                    return Version.getVersionForNumber(26);
                case VersionSize.LARGE:
                default:
                    return Version.getVersionForNumber(40);
            }
        }

        static bool isNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        static bool isDoubleByteKanji(char c)
        {
            return Encoder.isOnlyDoubleByteKanji(new String(new[] { c }));
        }

        static bool isAlphanumeric(char c)
        {
            return Encoder.getAlphanumericCode(c) != -1;
        }

        /// <summary>
        /// Returns the maximum number of encodeable characters in the given mode for the given version. Example: in
        /// Version 1, 2^10 digits or 2^8 bytes can be encoded. In Version 3 it is 2^14 digits and 2^16 bytes
        /// </summary>
        /// <param name="version"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static int getMaximumNumberOfEncodeableCharacters(Version version, Mode mode)
        {
            int count = mode.getCharacterCountBits(version);
            return count == 0 ? 0 : 1 << count;
        }

        public bool canEncode(Mode mode, char c)
        {
            switch (mode.Name)
            {
                case Mode.Names.KANJI:
                    return isDoubleByteKanji(c);
                case Mode.Names.ALPHANUMERIC:
                    return isAlphanumeric(c);
                case Mode.Names.NUMERIC:
                    return isNumeric(c);
                case Mode.Names.BYTE:
                    return true; //any character can be encoded as byte(s). Up to the caller to manage splitting into
                                 //multiple bytes when String.getBytes(Charset) return more than one byte.
                default:
                    return false;
            }
        }

        private static int getCompactedOrdinal(Mode mode)
        {
            if (mode == null)
            {
                return 0;
            }
            switch (mode.Name)
            {
                case Mode.Names.KANJI:
                    return 0;
                case Mode.Names.ALPHANUMERIC:
                    return 1;
                case Mode.Names.NUMERIC:
                    return 2;
                case Mode.Names.ECI:
                case Mode.Names.BYTE:
                    return 3;
                default:
                    throw new InvalidOperationException("Illegal mode " + mode);
            }
        }

        private static ResultList smallest(ResultList[] results)
        {
            ResultList smallestResult = null;
            foreach (ResultList result in results)
            {
                if (smallestResult == null || (result != null && result.Size < smallestResult.Size))
                {
                    smallestResult = result;
                }
            }
            return smallestResult;
        }

        private ResultList postProcess(ResultList result)
        {
            if (isGS1)
            {
                var first = result.First;
                if (first != null)
                {
                    if (first.Value.mode != Mode.ECI)
                    {
                        bool haveECI = false;
                        foreach (var node in result)
                        {
                            if (node.mode == Mode.ECI)
                            {
                                haveECI = true;
                                break;
                            }
                        }
                        if (haveECI)
                        {
                            //prepend a default character set ECI
                            result.addFirst(new ResultList.ResultNode(Mode.ECI, 0, 0, 0, this, result));
                        }
                    }
                }

                first = result.First;
                if (first.Value.mode != Mode.ECI)
                {
                    //prepend a FNC1_FIRST_POSITION
                    result.addFirst(new ResultList.ResultNode(Mode.FNC1_FIRST_POSITION, 0, 0, 0, this, result));
                }
                else
                {
                    //insert a FNC1_FIRST_POSITION after the ECI
                    result.AddAfter(first, new ResultList.ResultNode(Mode.FNC1_FIRST_POSITION, 0, 0, 0, this, result));
                }
            }
            //Add TERMINATOR according to "8.4.8 Terminator"
            //TODO: The terminator can be omitted if there are less than 4 bit in the capacity of the symbol.
            result.AddLast(new ResultList.ResultNode(Mode.TERMINATOR, stringToEncode.Length, 0, 0, this, result));
            return result;
        }

        private int getEdgeCharsetEncoderIndex(ResultList edge)
        {
            var last = edge.Last;
            return last != null ? last.Value.charsetEncoderIndex : 0;
        }

        private Mode getEdgeMode(ResultList edge)
        {
            var last = edge.Last;
            return last != null ? last.Value.mode : Mode.BYTE;
        }

        private int getEdgePosition(ResultList edge)
        {
            // The algorithm appends an edge at some point (in the method addEdge() with a minimal solution.
            // This function works regardless if the concatenation has already taken place or not.
            var last = edge.Last;
            return last != null ? last.Value.position : 0;
        }

        private int getEdgeLength(ResultList edge)
        {
            // The algorithm appends an edge at some point (in the method addEdge() with a minimal solution.
            // This function works regardless if the concatenation has already taken place or not.
            var last = edge.Last;
            return last != null ? last.Value.CharacterLength : 0;
        }

        private void addEdge(List<ResultList>[][][] vertices, ResultList edge, ResultList previous)
        {
            int vertexIndex = getEdgePosition(edge) + getEdgeLength(edge);
            if (vertices[vertexIndex][getEdgeCharsetEncoderIndex(edge)][getCompactedOrdinal(getEdgeMode(edge))] == null)
            {
                vertices[vertexIndex][getEdgeCharsetEncoderIndex(edge)][getCompactedOrdinal(getEdgeMode(edge))] = new List<ResultList>();
            }
            vertices[vertexIndex][getEdgeCharsetEncoderIndex(edge)][getCompactedOrdinal(getEdgeMode(edge))].Add(edge);

            if (previous != null)
            {
                edge.addFirst(previous);
            }
        }

        private void addEdges(Version version, List<ResultList>[][][] vertices, int from, ResultList previous)
        {
            for (int i = 0; i < encoders.Length; i++)
            {
                if (canEncode(encoders[i], stringToEncode[from]))
                {
                    ResultList edge = new ResultList(version, Mode.BYTE, from, i, 1, this);
                    bool needECI = (previous == null && i > 0) ||
                                      (previous != null && getEdgeCharsetEncoderIndex(previous) != i);
                    if (needECI)
                    {
                        var eci = new ResultList.ResultNode(Mode.ECI, from, i, 0, this, edge);
                        edge.AddFirst(eci);
                    }
                    addEdge(vertices, edge, previous);
                }
            }
            if (canEncode(Mode.KANJI, stringToEncode[from]))
            {
                addEdge(vertices, new ResultList(version, Mode.KANJI, from, 0, 1, this), previous);
            }
            int inputLength = stringToEncode.Length;
            if (canEncode(Mode.ALPHANUMERIC, stringToEncode[from]))
            {
                if (from + 1 >= inputLength || !canEncode(Mode.ALPHANUMERIC, stringToEncode[from + 1]))
                {
                    addEdge(vertices, new ResultList(version, Mode.ALPHANUMERIC, from, 0, 1, this), previous);
                }
                else
                {
                    addEdge(vertices, new ResultList(version, Mode.ALPHANUMERIC, from, 0, 2, this), previous);
                }
            }
            if (canEncode(Mode.NUMERIC, stringToEncode[from]))
            {
                if (from + 1 >= inputLength || !canEncode(Mode.NUMERIC, stringToEncode[from + 1]))
                {
                    addEdge(vertices, new ResultList(version, Mode.NUMERIC, from, 0, 1, this), previous);
                }
                else if (from + 2 >= inputLength || !canEncode(Mode.NUMERIC, stringToEncode[from + 2]))
                {
                    addEdge(vertices, new ResultList(version, Mode.NUMERIC, from, 0, 2, this), previous);
                }
                else
                {
                    addEdge(vertices, new ResultList(version, Mode.NUMERIC, from, 0, 3, this), previous);
                }
            }
        }

        public ResultList encode(Version version)
        {
            /* A vertex represents a tuple of a position in the input, a mode and an a character encoding where position 0
             * denotes the position left of the first character, 1 the position left of the second character and so on.
             * Likewise the end vertices are located after the last character at position stringToEncode.length().
             *
             * An edge leading to such a vertex encodes one or more of the characters left of the position that the vertex
             * represents and encodes it in the same encoding and mode as the vertex on which the edge ends. In other words,
             * all edges leading to a particular vertex encode the same characters in the same mode with the same character
             * encoding. They differ only by their source vertices who are all located at i+1 minus the number of encoded
             * characters.
             *
             * The edges leading to a vertex are stored in such a way that there is a fast way to enumerate the edges ending
             * on a particular vertex.
             *
             * The algorithm processes the vertices in order of their position thereby performing the following:
             *
             * For every vertex at position i the algorithm enumerates the edges ending on the vertex and removes all but the
             * shortest from that list.
             * Then it processes the vertices for the position i+1. If i+1 == stringToEncode.length() then the algorithm ends
             * and chooses the the edge with the smallest size from any of the edges leading to vertices at this position.
             * Otherwise the algorithm computes all possible outgoing edges for the vertices at the position i+1
             *
             * Examples:
             * The process is illustrated by showing the graph (edges) after each iteration from left to right over the input:
             * An edge is drawn as follows "(" + fromVertex + ") -- " + encodingMode + "(" + encodedInput + ") (" +
             * accumulatedSize + ") --> (" + toVertex + ")"
             *
             * The coding conversions of this project require lines to not exceed 120 characters. In order to view the examples
             * below join lines that end with a backslash. This can be achieved by running the command
             * sed -e ':a' -e 'N' -e '$!ba' -e 's/\\\n *[*]/ /g' on this file.
             *
             * Example 1 encoding the string "ABCDE":
             *
             * Initial situation
             * (initial) -- BYTE(A) (20) --> (1_BYTE)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC)
             *
             * Situation after adding edges to vertices at position 1
             * (initial) -- BYTE(A) (20) --> (1_BYTE) -- BYTE(B) (28) --> (2_BYTE)
             *                               (1_BYTE) -- ALPHANUMERIC(BC)                             (44) --> (3_ALPHANUMERIC)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC)
             *
             * Situation after adding edges to vertices at position 2
             * (initial) -- BYTE(A) (20) --> (1_BYTE)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC)
             * (initial) -- BYTE(A) (20) --> (1_BYTE) -- BYTE(B) (28) --> (2_BYTE)
                                           * (1_BYTE) -- ALPHANUMERIC(BC)                             (44) --> (3_ALPHANUMERIC)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC) -- BYTE(C) (44) --> (3_BYTE)
             *                                                            (2_ALPHANUMERIC) -- ALPHANUMERIC(CD)                             (35) --> (4_ALPHANUMERIC)
             *
             * Situation after adding edges to vertices at position 3
             * (initial) -- BYTE(A) (20) --> (1_BYTE) -- BYTE(B) (28) --> (2_BYTE) -- BYTE(C)         (36) --> (3_BYTE)
             *                               (1_BYTE) -- ALPHANUMERIC(BC)                             (44) --> (3_ALPHANUMERIC) -- BYTE(D) (64) --> (4_BYTE)
             *                                                                                                 (3_ALPHANUMERIC) -- ALPHANUMERIC(DE)                             (55) --> (5_ALPHANUMERIC)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC) -- ALPHANUMERIC(CD)                             (35) --> (4_ALPHANUMERIC)
             *                                                            (2_ALPHANUMERIC) -- ALPHANUMERIC(CD)                             (35) --> (4_ALPHANUMERIC)
             *
             * Situation after adding edges to vertices at position 4
             * (initial) -- BYTE(A) (20) --> (1_BYTE) -- BYTE(B) (28) --> (2_BYTE) -- BYTE(C)         (36) --> (3_BYTE) -- BYTE(D) (44) --> (4_BYTE)
             *                               (1_BYTE) -- ALPHANUMERIC(BC)                             (44) --> (3_ALPHANUMERIC) -- ALPHANUMERIC(DE)                             (55) --> (5_ALPHANUMERIC)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC) -- ALPHANUMERIC(CD)                             (35) --> (4_ALPHANUMERIC) -- BYTE(E) (55) --> (5_BYTE)
             *
             * Situation after adding edges to vertices at position 5
             * (initial) -- BYTE(A) (20) --> (1_BYTE) -- BYTE(B) (28) --> (2_BYTE) -- BYTE(C)         (36) --> (3_BYTE) -- BYTE(D)         (44) --> (4_BYTE) -- BYTE(E)         (52) --> (5_BYTE)
             *                               (1_BYTE) -- ALPHANUMERIC(BC)                             (44) --> (3_ALPHANUMERIC) -- ALPHANUMERIC(DE)                             (55) --> (5_ALPHANUMERIC)
             * (initial) -- ALPHANUMERIC(AB)                     (24) --> (2_ALPHANUMERIC) -- ALPHANUMERIC(CD)                             (35) --> (4_ALPHANUMERIC)
             *
             * Encoding as BYTE(ABCDE) has the smallest size of 52 and is hence chosen. The encodation ALPHANUMERIC(ABCD),
             * BYTE(E) is longer with a size of 55.
             *
             * Example 2 encoding the string "XXYY" where X denotes a character unique to character set ISO-8859-2 and Y a
             * character unique to ISO-8859-3. Both characters encode as double byte in UTF-8:
             *
             * Initial situation
             * (initial) -- BYTE(X) (32) --> (1_BYTE_ISO-8859-2)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-8)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-16BE)
             *
             * Situation after adding edges to vertices at position 1
             * (initial) -- BYTE(X) (32) --> (1_BYTE_ISO-8859-2) -- BYTE(X) (40) --> (2_BYTE_ISO-8859-2)
             *                               (1_BYTE_ISO-8859-2) -- BYTE(X) (72) --> (2_BYTE_UTF-8)
             *                               (1_BYTE_ISO-8859-2) -- BYTE(X) (72) --> (2_BYTE_UTF-16BE)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-8)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-16BE)
             *
             * Situation after adding edges to vertices at position 2
             * (initial) -- BYTE(X) (32) --> (1_BYTE_ISO-8859-2) -- BYTE(X) (40) --> (2_BYTE_ISO-8859-2)
             *                                                                       (2_BYTE_ISO-8859-2) -- BYTE(Y) (72) --> (3_BYTE_ISO-8859-3)
             *                                                                       (2_BYTE_ISO-8859-2) -- BYTE(Y) (80) --> (3_BYTE_UTF-8)
             *                                                                       (2_BYTE_ISO-8859-2) -- BYTE(Y) (80) --> (3_BYTE_UTF-16BE)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-8) -- BYTE(X) (56) --> (2_BYTE_UTF-8)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-16BE) -- BYTE(X) (56) --> (2_BYTE_UTF-16BE)
             *
             * Situation after adding edges to vertices at position 3
             * (initial) -- BYTE(X) (32) --> (1_BYTE_ISO-8859-2) -- BYTE(X) (40) --> (2_BYTE_ISO-8859-2) -- BYTE(Y) (72) --> (3_BYTE_ISO-8859-3)
             *                                                                                                               (3_BYTE_ISO-8859-3) -- BYTE(Y) (80) --> (4_BYTE_ISO-8859-3)
             *                                                                                                               (3_BYTE_ISO-8859-3) -- BYTE(Y) (112) --> (4_BYTE_UTF-8)
             *                                                                                                               (3_BYTE_ISO-8859-3) -- BYTE(Y) (112) --> (4_BYTE_UTF-16BE)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-8) -- BYTE(X) (56) --> (2_BYTE_UTF-8) -- BYTE(Y) (72) --> (3_BYTE_UTF-8)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-16BE) -- BYTE(X) (56) --> (2_BYTE_UTF-16BE) -- BYTE(Y) (72) --> (3_BYTE_UTF-16BE)
             *
             * Situation after adding edges to vertices at position 4
             * (initial) -- BYTE(X) (32) --> (1_BYTE_ISO-8859-2) -- BYTE(X) (40) --> (2_BYTE_ISO-8859-2) -- BYTE(Y) (72) --> (3_BYTE_ISO-8859-3) -- BYTE(Y) (80) --> (4_BYTE_ISO-8859-3)
             *                                                                                                               (3_BYTE_UTF-8) -- BYTE(Y) (88) --> (4_BYTE_UTF-8)
             *                                                                                                               (3_BYTE_UTF-16BE) -- BYTE(Y) (88) --> (4_BYTE_UTF-16BE)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-8) -- BYTE(X) (56) --> (2_BYTE_UTF-8) -- BYTE(Y) (72) --> (3_BYTE_UTF-8)
             * (initial) -- BYTE(X) (40) --> (1_BYTE_UTF-16BE) -- BYTE(X) (56) --> (2_BYTE_UTF-16BE) -- BYTE(Y) (72) --> (3_BYTE_UTF-16BE)
             *
             * Encoding as ECI(ISO-8859-2),BYTE(XX),ECI(ISO-8859-3),BYTE(YY) has the smallest size of 80 and is hence chosen.
             * The encodation ECI(UTF-8),BYTE(XXYY) is longer with a size of 88.
             */
            int inputLength = stringToEncode.Length;

            //Array that represents vertices. There is a vertex for every character, encoding and mode. The vertex contains a list
            //of all edges that lead to it that have the same encoding and mode.
            //The lists are created lazily

            //The last dimension in the array below encodes the 4 modes KANJI, ALPHANUMERIC, NUMERIC and BYTE via the
            //function getCompactedOrdinal(Mode)
            List<ResultList>[][][] vertices = new List<ResultList>[inputLength + 1][][];
            for (var indexDim1 = 0; indexDim1 < inputLength + 1; indexDim1++)
            {
                vertices[indexDim1] = new List<ResultList>[encoders.Length][];
                for (var indexDim2 = 0; indexDim2 < encoders.Length; indexDim2++)
                {
                    vertices[indexDim1][indexDim2] = new List<ResultList>[4];
                }
            }
            addEdges(version, vertices, 0, null);

            for (int i = 1; i <= inputLength; i++)
            {
                for (int j = 0; j < encoders.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        ResultList minimalEdge;
                        if (vertices[i][j][k] != null)
                        {
                            List<ResultList> edges = vertices[i][j][k];
                            if (edges.Count == 1)
                            {
                                //Optimization: if there is only one edge then that's the minimal one
                                minimalEdge = edges[0];
                            }
                            else
                            {
                                int minimalIndex = -1;
                                int minimalSize = int.MaxValue;
                                for (int l = 0; l < edges.Count; l++)
                                {
                                    ResultList edge = edges[l];
                                    if (edge.Size < minimalSize)
                                    {
                                        minimalIndex = l;
                                        minimalSize = edge.Size;
                                    }
                                }
                                minimalEdge = edges[minimalIndex];
                                edges.Clear();
                                edges.Add(minimalEdge);
                            }
                            if (i < inputLength)
                            {
                                addEdges(version, vertices, i, minimalEdge);
                            }
                        }
                    }
                }
            }
            {
                int minimalJ = -1;
                int minimalK = -1;
                int minimalSize = int.MaxValue;
                for (int j = 0; j < encoders.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (vertices[inputLength][j][k] != null)
                        {
                            List<ResultList> edges = vertices[inputLength][j][k];
                            ResultList edge = edges[0];
                            if (edge.Size < minimalSize)
                            {
                                minimalSize = edge.Size;
                                minimalJ = j;
                                minimalK = k;
                            }
                        }
                    }
                }
                if (minimalJ < 0)
                {
                    throw new WriterException("Internal error: failed to encode");
                }
                return vertices[inputLength][minimalJ][minimalK][0];
            }
        }

        private byte[] getBytesOfCharacter(int position, int charsetEncoderIndex)
        {
            //TODO: Is there a more efficient way for a single character?
            return encoders[charsetEncoderIndex].GetBytes(stringToEncode.Substring(position, 1));
        }

        internal class ResultList : LinkedList<ResultList.ResultNode> {

            private Version version;

            public ResultList(Version version)
            {
                this.version = version;
            }

            /// <summary>
            /// Short for rl=new ResultList(version); rl.add(rl.new ResultNode(modes, position, charsetEncoderIndex, length));                          
            /// </summary>
            /// <param name="version"></param>
            /// <param name="mode"></param>
            /// <param name="position"></param>
            /// <param name="charsetEncoderIndex"></param>
            public ResultList(Version version, Mode mode, int position, int charsetEncoderIndex, int length, MinimalEncoder encoder)
                : this(version)
            {

                AddLast(new ResultList.ResultNode(mode, position, charsetEncoderIndex, length, encoder, this));
            }

            public void addFirst(ResultList resultList)
            {
                LinkedListNode<ResultNode> node = resultList.Last;
                while (node != null)
                {
                    addFirst(node.Value);
                    node = node.Previous;
                }
            }

            /**
             * Prepends n and may modify this.getFirst().declaresMode before doing so.
             */

            public LinkedListNode<ResultNode> addFirst(ResultNode n)
            {
                var next = First;
                if (next != null)
                {
                    next.Value.declaresMode = n.mode != next.Value.mode ||
                        next.Value.mode == Mode.ECI ||
                        n.CharacterLength + next.Value.CharacterLength >= getMaximumNumberOfEncodeableCharacters(version, next.Value.mode);
                }

                return AddFirst(n);
            }

            /// <summary>
            /// returns the size in bits
            /// </summary>
            public int Size
            {
                get
                {
                    int result = 0;
                    foreach (var item in this)
                    {
                        result += item.Size;
                    }
                    return result;
                }
            }

            /// <summary>
            /// appends the bits
            /// </summary>
            /// <param name="bits"></param>
            public void getBits(BitArray bits)
            {
                var rni = First;
                while (rni != null)
                {
                    if (rni.Value.declaresMode)
                    {
                        // append mode
                        bits.appendBits(rni.Value.mode.Bits, 4);
                        if (rni.Value.CharacterLength > 0)
                        {
                            int length = rni.Value.CharacterCountIndicator;
                            var rnj = rni.Next;
                            while (rnj != null)
                            {
                                if (rnj.Value.declaresMode)
                                {
                                    break;
                                }
                                length += rnj.Value.CharacterCountIndicator;
                            }
                            bits.appendBits(length, rni.Value.mode.getCharacterCountBits(version));
                        }
                    }
                    rni.Value.getBits(bits);
                }
            }

            public Version getVersion(ErrorCorrectionLevel ecLevel)
            {
                int versionNumber = version.VersionNumber;
                int lowerLimit;
                int upperLimit;
                switch (getVersionSize(version))
                {
                    case VersionSize.SMALL:
                        lowerLimit = 1;
                        upperLimit = 9;
                        break;
                    case VersionSize.MEDIUM:
                        lowerLimit = 10;
                        upperLimit = 26;
                        break;
                    case VersionSize.LARGE:
                    default:
                        lowerLimit = 27;
                        upperLimit = 40;
                        break;
                }
                // increase version if needed
                while (versionNumber < upperLimit && !Encoder.willFit(Size, Version.getVersionForNumber(versionNumber), ecLevel))
                {
                    versionNumber++;
                }
                // shrink version if possible
                while (versionNumber > lowerLimit && Encoder.willFit(Size, Version.getVersionForNumber(versionNumber - 1), ecLevel))
                {
                    versionNumber--;
                }
                return Version.getVersionForNumber(versionNumber);
            }

            public override String ToString()
            {
                var result = new StringBuilder();
                ResultNode previous = null;
                foreach (var current in this)
                {
                    if (previous != null)
                    {
                        if (current.declaresMode)
                        {
                            result.Append(")");
                        }
                        result.Append(",");
                    }
                    result.Append(current.ToString());
                    previous = current;
                }
                if (previous != null)
                {
                    result.Append(")");
                }
                return result.ToString();
            }

            internal class ResultNode
            {
                public Mode mode;
                public bool declaresMode = true;
                public int position;
                public int charsetEncoderIndex;
                public int length;
                public ResultList resultList;
                public MinimalEncoder encoder;

                public ResultNode(Mode mode, int position, int charsetEncoderIndex, int length, MinimalEncoder encoder, ResultList resultList)
                {
                    this.mode = mode;
                    this.position = position;
                    this.charsetEncoderIndex = charsetEncoderIndex;
                    this.length = length;
                    this.encoder = encoder;
                    this.resultList = resultList;
                }

                /// <summary>
                /// returns the size in bits
                /// </summary>
                /// <returns></returns>
                public int Size
                {
                    get
                    {
                        int size = declaresMode ? 4 + mode.getCharacterCountBits(resultList.version) : 0;
                        switch (mode.Name)
                        {
                            case Mode.Names.KANJI:
                                size += 13;
                                break;
                            case Mode.Names.ALPHANUMERIC:
                                size += length == 1 ? 6 : 11;
                                break;
                            case Mode.Names.NUMERIC:
                                size += length == 1 ? 4 : length == 2 ? 7 : 10;
                                break;
                            case Mode.Names.BYTE:
                                size += 8 * encoder.getBytesOfCharacter(position, charsetEncoderIndex).Length;
                                break;
                            case Mode.Names.ECI:
                                size += 8; // the ECI assignment numbers for ISO-8859-x, UTF-8 and UTF-16 are all 8 bit long
                                break;
                        }
                        return size;
                    }
                }

                /// <summary>
                /// returns the length in characters
                /// </summary>
                /// <returns></returns>
                public int CharacterLength
                {
                    get
                    {
                        return length;
                    }
                }

                /// <summary>
                /// returns the length in characters according to the specification (differs from getCharacterLength() in BYTE mode
                /// for multi byte encoded characters)
                /// </summary>
                /// <returns></returns>
                public int CharacterCountIndicator
                {
                    get
                    {
                        return mode == Mode.BYTE ? encoder.getBytesOfCharacter(position, charsetEncoderIndex).Length : CharacterLength;
                    }
                }

                /// <summary>
                /// appends the bits
                /// </summary>
                /// <param name="bits"></param>
                public void getBits(BitArray bits)
                {
                    if (mode == Mode.ECI)
                    {
                        bits.appendBits(CharacterSetECI.getCharacterSetECI(encoder.encoders[charsetEncoderIndex]).Value, 8);
                    }
                    else if (CharacterLength > 0)
                    {
                        // append data
                        Encoder.appendBytes(encoder.stringToEncode.Substring(position, CharacterLength), mode, bits, encoder.encoders[charsetEncoderIndex]);
                    }
                }

                public override String ToString()
                {
                    var result = new StringBuilder();
                    if (declaresMode)
                    {
                        result.Append(mode).Append('(');
                    }
                    if (mode == Mode.ECI)
                    {
                        result.Append(encoder.encoders[charsetEncoderIndex].WebName.ToUpper());
                    }
                    else
                    {
                        result.Append(makePrintable(encoder.stringToEncode.Substring(position, length)));
                    }
                    return result.ToString();
                }

                private String makePrintable(String s)
                {
                    var result = new StringBuilder();
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] < 32 || s[i] > 126)
                        {
                            result.Append('.');
                        }
                        else
                        {
                            result.Append(s[i]);
                        }
                    }
                    return result.ToString();
                }
            }
        }
    }
}
