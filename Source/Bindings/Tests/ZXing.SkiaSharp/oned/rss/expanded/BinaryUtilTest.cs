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
using System.Text.RegularExpressions;

using NUnit.Framework;
using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public sealed class BinaryUtilTest
   {
      private const String SPACE = " ";

      [Test]
      public void testBuildBitArrayFromString()
      {

         String data = " ..X..X.. ..XXX... XXXXXXXX ........";
         check(data);

         data = " XXX..X..";
         check(data);

         data = " XX";
         check(data);

         data = " ....XX.. ..XX";
         check(data);

         data = " ....XX.. ..XX..XX ....X.X. ........";
         check(data);
      }

      private static void check(String data)
      {
         BitArray binary = BinaryUtil.buildBitArrayFromString(data);
         Assert.AreEqual(data, binary.ToString());
      }

      [Test]
      public void testBuildBitArrayFromStringWithoutSpaces()
      {
         String data = " ..X..X.. ..XXX... XXXXXXXX ........";
         checkWithoutSpaces(data);

         data = " XXX..X..";
         checkWithoutSpaces(data);

         data = " XX";
         checkWithoutSpaces(data);

         data = " ....XX.. ..XX";
         checkWithoutSpaces(data);

         data = " ....XX.. ..XX..XX ....X.X. ........";
         checkWithoutSpaces(data);
      }

      private static void checkWithoutSpaces(String data)
      {
         String dataWithoutSpaces = Regex.Replace(data, SPACE, "");
         BitArray binary = BinaryUtil.buildBitArrayFromStringWithoutSpaces(dataWithoutSpaces);
         Assert.AreEqual(data, binary.ToString());
      }
   }
}