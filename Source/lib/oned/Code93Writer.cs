/*
 * Copyright 2015 ZXing authors
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

using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// This object renders a CODE93 code as a BitMatrix
    /// </summary>
    public class Code93Writer : OneDimensionalCodeWriter
    {
        public override BitMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
        {
            if (format != BarcodeFormat.CODE_93)
            {
                throw new ArgumentException("Can only encode CODE_93, but got " + format);
            }
            return base.encode(contents, format, width, height, hints);
        }

        /// <summary>
        /// </summary>
        /// <param name="contents">barcode contents to encode.It should not be encoded for extended characters.</param>
        /// <returns>a { @code bool[]} of horizontal pixels(false = white, true = black)</returns>
        public override bool[] encode(String contents)
        {
            contents = convertToExtended(contents);
            int length = contents.Length;
            if (length > 80)
            {
                throw new ArgumentException(
                    "Requested contents should be less than 80 digits long after converting to extended encoding, but got " + length);
            }

            //length of code + 2 start/stop characters + 2 checksums, each of 9 bits, plus a termination bar
            int codeWidth = (contents.Length + 2 + 2) * 9 + 1;

            bool[] result = new bool[codeWidth];

            //start character (*)
            int pos = appendPattern(result, 0, Code93Reader.ASTERISK_ENCODING);
            for (int i = 0; i < length; i++)
            {
                int indexInString = Code93Reader.ALPHABET_STRING.IndexOf(contents[i]);
                pos += appendPattern(result, pos, Code93Reader.CHARACTER_ENCODINGS[indexInString]);
            }

            //add two checksums
            int check1 = computeChecksumIndex(contents, 20);
            pos += appendPattern(result, pos, Code93Reader.CHARACTER_ENCODINGS[check1]);

            //append the contents to reflect the first checksum added
            contents += Code93Reader.ALPHABET_STRING[check1];

            int check2 = computeChecksumIndex(contents, 15);
            pos += appendPattern(result, pos, Code93Reader.CHARACTER_ENCODINGS[check2]);

            //end character (*)
            pos += appendPattern(result, pos, Code93Reader.ASTERISK_ENCODING);

            //termination bar (single black bar)
            result[pos] = true;

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="target">output to append to</param>
        /// <param name="pos">start position</param>
        /// <param name="pattern">pattern to append</param>
        /// <param name="startColor">unused</param>
        /// <returns>9</returns>
        [Obsolete("without replacement; intended as an internal-only method")]
        protected new static int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
        {
            foreach (var bit in pattern)
            {
                target[pos++] = bit != 0;
            }
            return 9;
        }

        private static int appendPattern(bool[] target, int pos, int a)
        {
            for (int i = 0; i < 9; i++)
            {
                int temp = a & (1 << (8 - i));
                target[pos + i] = temp != 0;
            }
            return 9;
        }

        private static int computeChecksumIndex(String contents, int maxWeight)
        {
            int weight = 1;
            int total = 0;

            for (int i = contents.Length - 1; i >= 0; i--)
            {
                int indexInString = Code93Reader.ALPHABET_STRING.IndexOf(contents[i]);
                total += indexInString * weight;
                if (++weight > maxWeight)
                {
                    weight = 1;
                }
            }
            return total % 47;
        }

        internal static String convertToExtended(String contents)
        {
            int length = contents.Length;
            var extendedContent = new System.Text.StringBuilder(length * 2);
            for (int i = 0; i < length; i++)
            {
                char character = contents[i];
                // ($)=a, (%)=b, (/)=c, (+)=d. see Code93Reader.ALPHABET_STRING
                if (character == 0)
                {
                    // NUL: (%)U
                    extendedContent.Append("bU");
                }
                else if (character <= 26)
                {
                    // SOH - SUB: ($)A - ($)Z
                    extendedContent.Append('a');
                    extendedContent.Append((char) ('A' + character - 1));
                }
                else if (character <= 31)
                {
                    // ESC - US: (%)A - (%)E
                    extendedContent.Append('b');
                    extendedContent.Append((char) ('A' + character - 27));
                }
                else if (character == ' ' || character == '$' || character == '%' || character == '+')
                {
                    // space $ % +
                    extendedContent.Append(character);
                }
                else if (character <= ',')
                {
                    // ! " # & ' ( ) * ,: (/)A - (/)L
                    extendedContent.Append('c');
                    extendedContent.Append((char) ('A' + character - '!'));
                }
                else if (character <= '9')
                {
                    extendedContent.Append(character);
                }
                else if (character == ':')
                {
                    // :: (/)Z
                    extendedContent.Append("cZ");
                }
                else if (character <= '?')
                {
                    // ; - ?: (%)F - (%)J
                    extendedContent.Append('b');
                    extendedContent.Append((char) ('F' + character - ';'));
                }
                else if (character == '@')
                {
                    // @: (%)V
                    extendedContent.Append("bV");
                }
                else if (character <= 'Z')
                {
                    // A - Z
                    extendedContent.Append(character);
                }
                else if (character <= '_')
                {
                    // [ - _: (%)K - (%)O
                    extendedContent.Append('b');
                    extendedContent.Append((char) ('K' + character - '['));
                }
                else if (character == '`')
                {
                    // `: (%)W
                    extendedContent.Append("bW");
                }
                else if (character <= 'z')
                {
                    // a - z: (*)A - (*)Z
                    extendedContent.Append('d');
                    extendedContent.Append((char) ('A' + character - 'a'));
                }
                else if (character <= 127)
                {
                    // { - DEL: (%)P - (%)T
                    extendedContent.Append('b');
                    extendedContent.Append((char) ('P' + character - '{'));
                }
                else
                {
                    throw new ArgumentException(
                        "Requested content contains a non-encodable character: '" + character + "'");
                }
            }
            return extendedContent.ToString();
        }
    }
}