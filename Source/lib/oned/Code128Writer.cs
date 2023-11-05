/*
 * Copyright 2010 ZXing authors
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

namespace ZXing.OneD
{
    using System;
    using System.Collections.Generic;
    using ZXing.Common;

    /// <summary>
    /// This object renders a CODE128 code as a <see cref="BitMatrix" />.
    /// 
    /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
    /// </summary>
    public sealed class Code128Writer : OneDimensionalCodeWriter
    {
        internal const int CODE_START_A = 103;
        internal const int CODE_START_B = 104;
        internal const int CODE_START_C = 105;
        internal const int CODE_CODE_A = 101;
        internal const int CODE_CODE_B = 100;
        internal const int CODE_CODE_C = 99;
        internal const int CODE_STOP = 106;

        // Dummy characters used to specify control characters in input
        internal const char ESCAPE_FNC_1 = '\u00f1';
        internal const char ESCAPE_FNC_2 = '\u00f2';
        internal const char ESCAPE_FNC_3 = '\u00f3';
        internal const char ESCAPE_FNC_4 = '\u00f4';

        internal const int CODE_FNC_1 = 102; // Code A, Code B, Code C
        internal const int CODE_FNC_2 = 97; // Code A, Code B
        internal const int CODE_FNC_3 = 96; // Code A, Code B
        internal const int CODE_FNC_4_A = 101; // Code A
        internal const int CODE_FNC_4_B = 100; // Code B

        // Results of minimal lookahead for code C
        private enum CType
        {
            UNCODABLE,
            ONE_DIGIT,
            TWO_DIGITS,
            FNC_1
        }

        private bool forceCodesetB;

        private static readonly IList<BarcodeFormat> supportedWriteFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 };

        /// <summary>
        /// returns supported formats
        /// </summary>
        protected override IList<BarcodeFormat> SupportedWriteFormats
        {
            get { return supportedWriteFormats; }
        }

        /// <summary>
        /// Encode the contents following specified format.
        /// </summary>
        public override bool[] encode(String contents)
        {
            return encode(contents, null);
        }

        /// <summary>
        /// starts encoding
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        protected override bool[] encode(String contents, IDictionary<EncodeHintType, object> hints)
        {
            if (IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.GS1_FORMAT))
            {
                // append the FNC1 character at the first position if not already present
                if (!string.IsNullOrEmpty(contents) && contents[0] != ESCAPE_FNC_1)
                    contents = ESCAPE_FNC_1 + contents;
            }

            int forcedCodeSet = check(contents, hints);
            bool hasCompactionHint = IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.CODE128_COMPACT);

            return hasCompactionHint ? new MinimalEncoder().encode(contents) : encodeFast(contents, forcedCodeSet);
        }

        private int check(String contents, IDictionary<EncodeHintType, object> hints)
        {
            int forcedCodeSet = -1;

            if (hints != null)
            {
                forceCodesetB = IDictionaryExtensions.IsBooleanFlagSet(hints, EncodeHintType.CODE128_FORCE_CODESET_B);

                // Check for forced code set hint.
                if (hints.ContainsKey(EncodeHintType.FORCE_CODE_SET))
                {
                    var hintCodeset = hints[EncodeHintType.FORCE_CODE_SET];
                    if (hintCodeset != null)
                    {
                        String codeSetHint = hintCodeset.ToString();
                        switch (codeSetHint)
                        {
                            case "A":
                                forcedCodeSet = CODE_CODE_A;
                                break;
                            case "B":
                                forcedCodeSet = CODE_CODE_B;
                                break;
                            case "C":
                                forcedCodeSet = CODE_CODE_C;
                                break;
                            default:
                                throw new ArgumentException("Unsupported code set hint: " + codeSetHint);
                        }
                    }
                }
            }

            int length = contents.Length;
            // Check content
            for (int i = 0; i < length; i++)
            {
                char c = contents[i];
                // check for non ascii characters that are not special GS1 characters
                switch (c)
                {
                    case ESCAPE_FNC_1:
                    case ESCAPE_FNC_2:
                    case ESCAPE_FNC_3:
                    case ESCAPE_FNC_4:
                        break;
                    // non ascii characters
                    default:
                        if (c > 127)
                            // no full Latin-1 character set available at the moment
                            // shift and manual code change are not supported
                            throw new ArgumentException("Bad character in input: ASCII value=" + (int)c);
                        break;
                }
                // check characters for compatibility with forced code set
                switch (forcedCodeSet)
                {
                    case CODE_CODE_A:
                        // allows no ascii above 95 (no lower caps, no special symbols)
                        if (c > 95 && c <= 127)
                        {
                            throw new ArgumentException("Bad character in input for forced code set A: ASCII value=" + (int)c);
                        }
                        break;
                    case CODE_CODE_B:
                        // allows no ascii below 32 (terminal symbols)
                        if (c <= 32)
                        {
                            throw new ArgumentException("Bad character in input for forced code set B: ASCII value=" + (int)c);
                        }
                        break;
                    case CODE_CODE_C:
                        // allows only numbers and no FNC 2/3/4
                        if (c < 48 || (c > 57 && c <= 127) || c == ESCAPE_FNC_2 || c == ESCAPE_FNC_3 || c == ESCAPE_FNC_4)
                        {
                            throw new ArgumentException("Bad character in input for forced code set C: ASCII value=" + (int)c);
                        }
                        break;
                }

            }
            return forcedCodeSet;
        }

        private bool[] encodeFast(String contents, int forcedCodeSet)
        {
            int length = contents.Length;
            var patterns = new List<int[]>(); // temporary storage for patterns
            int checkSum = 0;
            int checkWeight = 1;
            int codeSet = 0; // selected code (CODE_CODE_B or CODE_CODE_C)
            int position = 0; // position in contents

            while (position < length)
            {
                //Select code to use
                int newCodeSet;
                if (forcedCodeSet == -1)
                {
                    newCodeSet = chooseCode(contents, position, codeSet);
                }
                else
                {
                    newCodeSet = forcedCodeSet;
                }

                //Get the pattern index
                int patternIndex;
                if (newCodeSet == codeSet)
                {
                    // Encode the current character
                    // First handle escapes
                    switch (contents[position])
                    {
                        case ESCAPE_FNC_1:
                            patternIndex = CODE_FNC_1;
                            break;
                        case ESCAPE_FNC_2:
                            patternIndex = CODE_FNC_2;
                            break;
                        case ESCAPE_FNC_3:
                            patternIndex = CODE_FNC_3;
                            break;
                        case ESCAPE_FNC_4:
                            if (newCodeSet == CODE_CODE_A)
                                patternIndex = CODE_FNC_4_A;
                            else
                                patternIndex = CODE_FNC_4_B;
                            break;
                        default:
                            // Then handle normal characters otherwise
                            switch (codeSet)
                            {
                                case CODE_CODE_A:
                                    patternIndex = contents[position] - ' ';
                                    if (patternIndex < 0)
                                    {
                                        // everything below a space character comes behind the underscore in the code patterns table
                                        patternIndex += '`';
                                    }
                                    break;
                                case CODE_CODE_B:
                                    patternIndex = contents[position] - ' ';
                                    break;
                                default:
                                    // CODE_CODE_C
                                    if (position + 1 == length)
                                    {
                                        // this is the last character, but the encoding is C, which always encodes two characers
                                        throw new ArgumentException("Bad number of characters for digit only encoding.");
                                    }
                                    patternIndex = Int32.Parse(contents.Substring(position, 2));
                                    position++; // Also incremented below
                                    break;
                            }
                            break;
                    }
                    position++;
                }
                else
                {
                    // Should we change the current code?
                    // Do we have a code set?
                    if (codeSet == 0)
                    {
                        // No, we don't have a code set
                        switch (newCodeSet)
                        {
                            case CODE_CODE_A:
                                patternIndex = CODE_START_A;
                                break;
                            case CODE_CODE_B:
                                patternIndex = CODE_START_B;
                                break;
                            default:
                                patternIndex = CODE_START_C;
                                break;
                        }
                    }
                    else
                    {
                        // Yes, we have a code set
                        patternIndex = newCodeSet;
                    }
                    codeSet = newCodeSet;
                }

                // Get the pattern
                patterns.Add(Code128Reader.CODE_PATTERNS[patternIndex]);

                // Compute checksum
                checkSum += patternIndex * checkWeight;
                if (position != 0)
                {
                    checkWeight++;
                }
            }
            return produceResult(patterns, checkSum);
        }

        internal static bool[] produceResult(List<int[]> patterns, int checkSum)
        {
            // Compute and append checksum
            checkSum %= 103;
            if (checkSum < 0)
            {
                throw new InvalidOperationException("Unable to compute a valid input checksum");
            }
            patterns.Add(Code128Reader.CODE_PATTERNS[checkSum]);

            // Append stop code
            patterns.Add(Code128Reader.CODE_PATTERNS[CODE_STOP]);

            // Compute code width
            int codeWidth = 0;
            foreach (int[] pattern in patterns)
            {
                foreach (int width in pattern)
                {
                    codeWidth += width;
                }
            }

            // Compute result
            var result = new bool[codeWidth];
            int pos = 0;
            foreach (int[] pattern in patterns)
            {
                pos += appendPattern(result, pos, pattern, true);
            }

            return result;
        }


        private static CType findCType(String value, int start)
        {
            int last = value.Length;
            if (start >= last)
            {
                return CType.UNCODABLE;
            }
            char c = value[start];
            if (c == ESCAPE_FNC_1)
            {
                return CType.FNC_1;
            }
            if (c < '0' || c > '9')
            {
                return CType.UNCODABLE;
            }
            if (start + 1 >= last)
            {
                return CType.ONE_DIGIT;
            }
            c = value[start + 1];
            if (c < '0' || c > '9')
            {
                return CType.ONE_DIGIT;
            }
            return CType.TWO_DIGITS;
        }

        private int chooseCode(String value, int start, int oldCode)
        {
            CType lookahead = findCType(value, start);
            if (lookahead == CType.ONE_DIGIT)
            {
                if (oldCode == CODE_CODE_A)
                {
                    return CODE_CODE_A;
                }
                return CODE_CODE_B;
            }
            if (lookahead == CType.UNCODABLE)
            {
                if (start < value.Length)
                {
                    var c = value[start];
                    if (c < ' ' || (oldCode == CODE_CODE_A && (c < '`' || (c >= ESCAPE_FNC_1 && c <= ESCAPE_FNC_4))))
                    {
                        // can continue in code A, encodes ASCII 0 to 95 or FNC1 to FNC4
                        return CODE_CODE_A;
                    }
                }
                return CODE_CODE_B; // no choice
            }
            if (oldCode == CODE_CODE_A && lookahead == CType.FNC_1)
            {
                return CODE_CODE_A;
            }
            if (oldCode == CODE_CODE_C)
            {
                // can continue in code C
                return CODE_CODE_C;
            }
            if (oldCode == CODE_CODE_B)
            {
                if (lookahead == CType.FNC_1)
                {
                    return CODE_CODE_B; // can continue in code B
                }
                // Seen two consecutive digits, see what follows
                lookahead = findCType(value, start + 2);
                if (lookahead == CType.UNCODABLE || lookahead == CType.ONE_DIGIT)
                {
                    return CODE_CODE_B; // not worth switching now
                }
                if (lookahead == CType.FNC_1)
                {
                    // two digits, then FNC_1...
                    lookahead = findCType(value, start + 3);
                    if (lookahead == CType.TWO_DIGITS)
                    {
                        // then two more digits, switch
                        return forceCodesetB ? CODE_CODE_B : CODE_CODE_C;
                    }
                    else
                    {
                        return CODE_CODE_B; // otherwise not worth switching
                    }
                }
                // At this point, there are at least 4 consecutive digits.
                // Look ahead to choose whether to switch now or on the next round.
                int index = start + 4;
                while ((lookahead = findCType(value, index)) == CType.TWO_DIGITS)
                {
                    index += 2;
                }
                if (lookahead == CType.ONE_DIGIT)
                {
                    // odd number of digits, switch later
                    return CODE_CODE_B;
                }
                return forceCodesetB ? CODE_CODE_B : CODE_CODE_C; // even number of digits, switch now
            }
            // Here oldCode == 0, which means we are choosing the initial code
            if (lookahead == CType.FNC_1)
            {
                // ignore FNC_1
                lookahead = findCType(value, start + 1);
            }
            if (lookahead == CType.TWO_DIGITS)
            {
                // at least two digits, start in code C
                return forceCodesetB ? CODE_CODE_B : CODE_CODE_C;
            }
            return CODE_CODE_B;
        }
    }

    /**
     * Encodes minimally using Divide-And-Conquer with Memoization
     **/
    internal sealed class MinimalEncoder
    {
        private enum Charset { A, B, C, NONE };
        private enum Latch { A, B, C, SHIFT, NONE };

        static String A = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_\u0000\u0001\u0002" +
                            "\u0003\u0004\u0005\u0006\u0007\u0008\u0009\n\u000B\u000C\r\u000E\u000F\u0010\u0011" +
                            "\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F" +
                            "\u00FF";
        static String B = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqr" +
                                "stuvwxyz{|}~\u007F\u00FF";

        private static int CODE_SHIFT = 98;

        private int[][] memoizedCost;
        private Latch[][] minPath;

        public bool[] encode(String contents)
        {
            memoizedCost = new int[4][];
            for (var x = 0; x < memoizedCost.Length; x++)
                memoizedCost[x] = new int[contents.Length];
            minPath = new Latch[4][];
            for (var x = 0; x < minPath.Length; x++)
                minPath[x] = new Latch[contents.Length];

            encode(contents, Charset.NONE, 0);

            var patterns = new List<int[]>();
            var checkSum = new int[] { 0 };
            var checkWeight = new int[] { 1 };
            int length = contents.Length;
            Charset charset = Charset.NONE;
            for (int i = 0; i < length; i++)
            {
                Latch latch = minPath[(int)charset][i];
                switch (latch)
                {
                    case Latch.A:
                        charset = Charset.A;
                        addPattern(patterns, i == 0 ? Code128Writer.CODE_START_A : Code128Writer.CODE_CODE_A, checkSum, checkWeight, i);
                        break;
                    case Latch.B:
                        charset = Charset.B;
                        addPattern(patterns, i == 0 ? Code128Writer.CODE_START_B : Code128Writer.CODE_CODE_B, checkSum, checkWeight, i);
                        break;
                    case Latch.C:
                        charset = Charset.C;
                        addPattern(patterns, i == 0 ? Code128Writer.CODE_START_C : Code128Writer.CODE_CODE_C, checkSum, checkWeight, i);
                        break;
                    case Latch.SHIFT:
                        addPattern(patterns, CODE_SHIFT, checkSum, checkWeight, i);
                        break;
                }
                if (charset == Charset.C)
                {
                    if (contents[i] == Code128Writer.ESCAPE_FNC_1)
                    {
                        addPattern(patterns, Code128Writer.CODE_FNC_1, checkSum, checkWeight, i);
                    }
                    else
                    {
                        addPattern(patterns, Int32.Parse(contents.Substring(i, 2)), checkSum, checkWeight, i);
                        //assert i +1 < length; //the algorithm never leads to a single trailing digit in character set C
                        if (i + 1 < length)
                        {
                            i++;
                        }
                    }
                }
                else
                { // charset A or B
                    int patternIndex;
                    switch (contents[i])
                    {
                        case Code128Writer.ESCAPE_FNC_1:
                            patternIndex = Code128Writer.CODE_FNC_1;
                            break;
                        case Code128Writer.ESCAPE_FNC_2:
                            patternIndex = Code128Writer.CODE_FNC_2;
                            break;
                        case Code128Writer.ESCAPE_FNC_3:
                            patternIndex = Code128Writer.CODE_FNC_3;
                            break;
                        case Code128Writer.ESCAPE_FNC_4:
                            if ((charset == Charset.A && latch != Latch.SHIFT) ||
                                (charset == Charset.B && latch == Latch.SHIFT))
                            {
                                patternIndex = Code128Writer.CODE_FNC_4_A;
                            }
                            else
                            {
                                patternIndex = Code128Writer.CODE_FNC_4_B;
                            }
                            break;
                        default:
                            patternIndex = contents[i] - ' ';
                            break;
                    }
                    if ((charset == Charset.A && latch != Latch.SHIFT) ||
                        (charset == Charset.B && latch == Latch.SHIFT))
                    {
                        if (patternIndex < 0)
                        {
                            patternIndex += '`';
                        }
                    }
                    addPattern(patterns, patternIndex, checkSum, checkWeight, i);
                }
            }
            memoizedCost = null;
            minPath = null;
            return Code128Writer.produceResult(patterns, checkSum[0]);
        }

        private static void addPattern(List<int[]> patterns,
                                      int patternIndex,
                                      int[] checkSum,
                                      int[] checkWeight,
                                      int position)
        {
            patterns.Add(Code128Reader.CODE_PATTERNS[patternIndex]);
            if (position != 0)
            {
                checkWeight[0]++;
            }
            checkSum[0] += patternIndex * checkWeight[0];
        }

        private static bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool canEncode(String contents, Charset charset, int position)
        {
            char c = contents[position];
            switch (charset)
            {
                case Charset.A:
                    return c == Code128Writer.ESCAPE_FNC_1 ||
                               c == Code128Writer.ESCAPE_FNC_2 ||
                               c == Code128Writer.ESCAPE_FNC_3 ||
                               c == Code128Writer.ESCAPE_FNC_4 ||
                               A.IndexOf(c) >= 0;
                case Charset.B:
                    return c == Code128Writer.ESCAPE_FNC_1 ||
                               c == Code128Writer.ESCAPE_FNC_2 ||
                               c == Code128Writer.ESCAPE_FNC_3 ||
                               c == Code128Writer.ESCAPE_FNC_4 ||
                               B.IndexOf(c) >= 0;
                case Charset.C:
                    return c == Code128Writer.ESCAPE_FNC_1 ||
                               (position + 1 < contents.Length &&
                                isDigit(c) &&
                                isDigit(contents[position + 1]));
                default:
                    return false;
            }
        }

        /**
         * Encode the string starting at position position starting with the character set charset
         **/
        private int encode(String contents, Charset charset, int position)
        {
            // assert position<contents.Length;
            int mCost = memoizedCost[(int)charset][position];
            if (mCost > 0)
            {
                return mCost;
            }

            int minCost = Int32.MaxValue;
            Latch minLatch = Latch.NONE;
            bool atEnd = position + 1 >= contents.Length;

            var sets = new Charset[] { Charset.A, Charset.B };
            for (int i = 0; i <= 1; i++)
            {
                if (canEncode(contents, sets[i], position))
                {
                    int cost = 1;
                    Latch latch = Latch.NONE;
                    if (charset != sets[i])
                    {
                        cost++;
                        latch = (Latch)Enum.Parse(typeof(Latch), sets[i].ToString(), true);
                    }
                    if (!atEnd)
                    {
                        cost += encode(contents, sets[i], position + 1);
                    }
                    if (cost < minCost)
                    {
                        minCost = cost;
                        minLatch = latch;
                    }
                    cost = 1;
                    if (charset == sets[(i + 1) % 2])
                    {
                        cost++;
                        latch = Latch.SHIFT;
                        if (!atEnd)
                        {
                            cost += encode(contents, charset, position + 1);
                        }
                        if (cost < minCost)
                        {
                            minCost = cost;
                            minLatch = latch;
                        }
                    }
                }
            }
            if (canEncode(contents, Charset.C, position))
            {
                int cost = 1;
                Latch latch = Latch.NONE;
                if (charset != Charset.C)
                {
                    cost++;
                    latch = Latch.C;
                }
                int advance = contents[position] == Code128Writer.ESCAPE_FNC_1 ? 1 : 2;
                if (position + advance < contents.Length)
                {
                    cost += encode(contents, Charset.C, position + advance);
                }
                if (cost < minCost)
                {
                    minCost = cost;
                    minLatch = latch;
                }
            }
            if (minCost == Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Bad character in input: ASCII value=" + (int)contents[position]);
            }
            memoizedCost[(int)charset][position] = minCost;
            minPath[(int)charset][position] = minLatch;
            return minCost;
        }
    }
}