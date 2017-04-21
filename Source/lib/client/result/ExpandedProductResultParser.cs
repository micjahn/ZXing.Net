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
using System.Text;

namespace ZXing.Client.Result
{
   /// <summary>
   /// Parses strings of digits that represent a RSS Extended code.
   /// </summary>
   /// <author>Antonio Manuel Benjumea Conde, Servinform, S.A.</author>
   /// <author>Agustín Delgado, Servinform, S.A.</author>
   public class ExpandedProductResultParser : ResultParser
   {
      /// <summary>
      /// tries to parse a text representation to a specific result object
      /// </summary>
      /// <param name="result"></param>
      /// <returns></returns>
      public override ParsedResult parse(ZXing.Result result)
      {
         BarcodeFormat format = result.BarcodeFormat;
         if (format != BarcodeFormat.RSS_EXPANDED)
         {
            // ExtendedProductParsedResult NOT created. Not a RSS Expanded barcode
            return null;
         }
         String rawText = result.Text;

         String productID = null;
         String sscc = null;
         String lotNumber = null;
         String productionDate = null;
         String packagingDate = null;
         String bestBeforeDate = null;
         String expirationDate = null;
         String weight = null;
         String weightType = null;
         String weightIncrement = null;
         String price = null;
         String priceIncrement = null;
         String priceCurrency = null;
         var uncommonAIs = new Dictionary<String, String>();

         int i = 0;

         while (i < rawText.Length)
         {
            String ai = findAIvalue(i, rawText);
            if (ai == null)
            {
               // Error. Code doesn't match with RSS expanded pattern
               // ExtendedProductParsedResult NOT created. Not match with RSS Expanded pattern
               return null;
            }
            i += ai.Length + 2;
            String value = findValue(i, rawText);
            i += value.Length;

            if ("00".Equals(ai))
            {
               sscc = value;
            }
            else if ("01".Equals(ai))
            {
               productID = value;
            }
            else if ("10".Equals(ai))
            {
               lotNumber = value;
            }
            else if ("11".Equals(ai))
            {
               productionDate = value;
            }
            else if ("13".Equals(ai))
            {
               packagingDate = value;
            }
            else if ("15".Equals(ai))
            {
               bestBeforeDate = value;
            }
            else if ("17".Equals(ai))
            {
               expirationDate = value;
            }
            else if ("3100".Equals(ai) || "3101".Equals(ai)
              || "3102".Equals(ai) || "3103".Equals(ai)
              || "3104".Equals(ai) || "3105".Equals(ai)
              || "3106".Equals(ai) || "3107".Equals(ai)
              || "3108".Equals(ai) || "3109".Equals(ai))
            {
               weight = value;
               weightType = ExpandedProductParsedResult.KILOGRAM;
               weightIncrement = ai.Substring(3);
            }
            else if ("3200".Equals(ai) || "3201".Equals(ai)
              || "3202".Equals(ai) || "3203".Equals(ai)
              || "3204".Equals(ai) || "3205".Equals(ai)
              || "3206".Equals(ai) || "3207".Equals(ai)
              || "3208".Equals(ai) || "3209".Equals(ai))
            {
               weight = value;
               weightType = ExpandedProductParsedResult.POUND;
               weightIncrement = ai.Substring(3);
            }
            else if ("3920".Equals(ai) || "3921".Equals(ai)
              || "3922".Equals(ai) || "3923".Equals(ai))
            {
               price = value;
               priceIncrement = ai.Substring(3);
            }
            else if ("3930".Equals(ai) || "3931".Equals(ai)
              || "3932".Equals(ai) || "3933".Equals(ai))
            {
               if (value.Length < 4)
               {
                  // The value must have more of 3 symbols (3 for currency and
                  // 1 at least for the price)
                  // ExtendedProductParsedResult NOT created. Not match with RSS Expanded pattern
                  return null;
               }
               price = value.Substring(3);
               priceCurrency = value.Substring(0, 3);
               priceIncrement = ai.Substring(3);
            }
            else
            {
               // No match with common AIs
               uncommonAIs[ai] = value;
            }
         }

         return new ExpandedProductParsedResult(rawText,
                                                productID,
                                                sscc,
                                                lotNumber,
                                                productionDate,
                                                packagingDate,
                                                bestBeforeDate,
                                                expirationDate,
                                                weight,
                                                weightType,
                                                weightIncrement,
                                                price,
                                                priceIncrement,
                                                priceCurrency,
                                                uncommonAIs);
      }

      private static String findAIvalue(int i, String rawText)
      {
         char c = rawText[i];
         // First character must be a open parenthesis.If not, ERROR
         if (c != '(')
         {
            return null;
         }

         var rawTextAux = rawText.Substring(i + 1);
         var buf = new StringBuilder();

         for (int index = 0; index < rawTextAux.Length; index++)
         {
            char currentChar = rawTextAux[index];
            if (currentChar == ')')
            {
               return buf.ToString();
            }
            if (currentChar < '0' || currentChar > '9')
            {
               return null;
            }
            buf.Append(currentChar);
         }
         return buf.ToString();
      }

      private static String findValue(int i, String rawText)
      {
         var buf = new StringBuilder();
         var rawTextAux = rawText.Substring(i);

         for (int index = 0; index < rawTextAux.Length; index++)
         {
            char c = rawTextAux[index];
            if (c == '(')
            {
               // We look for a new AI. If it doesn't exist (ERROR), we continue
               // with the iteration
               if (findAIvalue(index, rawTextAux) != null)
               {
                  break;
               }
               buf.Append('(');
            }
            else
            {
               buf.Append(c);
            }
         }
         return buf.ToString();
      }
   }
}