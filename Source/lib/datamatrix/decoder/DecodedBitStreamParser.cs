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

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

namespace ZXing.Datamatrix.Internal
{
   /// <summary>
   /// <p>Data Matrix Codes can encode text as bits in one of several modes, and can use multiple modes
   /// in one Data Matrix Code. This class decodes the bits back into text.</p>
   ///
   /// <p>See ISO 16022:2006, 5.2.1 - 5.2.9.2</p>
   ///
   /// <author>bbrown@google.com (Brian Brown)</author>
   /// <author>Sean Owen</author>
   /// </summary>
   internal static class DecodedBitStreamParser
   {
      private enum Mode
      {
         PAD_ENCODE, // Not really a mode
         ASCII_ENCODE,
         C40_ENCODE,
         TEXT_ENCODE,
         ANSIX12_ENCODE,
         EDIFACT_ENCODE,
         BASE256_ENCODE
      }

      /// <summary>
      /// See ISO 16022:2006, Annex C Table C.1
      /// The C40 Basic Character Set (*'s used for placeholders for the shift values)
      /// </summary>
      private static readonly char[] C40_BASIC_SET_CHARS =
         {
            '*', '*', '*', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
         };

      private static readonly char[] C40_SHIFT2_SET_CHARS =
         {
            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.',
            '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_'
         };

      /// <summary>
      /// See ISO 16022:2006, Annex C Table C.2
      /// The Text Basic Character Set (*'s used for placeholders for the shift values)
      /// </summary>
      private static readonly char[] TEXT_BASIC_SET_CHARS =
         {
            '*', '*', '*', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
         };

      // Shift 2 for Text is the same encoding as C40
      private static readonly char[] TEXT_SHIFT2_SET_CHARS = C40_SHIFT2_SET_CHARS;

      private static readonly char[] TEXT_SHIFT3_SET_CHARS =
         {
            '`', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '{',
            '|', '}', '~', (char) 127
         };

