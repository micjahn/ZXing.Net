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

using NUnit.Framework;

namespace ZXing.Client.Result.Test
{
   /// <summary>
   /// <author>Antonio Manuel Benjumea Conde, Servinform, S.A.</author>
   /// <author>Agustín Delgado, Servinform, S.A.</author>
   /// </summary>
   [TestFixture]
   public sealed class ExpandedProductParsedResultTestCase
   {
      [Test]
      public void test_RSSExpanded()
      {
         IDictionary<String, String> uncommonAIs = new Dictionary<String, String>();
         uncommonAIs["123"] = "544654";
         ZXing.Result result =
             new ZXing.Result("(01)66546(13)001205(3932)4455(3102)6544(123)544654", null, null, BarcodeFormat.RSS_EXPANDED);
         var o = (ExpandedProductParsedResult)new ExpandedProductResultParser().parse(result);
         Assert.IsNotNull(o);
         Assert.AreEqual("66546", o.ProductID);
         Assert.IsNull(o.Sscc);
         Assert.IsNull(o.LotNumber);
         Assert.IsNull(o.ProductionDate);
         Assert.AreEqual("001205", o.PackagingDate);
         Assert.IsNull(o.BestBeforeDate);
         Assert.IsNull(o.ExpirationDate);
         Assert.AreEqual("6544", o.Weight);
         Assert.AreEqual("KG", o.WeightType);
         Assert.AreEqual("2", o.WeightIncrement);
         Assert.AreEqual("5", o.Price);
         Assert.AreEqual("2", o.PriceIncrement);
         Assert.AreEqual("445", o.PriceCurrency);
         Assert.AreEqual(uncommonAIs, o.UncommonAIs);
      }
   }
}