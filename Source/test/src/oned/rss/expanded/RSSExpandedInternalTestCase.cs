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
using System.IO;

using NUnit.Framework;
using ZXing.Common;
using ZXing.Test;

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
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         String path = "test/data/blackbox/rssexpanded-1/2.png";

         if (!File.Exists(path))
         {
            // Support running from project root too
            path = Path.Combine("..\\..\\..\\Source", path);
         }

#if !SILVERLIGHT
         var image = new Bitmap(Image.FromFile(path));
#else
         var image = new WriteableBitmap(0, 0);
         image.SetSource(File.OpenRead(path));
#endif
         BinaryBitmap binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         int rowNumber = binaryMap.Height / 2;
         BitArray row = binaryMap.getBlackRow(rowNumber, null);
         List<ExpandedPair> previousPairs = new List<ExpandedPair>();

         ExpandedPair pair1 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair1);
         FinderPattern finderPattern = pair1.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);

         ExpandedPair pair2 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair2);
         finderPattern = pair2.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(1, finderPattern.Value);

         ExpandedPair pair3 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
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
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         String path = "test/data/blackbox/rssexpanded-1/3.png";
         if (!File.Exists(path))
         {
            // Support running from project root too
            path = Path.Combine("..\\..\\..\\Source", path);
         }

#if !SILVERLIGHT
         var image = new Bitmap(Image.FromFile(path));
#else
         var image = new WriteableBitmap(0, 0);
         image.SetSource(File.OpenRead(path));
#endif
         BinaryBitmap binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         int rowNumber = binaryMap.Height / 2;
         BitArray row = binaryMap.getBlackRow(rowNumber, null);
         List<ExpandedPair> previousPairs = new List<ExpandedPair>();

         ExpandedPair pair1 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair1);
         FinderPattern finderPattern = pair1.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);

         ExpandedPair pair2 = rssExpandedReader.retrieveNextPair(row, previousPairs, rowNumber);
         previousPairs.Add(pair2);
         finderPattern = pair2.FinderPattern;
         Assert.IsNotNull(finderPattern);
         Assert.AreEqual(0, finderPattern.Value);
      }

      [Test]
      public void testDecodeCheckCharacter()
      {
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         String path = "test/data/blackbox/rssexpanded-1/3.png";
         if (!File.Exists(path))
         {
            // Support running from project root too
            path = Path.Combine("..\\..\\..\\Source", path);
         }

#if !SILVERLIGHT
         var image = new Bitmap(Image.FromFile(path));
#else
         var image = new WriteableBitmap(0, 0);
         image.SetSource(File.OpenRead(path));
#endif
         BinaryBitmap binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         BitArray row = binaryMap.getBlackRow(binaryMap.Height / 2, null);

         int[] startEnd = { 145, 243 };//image pixels where the A1 pattern starts (at 124) and ends (at 214)
         int value = 0;// A
#if !SILVERLIGHT
         FinderPattern finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.Height / 2);
#else
         FinderPattern finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.PixelHeight / 2);
#endif
         //{1, 8, 4, 1, 1};
         DataCharacter dataCharacter = rssExpandedReader.decodeDataCharacter(row, finderPatternA1, true, true);

         Assert.AreEqual(98, dataCharacter.Value);
      }

      [Test]
      public void testDecodeDataCharacter()
      {
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

         String path = "test/data/blackbox/rssexpanded-1/3.png";
         if (!File.Exists(path))
         {
            // Support running from project root too
            path = Path.Combine("..\\..\\..\\Source", path);
         }

#if !SILVERLIGHT
         var image = new Bitmap(Image.FromFile(path));
#else
         var image = new WriteableBitmap(0, 0);
         image.SetSource(File.OpenRead(path));
#endif
         BinaryBitmap binaryMap = new BinaryBitmap(new GlobalHistogramBinarizer(new BitmapLuminanceSource(image)));
         BitArray row = binaryMap.getBlackRow(binaryMap.Height / 2, null);

         int[] startEnd = { 145, 243 };//image pixels where the A1 pattern starts (at 124) and ends (at 214)
         int value = 0; // A
#if !SILVERLIGHT
         FinderPattern finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.Height / 2);
#else
         FinderPattern finderPatternA1 = new FinderPattern(value, startEnd, startEnd[0], startEnd[1], image.PixelHeight / 2);
#endif
         //{1, 8, 4, 1, 1};
         DataCharacter dataCharacter = rssExpandedReader.decodeDataCharacter(row, finderPatternA1, true, false);

         Assert.AreEqual(19, dataCharacter.Value);
         Assert.AreEqual(1007, dataCharacter.ChecksumPortion);
      }
   }
}