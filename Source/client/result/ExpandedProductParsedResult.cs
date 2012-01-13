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

namespace com.google.zxing.client.result
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

      private String productID;
      private String sscc;
      private String lotNumber;
      private String productionDate;
      private String packagingDate;
      private String bestBeforeDate;
      private String expirationDate;
      private String weight;
      private String weightType;
      private String weightIncrement;
      private String price;
      private String priceIncrement;
      private String priceCurrency;
      // For AIS that not exist in this object
      private IDictionary<String, String> uncommonAIs;

      public ExpandedProductParsedResult(String productID,
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

      override public String DisplayResult
      {
         get { return productID; }
      }

      public override string ToString()
      {
         return DisplayResult;
      }
   }
}