      internal static DecoderResult decode(byte[] bytes)
      {
         BitSource bits = new BitSource(bytes);
         StringBuilder result = new StringBuilder(100);
         StringBuilder resultTrailer = new StringBuilder(0);
         List<byte[]> byteSegments = new List<byte[]>(1);
         Mode mode = Mode.ASCII_ENCODE;
         do
         {
            if (mode == Mode.ASCII_ENCODE)
            {
               if (!decodeAsciiSegment(bits, result, resultTrailer, out mode))
                  return null;
            }
            else
            {
               switch (mode)
               {
                  case Mode.C40_ENCODE:
                     if (!decodeC40Segment(bits, result))
                        return null;
                     break;
                  case Mode.TEXT_ENCODE:
                     if (!decodeTextSegment(bits, result))
                        return null;
                     break;
                  case Mode.ANSIX12_ENCODE:
                     if (!decodeAnsiX12Segment(bits, result))
                        return null;
                     break;
                  case Mode.EDIFACT_ENCODE:
                     if (!decodeEdifactSegment(bits, result))
                        return null;
                     break;
                  case Mode.BASE256_ENCODE:
                     if (!decodeBase256Segment(bits, result, byteSegments))
                        return null;
                     break;
                  default:
                     return null;
               }
               mode = Mode.ASCII_ENCODE;
            }
         } while (mode != Mode.PAD_ENCODE && bits.available() > 0);
         if (resultTrailer.Length > 0)
         {
            result.Append(resultTrailer.ToString());
         }
         return new DecoderResult(bytes, result.ToString(), byteSegments.Count == 0 ? null : byteSegments, null);
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.3 and Annex C, Table C.2
      /// </summary>
      private static bool decodeAsciiSegment(BitSource bits,
                                             StringBuilder result,
                                             StringBuilder resultTrailer,
                                             out Mode mode)
      {
         bool upperShift = false;
         mode = Mode.ASCII_ENCODE;
         do
         {
            int oneByte = bits.readBits(8);
            if (oneByte == 0)
            {
               return false;
            }
            else if (oneByte <= 128)
            {
               // ASCII data (ASCII value + 1)
               if (upperShift)
               {
                  oneByte += 128;
                  //upperShift = false;
               }
               result.Append((char) (oneByte - 1));
               mode = Mode.ASCII_ENCODE;
               return true;
            }
            else if (oneByte == 129)
            {
               // Pad
               mode = Mode.PAD_ENCODE;
               return true;
            }
            else if (oneByte <= 229)
            {
               // 2-digit data 00-99 (Numeric Value + 130)
               int value = oneByte - 130;
               if (value < 10)
               {
                  // pad with '0' for single digit values
                  result.Append('0');
               }
               result.Append(value);
            }
            else
            {
               switch (oneByte)
               {
                  case 230: // Latch to C40 encodation
                     mode = Mode.C40_ENCODE;
                     return true;
                  case 231: // Latch to Base 256 encodation
                     mode = Mode.BASE256_ENCODE;
                     return true;
                  case 232: // FNC1
                     result.Append((char) 29); // translate as ASCII 29
                     break;
                  case 233: // Structured Append
                  case 234: // Reader Programming
                     // Ignore these symbols for now
                     //throw ReaderException.getInstance();
                     break;
                  case 235: // Upper Shift (shift to Extended ASCII)
                     upperShift = true;
                     break;
                  case 236: // 05 Macro
                     result.Append("[)>\u001E05\u001D");
                     resultTrailer.Insert(0, "\u001E\u0004");
                     break;
                  case 237: // 06 Macro
                     result.Append("[)>\u001E06\u001D");
                     resultTrailer.Insert(0, "\u001E\u0004");
                     break;
                  case 238: // Latch to ANSI X12 encodation
                     mode = Mode.ANSIX12_ENCODE;
                     return true;
                  case 239: // Latch to Text encodation
                     mode = Mode.TEXT_ENCODE;
                     return true;
                  case 240: // Latch to EDIFACT encodation
                     mode = Mode.EDIFACT_ENCODE;
                     return true;
                  case 241: // ECI Character
                     // TODO(bbrown): I think we need to support ECI
                     //throw ReaderException.getInstance();
                     // Ignore this symbol for now
                     break;
                  default:
                     // Not to be used in ASCII encodation
                     // but work around encoders that end with 254, latch back to ASCII
                     if (oneByte >= 242 && (oneByte != 254 || bits.available() != 0))
                     {
                        return false;
                     }
                     break;
               }
            }
         } while (bits.available() > 0);
         mode = Mode.ASCII_ENCODE;
         return true;
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.5 and Annex C, Table C.1
      /// </summary>
      private static bool decodeC40Segment(BitSource bits, StringBuilder result)
      {
         // Three C40 values are encoded in a 16-bit value as
         // (1600 * C1) + (40 * C2) + C3 + 1
         // TODO(bbrown): The Upper Shift with C40 doesn't work in the 4 value scenario all the time
         bool upperShift = false;

         int[] cValues = new int[3];
         int shift = 0;

         do
         {
            // If there is only one byte left then it will be encoded as ASCII
            if (bits.available() == 8)
            {
               return true;
            }
            int firstByte = bits.readBits(8);
            if (firstByte == 254)
            {
               // Unlatch codeword
               return true;
            }

            parseTwoBytes(firstByte, bits.readBits(8), cValues);

            for (int i = 0; i < 3; i++)
            {
               int cValue = cValues[i];
               switch (shift)
               {
                  case 0:
                     if (cValue < 3)
                     {
                        shift = cValue + 1;
                     }
                     else if (cValue < C40_BASIC_SET_CHARS.Length)
                     {
                        char c40char = C40_BASIC_SET_CHARS[cValue];
                        if (upperShift)
                        {
                           result.Append((char) (c40char + 128));
                           upperShift = false;
                        }
                        else
                        {
                           result.Append(c40char);
                        }
                     }
                     else
                     {
                        return false;
                     }
                     break;
                  case 1:
                     if (upperShift)
                     {
                        result.Append((char) (cValue + 128));
                        upperShift = false;
                     }
                     else
                     {
                        result.Append((char) cValue);
                     }
                     shift = 0;
                     break;
                  case 2:
                     if (cValue < C40_SHIFT2_SET_CHARS.Length)
                     {
                        char c40char = C40_SHIFT2_SET_CHARS[cValue];
                        if (upperShift)
                        {
                           result.Append((char) (c40char + 128));
                           upperShift = false;
                        }
                        else
                        {
                           result.Append(c40char);
                        }
                     }
                     else
                     {
                        switch (cValue)
                        {
                           case 27: // FNC1
                              result.Append((char) 29); // translate as ASCII 29
                              break;
                           case 30: // Upper Shift
                              upperShift = true;
                              break;
                           default:
                              return false;
                        }
                     }
                     shift = 0;
                     break;
                  case 3:
                     if (upperShift)
                     {
                        result.Append((char) (cValue + 224));
                        upperShift = false;
                     }
                     else
                     {
                        result.Append((char) (cValue + 96));
                     }
                     shift = 0;
                     break;
                  default:
                     return false;
               }
            }
         } while (bits.available() > 0);

         return true;
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.6 and Annex C, Table C.2
      /// </summary>
      private static bool decodeTextSegment(BitSource bits, StringBuilder result)
      {
         // Three Text values are encoded in a 16-bit value as
         // (1600 * C1) + (40 * C2) + C3 + 1
         // TODO(bbrown): The Upper Shift with Text doesn't work in the 4 value scenario all the time
         bool upperShift = false;

         int[] cValues = new int[3];
         int shift = 0;
         do
         {
            // If there is only one byte left then it will be encoded as ASCII
            if (bits.available() == 8)
            {
               return true;
            }
            int firstByte = bits.readBits(8);
            if (firstByte == 254)
            {
               // Unlatch codeword
               return true;
            }

            parseTwoBytes(firstByte, bits.readBits(8), cValues);

            for (int i = 0; i < 3; i++)
            {
               int cValue = cValues[i];
               switch (shift)
               {
                  case 0:
                     if (cValue < 3)
                     {
                        shift = cValue + 1;
                     }
                     else if (cValue < TEXT_BASIC_SET_CHARS.Length)
                     {
                        char textChar = TEXT_BASIC_SET_CHARS[cValue];
                        if (upperShift)
                        {
                           result.Append((char) (textChar + 128));
                           upperShift = false;
                        }
                        else
                        {
                           result.Append(textChar);
                        }
                     }
                     else
                     {
                        return false;
                     }
                     break;
                  case 1:
                     if (upperShift)
                     {
                        result.Append((char) (cValue + 128));
                        upperShift = false;
                     }
                     else
                     {
                        result.Append((char) cValue);
                     }
                     shift = 0;
                     break;
                  case 2:
                     // Shift 2 for Text is the same encoding as C40
                     if (cValue < TEXT_SHIFT2_SET_CHARS.Length)
                     {
                        char textChar = TEXT_SHIFT2_SET_CHARS[cValue];
                        if (upperShift)
                        {
                           result.Append((char) (textChar + 128));
                           upperShift = false;
                        }
                        else
                        {
                           result.Append(textChar);
                        }
                     }
                     else
                     {
                        switch (cValue)
                        {
                           case 27: // FNC1
                              result.Append((char) 29); // translate as ASCII 29
                              break;
                           case 30: // Upper Shift
                              upperShift = true;
                              break;
                           default:
                              return false;
                        }
                     }
                     shift = 0;
                     break;
                  case 3:
                     if (cValue < TEXT_SHIFT3_SET_CHARS.Length)
                     {
                        char textChar = TEXT_SHIFT3_SET_CHARS[cValue];
                        if (upperShift)
                        {
                           result.Append((char) (textChar + 128));
                           upperShift = false;
                        }
                        else
                        {
                           result.Append(textChar);
                        }
                        shift = 0;
                     }
                     else
                     {
                        return false;
                     }
                     break;
                  default:
                     return false;
               }
            }
         } while (bits.available() > 0);

         return true;
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.7
      /// </summary>
      private static bool decodeAnsiX12Segment(BitSource bits,
                                               StringBuilder result)
      {
         // Three ANSI X12 values are encoded in a 16-bit value as
         // (1600 * C1) + (40 * C2) + C3 + 1

         int[] cValues = new int[3];
         do
         {
            // If there is only one byte left then it will be encoded as ASCII
            if (bits.available() == 8)
            {
               return true;
            }
            int firstByte = bits.readBits(8);
            if (firstByte == 254)
            {
               // Unlatch codeword
               return true;
            }

            parseTwoBytes(firstByte, bits.readBits(8), cValues);

            for (int i = 0; i < 3; i++)
            {
               int cValue = cValues[i];
               switch (cValue)
               {
                  case 0: // X12 segment terminator <CR>
                     result.Append('\r');
                     break;
                  case 1: // X12 segment separator *
                     result.Append('*');
                     break;
                  case 2: // X12 sub-element separator >
                     result.Append('>');
                     break;
                  case 3: // space
                     result.Append(' ');
                     break;
                  default:
                     if (cValue < 14)
                     {
                        // 0 - 9
                        result.Append((char) (cValue + 44));
                     }
                     else if (cValue < 40)
                     {
                        // A - Z
                        result.Append((char) (cValue + 51));
                     }
                     else
                     {
                        return false;
                     }
                     break;
               }
            }
         } while (bits.available() > 0);

         return true;
      }

      private static void parseTwoBytes(int firstByte, int secondByte, int[] result)
      {
         int fullBitValue = (firstByte << 8) + secondByte - 1;
         int temp = fullBitValue/1600;
         result[0] = temp;
         fullBitValue -= temp*1600;
         temp = fullBitValue/40;
         result[1] = temp;
         result[2] = fullBitValue - temp*40;
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.8 and Annex C Table C.3
      /// </summary>
      private static bool decodeEdifactSegment(BitSource bits, StringBuilder result)
      {
         do
         {
            // If there is only two or less bytes left then it will be encoded as ASCII
            if (bits.available() <= 16)
            {
               return true;
            }

            for (int i = 0; i < 4; i++)
            {
               int edifactValue = bits.readBits(6);

               // Check for the unlatch character
               if (edifactValue == 0x1F)
               {
                  // 011111
                  // Read rest of byte, which should be 0, and stop
                  int bitsLeft = 8 - bits.BitOffset;
                  if (bitsLeft != 8)
                  {
                     bits.readBits(bitsLeft);
                  }
                  return true;
               }

               if ((edifactValue & 0x20) == 0)
               {
                  // no 1 in the leading (6th) bit
                  edifactValue |= 0x40; // Add a leading 01 to the 6 bit binary value
               }
               result.Append((char) edifactValue);
            }
         } while (bits.available() > 0);

         return true;
      }

      /// <summary>
      /// See ISO 16022:2006, 5.2.9 and Annex B, B.2
      /// </summary>
      private static bool decodeBase256Segment(BitSource bits,
                                               StringBuilder result,
                                               IList<byte[]> byteSegments)
      {
         // Figure out how long the Base 256 Segment is.
         int codewordPosition = 1 + bits.ByteOffset; // position is 1-indexed
         int d1 = unrandomize255State(bits.readBits(8), codewordPosition++);
         int count;
         if (d1 == 0)
         {
            // Read the remainder of the symbol
            count = bits.available()/8;
         }
         else if (d1 < 250)
         {
            count = d1;
         }
         else
         {
            count = 250*(d1 - 249) + unrandomize255State(bits.readBits(8), codewordPosition++);
         }

         // We're seeing NegativeArraySizeException errors from users.
         if (count < 0)
         {
            return false;
         }

         byte[] bytes = new byte[count];
         for (int i = 0; i < count; i++)
         {
            // Have seen this particular error in the wild, such as at
            // http://www.bcgen.com/demo/IDAutomationStreamingDataMatrix.aspx?MODE=3&D=Fred&PFMT=3&PT=F&X=0.3&O=0&LM=0.2
            if (bits.available() < 8)
            {
               return false;
            }
            bytes[i] = (byte) unrandomize255State(bits.readBits(8), codewordPosition++);
         }
         byteSegments.Add(bytes);
         try
         {
#if (WINDOWS_PHONE || SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || WindowsCE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
#if WindowsCE
            result.Append(Encoding.GetEncoding(1252).GetString(bytes, 0, bytes.Length));
#else
            result.Append(Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, bytes.Length));
#endif
#else
            result.Append(Encoding.GetEncoding("ISO-8859-1").GetString(bytes));
#endif
         }
         catch (Exception uee)
         {
            throw new InvalidOperationException("Platform does not support required encoding: " + uee);
         }

         return true;
      }

      /// <summary>
      /// See ISO 16022:2006, Annex B, B.2
      /// </summary>
      private static int unrandomize255State(int randomizedBase256Codeword,
                                             int base256CodewordPosition)
      {
         int pseudoRandomNumber = ((149*base256CodewordPosition)%255) + 1;
         int tempVariable = randomizedBase256Codeword - pseudoRandomNumber;
         return tempVariable >= 0 ? tempVariable : tempVariable + 256;
      }

   }
}