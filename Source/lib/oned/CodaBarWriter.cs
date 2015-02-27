/*
 * Copyright 2011 ZXing authors
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

namespace ZXing.OneD
{
   /// <summary>
   /// This class renders CodaBar as <see cref="bool" />[].
   /// </summary>
   /// <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
   public sealed class CodaBarWriter : OneDimensionalCodeWriter
   {
      private static readonly char[] START_END_CHARS = {'A', 'B', 'C', 'D'};
      private static readonly char[] ALT_START_END_CHARS = {'T', 'N', '*', 'E'};
      private static readonly char[] CHARS_WHICH_ARE_TEN_LENGTH_EACH_AFTER_DECODED = {'/', ':', '+', '.'};
      private static readonly char DEFAULT_GUARD = START_END_CHARS[0];

      public override bool[] encode(String contents)
      {
         if (contents.Length < 2)
         {
            // Can't have a start/end guard, so tentatively add default guards
            contents = DEFAULT_GUARD + contents + DEFAULT_GUARD;
         }
         else
         {
            // Verify input and calculate decoded length.
            char firstChar = Char.ToUpper(contents[0]);
            char lastChar = Char.ToUpper(contents[contents.Length - 1]);
            bool startsNormal = CodaBarReader.arrayContains(START_END_CHARS, firstChar);
            bool endsNormal = CodaBarReader.arrayContains(START_END_CHARS, lastChar);
            bool startsAlt = CodaBarReader.arrayContains(ALT_START_END_CHARS, firstChar);
            bool endsAlt = CodaBarReader.arrayContains(ALT_START_END_CHARS, lastChar);
            if (startsNormal)
            {
               if (!endsNormal)
               {
                  throw new ArgumentException("Invalid start/end guards: " + contents);
               }
               // else already has valid start/end
            }
            else if (startsAlt)
            {
               if (!endsAlt)
               {
                  throw new ArgumentException("Invalid start/end guards: " + contents);
               }
               // else already has valid start/end
            }
            else
            {
               // Doesn't start with a guard
               if (endsNormal || endsAlt)
               {
                  throw new ArgumentException("Invalid start/end guards: " + contents);
               }
               // else doesn't end with guard either, so add a default
               contents = DEFAULT_GUARD + contents + DEFAULT_GUARD;
            }
         }

         // The start character and the end character are decoded to 10 length each.
         int resultLength = 20;
         for (int i = 1; i < contents.Length - 1; i++)
         {
            if (Char.IsDigit(contents[i]) || contents[i] == '-' || contents[i] == '$')
            {
               resultLength += 9;
            }
            else if (CodaBarReader.arrayContains(CHARS_WHICH_ARE_TEN_LENGTH_EACH_AFTER_DECODED, contents[i]))
            {
               resultLength += 10;
            }
            else
            {
               throw new ArgumentException("Cannot encode : '" + contents[i] + '\'');
            }
         }
         // A blank is placed between each character.
         resultLength += contents.Length - 1;

         var result = new bool[resultLength];
         int position = 0;
         for (int index = 0; index < contents.Length; index++)
         {
            char c = Char.ToUpper(contents[index]);
            if (index == 0 || index == contents.Length - 1)
            {
               // The start/end chars are not in the CodaBarReader.ALPHABET.
               switch (c)
               {
                  case 'T':
                     c = 'A';
                     break;
                  case 'N':
                     c = 'B';
                     break;
                  case '*':
                     c = 'C';
                     break;
                  case 'E':
                     c = 'D';
                     break;
               }
            }
            int code = 0;
            for (int i = 0; i < CodaBarReader.ALPHABET.Length; i++)
            {
               // Found any, because I checked above.
               if (c == CodaBarReader.ALPHABET[i])
               {
                  code = CodaBarReader.CHARACTER_ENCODINGS[i];
                  break;
               }
            }
            bool color = true;
            int counter = 0;
            int bit = 0;
            while (bit < 7)
            {
               // A character consists of 7 digit.
               result[position] = color;
               position++;
               if (((code >> (6 - bit)) & 1) == 0 || counter == 1)
               {
                  color = !color; // Flip the color.
                  bit++;
                  counter = 0;
               }
               else
               {
                  counter++;
               }
            }
            if (index < contents.Length - 1)
            {
               result[position] = false;
               position++;
            }
         }
         return result;
      }
   }
}
