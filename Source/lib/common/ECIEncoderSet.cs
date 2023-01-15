/*
 * Copyright 2021 ZXing authors
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
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Set of CharsetEncoders for a given input string
	/// Invariants:
	/// - The list contains only encoders from CharacterSetECI(list is shorter then the list of encoders available on
	///   the platform for which ECI values are defined).
	/// - The list contains encoders at least one encoder for every character in the input.
	/// - The first encoder in the list is always the ISO-8859-1 encoder even of no character in the input can be encoded
	/// by it.
	/// - If the input contains a character that is not in ISO-8859-1 then the last two entries in the list will be the
	/// UTF-8 encoder and the UTF-16BE encoder.
	/// 
	/// @author Alex Geller
	/// </summary>
	public class ECIEncoderSet
    {
        // List of encoders that potentially encode characters not in ISO-8859-1 in one byte.
        private static List<Encoding> ENCODERS = new List<Encoding>();
        static ECIEncoderSet()
        {
            String[] names =
            {
                "IBM437",
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
                "windows-1256",
                "Shift_JIS"
            };
			foreach (var name in names)
			{
				if (CharacterSetECI.getCharacterSetECIByName(name) != null)
				{
					try
					{
						ENCODERS.Add(Clone(Encoding.GetEncoding(name)));
					}
					catch (Exception)
					{
						// continue
					}
				}
			}
        }

        private Encoding[] encoders;
        private int priorityEncoderIndex;

        /// <summary>
        /// Constructs an encoder set
        /// </summary>
        /// <param name="stringToEncode">the string that needs to be encoded</param>
        /// <param name="priorityCharset">The preferred { @link Charset } or null.</param>
        /// <param name="fnc1">fnc1 denotes the character in the input that represents the FNC1 character or -1 for a non-GS1 bar
        /// code.When specified, it is considered an error to pass it as argument to the methods canEncode() or encode().</param>
        public ECIEncoderSet(String stringToEncode, Encoding priorityCharset, int fnc1)
        {
            var neededEncoders = new List<Encoding>();

			//we always need the ISO-8859-1 encoder. It is the default encoding
			neededEncoders.Add(Clone(StringUtils.ISO88591_ENCODING));
			var needUnicodeEncoder = priorityCharset != null && priorityCharset.WebName.StartsWith("UTF", StringComparison.OrdinalIgnoreCase);

            //Walk over the input string and see if all characters can be encoded with the list of encoders 
            for (int i = 0; i < stringToEncode.Length; i++)
            {
                bool canEnc = false;
                foreach (var encoder in neededEncoders)
                {
                    var c = stringToEncode[i];
                    if (c == fnc1 || canEncode(encoder, c))
                    {
						canEnc = true;
                        break;
                    }
                }

                if (!canEnc)
                {
                    //for the character at position i we don't yet have an encoder in the list
                    foreach (var encoder in ENCODERS)
                    {
                        if (canEncode(encoder, stringToEncode[i]))
                        {
                            //Good, we found an encoder that can encode the character. We add him to the list and continue scanning
                            //the input
                            neededEncoders.Add(encoder);
							canEnc = true;
                            break;
                        }
                    }
                }

                if (!canEnc) {
                    //The character is not encodeable by any of the single byte encoders so we remember that we will need a
                    //Unicode encoder.
                    needUnicodeEncoder = true;
                }
            }

            if (neededEncoders.Count == 1 && !needUnicodeEncoder)
            {
                //the entire input can be encoded by the ISO-8859-1 encoder
                encoders = new Encoding[] { neededEncoders[0] };
            }
            else
            {
                // we need more than one single byte encoder or we need a Unicode encoder.
                // In this case we append a UTF-8 and UTF-16 encoder to the list
                encoders = new Encoding[neededEncoders.Count + 2];
                int index = 0;
                foreach (var encoder in neededEncoders)
                {
                    encoders[index++] = encoder;
                }
				encoders[index] = Clone(Encoding.UTF8);
				encoders[index + 1] = Clone(Encoding.BigEndianUnicode);
			}

            //Compute priorityEncoderIndex by looking up priorityCharset in encoders
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
            //invariants
            //assert encoders[0].charset().equals(StandardCharsets.ISO_8859_1);
        }

#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || WINDOWS_UWP || PORTABLE || NETFX_CORE
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

		public int Length
        {
            get
            {
                return encoders.Length;
            }
        }

        public String getCharsetName(int index)
        {
            if (index >= Length)
                throw new IndexOutOfRangeException();
            return encoders[index].WebName.ToUpper();
        }

        public Encoding getCharset(int index)
        {
            if (index >= Length)
                throw new IndexOutOfRangeException();
            return encoders[index];
        }

        public int getECIValue(int encoderIndex)
        {
            return CharacterSetECI.getCharacterSetECI(encoders[encoderIndex]).Value;
        }

        /// <summary>
        /// returns -1 if no priority charset was defined
        /// </summary>
        /// <returns>-1 if no priority charset was defined</returns>
        public int getPriorityEncoderIndex()
        {
            return priorityEncoderIndex;
        }

        public bool canEncode(char c, int encoderIndex)
        {
            if (encoderIndex >= Length)
                throw new IndexOutOfRangeException();
            var encoder = encoders[encoderIndex];
            return canEncode(encoder, c);
        }

        public byte[] encode(char c, int encoderIndex)
        {
            if (encoderIndex >= Length)
                throw new IndexOutOfRangeException();
            var encoder = encoders[encoderIndex];
            //assert encoder.canEncode("" + c);
            return encoder.GetBytes("" + c);
        }

        public byte[] encode(String s, int encoderIndex)
        {
            if (encoderIndex >= Length)
                throw new IndexOutOfRangeException();
            var encoder = encoders[encoderIndex];
            return encoder.GetBytes(s);
        }

		private static Encoding Clone(Encoding encoding)
		{
			// encodings have to be cloned to change the EncoderFallback property later

#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !PORTABLE && !NETFX_CORE
			// Clone isn't supported by .net standard 1.0, 1.1 and portable
			return (Encoding)encoding.Clone();
#else
            return encoding;
#endif
		}
	}
}