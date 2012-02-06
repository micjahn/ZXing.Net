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

using com.google.zxing.common;

namespace com.google.zxing.oned
{
   /// <summary>
   /// This class renders CodaBar as <see cref="BitMatrix" />.
   /// </summary>
   /// <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
   public class CodaBarWriter : OneDimensionalCodeWriter
   {
      public CodaBarWriter()
         : base(20)
      {
         // Super constructor requires the sum of the left and right margin length.
         // CodaBar spec requires a side margin to be more than ten times wider than narrow space.
         // In this implementation, narrow space has a unit length, so 20 is required minimum.
      }

      /// <summary>
      /// @see OneDimensionalCodeWriter#encode(java.lang.String)
      /// </summary>
      override public sbyte[] encode(String contents)
      {

         // Verify input and calculate decoded length.
         if (!CodaBarReader.arrayContains(
             new char[] { 'A', 'B', 'C', 'D' }, Char.ToUpper(contents[0])))
         {
            throw new ArgumentException(
                "Codabar should start with one of the following: 'A', 'B', 'C' or 'D'");
         }
         if (!CodaBarReader.arrayContains(new char[] { 'T', 'N', '*', 'E' },
                                          Char.ToUpper(contents[contents.Length - 1])))
         {
            throw new ArgumentException(
                "Codabar should end with one of the following: 'T', 'N', '*' or 'E'");
         }
         // The start character and the end character are decoded to 10 length each.
         int resultLength = 20;
         char[] charsWhichAreTenLengthEachAfterDecoded = { '/', ':', '+', '.' };
         for (int i = 1; i < contents.Length - 1; i++)
         {
            if (Char.IsDigit(contents[i]) || contents[i] == '-'
                || contents[i] == '$')
            {
               resultLength += 9;
            }
            else if (CodaBarReader.arrayContains(
              charsWhichAreTenLengthEachAfterDecoded, contents[i]))
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

         sbyte[] result = new sbyte[resultLength];
         int position = 0;
         for (int index = 0; index < contents.Length; index++)
         {
            char c = Char.ToUpper(contents[index]);
            if (index == contents.Length - 1)
            {
               // The end chars are not in the CodaBarReader.ALPHABET.
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
            sbyte color = 1;
            int counter = 0;
            int bit = 0;
            while (bit < 7)
            { // A character consists of 7 digit.
               result[position] = color;
               position++;
               if (((code >> (6 - bit)) & 1) == 0 || counter == 1)
               {
                  color ^= 1; // Flip the color.
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
               result[position] = 0;
               position++;
            }
         }
         return result;
      }
   }
}
