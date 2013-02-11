/**
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
 *
 * This software consists of contributions made by many individuals,
 * listed below:
 *
 * <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
 * <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
 *
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", leaded by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 *
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
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Test;

namespace ZXing.OneD.RSS.Expanded.Test
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   [TestFixture]
   public sealed class RSSExpandedImage2resultTestCase
   {
      [Test]
      public void testDecodeRow2result_2()
      {
         // (01)90012345678908(3103)001750
         String path = "test/data/blackbox/rssexpanded-1/2.png";
         ExpandedProductParsedResult expected =
             new ExpandedProductParsedResult("(01)90012345678908(3103)001750", 
                                             "90012345678908",
                                             null, null, null, null, null, null,
                                             "001750",
                                             ExpandedProductParsedResult.KILOGRAM,
                                             "3", null, null, null, new Dictionary<String, String>());

         assertCorrectImage2result(path, expected);
      }

      private static void assertCorrectImage2result(String path, ExpandedProductParsedResult expected)
      {
         RSSExpandedReader rssExpandedReader = new RSSExpandedReader();

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

         Result theResult = rssExpandedReader.decodeRow(rowNumber, row, null);
         Assert.IsNotNull(theResult);

         Assert.AreEqual(BarcodeFormat.RSS_EXPANDED, theResult.BarcodeFormat);

         ParsedResult result = ResultParser.parseResult(theResult);

         Assert.AreEqual(expected, result);
      }
   }
}