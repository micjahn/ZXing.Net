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

namespace ZXing.OneD.RSS.Expanded.Decoders.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// </summary>
   public class AnyAIDecoderTest : AbstractDecoderTest
   {
      private static String header = ".....";

      [Test]
      public void testAnyAIDecoder_1()
      {
         String data = header + numeric_10 + numeric_12 + numeric2alpha + alpha_A + alpha2numeric + numeric_12;
         String expected = "(10)12A12";

         assertCorrectBinaryString(data, expected);
      }

      [Test]
      public void testAnyAIDecoder_2()
      {
         String data = header + numeric_10 + numeric_12 + numeric2alpha + alpha_A + alpha2isoiec646 + i646_B;
         String expected = "(10)12AB";

         assertCorrectBinaryString(data, expected);
      }

      [Test]
      public void testAnyAIDecoder_3()
      {
         String data = header + numeric_10 + numeric2alpha + alpha2isoiec646 + i646_B + i646_C + isoiec646_2alpha + alpha_A + alpha2numeric + numeric_10;
         String expected = "(10)BCA10";

         assertCorrectBinaryString(data, expected);
      }

      [Test]
      public void testAnyAIDecoder_numericFNC1_secondDigit()
      {
         String data = header + numeric_10 + numeric_1FNC1;
         String expected = "(10)1";

         assertCorrectBinaryString(data, expected);
      }

      [Test]
      public void testAnyAIDecoder_alphaFNC1()
      {
         String data = header + numeric_10 + numeric2alpha + alpha_A + alpha_FNC1;
         String expected = "(10)A";

         assertCorrectBinaryString(data, expected);
      }

      [Test]
      public void testAnyAIDecoder_646FNC1()
      {
         String data = header + numeric_10 + numeric2alpha + alpha_A + isoiec646_2alpha + i646_B + i646_FNC1;
         String expected = "(10)AB";

         assertCorrectBinaryString(data, expected);
      }
   }
}