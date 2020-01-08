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
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif
using System.IO;

using NUnit.Framework;
using ZXing.Common;
using ZXing.Common.Test;

namespace ZXing.OneD.RSS.Expanded.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public sealed class RSSExpandedImage2stringTestCase
   {
      [TestCase("1.png", "(11)100224(17)110224(3102)000100", TestName = "(11)100224(17)110224(3102)000100")]
      [TestCase("2.png", "(01)90012345678908(3103)001750", TestName = "(01)90012345678908(3103)001750")]
      [TestCase("3.png", "(10)12A", TestName = "(10)12A")]
      [TestCase("4.png", "(01)98898765432106(3202)012345(15)991231", TestName = "(01)98898765432106(3202)012345(15)991231")]
      [TestCase("5.png", "(01)90614141000015(3202)000150", TestName = "(01)90614141000015(3202)000150")]
      [TestCase("7.png", "(10)567(11)010101", TestName = "(10)567(11)010101")]
      [TestCase("10.png", "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456(423)012345678901", TestName = "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456(423)012345678901")]
      [TestCase("11.png", "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456", TestName = "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456")]
      [TestCase("12.png", "(01)98898765432106(3103)001750", TestName = "(01)98898765432106(3103)001750")]
      [TestCase("13.png", "(01)90012345678908(3922)795", TestName = "(01)90012345678908(3922)795")]
      [TestCase("14.png", "(01)90012345678908(3932)0401234", TestName = "(01)90012345678908(3932)0401234")]
      [TestCase("15.png", "(01)90012345678908(3102)001750(11)100312", TestName = "(01)90012345678908(3102)001750(11)100312")]
      [TestCase("16.png", "(01)90012345678908(3202)001750(11)100312", TestName = "(01)90012345678908(3202)001750(11)100312")]
      [TestCase("17.png", "(01)90012345678908(3102)001750(13)100312", TestName = "(01)90012345678908(3102)001750(13)100312")]
      [TestCase("18.png", "(01)90012345678908(3202)001750(13)100312", TestName = "(01)90012345678908(3202)001750(13)100312")]
      [TestCase("19.png", "(01)90012345678908(3102)001750(15)100312", TestName = "(01)90012345678908(3102)001750(15)100312")]
      [TestCase("20.png", "(01)90012345678908(3202)001750(15)100312", TestName = "(01)90012345678908(3202)001750(15)100312")]
      [TestCase("21.png", "(01)90012345678908(3102)001750(17)100312", TestName = "(01)90012345678908(3102)001750(17)100312")]
      [TestCase("22.png", "(01)90012345678908(3202)001750(17)100312", TestName = "(01)90012345678908(3202)001750(17)100312")]
      [TestCase("25.png", "(10)123", TestName = "(10)123")]
      [TestCase("26.png", "(10)5678(11)010101", TestName = "(10)5678(11)010101")]
      [TestCase("27.png", "(10)1098-1234", TestName = "(10)1098-1234")]
      [TestCase("28.png", "(10)1098/1234", TestName = "(10)1098/1234")]
      [TestCase("29.png", "(10)1098.1234", TestName = "(10)1098.1234")]
      [TestCase("30.png", "(10)1098*1234", TestName = "(10)1098*1234")]
      [TestCase("31.png", "(10)1098,1234", TestName = "(10)1098,1234")]
      [TestCase("32.png", "(15)991231(3103)001750(10)12A(422)123(21)123456(423)0123456789012", TestName = "(15)991231(3103)001750(10)12A(422)123(21)123456(423)0123456789012")]
      public void testDecodeRow2string(String imageFileName, String expected)
      {
         assertCorrectImage2string(imageFileName, expected);
      }
      
      private static void assertCorrectImage2string(String imageFileName, String expected)
      {
         var rssExpandedReader = new RSSExpandedReader();

         var binaryMap = TestCaseUtil.getBinaryBitmap("test/data/blackbox/rssexpanded-1", imageFileName);
         var rowNumber = binaryMap.Height / 2;
         var row = binaryMap.getBlackRow(rowNumber, null);

         var result = rssExpandedReader.decodeRow(rowNumber, row, null);
         Assert.IsNotNull(result);

         Assert.AreEqual(BarcodeFormat.RSS_EXPANDED, result.BarcodeFormat);
         Assert.AreEqual(expected, result.Text);
      }
   }
}