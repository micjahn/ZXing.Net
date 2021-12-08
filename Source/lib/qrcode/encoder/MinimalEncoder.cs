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
    /// * Algorithm:
    /// *
    /// * The eleventh commandment was "Thou Shalt Compute" or "Thou Shalt Not Compute" - I forget which(Alan Perilis).
    /// *
    /// * This implementation computes.As an alternative, the QR-Code specification suggests heuristics like this one:
    /// *
    /// * If initial input data is in the exclusive subset of the Alphanumeric character set AND if there are less than
    /// * [6,7,8] characters followed by data from the remainder of the 8-bit byte character set, THEN select the 8-
    /// * bit byte mode ELSE select Alphanumeric mode;
    /// *
    /// * This is probably right for 99.99% of cases but there is at least this one counter example: The string "AAAAAAa"
    /// * encodes 2 bits smaller as ALPHANUMERIC(AAAAAA), BYTE(a) than by encoding it as BYTE(AAAAAAa).
    /// * Perhaps that is the only counter example but without having proof, it remains unclear.
    /// *
    /// * ECI switching:
    /// *
    /// * In multi language content the algorithm selects the most compact representation using ECI modes.
    /// * For example the most compact representation of the string "\u0150\u015C" (O-double-acute, S-circumflex) is
    /// * ECI(UTF-8), BYTE(\u0150\u015C) while prepending one or more times the same leading character as in
    /// * "\u0150\u0150\u015C", the most compact representation  uses two ECIs so that the string is encoded as
    /// * ECI(ISO-8859-2), BYTE(\u0150\u0150), ECI(ISO-8859-3), BYTE(\u015C).
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

        // List of encoders that potentially encode characters not in ISO-8859-1 in one byte.
        private static List<Encoding> ENCODERS = new List<Encoding>();
        static MinimalEncoder()
        {
            var names = new[]
            {
                "ISO-8859-2",
                "ISO-8859-3",
                "ISO-8859-4",
                "ISO-8859-5",
                "ISO-8859-6",
                "ISO-8859-7",
                "ISO-8859-8",
                "ISO-8859-9",
                "ISO-8859-10",
                "ISO-8859-11",
                "ISO-8859-13",
                "ISO-8859-14",
                "ISO-8859-15",
                "ISO-8859-16",
                "windows-1250",
                "windows-1251",
                "windows-1252",
                "windows-1253",
                "windows-1254",
                "windows-1255",
                "windows-1256",
                "windows-1257",
                "windows-1258",
                "Shift_JIS"
            };
            foreach (String name in names)
            {
                if (CharacterSetECI.getCharacterSetECIByName(name) != null)
                {
                    try
                    {
                        ENCODERS.Add(Clone(Encoding.GetEncoding(name)));
                    }
                    catch (Exception )
                    {
                        // continue
                    }
                }
            }
        }

        private String stringToEncode;
        private bool isGS1;
        private Encoding[] encoders;
        private int priorityEncoderIndex;
        private ErrorCorrectionLevel ecLevel;

