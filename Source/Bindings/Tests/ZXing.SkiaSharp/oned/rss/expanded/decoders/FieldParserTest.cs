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
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public class FieldParserTest
   {
      private static void checkFields(String expected)
      {
         String field = expected.Replace("(", "").Replace(")", "");
         String actual = FieldParser.parseFieldsInGeneralPurpose(field);
         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void testParseField()
      {
         checkFields("(15)991231(3103)001750(10)12A");
      }

      [Test]
      public void testParseField2()
      {
         checkFields("(15)991231(15)991231(3103)001750(10)12A");
      }
   }
}