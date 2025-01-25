/*
 * Copyright (C) 2010 ZXing authors
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

namespace ZXing.Common
{
    /// <summary>
    /// Common string-related functions.
    /// </summary>
    /// <author>Sean Owen</author>
    /// <author>Alex Dupre</author>
    public static class StringUtils
    {
        /// <summary>
        /// default encoding of the current platform (name)
        /// </summary>
        public static readonly String PLATFORM_DEFAULT_ENCODING;
        /// <summary>
        /// default encoding of the current platform (type)
        /// </summary>
        public static readonly Encoding PLATFORM_DEFAULT_ENCODING_T;
        /// <summary>
        /// Shift JIS encoding if available
        /// </summary>
        public static readonly Encoding SHIFT_JIS_ENCODING;
        /// <summary>
        /// GB 2312 encoding if available
        /// </summary>
        public static readonly Encoding GB2312_ENCODING;
        /// <summary>
        /// ECU JP encoding if available
        /// </summary>
        public static readonly Encoding EUC_JP_ENCODING;
        /// <summary>
        /// ISO8859-1 encoding if available
        /// </summary>
        public static readonly Encoding ISO88591_ENCODING;
        private static readonly bool ASSUME_SHIFT_JIS;
        /// <summary>
        /// JIS_IS is supported or not
        /// </summary>
        public static readonly bool JIS_IS_SUPPORTED;
        /// <summary>
        /// EUC_JP is supported or not
        /// </summary>
        public static readonly bool EUC_JP_IS_SUPPORTED;

        // Retained for ABI compatibility with earlier versions
        /// <summary>
        /// SJIS
        /// </summary>
        public const String SHIFT_JIS = "SJIS";
        /// <summary>
        /// GB2312
        /// </summary>
        public const String GB2312 = "GB2312";
        /// <summary>
        /// EUC-JP
        /// </summary>
        public const String EUC_JP = "EUC-JP";
        /// <summary>
        /// UTF-8
        /// </summary>
        public const String UTF8 = "UTF-8";
        /// <summary>
        /// ISO-8859-1
        /// </summary>
        public const String ISO88591 = "ISO-8859-1";

        static StringUtils()
        {
#if (WINDOWS_PHONE || SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3)
            PLATFORM_DEFAULT_ENCODING = UTF8;
            PLATFORM_DEFAULT_ENCODING_T = Encoding.UTF8;
#else
            PLATFORM_DEFAULT_ENCODING = Encoding.Default.WebName.ToUpper();
            PLATFORM_DEFAULT_ENCODING_T = Encoding.Default;
#endif
            SHIFT_JIS_ENCODING = CharacterSetECI.getEncoding(SHIFT_JIS);
            GB2312_ENCODING = CharacterSetECI.getEncoding(GB2312) ?? PLATFORM_DEFAULT_ENCODING_T;
            EUC_JP_ENCODING = CharacterSetECI.getEncoding(EUC_JP) ?? PLATFORM_DEFAULT_ENCODING_T;
            ISO88591_ENCODING = CharacterSetECI.getEncoding(ISO88591) ?? PLATFORM_DEFAULT_ENCODING_T;
            JIS_IS_SUPPORTED = true;
            if (SHIFT_JIS_ENCODING == null)
            {
                SHIFT_JIS_ENCODING = PLATFORM_DEFAULT_ENCODING_T;
                JIS_IS_SUPPORTED = false;
            }
            EUC_JP_IS_SUPPORTED = true;
            if (EUC_JP_ENCODING == null)
            {
                EUC_JP_ENCODING = PLATFORM_DEFAULT_ENCODING_T;
                EUC_JP_IS_SUPPORTED = false;
            }
            ASSUME_SHIFT_JIS = JIS_IS_SUPPORTED || EUC_JP_IS_SUPPORTED;
        }

        /// <summary>
        /// Guesses the encoding.
        /// </summary>
        /// <param name="bytes">bytes encoding a string, whose encoding should be guessed</param>
        /// <param name="hints">decode hints if applicable</param>
        /// <return> name of guessed encoding; at the moment will only guess one of:
        /// "SJIS", "UTF8", "ISO8859_1", or the platform default encoding if none
        /// of these can possibly be correct</return>
        public static String guessEncoding(byte[] bytes, IDictionary<DecodeHintType, object> hints)
        {
            var c = guessCharset(bytes, hints);
            if (c == SHIFT_JIS_ENCODING && SHIFT_JIS_ENCODING != null)
            {
                return "SJIS";
            }
            return c.WebName.ToUpper();
        }

        /// <summary></summary>
        /// <param name="bytes">bytes encoding a string, whose encoding should be guessed</param>
        /// <param name="hints">decode hints if applicable</param>
        /// <returns>Charset of guessed encoding; at the moment will only guess one of:
        ///  {@link #SHIFT_JIS_CHARSET}, {@link StandardCharsets#UTF_8},
        ///  {@link StandardCharsets#ISO_8859_1}, {@link StandardCharsets#UTF_16},
        ///  or the platform default encoding if
        ///  none of these can possibly be correct</returns>
        public static Encoding guessCharset(byte[] bytes, IDictionary<DecodeHintType, object> hints)
        {
            if (hints != null && hints.ContainsKey(DecodeHintType.CHARACTER_SET))
            {
                String characterSet = (String)hints[DecodeHintType.CHARACTER_SET];
                if (characterSet != null)
                {
                    var encoding = CharacterSetECI.getEncoding(characterSet);
                    if (encoding != null)
                        return encoding;
                }
            }

            // First try UTF-16, assuming anything with its BOM is UTF-16
            if (bytes.Length > 2 &&
                ((bytes[0] == (byte)0xFE && bytes[1] == (byte)0xFF) ||
                 (bytes[0] == (byte)0xFF && bytes[1] == (byte)0xFE)))
            {
                return Encoding.Unicode;
            }

            // For now, merely tries to distinguish ISO-8859-1, UTF-8 and Shift_JIS,
            // which should be by far the most common encodings.
            int length = bytes.Length;
            bool canBeISO88591 = true;
            bool canBeShiftJIS = JIS_IS_SUPPORTED;
            bool canBeUTF8 = true;
            int utf8BytesLeft = 0;
            int utf2BytesChars = 0;
            int utf3BytesChars = 0;
            int utf4BytesChars = 0;
            int sjisBytesLeft = 0;
            int sjisKatakanaChars = 0;
            int sjisCurKatakanaWordLength = 0;
            int sjisCurDoubleBytesWordLength = 0;
            int sjisMaxKatakanaWordLength = 0;
            int sjisMaxDoubleBytesWordLength = 0;
            int isoHighOther = 0;

            bool utf8bom = bytes.Length > 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF;

            for (int i = 0;
                 i < length && (canBeISO88591 || canBeShiftJIS || canBeUTF8);
                 i++)
            {

                int value = bytes[i] & 0xFF;

                // UTF-8 stuff
                if (canBeUTF8)
                {
                    if (utf8BytesLeft > 0)
                    {
                        if ((value & 0x80) == 0)
                        {
                            canBeUTF8 = false;
                        }
                        else
                        {
                            utf8BytesLeft--;
                        }
                    }
                    else if ((value & 0x80) != 0)
                    {
                        if ((value & 0x40) == 0)
                        {
                            canBeUTF8 = false;
                        }
                        else
                        {
                            utf8BytesLeft++;
                            if ((value & 0x20) == 0)
                            {
                                utf2BytesChars++;
                            }
                            else
                            {
                                utf8BytesLeft++;
                                if ((value & 0x10) == 0)
                                {
                                    utf3BytesChars++;
                                }
                                else
                                {
                                    utf8BytesLeft++;
                                    if ((value & 0x08) == 0)
                                    {
                                        utf4BytesChars++;
                                    }
                                    else
                                    {
                                        canBeUTF8 = false;
                                    }
                                }
                            }
                        }
                    }
                }

                // ISO-8859-1 stuff
                if (canBeISO88591)
                {
                    if (value > 0x7F && value < 0xA0)
                    {
                        canBeISO88591 = false;
                    }
                    else if (value > 0x9F)
                    {
                        if (value < 0xC0 || value == 0xD7 || value == 0xF7)
                        {
                            isoHighOther++;
                        }
                    }
                }

                // Shift_JIS stuff
                if (canBeShiftJIS)
                {
                    if (sjisBytesLeft > 0)
                    {
                        if (value < 0x40 || value == 0x7F || value > 0xFC)
                        {
                            canBeShiftJIS = false;
                        }
                        else
                        {
                            sjisBytesLeft--;
                        }
                    }
                    else if (value == 0x80 || value == 0xA0 || value > 0xEF)
                    {
                        canBeShiftJIS = false;
                    }
                    else if (value > 0xA0 && value < 0xE0)
                    {
                        sjisKatakanaChars++;
                        sjisCurDoubleBytesWordLength = 0;
                        sjisCurKatakanaWordLength++;
                        if (sjisCurKatakanaWordLength > sjisMaxKatakanaWordLength)
                        {
                            sjisMaxKatakanaWordLength = sjisCurKatakanaWordLength;
                        }
                    }
                    else if (value > 0x7F)
                    {
                        sjisBytesLeft++;
                        //sjisDoubleBytesChars++;
                        sjisCurKatakanaWordLength = 0;
                        sjisCurDoubleBytesWordLength++;
                        if (sjisCurDoubleBytesWordLength > sjisMaxDoubleBytesWordLength)
                        {
                            sjisMaxDoubleBytesWordLength = sjisCurDoubleBytesWordLength;
                        }
                    }
                    else
                    {
                        //sjisLowChars++;
                        sjisCurKatakanaWordLength = 0;
                        sjisCurDoubleBytesWordLength = 0;
                    }
                }
            }

            if (canBeUTF8 && utf8BytesLeft > 0)
            {
                canBeUTF8 = false;
            }
            if (canBeShiftJIS && sjisBytesLeft > 0)
            {
                canBeShiftJIS = false;
            }

            // Easy -- if there is BOM or at least 1 valid not-single byte character (and no evidence it can't be UTF-8), done
            if (canBeUTF8 && (utf8bom || utf2BytesChars + utf3BytesChars + utf4BytesChars > 0))
            {
                return Encoding.UTF8;
            }
            // Easy -- if assuming Shift_JIS or >= 3 valid consecutive not-ascii characters (and no evidence it can't be), done
            if (canBeShiftJIS && (ASSUME_SHIFT_JIS || sjisMaxKatakanaWordLength >= 3 || sjisMaxDoubleBytesWordLength >= 3) && SHIFT_JIS_ENCODING != null)
            {
                return SHIFT_JIS_ENCODING;
            }
            // Distinguishing Shift_JIS and ISO-8859-1 can be a little tough for short words. The crude heuristic is:
            // - If we saw
            //   - only two consecutive katakana chars in the whole text, or
            //   - at least 10% of bytes that could be "upper" not-alphanumeric Latin1,
            // - then we conclude Shift_JIS, else ISO-8859-1
            if (canBeISO88591 && canBeShiftJIS && ISO88591_ENCODING != null && SHIFT_JIS_ENCODING != null)
            {
                return (sjisMaxKatakanaWordLength == 2 && sjisKatakanaChars == 2) || isoHighOther * 10 >= length
                    ? SHIFT_JIS_ENCODING : ISO88591_ENCODING;
            }

            // Otherwise, try in order ISO-8859-1, Shift JIS, UTF-8 and fall back to default platform encoding
            if (canBeISO88591 && ISO88591_ENCODING != null)
            {
                return ISO88591_ENCODING;
            }
            if (canBeShiftJIS && SHIFT_JIS_ENCODING != null)
            {
                return SHIFT_JIS_ENCODING;
            }
            if (canBeUTF8)
            {
                return Encoding.UTF8;
            }
            // Otherwise, we take a wild guess with platform encoding
            return PLATFORM_DEFAULT_ENCODING_T;
        }
    }
}