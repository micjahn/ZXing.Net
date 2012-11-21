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

namespace ZXing.Client.Result
{
   /// <summary>
   /// 
   /// </summary>
   /// <author> Antonio Manuel Benjumea Conde, Servinform, S.A.</author>
   /// <author> Agustín Delgado, Servinform, S.A.</author>
   public class ExpandedProductParsedResult : ParsedResult
   {
      public static String KILOGRAM = "KG";
      public static String POUND = "LB";

      private readonly String rawText;
      private readonly String productID;
      private readonly String sscc;
      private readonly String lotNumber;
      private readonly String productionDate;
      private readonly String packagingDate;
      private readonly String bestBeforeDate;
      private readonly String expirationDate;
      private readonly String weight;
      private readonly String weightType;
      private readonly String weightIncrement;
      private readonly String price;
      private readonly String priceIncrement;
      private readonly String priceCurrency;
      // For AIS that not exist in this object
      private readonly IDictionary<String, String> uncommonAIs;

      public ExpandedProductParsedResult(String rawText,
                                         String productID,
                                         String sscc,
                                         String lotNumber,
                                         String productionDate,
                                         String packagingDate,
                                         String bestBeforeDate,
                                         String expirationDate,
                                         String weight,
                                         String weightType,
                                         String weightIncrement,
                                         String price,
                                         String priceIncrement,
                                         String priceCurrency,
                                         IDictionary<String, String> uncommonAIs)
         : base(ParsedResultType.PRODUCT)
      {
         this.rawText = rawText;
         this.productID = productID;
         this.sscc = sscc;
         this.lotNumber = lotNumber;
         this.productionDate = productionDate;
         this.packagingDate = packagingDate;
         this.bestBeforeDate = bestBeforeDate;
         this.expirationDate = expirationDate;
         this.weight = weight;
         this.weightType = weightType;
         this.weightIncrement = weightIncrement;
         this.price = price;
         this.priceIncrement = priceIncrement;
         this.priceCurrency = priceCurrency;
         this.uncommonAIs = uncommonAIs;

         displayResultValue = productID;
      }

      override public bool Equals(Object o)
      {
         if (!(o is ExpandedProductParsedResult))
         {
            return false;
         }

         var other = (ExpandedProductParsedResult)o;

         return equalsOrNull(productID, other.productID)
             && equalsOrNull(sscc, other.sscc)
             && equalsOrNull(lotNumber, other.lotNumber)
             && equalsOrNull(productionDate, other.productionDate)
             && equalsOrNull(bestBeforeDate, other.bestBeforeDate)
             && equalsOrNull(expirationDate, other.expirationDate)
             && equalsOrNull(weight, other.weight)
             && equalsOrNull(weightType, other.weightType)
             && equalsOrNull(weightIncrement, other.weightIncrement)
             && equalsOrNull(price, other.price)
             && equalsOrNull(priceIncrement, other.priceIncrement)
             && equalsOrNull(priceCurrency, other.priceCurrency)
             && equalsOrNull(uncommonAIs, other.uncommonAIs);
      }

      private static bool equalsOrNull(Object o1, Object o2)
      {
         return o1 == null ? o2 == null : o1.Equals(o2);
      }

      private static bool equalsOrNull(IDictionary<String, String> o1, IDictionary<String, String> o2)
      {
         if (o1 == null)
            return o2 == null;
         if (o1.Count != o2.Count)
            return false;
         foreach (var entry in o1)
         {
            if (!o2.ContainsKey(entry.Key))
               return false;
            if (!entry.Value.Equals(o2[entry.Key]))
               return false;
         }
         return true;
      }

      override public int GetHashCode()
      {
         int hash = 0;
         hash ^= hashNotNull(productID);
         hash ^= hashNotNull(sscc);
         hash ^= hashNotNull(lotNumber);
         hash ^= hashNotNull(productionDate);
         hash ^= hashNotNull(bestBeforeDate);
         hash ^= hashNotNull(expirationDate);
         hash ^= hashNotNull(weight);
         hash ^= hashNotNull(weightType);
         hash ^= hashNotNull(weightIncrement);
         hash ^= hashNotNull(price);
         hash ^= hashNotNull(priceIncrement);
         hash ^= hashNotNull(priceCurrency);
         hash ^= hashNotNull(uncommonAIs);
         return hash;
      }

      private static int hashNotNull(Object o)
      {
         return o == null ? 0 : o.GetHashCode();
      }

      public String RawText
      {
         get { return rawText; }
      }

      public String ProductID
      {
         get { return productID; }
      }

      public String Sscc
      {
         get { return sscc; }
      }

      public String LotNumber
      {
         get { return lotNumber; }
      }

      public String ProductionDate
      {
         get { return productionDate; }
      }

      public String PackagingDate
      {
         get { return packagingDate; }
      }

      public String BestBeforeDate
      {
         get { return bestBeforeDate; }
      }

      public String ExpirationDate
      {
         get { return expirationDate; }
      }

      public String Weight
      {
         get { return weight; }
      }

      public String WeightType
      {
         get { return weightType; }
      }

      public String WeightIncrement
      {
         get { return weightIncrement; }
      }

      public String Price
      {
         get { return price; }
      }

      public String PriceIncrement
      {
         get { return priceIncrement; }
      }

      public String PriceCurrency
      {
         get { return priceCurrency; }
      }

      public IDictionary<String, String> UncommonAIs
      {
         get { return uncommonAIs; }
      }

      public override string DisplayResult
      {
         get
         {
            return rawText;
         }
      }
   }
}