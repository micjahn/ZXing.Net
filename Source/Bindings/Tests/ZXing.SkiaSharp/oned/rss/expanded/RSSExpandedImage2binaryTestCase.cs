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
   public sealed class RSSExpandedImage2binaryTestCase
   {
      [TestCase("1.png", " ...X...X .X....X. .XX...X. X..X...X ...XX.X. ..X.X... ..X.X..X ...X..X. X.X....X .X....X. .....X.. X...X...", TestName = "(11)100224(17)110224(3102)000100")]
      [TestCase("2.png", " ..X..... ......X. .XXX.X.X .X...XX. XXXXX.XX XX.X.... .XX.XX.X .XX.", TestName = "(01)90012345678908(3103)001750")]
      [TestCase("3.png", " .......X ..XX..X. X.X....X .......X ....", TestName = "(10)12A")]
      [TestCase("4.png", " ..XXXX.X XX.XXXX. .XXX.XX. XX..X... .XXXXX.. XX.X..X. ..XX..XX XX.X.XXX X..XX..X .X.XXXXX XXXX", TestName = "(01)98898765432106(3202)012345(15)991231")]
      [TestCase("5.png", " ..X.X... .XXXX.X. XX..XXXX ....XX.. X....... ....X... ....X..X .XX.", TestName = "(01)90614141000015(3202)000150")]
      [TestCase("10.png", " .X.XX..X XX.XXXX. .XXX.XX. XX..X... .XXXXX.. XX.X..X. ..XX...X XX.X.... X.X.X.X. X.X..X.X .X....X. XX...X.. ...XX.X. .XXXXXX. .X..XX.. X.X.X... .X...... XXXX.... XX.XX... XXXXX.X. ...XXXXX .....X.X ...X.... X.XXX..X X.X.X... XX.XX..X .X..X..X .X.X.X.X X.XX...X .XX.XXX. XXX.X.XX ..X.", TestName = "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456(423)0123456789012")]
      [TestCase("11.png", " .X.XX..X XX.XXXX. .XXX.XX. XX..X... .XXXXX.. XX.X..X. ..XX...X XX.X.... X.X.X.X. X.X..X.X .X....X. XX...X.. ...XX.X. .XXXXXX. .X..XX.. X.X.X... .X...... XXXX.... XX.XX... XXXXX.X. ...XXXXX .....X.X ...X.... X.XXX..X X.X.X... ....", TestName = "(01)98898765432106(15)991231(3103)001750(10)12A(422)123(21)123456")]
      [TestCase("12.png", " ..X..XX. XXXX..XX X.XX.XX. .X....XX XXX..XX. X..X.... .XX.XX.X .XX.", TestName = "(01)98898765432106(3103)001750")]
      [TestCase("13.png", " ..XX..X. ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. X.X.XXXX .X..X..X ......X.", TestName = "(01)90012345678908(3922)795")]
      [TestCase("14.png", " ..XX.X.. ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. X.....X. X.....X. X.X.X.XX .X...... X...", TestName = "(01)90012345678908(3932)0401234")]
      [TestCase("15.png", " ..XXX... ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3102)001750(11)100312")]
      [TestCase("16.png", " ..XXX..X ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3202)001750(11)100312")]
      [TestCase("17.png", " ..XXX.X. ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3102)001750(13)100312")]
      [TestCase("18.png", " ..XXX.XX ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3202)001750(13)100312")]
      [TestCase("19.png", " ..XXXX.. ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3102)001750(15)100312")]
      [TestCase("20.png", " ..XXXX.X ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3202)001750(15)100312")]
      [TestCase("21.png", " ..XXXXX. ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3102)001750(17)100312")]
      [TestCase("22.png", " ..XXXXXX ........ .X..XXX. X.X.X... XX.XXXXX .XXXX.X. ..XX...X .X.....X .XX..... XXXX.X.. XX..", TestName = "(01)90012345678908(3202)001750(17)100312")]
      public void testDecodeRow2binary(String imageFileName, String expectedResult)
      {
         assertCorrectImage2binary(imageFileName, expectedResult);
      }

      private static void assertCorrectImage2binary(String imageFileName, String expected)
      {
         var rssExpandedReader = new RSSExpandedReader();

         var binaryMap = TestCaseUtil.getBinaryBitmap("test/data/blackbox/rssexpanded-1", imageFileName);
         var rowNumber = binaryMap.Height / 2;
         var row = binaryMap.getBlackRow(rowNumber, null);

         Assert.IsTrue(rssExpandedReader.decodeRow2pairs(rowNumber, row));

         var binary = BitArrayBuilder.buildBitArray(rssExpandedReader.Pairs);
         Assert.AreEqual(expected, binary.ToString());
      }
   }
}