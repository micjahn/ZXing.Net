/*
 * Copyright 2006 Jeremias Maerki in part, and ZXing Authors in part
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/*
 * This file has been modified from its original form in Barcode4J.
 */

using System;
#if (NET40 || NET45 || NET46 || NET47 || NET48 || NETFX_CORE || NETSTANDARD) && !NETSTANDARD1_0
using System.Numerics;
#else
using BigIntegerLibrary;
#endif
using System.Text;

using ZXing.Common;

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// PDF417 high-level encoder following the algorithm described in ISO/IEC 15438:2001(E) in
    /// annex P.
    /// </summary>
    internal static class PDF417HighLevelEncoder
    {
        /// <summary>
        /// code for Text compaction
        /// </summary>
        private const int TEXT_COMPACTION = 0;

        /// <summary>
        /// code for Byte compaction
        /// </summary>
        private const int BYTE_COMPACTION = 1;

        /// <summary>
        /// code for Numeric compaction
        /// </summary>
        private const int NUMERIC_COMPACTION = 2;

        /// <summary>
        /// Text compaction submode Alpha
        /// </summary>
        private const int SUBMODE_ALPHA = 0;

        /// <summary>
        /// Text compaction submode Lower
        /// </summary>
        private const int SUBMODE_LOWER = 1;

        /// <summary>
        /// Text compaction submode Mixed
        /// </summary>
        private const int SUBMODE_MIXED = 2;

        /// <summary>
        /// Text compaction submode Punctuation
        /// </summary>
        private const int SUBMODE_PUNCTUATION = 3;

        /// <summary>
        /// mode latch to Text Compaction mode
        /// </summary>
        private const int LATCH_TO_TEXT = 900;

        /// <summary>
        /// mode latch to Byte Compaction mode (number of characters NOT a multiple of 6)
        /// </summary>
        private const int LATCH_TO_BYTE_PADDED = 901;

        /// <summary>
        /// mode latch to Numeric Compaction mode
        /// </summary>
        private const int LATCH_TO_NUMERIC = 902;

        /// <summary>
        /// mode shift to Byte Compaction mode
        /// </summary>
        private const int SHIFT_TO_BYTE = 913;

        /// <summary>
        /// mode latch to Byte Compaction mode (number of characters a multiple of 6)
        /// </summary>
        private const int LATCH_TO_BYTE = 924;

        /// <summary>
        /// identifier for a user defined Extended Channel Interpretation (ECI)
        /// </summary>
        private const int ECI_USER_DEFINED = 925;

        /// <summary>
        /// identifier for a general purpose ECO format
        /// </summary>
        private const int ECI_GENERAL_PURPOSE = 926;

        /// <summary>
        /// identifier for an ECI of a character set of code page
        /// </summary>
        private const int ECI_CHARSET = 927;

        /// <summary>
        /// Raw code table for text compaction Mixed sub-mode
        /// </summary>
        private static readonly sbyte[] TEXT_MIXED_RAW =
           {
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 38, 13, 9, 44, 58,
            35, 45, 46, 36, 47, 43, 37, 42, 61, 94, 0, 32, 0, 0, 0
         };

        /// <summary>
        /// Raw code table for text compaction: Punctuation sub-mode
        /// </summary>
        private static readonly sbyte[] TEXT_PUNCTUATION_RAW =
           {
            59, 60, 62, 64, 91, 92, 93, 95, 96, 126, 33, 13, 9, 44, 58,
            10, 45, 46, 36, 47, 34, 124, 42, 40, 41, 63, 123, 125, 39, 0
         };

        private static readonly sbyte[] MIXED = new sbyte[128];
        private static readonly sbyte[] PUNCTUATION = new sbyte[128];

        internal static string DEFAULT_ENCODING_NAME = "ISO-8859-1";
        private static Encoding DEFAULT_ENCODING; // initialize with first call to getEncoder

        static PDF417HighLevelEncoder()
        {
            //Construct inverse lookups
            for (int idx = 0; idx < MIXED.Length; idx++)
                MIXED[idx] = -1;
            for (int i = 0; i < TEXT_MIXED_RAW.Length; i++)
            {
                sbyte b = TEXT_MIXED_RAW[i];
                if (b > 0)
                {
                    MIXED[b] = (sbyte)i;
                }
            }
            for (int idx = 0; idx < PUNCTUATION.Length; idx++)
                PUNCTUATION[idx] = -1;
            for (int i = 0; i < TEXT_PUNCTUATION_RAW.Length; i++)
            {
                sbyte b = TEXT_PUNCTUATION_RAW[i];
                if (b > 0)
                {
                    PUNCTUATION[b] = (sbyte)i;
                }
            }
        }

        /// <summary>
        /// Performs high-level encoding of a PDF417 message using the algorithm described in annex P
        /// of ISO/IEC 15438:2001(E). If byte compaction has been selected, then only byte compaction
        /// is used.
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="compaction">compaction mode to use</param>
        /// <param name="encoding">character encoding used to encode in default or byte compaction
        /// or null for default / not applicable</param>
        /// <param name="disableEci">if true, don't add an ECI segment for different encodings than default</param>
        /// <param name="autoECI">encode input minimally using multiple ECIs if needed
        /// If autoECI encoding is specified and additionally {@code encoding} is specified, then the encoder
        /// will use the specified {@link Charset}
        /// for any character that can be encoded by it, regardless
        /// if a different encoding would lead to a more compact encoding. When no {@code encoding} is specified
        /// then charsets will be chosen so that the byte representation is minimal.</param>
        /// <returns>the encoded message (the char values range from 0 to 928)</returns>
        internal static String encodeHighLevel(String msg, Compaction compaction, Encoding encoding, bool disableEci, bool autoECI)
        {
            if (encoding == null && !autoECI)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] > 255)
                    {
                        throw new WriterException("Non-encodable character detected: " + msg[i] + " (Unicode: " +
                            (int)msg[i] +
                            "). Consider specifying EncodeHintType.PDF417_AUTO_ECI and/or EncodeTypeHint.CHARACTER_SET.");
                    }
                }
            }
            encoding = ECIEncoderSet.Clone(encoding); // needed because of ECIEncoderSet.canEncode

            //the codewords 0..928 are encoded as Unicode characters
            var sb = new StringBuilder(msg.Length);

            ECIInput input;
            if (autoECI)
            {
                input = new MinimalECIInput(msg, encoding, -1);
            }
            else
            {
                input = new NoECIInput(msg);
                if (encoding != null && !disableEci && String.Compare(DEFAULT_ENCODING_NAME, encoding.WebName.ToUpper(), StringComparison.Ordinal) != 0)
                {
                    var eci = CharacterSetECI.getCharacterSetECI(encoding);
                    if (eci != null)
                    {
                        encodingECI(eci.Value, sb);
                    }
                }
            }

            int len = input.Length;
            int p = 0;
            int textSubMode = SUBMODE_ALPHA;

            // User selected encoding mode
            switch (compaction)
            {
                case Compaction.TEXT:
                    encodeText(input, p, len, sb, textSubMode);
                    break;
                case Compaction.BYTE:
                    if (autoECI)
                    {
                        encodeMultiECIBinary(input, 0, input.Length, TEXT_COMPACTION, sb);
                    }
                    else
                    {
                        var msgBytes = toBytes(msg, encoding);
                        encodeBinary(msgBytes, p, msgBytes.Length, BYTE_COMPACTION, sb);
                    }
                    break;
                case Compaction.NUMERIC:
                    sb.Append((char)LATCH_TO_NUMERIC);
                    encodeNumeric(input, p, len, sb);
                    break;
                default:
                    int encodingMode = TEXT_COMPACTION; //Default mode, see 4.4.2.1
                    while (p < len)
                    {
                        while (p < len && input.isECI(p))
                        {
                            encodingECI(input.getECIValue(p), sb);
                            p++;
                        }
                        if (p >= len)
                        {
                            break;
                        }
                        int n = determineConsecutiveDigitCount(input, p);
                        if (n >= 13)
                        {
                            sb.Append((char)LATCH_TO_NUMERIC);
                            encodingMode = NUMERIC_COMPACTION;
                            textSubMode = SUBMODE_ALPHA; //Reset after latch
                            encodeNumeric(input, p, n, sb);
                            p += n;
                        }
                        else
                        {
                            int t = determineConsecutiveTextCount(input, p);
                            if (t >= 5 || n == len)
                            {
                                if (encodingMode != TEXT_COMPACTION)
                                {
                                    sb.Append((char)LATCH_TO_TEXT);
                                    encodingMode = TEXT_COMPACTION;
                                    textSubMode = SUBMODE_ALPHA; //start with submode alpha after latch
                                }
                                textSubMode = encodeText(input, p, t, sb, textSubMode);
                                p += t;
                            }
                            else
                            {
                                int b = determineConsecutiveBinaryCount(input, p, autoECI ? null : encoding);
                                if (b == 0)
                                {
                                    b = 1;
                                }
                                var bytes = autoECI ? null : toBytes(input.subSequence(p, p + b), encoding);
                                if (((bytes == null && b == 1) || (bytes != null && bytes.Length == 1))
                                    && encodingMode == TEXT_COMPACTION)
                                {
                                    //Switch for one byte (instead of latch)
                                    if (autoECI)
                                    {
                                        encodeMultiECIBinary(input, p, 1, TEXT_COMPACTION, sb);
                                    }
                                    else
                                    {
                                        encodeBinary(toBytes(input.subSequence(p, p + b), encoding), 0, 1, TEXT_COMPACTION, sb);
                                    }
                                }
                                else
                                {
                                    //Mode latch performed by encodeBinary()
                                    if (autoECI)
                                    {
                                        encodeMultiECIBinary(input, p, p + b, encodingMode, sb);
                                    }
                                    else
                                    {
                                        encodeBinary(bytes, 0, bytes.Length, encodingMode, sb);
                                    }
                                    encodingMode = BYTE_COMPACTION;
                                    textSubMode = SUBMODE_ALPHA; //Reset after latch
                                }
                                p += b;
                            }
                        }
                    }
                    break;
            }

            return sb.ToString();
        }

        internal static Encoding getEncoder(Encoding encoding)
        {
            // Defer instantiating default Charset until needed, since it may be for an unsupported
            // encoding.
            if (encoding == null)
            {
                try
                {
                    encoding = DEFAULT_ENCODING ?? (DEFAULT_ENCODING = Encoding.GetEncoding(DEFAULT_ENCODING_NAME));
                }
                catch (Exception)
                {
                    // continue
                }
                if (encoding == null)
                {
                    // Fallbacks
                    try
                    {
                        // these .NET profiles support only UTF-8 and UTF-16 out-of-the-box
                        encoding = DEFAULT_ENCODING ?? (DEFAULT_ENCODING = Encoding.GetEncoding(StringUtils.UTF8));
                    }
                    catch (Exception uce)
                    {
                        throw new WriterException("No support for any encoding: " + DEFAULT_ENCODING_NAME, uce);
                    }
                }
            }

            return encoding;
        }

        private static byte[] toBytes(String msg, Encoding encoding)
        {
            return getEncoder(encoding).GetBytes(msg);
        }

        private static byte[] toBytes(char msg, Encoding encoding)
        {
            return getEncoder(encoding).GetBytes(new[] { msg });
        }

        /// <summary>
        /// Encode parts of the message using Text Compaction as described in ISO/IEC 15438:2001(E),
        /// chapter 4.4.2.
        ///
        /// <param name="input">the input</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="count">the number of characters to encode</param>
        /// <param name="sb">receives the encoded codewords</param>
        /// <param name="initialSubmode">should normally be SUBMODE_ALPHA</param>
        /// <returns>the text submode in which this method ends</returns>
        /// </summary>
        private static int encodeText(ECIInput input,
                                      int startpos,
                                      int count,
                                      StringBuilder sb,
                                      int initialSubmode)
        {
            var tmp = new StringBuilder(count);
            var submode = initialSubmode;
            var idx = 0;
            while (true)
            {
                if (input.isECI(startpos + idx))
                {
                    encodingECI(input.getECIValue(startpos + idx), sb);
                    idx++;
                }
                else
                {
                    char ch = input.charAt(startpos + idx);
                    switch (submode)
                    {
                        case SUBMODE_ALPHA:
                            if (isAlphaUpper(ch))
                            {
                                if (ch == ' ')
                                {
                                    tmp.Append((char)26); //space
                                }
                                else
                                {
                                    tmp.Append((char)(ch - 65));
                                }
                            }
                            else
                            {
                                if (isAlphaLower(ch))
                                {
                                    submode = SUBMODE_LOWER;
                                    tmp.Append((char)27); //ll
                                    continue;
                                }
                                else if (isMixed(ch))
                                {
                                    submode = SUBMODE_MIXED;
                                    tmp.Append((char)28); //ml
                                    continue;
                                }
                                else
                                {
                                    tmp.Append((char)29); //ps
                                    tmp.Append((char)PUNCTUATION[ch]);
                                    break;
                                }
                            }
                            break;
                        case SUBMODE_LOWER:
                            if (isAlphaLower(ch))
                            {
                                if (ch == ' ')
                                {
                                    tmp.Append((char)26); //space
                                }
                                else
                                {
                                    tmp.Append((char)(ch - 97));
                                }
                            }
                            else
                            {
                                if (isAlphaUpper(ch))
                                {
                                    tmp.Append((char)27); //as
                                    tmp.Append((char)(ch - 65));
                                    //space cannot happen here, it is also in "Lower"
                                    break;
                                }
                                else if (isMixed(ch))
                                {
                                    submode = SUBMODE_MIXED;
                                    tmp.Append((char)28); //ml
                                    continue;
                                }
                                else
                                {
                                    tmp.Append((char)29); //ps
                                    tmp.Append((char)PUNCTUATION[ch]);
                                    break;
                                }
                            }
                            break;
                        case SUBMODE_MIXED:
                            if (isMixed(ch))
                            {
                                tmp.Append((char)MIXED[ch]);
                            }
                            else
                            {
                                if (isAlphaUpper(ch))
                                {
                                    submode = SUBMODE_ALPHA;
                                    tmp.Append((char)28); //al
                                    continue;
                                }
                                else if (isAlphaLower(ch))
                                {
                                    submode = SUBMODE_LOWER;
                                    tmp.Append((char)27); //ll
                                    continue;
                                }
                                else
                                {
                                    if (startpos + idx + 1 < count)
                                    {
                                        if (!input.isECI(startpos + idx + 1) && isPunctuation(input.charAt(startpos + idx + 1)))
                                        {
                                            submode = SUBMODE_PUNCTUATION;
                                            tmp.Append((char)25); //pl
                                            continue;
                                        }
                                    }
                                    tmp.Append((char)29); //ps
                                    tmp.Append((char)PUNCTUATION[ch]);
                                }
                            }
                            break;
                        default: //SUBMODE_PUNCTUATION
                            if (isPunctuation(ch))
                            {
                                tmp.Append((char)PUNCTUATION[ch]);
                            }
                            else
                            {
                                submode = SUBMODE_ALPHA;
                                tmp.Append((char)29); //al
                                continue;
                            }
                            break;
                    }
                    idx++;
                    if (idx >= count)
                    {
                        break;
                    }
                }
            }
            char h = (char)0;
            int len = tmp.Length;
            for (int i = 0; i < len; i++)
            {
                bool odd = (i % 2) != 0;
                if (odd)
                {
                    h = (char)((h * 30) + tmp[i]);
                    sb.Append(h);
                }
                else
                {
                    h = tmp[i];
                }
            }
            if ((len % 2) != 0)
            {
                sb.Append((char)((h * 30) + 29)); //ps
            }
            return submode;
        }

        /// <summary>
        /// Encode all of the message using Byte Compaction as described in ISO/IEC 15438:2001(E)
        /// </summary>
        /// <param name="input">the input</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="count">the number of bytes to encode</param>
        /// <param name="startmode">the mode from which this method starts</param>
        /// <param name="sb">receives the encoded codewords</param>
        private static void encodeMultiECIBinary(ECIInput input,
                                          int startpos,
                                          int count,
                                          int startmode,
                                          StringBuilder sb)
        {
            int end = Math.Min(startpos + count, input.Length);
            int localStart = startpos;
            while (true)
            {
                //encode all leading ECIs and advance localStart
                while (localStart < end && input.isECI(localStart))
                {
                    encodingECI(input.getECIValue(localStart), sb);
                    localStart++;
                }
                int localEnd = localStart;
                //advance end until before the next ECI
                while (localEnd < end && !input.isECI(localEnd))
                {
                    localEnd++;
                }

                int localCount = localEnd - localStart;
                if (localCount <= 0)
                {
                    //done
                    break;
                }
                else
                {
                    //encode the segment
                    encodeBinary(subBytes(input, localStart, localEnd),
                        0, localCount, localStart == startpos ? startmode : BYTE_COMPACTION, sb);
                    localStart = localEnd;
                }
            }
        }

        static byte[] subBytes(ECIInput input, int start, int end)
        {
            int count = end - start;
            byte[] result = new byte[count];
            for (int i = start; i < end; i++)
            {
                result[i - start] = (byte)(input.charAt(i) & 0xff);
            }
            return result;
        }

        /// <summary>
        /// Encode parts of the message using Byte Compaction as described in ISO/IEC 15438:2001(E),
        /// chapter 4.4.3. The Unicode characters will be converted to binary using the cp437
        /// codepage.
        ///
        /// <param name="bytes">the message converted to a byte array</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="count">the number of bytes to encode</param>
        /// <param name="startmode">the mode from which this method starts</param>
        /// <param name="sb">receives the encoded codewords</param>
        /// </summary>
        private static void encodeBinary(byte[] bytes,
                                         int startpos,
                                         int count,
                                         int startmode,
                                         StringBuilder sb)
        {
            if (count == 1 && startmode == TEXT_COMPACTION)
            {
                sb.Append((char)SHIFT_TO_BYTE);
            }
            else
            {
                if ((count % 6) == 0)
                {
                    sb.Append((char)LATCH_TO_BYTE);
                }
                else
                {
                    sb.Append((char)LATCH_TO_BYTE_PADDED);
                }
            }

            int idx = startpos;
            // Encode sixpacks
            if (count >= 6)
            {
                char[] chars = new char[5];
                while ((startpos + count - idx) >= 6)
                {
                    long t = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        t <<= 8;
                        t += bytes[idx + i] & 0xff;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        chars[i] = (char)(t % 900);
                        t /= 900;
                    }
                    for (int i = chars.Length - 1; i >= 0; i--)
                    {
                        sb.Append(chars[i]);
                    }
                    idx += 6;
                }
            }
            //Encode rest (remaining n<5 bytes if any)
            for (int i = idx; i < startpos + count; i++)
            {
                int ch = bytes[i] & 0xff;
                sb.Append((char)ch);
            }
        }

        private static void encodeNumeric(ECIInput input, int startpos, int count, StringBuilder sb)
        {
#if (NET40 || NET45 || NET46 || NET47 || NET48 || NETFX_CORE || NETSTANDARD) && !NETSTANDARD1_0
            int idx = 0;
            StringBuilder tmp = new StringBuilder(count / 3 + 1);
            BigInteger num900 = new BigInteger(900);
            BigInteger num0 = new BigInteger(0);
            while (idx < count)
            {
                tmp.Length = 0;
                int len = Math.Min(44, count - idx);
                var part = "1" + input.subSequence(startpos + idx, startpos + idx + len);
                var bigint = BigInteger.Parse(part);
                do
                {
                    BigInteger c = bigint % num900;
                    tmp.Append((char)c);
                    bigint = BigInteger.Divide(bigint, num900);
                } while (!bigint.Equals(num0));

                //Reverse temporary string
                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    sb.Append(tmp[i]);
                }
                idx += len;
            }
