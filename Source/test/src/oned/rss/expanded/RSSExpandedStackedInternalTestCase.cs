/*
 * Copyright (C) 2012 ZXing authors
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

using NUnit.Framework;

using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Test
{
   public sealed class RSSExpandedStackedInternalTestCase
   {
      [Test]
      public void testDecodingRowByRow()
      {
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         BinaryBitmap binaryMap = TestCaseUtil.getBinaryBitmap("test/data/blackbox/rssexpandedstacked-2/1000.png");

         int firstRowNumber = binaryMap.Height / 3;
         BitArray firstRow = binaryMap.getBlackRow(firstRowNumber, null);
         Assert.IsFalse(rssExpandedReader.decodeRow2pairs(firstRowNumber, firstRow));

         Assert.AreEqual(1, rssExpandedReader.Rows.Count);
         ExpandedRow firstExpandedRow = rssExpandedReader.Rows[0];
         Assert.AreEqual(firstRowNumber, firstExpandedRow.RowNumber);

         Assert.AreEqual(2, firstExpandedRow.Pairs.Count);

         firstExpandedRow.Pairs[1].FinderPattern.StartEnd[1] = 0;

         int secondRowNumber = 2 * binaryMap.Height / 3;
         BitArray secondRow = binaryMap.getBlackRow(secondRowNumber, null);
         secondRow.reverse();

         Assert.IsTrue(rssExpandedReader.decodeRow2pairs(secondRowNumber, secondRow));
         var totalPairs = rssExpandedReader.Pairs;

         Result result = RSSExpandedReader.constructResult(totalPairs);
         Assert.AreEqual("(01)98898765432106(3202)012345(15)991231", result.Text);
      }

      [Test]
      public void testCompleteDecode()
      {
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         BinaryBitmap binaryMap = TestCaseUtil.getBinaryBitmap("test/data/blackbox/rssexpandedstacked-2/1000.png");

         Result result = rssExpandedReader.decode(binaryMap);
         Assert.AreEqual("(01)98898765432106(3202)012345(15)991231", result.Text);
      }
   }
}