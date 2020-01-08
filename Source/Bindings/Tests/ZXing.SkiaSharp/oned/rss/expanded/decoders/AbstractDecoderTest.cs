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

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;

using NUnit.Framework;
using ZXing.Common;
using ZXing.OneD.RSS.Expanded.Test;

namespace ZXing.OneD.RSS.Expanded.Decoders.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public abstract class AbstractDecoderTest
   {
      protected static String numeric_10 = "..X..XX";
      protected static String numeric_12 = "..X.X.X";
      protected static String numeric_1FNC1 = "..XXX.X";
      //protected static String numeric_FNC11 = "XXX.XXX";

      protected static String numeric2alpha = "....";

      protected static String alpha_A = "X.....";
      protected static String alpha_FNC1 = ".XXXX";
      protected static String alpha2numeric = "...";
      protected static String alpha2isoiec646 = "..X..";

      protected static String i646_B = "X.....X";
      protected static String i646_C = "X....X.";
      protected static String i646_FNC1 = ".XXXX";
      protected static String isoiec646_2alpha = "..X..";

      protected static String compressedGtin_900123456798908 = ".........X..XXX.X.X.X...XX.XXXXX.XXXX.X.";
      protected static String compressedGtin_900000000000008 = "........................................";

      protected static String compressed15bitWeight_1750 = "....XX.XX.X.XX.";
      protected static String compressed15bitWeight_11750 = ".X.XX.XXXX..XX.";
      protected static String compressed15bitWeight_0 = "...............";

      protected static String compressed20bitWeight_1750 = ".........XX.XX.X.XX.";

      protected static String compressedDate_March_12th_2010 = "....XXXX.X..XX..";
      protected static String compressedDate_End = "X..X.XX.........";

      protected static void assertCorrectBinaryString(String binaryString, String expectedNumber)
      {
         BitArray binary = BinaryUtil.buildBitArrayFromStringWithoutSpaces(binaryString);
         AbstractExpandedDecoder decoder = AbstractExpandedDecoder.createDecoder(binary);
         String result = decoder.parseInformation();
         Assert.AreEqual(expectedNumber, result);
      }
   }
}