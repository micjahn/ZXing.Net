/*
 * Copyright 2006-2007 Jeremias Maerki.
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
using System.Text;

namespace ZXing.Datamatrix.Encoder
{
   /// <summary>
   /// DataMatrix ECC 200 data encoder following the algorithm described in ISO/IEC 16022:200(E) in
   /// annex S.
   /// </summary>
   internal static class HighLevelEncoder
   {
      /// <summary>
      /// Padding character
      /// </summary>
      public const char PAD = (char)129;
      /// <summary>
      /// mode latch to C40 encodation mode
      /// </summary>
      public const char LATCH_TO_C40 = (char)230;
      /// <summary>
      /// mode latch to Base 256 encodation mode
      /// </summary>
      public const char LATCH_TO_BASE256 = (char)231;
      /// <summary>
      /// FNC1 Codeword
      /// </summary>
      public const char FNC1 = (char)232;
      /// <summary>
      /// Structured Append Codeword
      /// </summary>
      public const char STRUCTURED_APPEND = (char)233;
      /// <summary>
      /// Reader Programming
      /// </summary>
      public const char READER_PROGRAMMING = (char)234;
      /// <summary>
      /// Upper Shift
      /// </summary>
      public const char UPPER_SHIFT = (char)235;
      /// <summary>
      /// 05 Macro
      /// </summary>
      public const char MACRO_05 = (char)236;
      /// <summary>
      /// 06 Macro
      /// </summary>
      public const char MACRO_06 = (char)237;
      /// <summary>
      /// mode latch to ANSI X.12 encodation mode
      /// </summary>
      public const char LATCH_TO_ANSIX12 = (char)238;
      /// <summary>
      /// mode latch to Text encodation mode
      /// </summary>
      public const char LATCH_TO_TEXT = (char)239;
      /// <summary>
      /// mode latch to EDIFACT encodation mode
      /// </summary>
      public const char LATCH_TO_EDIFACT = (char)240;
      /// <summary>
      /// ECI character (Extended Channel Interpretation)
      /// </summary>
      public const char ECI = (char)241;

      /// <summary>
      /// Unlatch from C40 encodation
      /// </summary>
      public const char C40_UNLATCH = (char)254;
      /// <summary>
      /// Unlatch from X12 encodation
      /// </summary>
      public const char X12_UNLATCH = (char)254;

      /// <summary>
      /// 05 Macro header
      /// </summary>
      public const String MACRO_05_HEADER = "[)>\u001E05\u001D";
      /// <summary>
      /// 06 Macro header
      /// </summary>
      public const String MACRO_06_HEADER = "[)>\u001E06\u001D";
      /// <summary>
      /// Macro trailer
      /// </summary>
      public const String MACRO_TRAILER = "\u001E\u0004";

      public const int ASCII_ENCODATION = 0;
      public const int C40_ENCODATION = 1;
      public const int TEXT_ENCODATION = 2;
      public const int X12_ENCODATION = 3;
      public const int EDIFACT_ENCODATION = 4;
      public const int BASE256_ENCODATION = 5;

      /// <summary>
      /// Converts the message to a byte array using the default encoding (cp437) as defined by the
      /// specification
      /// </summary>
      /// <param name="msg">the message</param>
      /// <returns>the byte array of the message</returns>
      public static byte[] getBytesForMessage(String msg)
      {
         return System.Text.Encoding.GetEncoding("CP437").GetBytes(msg); //See 4.4.3 and annex B of ISO/IEC 15438:2001(E)
      }

      private static char randomize253State(char ch, int codewordPosition)
      {
         int pseudoRandom = ((149 * codewordPosition) % 253) + 1;
         int tempVariable = ch + pseudoRandom;
         return tempVariable <= 254 ? (char)tempVariable : (char)(tempVariable - 254);
      }

      /// <summary>
      /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
      /// of ISO/IEC 16022:2000(E).
      /// </summary>
      /// <param name="msg">the message</param>
      /// <returns>the encoded message (the char values range from 0 to 255)</returns>
      public static String encodeHighLevel(String msg)
      {
         return encodeHighLevel(msg, SymbolShapeHint.FORCE_NONE, null, null);
      }

      /// <summary>
      /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
      /// of ISO/IEC 16022:2000(E).
      /// </summary>
      /// <param name="msg">the message</param>
      /// <param name="shape">requested shape. May be {@code SymbolShapeHint.FORCE_NONE},{@code SymbolShapeHint.FORCE_SQUARE} or {@code SymbolShapeHint.FORCE_RECTANGLE}.</param>
      /// <param name="minSize">the minimum symbol size constraint or null for no constraint</param>
      /// <param name="maxSize">the maximum symbol size constraint or null for no constraint</param>
      /// <returns>the encoded message (the char values range from 0 to 255)</returns>
      public static String encodeHighLevel(String msg,
                                           SymbolShapeHint shape,
                                           Dimension minSize,
                                           Dimension maxSize)
      {
         //the codewords 0..255 are encoded as Unicode characters
         Encoder[] encoders = {
        new ASCIIEncoder(), new C40Encoder(), new TextEncoder(), 
        new X12Encoder(), new EdifactEncoder(),  new Base256Encoder()
    };

         var context = new EncoderContext(msg);
         context.setSymbolShape(shape);
         context.setSizeConstraints(minSize, maxSize);

         if (msg.StartsWith(MACRO_05_HEADER) && msg.EndsWith(MACRO_TRAILER))
         {
            context.writeCodeword(MACRO_05);
            context.setSkipAtEnd(2);
            context.Pos += MACRO_05_HEADER.Length;
         }
         else if (msg.StartsWith(MACRO_06_HEADER) && msg.EndsWith(MACRO_TRAILER))
         {
            context.writeCodeword(MACRO_06);
            context.setSkipAtEnd(2);
            context.Pos += MACRO_06_HEADER.Length;
         }

         int encodingMode = ASCII_ENCODATION; //Default mode
         while (context.HasMoreCharacters)
         {
            encoders[encodingMode].encode(context);
            if (context.NewEncoding >= 0)
            {
               encodingMode = context.NewEncoding;
               context.resetEncoderSignal();
            }
         }
         int len = context.Codewords.Length;
         context.updateSymbolInfo();
         int capacity = context.SymbolInfo.dataCapacity;
         if (len < capacity)
         {
            if (encodingMode != ASCII_ENCODATION && encodingMode != BASE256_ENCODATION)
            {
               context.writeCodeword('\u00fe'); //Unlatch (254)
            }
         }
         //Padding
         StringBuilder codewords = context.Codewords;
         if (codewords.Length < capacity)
         {
            codewords.Append(PAD);
         }
         while (codewords.Length < capacity)
         {
            codewords.Append(randomize253State(PAD, codewords.Length + 1));
         }

         return context.Codewords.ToString();
      }

      internal static int lookAheadTest(String msg, int startpos, int currentMode)
      {
         if (startpos >= msg.Length)
         {
            return currentMode;
         }
         float[] charCounts;
         //step J
         if (currentMode == ASCII_ENCODATION)
         {
            charCounts = new float[] { 0, 1, 1, 1, 1, 1.25f };
         }
         else
         {
            charCounts = new float[] { 1, 2, 2, 2, 2, 2.25f };
            charCounts[currentMode] = 0;
         }

         int charsProcessed = 0;
         while (true)
         {
            //step K
            if ((startpos + charsProcessed) == msg.Length)
            {
               int min = Int32.MaxValue;
               byte[] mins = new byte[6];
               int[] intCharCounts = new int[6];
               min = findMinimums(charCounts, intCharCounts, min, mins);
               int minCount = getMinimumCount(mins);

               if (intCharCounts[ASCII_ENCODATION] == min)
               {
                  return ASCII_ENCODATION;
               }
               if (minCount == 1 && mins[BASE256_ENCODATION] > 0)
               {
                  return BASE256_ENCODATION;
               }
               if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
               {
                  return EDIFACT_ENCODATION;
               }
               if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
               {
                  return TEXT_ENCODATION;
               }
               if (minCount == 1 && mins[X12_ENCODATION] > 0)
               {
                  return X12_ENCODATION;
               }
               return C40_ENCODATION;
            }

            char c = msg[startpos + charsProcessed];
            charsProcessed++;

            //step L
            if (isDigit(c))
            {
               charCounts[ASCII_ENCODATION] += 0.5f;
            }
            else if (isExtendedASCII(c))
            {
               charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
               charCounts[ASCII_ENCODATION] += 2;
            }
            else
            {
               charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
               charCounts[ASCII_ENCODATION]++;
            }

            //step M
            if (isNativeC40(c))
            {
               charCounts[C40_ENCODATION] += 2.0f / 3.0f;
            }
            else if (isExtendedASCII(c))
            {
               charCounts[C40_ENCODATION] += 8.0f / 3.0f;
            }
            else
            {
               charCounts[C40_ENCODATION] += 4.0f / 3.0f;
            }

            //step N
            if (isNativeText(c))
            {
               charCounts[TEXT_ENCODATION] += 2.0f / 3.0f;
            }
            else if (isExtendedASCII(c))
            {
               charCounts[TEXT_ENCODATION] += 8.0f / 3.0f;
            }
            else
            {
               charCounts[TEXT_ENCODATION] += 4.0f / 3.0f;
            }

            //step O
            if (isNativeX12(c))
            {
               charCounts[X12_ENCODATION] += 2.0f / 3.0f;
            }
            else if (isExtendedASCII(c))
            {
               charCounts[X12_ENCODATION] += 13.0f / 3.0f;
            }
            else
            {
               charCounts[X12_ENCODATION] += 10.0f / 3.0f;
            }

            //step P
            if (isNativeEDIFACT(c))
            {
               charCounts[EDIFACT_ENCODATION] += 3.0f / 4.0f;
            }
            else if (isExtendedASCII(c))
            {
               charCounts[EDIFACT_ENCODATION] += 17.0f / 4.0f;
            }
            else
            {
               charCounts[EDIFACT_ENCODATION] += 13.0f / 4.0f;
            }

            // step Q
            if (isSpecialB256(c))
            {
               charCounts[BASE256_ENCODATION] += 4;
            }
            else
            {
               charCounts[BASE256_ENCODATION]++;
            }

            //step R
            if (charsProcessed >= 4)
            {
               int[] intCharCounts = new int[6];
               byte[] mins = new byte[6];
               findMinimums(charCounts, intCharCounts, Int32.MaxValue, mins);
               int minCount = getMinimumCount(mins);

               if (intCharCounts[ASCII_ENCODATION] < intCharCounts[BASE256_ENCODATION]
                   && intCharCounts[ASCII_ENCODATION] < intCharCounts[C40_ENCODATION]
                   && intCharCounts[ASCII_ENCODATION] < intCharCounts[TEXT_ENCODATION]
                   && intCharCounts[ASCII_ENCODATION] < intCharCounts[X12_ENCODATION]
                   && intCharCounts[ASCII_ENCODATION] < intCharCounts[EDIFACT_ENCODATION])
               {
                  return ASCII_ENCODATION;
               }
               if (intCharCounts[BASE256_ENCODATION] < intCharCounts[ASCII_ENCODATION]
                   || (mins[C40_ENCODATION] + mins[TEXT_ENCODATION] + mins[X12_ENCODATION] + mins[EDIFACT_ENCODATION]) == 0)
               {
                  return BASE256_ENCODATION;
               }
               if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
               {
                  return EDIFACT_ENCODATION;
               }
               if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
               {
                  return TEXT_ENCODATION;
               }
               if (minCount == 1 && mins[X12_ENCODATION] > 0)
               {
                  return X12_ENCODATION;
               }
               if (intCharCounts[C40_ENCODATION] + 1 < intCharCounts[ASCII_ENCODATION]
                   && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[BASE256_ENCODATION]
                   && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[EDIFACT_ENCODATION]
                   && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[TEXT_ENCODATION])
               {
                  if (intCharCounts[C40_ENCODATION] < intCharCounts[X12_ENCODATION])
                  {
                     return C40_ENCODATION;
                  }
                  if (intCharCounts[C40_ENCODATION] == intCharCounts[X12_ENCODATION])
                  {
                     int p = startpos + charsProcessed + 1;
                     while (p < msg.Length)
                     {
                        char tc = msg[p];
                        if (isX12TermSep(tc))
                        {
                           return X12_ENCODATION;
                        }
                        if (!isNativeX12(tc))
                        {
                           break;
                        }
                        p++;
                     }
                     return C40_ENCODATION;
                  }
               }
            }
         }
      }

      private static int findMinimums(float[] charCounts, int[] intCharCounts, int min, byte[] mins)
      {
         SupportClass.Fill(mins, (byte)0);
         for (int i = 0; i < 6; i++)
         {
            intCharCounts[i] = (int)Math.Ceiling(charCounts[i]);
            int current = intCharCounts[i];
            if (min > current)
            {
               min = current;
               SupportClass.Fill(mins, (byte)0);
            }
            if (min == current)
            {
               mins[i]++;

            }
         }
         return min;
      }

      private static int getMinimumCount(byte[] mins)
      {
         int minCount = 0;
         for (int i = 0; i < 6; i++)
         {
            minCount += mins[i];
         }
         return minCount;
      }

      internal static bool isDigit(char ch)
      {
         return ch >= '0' && ch <= '9';
      }

      internal static bool isExtendedASCII(char ch)
      {
         return ch >= 128 && ch <= 255;
      }

      internal static bool isNativeC40(char ch)
      {
         return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
      }

      internal static bool isNativeText(char ch)
      {
         return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z');
      }

      internal static bool isNativeX12(char ch)
      {
         return isX12TermSep(ch) || (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
      }

      internal static bool isX12TermSep(char ch)
      {
         return (ch == '\r') //CR
             || (ch == '*')
             || (ch == '>');
      }

      internal static bool isNativeEDIFACT(char ch)
      {
         return ch >= ' ' && ch <= '^';
      }

      internal static bool isSpecialB256(char ch)
      {
         return false; //TODO NOT IMPLEMENTED YET!!!
      }

      /// <summary>
      /// Determines the number of consecutive characters that are encodable using numeric compaction.
      /// </summary>
      /// <param name="msg">the message</param>
      /// <param name="startpos">the start position within the message</param>
      /// <returns>the requested character count</returns>
      public static int determineConsecutiveDigitCount(String msg, int startpos)
      {
         int count = 0;
         int len = msg.Length;
         int idx = startpos;
         if (idx < len)
         {
            char ch = msg[idx];
            while (isDigit(ch) && idx < len)
            {
               count++;
               idx++;
               if (idx < len)
               {
                  ch = msg[idx];
               }
            }
         }
         return count;
      }

      internal static void illegalCharacter(char c)
      {
         throw new ArgumentException(String.Format("Illegal character: {0} (0x{1:X})", c, (int)c));
      }
   }
}