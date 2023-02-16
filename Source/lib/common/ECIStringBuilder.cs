/*
 * Copyright 2022 ZXing authors
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

namespace ZXing.Common
{
    using System;
    using System.Text;

    /// <summary>
    /// Class that converts a sequence of ECIs and bytes into a string
    ///
    /// @author Alex Geller
    /// </summary>
    public class ECIStringBuilder
    {
        private StringBuilder currentBytes;
        private StringBuilder result;
        private Encoding currentCharset;
        private Encoding standardEncoding;

        /// <summary>
        /// 
        /// </summary>
        public ECIStringBuilder()
            : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="standardEncoding"></param>
        public ECIStringBuilder(Encoding standardEncoding)
        {
            currentBytes = new StringBuilder();
            this.standardEncoding = currentCharset = (standardEncoding ?? (StringUtils.ISO88591_ENCODING ?? StringUtils.PLATFORM_DEFAULT_ENCODING_T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCapacity"></param>
        public ECIStringBuilder(int initialCapacity)
            : this(initialCapacity, null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCapacity"></param>
        /// <param name="standardEncoding"></param>
        /// <param name="startWithEncoding"></param>
        public ECIStringBuilder(int initialCapacity, Encoding standardEncoding, Encoding startWithEncoding)
        {
            currentBytes = new StringBuilder(initialCapacity);
            this.standardEncoding = standardEncoding ?? (StringUtils.ISO88591_ENCODING ?? StringUtils.PLATFORM_DEFAULT_ENCODING_T);
            currentCharset = startWithEncoding ?? this.standardEncoding;
        }

        /// <summary>
        /// Appends {@code value} as a byte value
        /// </summary>
        /// <param name="value">character whose lowest byte is to be appended</param>
        public void Append(char value)
        {
            currentBytes.Append((char)(value & 0xff));
        }

        /// <summary>
        /// Appends {@code value} as a byte value
        /// </summary>
        /// <param name="value">byte to append</param>
        public void Append(byte value)
        {
            currentBytes.Append((char)(value & 0xff));
        }

        /// <summary>
        /// Appends the characters in {@code value} as bytes values
        /// </summary>
        /// <param name="value">string to append</param>
        public void Append(String value)
        {
            currentBytes.Append(value);
        }

        /// <summary>
        /// Append the string repesentation of {@code value} (short for {@code append(String.valueOf(value))})
        /// </summary>
        /// <param name="value">int to append as a string</param>
        public void Append(int value)
        {
            Append(value.ToString());
        }

        /// <summary>
        /// Appends ECI value to output.
        /// </summary>
        /// <param name="value">ECI value to append, as an int</param>
        /// <returns></returns>
        public bool AppendECI(int value)
        {
            encodeCurrentBytesIfAny();
            CharacterSetECI characterSetECI = CharacterSetECI.getCharacterSetECIByValue(value);
            if (characterSetECI == null)
            {
                return false;
                //throw FormatException.getFormatInstance(new RuntimeException("Unsupported ECI value " + value));
            }
            currentCharset = CharacterSetECI.getEncoding(characterSetECI);
            return true;
        }

        private void encodeCurrentBytesIfAny()
        {
            if (currentCharset == standardEncoding)
            {
                if (currentBytes.Length > 0)
                {
                    if (result == null)
                    {
                        result = currentBytes;
                        currentBytes = new StringBuilder();
                    }
                    else
                    {
                        result.Append(currentBytes);
                        currentBytes = new StringBuilder();
                    }
                }
            }
            else if (currentBytes.Length > 0)
            {
                // byte[] bytes = currentBytes.toString().getBytes(StandardCharsets.ISO_8859_1);
                var bytes = new byte[currentBytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (byte)(currentBytes[i] & 0xff);
                }
                var encodedString = currentCharset.GetString(bytes, 0, bytes.Length);
                if (result == null)
                    result = new StringBuilder();
                result.Append(encodedString);
                currentBytes.Length = 0;
            }
        }


        /// <summary>
        /// Appends the characters from {@code value} (unlike all other append methods of this class who append bytes)
        /// </summary>
        /// <param name="value">characters to append</param>
        public void AppendCharacters(StringBuilder value)
        {
            encodeCurrentBytesIfAny();
            result.Append(value);
        }

        /// <summary>
        /// Short for {@code toString().length()} (if possible, use {@link #isEmpty()} instead)
        /// </summary>
        /// <returns>length of string representation in characters</returns>
        public int Length
        {
            get { return ToString().Length; }
        }

        /// <summary>
        /// true if nothing has been appended
        /// </summary>
        public bool isEmpty
        {
            get { return currentBytes.Length == 0 && (result == null || result.Length == 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            encodeCurrentBytesIfAny();
            return result == null ? "" : result.ToString();
        }
    }
}
