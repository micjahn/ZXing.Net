/*
 * Copyright 2006 Jeremias Maerki
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
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace ZXing.Datamatrix.Test
{
   /// <summary>
   /// Tests the DataMatrix placement algorithm.
   /// </summary>
   public sealed class PlacementTestCase
   {
      private static readonly Regex SPACE = new Regex(" ", RegexOptions.Compiled);

      [Test]
      public void testPlacement()
      {
         String codewords = unvisualize("66 74 78 66 74 78 129 56 35 102 192 96 226 100 156 1 107 221"); //"AIMAIM" encoded
         DebugPlacement placement = new DebugPlacement(codewords, 12, 12);
         placement.place();
         String[] expected = {
                                "011100001111",
                                "001010101000",
                                "010001010100",
                                "001010100010",
                                "000111000100",
                                "011000010100",
                                "000100001101",
                                "011000010000",
                                "001100001101",
                                "100010010111",
                                "011101011010",
                                "001011001010"
                             };
         String[] actual = placement.toBitFieldStringArray();
         for (int i = 0; i < actual.Length; i++)
         {
            Assert.AreEqual(expected[i], actual[i], "Row " + i);
         }
      }

      private static String unvisualize(String visualized)
      {
         var sb = new StringBuilder();
         foreach (var token in SPACE.Split(visualized))
         {
            sb.Append((char)Int32.Parse(token));
         }
         return sb.ToString();
      }
   }
}