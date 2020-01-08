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
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif

using NUnit.Framework;

using ZXing.Common;

namespace ZXing.OneD.RSS.Expanded.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public sealed class RSSExpandedInternalTestCase
   {

      [Test]
      public void testFindFinderPatterns()
      {
         var rssExpandedReader = new RSSExpandedReader();

         var image = TestCaseUtil.readImage("test/data/blackbox/rssexpanded-1", "2.png");
         var binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         var rowNumber = binaryMap.Height / 2;
         var row = binaryMap.getBlackRow(rowNumber, null);
         var previousPairs = new List<ExpandedPair>();

         var pair1 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair1);
         var finderPattern = pair1.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);

         var pair2 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair2);
         finderPattern = pair2.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(1, finderPattern.Value);

         var pair3 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair3);
         finderPattern = pair3.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(1, finderPattern.Value);

         //   the previous was the last pair
         Assert.IsNull(rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber));
      }

      [Test]
      public void testRetrieveNextPairPatterns()
      {
         var rssExpandedReader = new RSSExpandedReader();

         var image = TestCaseUtil.readImage("test/data/blackbox/rssexpanded-1", "3.png");
         var binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         var rowNumber = binaryMap.Height / 2;
         var row = binaryMap.getBlackRow(rowNumber, null);
         var previousPairs = new List<ExpandedPair>();

         var pair1 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair1);
         var finderPattern = pair1.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);

         var pair2 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair2);
         finderPattern = pair2.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);
      }

      [Test]
      public void testDecodeCheckCharacter()
      {
         var rssExpandedReader = new RSSExpandedReader();

         var image = TestCaseUtil.readImage("test/data/blackbox/rssexpanded-1", "3.png");
         var binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         var row = binaryMap.getBlackRow(binaryMap.Height / 2, null);

         int[] startEnd = { 145, 243 };//image pixels where the A1 pattern starts (at 124) and ends (at 214)
         int value = 0;// A
#if !SILVERLIGHT
         var finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.Height / 2);
#else
         var finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.PixelHeight / 2);
#endif
         //{1, 8, 4, 1, 1};
         var dataCharacter = rssExpandedReader.decodeDataCharacter(row, finderPatternA1, true, true);

         Assert.AreEqual(98, dataCharacter.Value);
      }

      [Test]
      public void testDecodeDataCharacter()
      {
         var rssExpandedReader = new RSSExpandedReader();

         var image = TestCaseUtil.readImage("test/data/blackbox/rssexpanded-1", "3.png");
         var binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         var row = binaryMap.getBlackRow(binaryMap.Height / 2, null);

         int[] startEnd = { 145, 243 };//image pixels where the A1 pattern starts (at 124) and ends (at 214)
         int value = 0; // A
#if !SILVERLIGHT
         var finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.Height / 2);
#else
         var finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.PixelHeight / 2);
#endif
         //{1, 8, 4, 1, 1};
         var dataCharacter = rssExpandedReader.decodeDataCharacter(row, finderPatternA1, true, false);

         Assert.AreEqual(19, dataCharacter.Value);
         Assert.AreEqual(1007, dataCharacter.ChecksumPortion);
      }
   }
}