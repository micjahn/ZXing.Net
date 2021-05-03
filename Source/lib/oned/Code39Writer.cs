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

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// This object renders a CODE39 code as a <see cref="BitMatrix"/>.
    /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
    /// </summary>
    public sealed class Code39Writer : OneDimensionalCodeWriter
    {
        private static readonly IList<BarcodeFormat> supportedWriteFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_39 };

        /// <summary>
        /// returns supported formats
        /// </summary>
        protected override IList<BarcodeFormat> SupportedWriteFormats
        {
            get { return supportedWriteFormats; }
        }

        /// <summary>
        /// Encode the contents to byte array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public override bool[] encode(String contents)
        {
            int length = contents.Length;
            if (length > 80)
            {
                throw new ArgumentException(
                    "Requested contents should be less than 80 digits long, but got " + length);
            }
            for (int i = 0; i < length; i++)
            {
                int indexInString = Code39Reader.ALPHABET_STRING.IndexOf(contents[i]);
                if (indexInString < 0)
                {
                    var unencodable = contents[i];
                    contents = tryToConvertToExtendedMode(contents);
                    if (contents == null)
                        throw new ArgumentException("Requested content contains a non-encodable character: '" + unencodable + "'");
                    length = contents.Length;
                    if (length > 80)
                    {
                        throw new ArgumentException(
                            "Requested contents should be less than 80 digits long, but got " + length + " (extended full ascii mode)");
                    }
                    break;
                }
            }

            int[] widths = new int[9];
            int codeWidth = 24 + 1 + (13 * length);
            var result = new bool[codeWidth];
            toIntArray(Code39Reader.ASTERISK_ENCODING, widths);
            int pos = appendPattern(result, 0, widths, true);
            int[] narrowWhite = {1};
            pos += appendPattern(result, pos, narrowWhite, false);
            //append next character to byte matrix
            for (int i = 0; i < length; i++)
            {
                int indexInString = Code39Reader.ALPHABET_STRING.IndexOf(contents[i]);
                toIntArray(Code39Reader.CHARACTER_ENCODINGS[indexInString], widths);
                pos += appendPattern(result, pos, widths, true);
                pos += appendPattern(result, pos, narrowWhite, false);
            }
            toIntArray(Code39Reader.ASTERISK_ENCODING, widths);
            appendPattern(result, pos, widths, true);
            return result;
        }

        private static void toIntArray(int a, int[] toReturn)
        {
            for (int i = 0; i < 9; i++)
            {
                int temp = a & (1 << (8 - i));
                toReturn[i] = temp == 0 ? 1 : 2;
            }
        }

        private static String tryToConvertToExtendedMode(String contents)
        {
            var length = contents.Length;
            var extendedContent = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var character = (int) contents[i];
                switch (character)
                {
                    case 0:
                        extendedContent.Append("%U");
                        break;
                    case 32:
                        extendedContent.Append(" ");
                        break;
                    case 45:
                        extendedContent.Append("-");
                        break;
                    case 46:
                        extendedContent.Append(".");
                        break;
                    case 64:
                        extendedContent.Append("%V");
                        break;
                    case 96:
                        extendedContent.Append("%W");
                        break;
                    default:
                        if (character <= 26)
                        {
                            extendedContent.Append("$");
                            extendedContent.Append((char) ('A' + (character - 1)));
                        }
                        else if (character < 32)
                        {
                            extendedContent.Append("%");
                            extendedContent.Append((char) ('A' + (character - 27)));
                        }
                        else if (character <= ',' || character == '/' || character == ':')
                        {
                            extendedContent.Append("/");
                            extendedContent.Append((char) ('A' + (character - 33)));
                        }
                        else if (character <= '9')
                        {
                            extendedContent.Append((char) ('0' + (character - 48)));
                        }
                        else if (character <= '?')
                        {
                            extendedContent.Append("%");
                            extendedContent.Append((char) ('F' + (character - 59)));
                        }
                        else if (character <= 'Z')
                        {
                            extendedContent.Append((char) ('A' + (character - 65)));
                        }
                        else if (character <= '_')
                        {
                            extendedContent.Append("%");
                            extendedContent.Append((char) ('K' + (character - 91)));
                        }
                        else if (character <= 'z')
                        {
                            extendedContent.Append("+");
                            extendedContent.Append((char) ('A' + (character - 97)));
                        }
                        else if (character <= 127)
                        {
                            extendedContent.Append("%");
                            extendedContent.Append((char) ('P' + (character - 123)));
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
            }

            return extendedContent.ToString();
        }
    }
}