#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || WINDOWS_UWP || PORTABLE || WINDOWS_PHONE || NETFX_CORE || WindowsCE || SILVERLIGHT
        private static bool canEncode(Encoding encoding, char c)
        {
            // very limited support on old platforms; not sure, if it would work; and not sure, if somebody need the old platform support
            try
            {
                var result = encoding.GetByteCount(new char[] { c });
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
#else
        private static bool canEncode(Encoding encoding, char c)
        {
            try
            {
                var prevFallback = encoding.EncoderFallback;
                try
                {
                    encoding.EncoderFallback = EncoderFallback.ExceptionFallback;
                    var result = encoding.GetByteCount(new char[] { c });
                    return result > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    encoding.EncoderFallback = prevFallback;
                }
            }
            catch
            {
                return false;
            }
        }
#endif

        private static Encoding Clone(Encoding encoding)
        {
            // encodings have to be cloned to change the EncoderFallback property later

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !PORTABLE && !WINDOWS_PHONE && !NETFX_CORE
            // Clone isn't supported by .net standard 1.0, 1.1 and portable
            return (Encoding)encoding.Clone();
#else
            return encoding;
#endif
        }

        /// <summary>
        /// Creates a MinimalEncoder
        /// </summary>
        /// <param name="stringToEncode">The string to encode</param>
        /// <param name="priorityCharset">The preferred <see cref="System.Text.Encoding"/>. When the value of the argument is null, the algorithm
        /// *   chooses charsets that leads to a minimal representation.Otherwise the algorithm will use the priority
        /// *   charset to encode any character in the input that can be encoded by it if the charset is among the
        /// *   supported charsets.</param>
        /// <param name="isGS1"> {@code true} if a FNC1 is to be prepended; {@code false} otherwise</param>
        /// <param name="ecLevel">The error correction level.</param>
        public MinimalEncoder(String stringToEncode, Encoding priorityCharset, bool isGS1, ErrorCorrectionLevel ecLevel)
        {
            this.stringToEncode = stringToEncode;
            this.isGS1 = isGS1;
            this.ecLevel = ecLevel;

            var neededEncoders = new List<Encoding>();
            neededEncoders.Add(Clone(StringUtils.ISO88591_ENCODING));
            var needUnicodeEncoder = priorityCharset != null && priorityCharset.WebName.StartsWith("UTF", StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < stringToEncode.Length; i++)
            {
                bool canEnc = false;
                foreach (var encoder in neededEncoders)
                {
                    if (canEncode(encoder, stringToEncode[i]))
                    {
                        canEnc = true;
                        break;
                    }
                }

                if (!canEnc)
                {
                    foreach (var encoder in ENCODERS)
                    {
                        if (canEncode(encoder, stringToEncode[i]))
                        {
                            neededEncoders.Add(encoder);
                            canEnc = true;
                            break;
                        }
                    }
                }

                if (!canEnc)
                {
                    needUnicodeEncoder = true;
                }
            }

            if (neededEncoders.Count == 1 && !needUnicodeEncoder)
            {
                encoders = new Encoding[] { neededEncoders[0] };
            }
            else
            {
                encoders = new Encoding[neededEncoders.Count + 2];
                int index = 0;
                foreach (var encoder in neededEncoders)
                {
                    encoders[index++] = encoder;
                }

                encoders[index] = Clone(Encoding.UTF8);
                encoders[index + 1] = Clone(Encoding.BigEndianUnicode);
            }

            int priorityEncoderIndexValue = -1;
            if (priorityCharset != null)
            {
                for (int i = 0; i < encoders.Length; i++)
                {
                    if (encoders[i] != null && priorityCharset.WebName.Equals(encoders[i].WebName))
                    {
                        priorityEncoderIndexValue = i;
                        break;
                    }
                }
            }
            priorityEncoderIndex = priorityEncoderIndexValue;
        }

        /// <summary>
        /// Encodes the string minimally
        /// </summary>
        /// <param name="stringToEncode">The string to encode</param>
        /// <param name="version">The preferred <see cref="Version"/>. A minimal version is computed(see
        ///  {@link ResultList#getVersion method} when the value of the argument is null</param>
        /// <param name="priorityCharset">The preferred { @link Charset}. When the value of the argument is null, the algorithm
        ///  chooses charsets that leads to a minimal representation.Otherwise the algorithm will use the priority
        /// charset to encode any character in the input that can be encoded by it if the charset is among the
        /// supported charsets.</param>
        /// <param name="isGS1">{ @code true} if a FNC1 is to be prepended;{ @code false}otherwise</param>
        /// <param name="ecLevel">The error correction level.</param>
        /// <returns>An instance of { @code ResultList}
        ///        representing the minimal solution.
        /// @see ResultList#getBits
        /// @see ResultList#getVersion
        /// @see ResultList#getSize</returns>
        public static ResultList encode(String stringToEncode, Version version, Encoding priorityCharset, bool isGS1, ErrorCorrectionLevel ecLevel)
        {
            return new MinimalEncoder(stringToEncode, priorityCharset, isGS1, ecLevel).encode(version);
        }
        public ResultList encode(Version version)
        {
            if (version == null)
            {
                // compute minimal encoding trying the three version sizes.
                Version[] versions =
                {
                    getVersion(VersionSize.SMALL),
                    getVersion(VersionSize.MEDIUM),
                    getVersion(VersionSize.LARGE)
                };
                ResultList[] results =
                {
                    encodeSpecificVersion(versions[0]),
                    encodeSpecificVersion(versions[1]),
                    encodeSpecificVersion(versions[2])
                };
                int smallestSize = Int32.MaxValue;
                int smallestResult = -1;
                for (int i = 0; i < 3; i++)
                {
                    int size = results[i].Size;
                    if (Encoder.willFit(size, versions[i], ecLevel) && size < smallestSize)
                    {
                        smallestSize = size;
                        smallestResult = i;
                    }
                }
                if (smallestResult < 0)
                {
                    throw new WriterException("Data too big for any version");
                }
                return results[smallestResult];
            }
            else
            {
                // compute minimal encoding for a given version
                ResultList result = encodeSpecificVersion(version);
                if (!Encoder.willFit(result.Size, getVersion(getVersionSize(result.getVersion())), ecLevel))
                {
                    throw new WriterException("Data too big for version" + version);
                }
                return result;
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
                    return true; // any character can be encoded as byte(s). Up to the caller to manage splitting into
                                 // multiple bytes when String.getBytes(Charset) return more than one byte.
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
                case Mode.Names.BYTE:
                    return 3;
                default:
                    throw new InvalidOperationException("Illegal mode " + mode);
            }
        }

        void addEdge(List<Edge>[][][] edges, int position, Edge edge)
        {
            int vertexIndex = position + edge.characterLength;
            if (edges[vertexIndex][edge.charsetEncoderIndex][getCompactedOrdinal(edge.mode)] == null)
            {
                edges[vertexIndex][edge.charsetEncoderIndex][getCompactedOrdinal(edge.mode)] = new List<Edge>();
            }
            edges[vertexIndex][edge.charsetEncoderIndex][getCompactedOrdinal(edge.mode)].Add(edge);
        }

        void addEdges(Version version, List<Edge>[][][] edges, int from, Edge previous)
        {
            int start = 0;
            int end = encoders.Length;
            if (priorityEncoderIndex >= 0 && canEncode(encoders[priorityEncoderIndex], stringToEncode[from]))
            {
                start = priorityEncoderIndex;
                end = priorityEncoderIndex + 1;
            }

            for (int i = start; i < end; i++)
            {
                if (canEncode(encoders[i], stringToEncode[from]))
                {
                    addEdge(edges, from, new Edge(Mode.BYTE, from, i, 1, previous, version, this));
                }
            }

            if (canEncode(Mode.KANJI, stringToEncode[from]))
            {
                addEdge(edges, from, new Edge(Mode.KANJI, from, 0, 1, previous, version, this));
            }

            int inputLength = stringToEncode.Length;
            if (canEncode(Mode.ALPHANUMERIC, stringToEncode[from]))
            {
                addEdge(edges, from, new Edge(Mode.ALPHANUMERIC, from, 0, from + 1 >= inputLength ||
                    !canEncode(Mode.ALPHANUMERIC, stringToEncode[from + 1]) ? 1 : 2, previous, version, this));
            }

            if (canEncode(Mode.NUMERIC, stringToEncode[from]))
            {
                addEdge(edges, from, new Edge(Mode.NUMERIC, from, 0, from + 1 >= inputLength ||
                    !canEncode(Mode.NUMERIC, stringToEncode[from + 1]) ? 1 : from + 2 >= inputLength ||
                    !canEncode(Mode.NUMERIC, stringToEncode[from + 2]) ? 2 : 3, previous, version, this));
            }
        }

        public ResultList encodeSpecificVersion(Version version)
        {
            /* A vertex represents a tuple of a position in the input, a mode and a character encoding where position 0
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
             * Example 1 encoding the string "ABCDE":
             * Note: This example assumes that alphanumeric encoding is only possible in multiples of two characters so that
             * the example is both short and showing the principle. In reality this restriction does not exist.
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
            var edges = new List<Edge>[inputLength + 1][][];
            for (var indexDim1 = 0; indexDim1 < inputLength + 1; indexDim1++)
            {
                edges[indexDim1] = new List<Edge>[encoders.Length][];
                for (var indexDim2 = 0; indexDim2 < encoders.Length; indexDim2++)
                {
                    edges[indexDim1][indexDim2] = new List<Edge>[4];
                }
            }
            addEdges(version, edges, 0, null);

            for (int i = 1; i <= inputLength; i++)
            {
                for (int j = 0; j < encoders.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Edge minimalEdge;
                        if (edges[i][j][k] != null)
                        {
                            var localEdges = edges[i][j][k];
                            int minimalIndex = -1;
                            int minimalSize = Int32.MaxValue;
                            for (int l = 0; l < localEdges.Count; l++)
                            {
                                var edge = localEdges[l];
                                if (edge.cachedTotalSize < minimalSize)
                                {
                                    minimalIndex = l;
                                    minimalSize = edge.cachedTotalSize;
                                }
                            }
                            // assert minimalIndex != -1;
                            minimalEdge = localEdges[minimalIndex];
                            localEdges.Clear();
                            localEdges.Add(minimalEdge);
                            if (i < inputLength)
                            {
                                addEdges(version, edges, i, minimalEdge);
                            }
                        }
                    }
                }
            }
            {
                int minimalJ = -1;
                int minimalK = -1;
                int minimalSize = Int32.MaxValue;
                for (int j = 0; j < encoders.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (edges[inputLength][j][k] != null)
                        {
                            var localEdges = edges[inputLength][j][k];
                            //assert localEdges.size() == 1;
                            var edge = localEdges[0];
                            if (edge.cachedTotalSize < minimalSize)
                            {
                                minimalSize = edge.cachedTotalSize;
                                minimalJ = j;
                                minimalK = k;
                            }
                        }
                    }
                }

                if (minimalJ < 0)
                {
                    throw new WriterException("Internal error: failed to encode \"" + stringToEncode + "\"");
                }
                return new ResultList(version, edges[inputLength][minimalJ][minimalK][0], this);
            }
        }

        internal sealed class Edge
        {
            public Mode mode;
            public int fromPosition;
            public int charsetEncoderIndex;
            public int characterLength;
            public Edge previous;
            public int cachedTotalSize;

            public Edge(Mode mode, int fromPosition, int charsetEncoderIndex, int characterLength, Edge previous, Version version, MinimalEncoder encoder)
            {
                this.mode = mode;
                this.fromPosition = fromPosition;
                this.charsetEncoderIndex = mode == Mode.BYTE || previous == null ? charsetEncoderIndex :
                    previous.charsetEncoderIndex; // inherit the encoding if not of type BYTE
                this.characterLength = characterLength;
                this.previous = previous;

                int size = previous != null ? previous.cachedTotalSize : 0;

                bool needECI = mode == Mode.BYTE &&
                    (previous == null && this.charsetEncoderIndex != 0) || // at the beginning and charset is not ISO-8859-1
                    (previous != null && this.charsetEncoderIndex != previous.charsetEncoderIndex);

                if (previous == null || mode != previous.mode || needECI)
                {
                    size += 4 + mode.getCharacterCountBits(version);
                }
                switch (mode.Name)
                {
                    case Mode.Names.KANJI:
                        size += 13;
                        break;
                    case Mode.Names.ALPHANUMERIC:
                        size += characterLength == 1 ? 6 : 11;
                        break;
                    case Mode.Names.NUMERIC:
                        size += characterLength == 1 ? 4 : characterLength == 2 ? 7 : 10;
                        break;
                    case Mode.Names.BYTE:
                        size += 8 * encoder.encoders[charsetEncoderIndex].GetBytes(encoder.stringToEncode.Substring(fromPosition, characterLength)).Length;
                        if (needECI)
                        {
                            size += 4 + 8; // the ECI assignment numbers for ISO-8859-x, UTF-8 and UTF-16 are all 8 bit long
                        }
                        break;
                }
                cachedTotalSize = size;
            }
        }

        internal class ResultList
        {
            private List<ResultList.ResultNode> list = new List<ResultList.ResultNode>();
            private Version version;
            private MinimalEncoder encoder;

            public ResultList(Version version, Edge solution, MinimalEncoder encoder)
            {
                this.encoder = encoder;
                this.version = version;
                var length = 0;
                var current = solution;
                var containsECI = false;
                while (current != null)
                {
                    length += current.characterLength;
                    Edge previous = current.previous;

                    bool needECI = current.mode == Mode.BYTE &&
                        (previous == null && current.charsetEncoderIndex != 0) || // at the beginning and charset is not ISO-8859-1
                        (previous != null && current.charsetEncoderIndex != previous.charsetEncoderIndex);

                    if (needECI)
                    {
                        containsECI = true;
                    }

                    if (previous == null || previous.mode != current.mode || needECI)
                    {
                        list.Insert(0, new ResultNode(current.mode, current.fromPosition, current.charsetEncoderIndex, length, encoder, this));
                        length = 0;
                    }

                    if (needECI)
                    {
                        list.Insert(0, new ResultNode(Mode.ECI, current.fromPosition, current.charsetEncoderIndex, 0, encoder, this));
                    }
                    current = previous;
                }

                // prepend FNC1 if needed. If the bits contain an ECI then the FNC1 must be preceeded by an ECI.
                // If there is no ECI at the beginning then we put an ECI to the default charset (ISO-8859-1)
                if (encoder.isGS1)
                {
                    var first = list[0];
                    if (first != null && first.mode != Mode.ECI && containsECI)
                    {
                        // prepend a default character set ECI
                        list.Insert(0, new ResultNode(Mode.ECI, 0, 0, 0, encoder, this));
                    }
                    first = list[0];
                    // prepend or insert a FNC1_FIRST_POSITION after the ECI (if any)
                    var node = new ResultNode(Mode.FNC1_FIRST_POSITION, 0, 0, 0, encoder, this);
                    if (first == null || first.mode != Mode.ECI)
                        list.Insert(0, node);
                    else
                        list.Insert(1, node);
                }

                // set version to smallest version into which the bits fit.
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
                int size = getSize(version);
                // increase version if needed
                while (versionNumber < upperLimit && !Encoder.willFit(size, Version.getVersionForNumber(versionNumber), encoder.ecLevel))
                {
                    versionNumber++;
                }
                // shrink version if possible
                while (versionNumber > lowerLimit && Encoder.willFit(size, Version.getVersionForNumber(versionNumber - 1), encoder.ecLevel))
                {
                    versionNumber--;
                }
                this.version = Version.getVersionForNumber(versionNumber);
            }

            /// <summary>
            /// returns the size in bits
            /// </summary>
            public int Size
            {
                get
                {
                    return getSize(version);
                }
            }

            private int getSize(Version version)
            {
                int result = 0;
                foreach (var resultNode in list)
                {
                    result += resultNode.getSize(version);
                }
                return result;
            }

            /// <summary>
            /// appends the bits
            /// </summary>
            /// <param name="bits"></param>
            public void getBits(BitArray bits)
            {
                foreach (ResultNode resultNode in list)
                {
                    resultNode.getBits(bits);
                }
            }

            public Version getVersion()
            {
                return version;
            }

            public override String ToString()
            {
                var result = new StringBuilder();
                ResultNode previous = null;
                foreach (var current in list)
                {
                    if (previous != null)
                    {
                        result.Append(",");
                    }
                    result.Append(current.ToString());
                    previous = current;
                }
                return result.ToString();
            }

            internal class ResultNode
            {
                public Mode mode;
                public int fromPosition;
                public int charsetEncoderIndex;
                public int characterLength;
                public ResultList resultList;
                public MinimalEncoder encoder;

                public ResultNode(Mode mode, int fromPosition, int charsetEncoderIndex, int characterLength, MinimalEncoder encoder, ResultList resultList)
                {
                    this.mode = mode;
                    this.fromPosition = fromPosition;
                    this.charsetEncoderIndex = charsetEncoderIndex;
                    this.characterLength = characterLength;
                    this.encoder = encoder;
                    this.resultList = resultList;
                }

                /// <summary>
                /// returns the size in bits
                /// </summary>
                /// <returns></returns>
                public int getSize(Version version)
                {
                    int size = 4 + mode.getCharacterCountBits(resultList.version);
                    switch (mode.Name)
                    {
                        case Mode.Names.KANJI:
                            size += 13 * characterLength;
                            break;
                        case Mode.Names.ALPHANUMERIC:
                            size += (characterLength / 2) * 11;
                            size += (characterLength % 2) == 1 ? 6 : 0;
                            break;
                        case Mode.Names.NUMERIC:
                            size += (characterLength / 3) * 10;
                            int rest = characterLength % 3;
                            size += rest == 1 ? 4 : rest == 2 ? 7 : 0;
                            break;
                        case Mode.Names.BYTE:
                            size += 8 * CharacterCountIndicator;
                            break;
                        case Mode.Names.ECI:
                            size += 8; // the ECI assignment numbers for ISO-8859-x, UTF-8 and UTF-16 are all 8 bit long
                            break;
                    }
                    return size;
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
                        return mode == Mode.BYTE ? encoder.encoders[charsetEncoderIndex].GetBytes(encoder.stringToEncode.Substring(fromPosition, characterLength)).Length : characterLength;
                    }
                }

                /// <summary>
                /// appends the bits
                /// </summary>
                /// <param name="bits"></param>
                public void getBits(BitArray bits)
                {
                    bits.appendBits(mode.Bits, 4);
                    if (characterLength > 0)
                    {
                        int length = CharacterCountIndicator;
                        bits.appendBits(length, mode.getCharacterCountBits(resultList.version));
                    }
                    if (mode == Mode.ECI)
                    {
                        bits.appendBits(CharacterSetECI.getCharacterSetECI(encoder.encoders[charsetEncoderIndex]).Value, 8);
                    }
                    else if (characterLength > 0)
                    {
                        // append data
                        Encoder.appendBytes(encoder.stringToEncode.Substring(fromPosition, characterLength), mode, bits,
                            encoder.encoders[charsetEncoderIndex]);
                    }
                }

                public override String ToString()
                {
                    var result = new StringBuilder();
                    result.Append(mode).Append('(');
                    if (mode == Mode.ECI)
                    {
                        result.Append(encoder.encoders[charsetEncoderIndex].WebName.ToUpper());
                    }
                    else
                    {
                        result.Append(makePrintable(encoder.stringToEncode.Substring(fromPosition, characterLength)));
                    }
                    result.Append(')');
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