#else
            int idx = 0;
            StringBuilder tmp = new StringBuilder(count / 3 + 1);
            BigInteger num900 = new BigInteger(900);
            BigInteger num0 = new BigInteger(0);
            while (idx < count)
            {
                tmp.Length = 0;
                int len = Math.Min(44, count - idx);
                var part = "1" + input.subSequence(startpos + idx, startpos + idx + len);
                var bigint = BigInteger.Parse(part);
                do
                {
                    BigInteger c = BigInteger.Modulo(bigint, num900);
                    tmp.Append((char)c.GetHashCode());
                    bigint = BigInteger.Division(bigint, num900);
                } while (!bigint.Equals(num0));

                //Reverse temporary string
                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    sb.Append(tmp[i]);
                }
                idx += len;
            }
#endif
        }


        private static bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool isAlphaUpper(char ch)
        {
            return ch == ' ' || (ch >= 'A' && ch <= 'Z');
        }

        private static bool isAlphaLower(char ch)
        {
            return ch == ' ' || (ch >= 'a' && ch <= 'z');
        }

        private static bool isMixed(char ch)
        {
            return MIXED[ch] != -1;
        }

        private static bool isPunctuation(char ch)
        {
            return PUNCTUATION[ch] != -1;
        }

        private static bool isText(char ch)
        {
            return ch == '\t' || ch == '\n' || ch == '\r' || (ch >= 32 && ch <= 126);
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using numeric compaction.
        ///
        /// <param name="input">the input</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>the requested character count</returns>
        /// </summary>
        private static int determineConsecutiveDigitCount(ECIInput input, int startpos)
        {
            int count = 0;
            int len = input.Length;
            int idx = startpos;
            if (idx < len)
            {
                while (idx < len && !input.isECI(idx) && isDigit(input.charAt(idx)))
                {
                    count++;
                    idx++;
                }
            }
            return count;
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using text compaction.
        ///
        /// <param name="input">the input</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>the requested character count</returns>
        /// </summary>
        private static int determineConsecutiveTextCount(ECIInput input, int startpos)
        {
            int len = input.Length;
            int idx = startpos;
            while (idx < len)
            {
                int numericCount = 0;
                while (numericCount < 13 && idx < len && !input.isECI(idx) && isDigit(input.charAt(idx)))
                {
                    numericCount++;
                    idx++;
                }
                if (numericCount >= 13)
                {
                    return idx - startpos - numericCount;
                }
                if (numericCount > 0)
                {
                    //Heuristic: All text-encodable chars or digits are binary encodable
                    continue;
                }

                //Check if character is encodable
                if (input.isECI(idx) || !isText(input.charAt(idx)))
                {
                    break;
                }
                idx++;
            }
            return idx - startpos;
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using binary compaction.
        /// </summary>
        /// <param name="input">the input</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="encoding"></param>
        /// <returns>the requested character count</returns>
        private static int determineConsecutiveBinaryCount(ECIInput input, int startpos, Encoding encoding)
        {
            int len = input.Length;
            int idx = startpos;
            while (idx < len)
            {
                int numericCount = 0;

                int i = idx;
                while (numericCount < 13 && !input.isECI(i) && isDigit(input.charAt(i)))
                {
                    numericCount++;
                    //textCount++;
                    i = idx + numericCount;
                    if (i >= len)
                    {
                        break;
                    }
                }
                if (numericCount >= 13)
                {
                    return idx - startpos;
                }
                if (encoding != null)
                {
                    var ch = input.charAt(idx);
                    if (!ECIEncoderSet.canEncode(encoding, ch))
                    {
                        // assert input instanceof NoECIInput;
                        throw new WriterException("Non-encodable character detected: " + ch + " (Unicode: " + (int)ch + ')');
                        // .Net fallback strategie: REPLACEMENT_CHARACTER 0x3F
                        //if (bytes[idxb] == 63 && ch != '?')
                        //{
                        //    throw new WriterException("Non-encodable character detected: " + ch + " (Unicode: " + (int)ch + ')');
                        //}
                    }
                }
                idx++;
            }
            return idx - startpos;
        }

        private static void encodingECI(int eci, StringBuilder sb)
        {
            if (eci >= 0 && eci < 900)
            {
                sb.Append((char)ECI_CHARSET);
                sb.Append((char)eci);
            }
            else if (eci < 810900)
            {
                sb.Append((char)ECI_GENERAL_PURPOSE);
                sb.Append((char)(eci / 900 - 1));
                sb.Append((char)(eci % 900));
            }
            else if (eci < 811800)
            {
                sb.Append((char)ECI_USER_DEFINED);
                sb.Append((char)(810900 - eci));
            }
            else
            {
                throw new WriterException("ECI number not in valid range from 0..811799, but was " + eci);
            }
        }

        private class NoECIInput : ECIInput
        {
            String input;

            public NoECIInput(String input)
            {
                this.input = input;
            }

            public int Length
            {
                get
                {
                    return input.Length;
                }
            }

            public char charAt(int index)
            {
                return input[index];
            }

            public bool isECI(int index)
            {
                return false;
            }

            public int getECIValue(int index)
            {
                return -1;
            }

            public bool haveNCharacters(int index, int n)
            {
                return index + n <= input.Length;
            }

            public String subSequence(int start, int end)
            {
                return input.Substring(start, end - start);
            }

            public String toString()
            {
                return input;
            }
        }
    }
}