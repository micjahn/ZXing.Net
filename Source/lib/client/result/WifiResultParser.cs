/*
 * Copyright 2010 ZXing authors
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

using System;

namespace ZXing.Client.Result
{
   /// <summary>
   /// Parses a WIFI configuration string.  Strings will be of the form:
   /// WIFI:T:WPA;S:mynetwork;P:mypass;;
   /// The fields can come in any order, and there should be tests to see
   /// if we can parse them all correctly.
   /// </summary>
   /// <author>Vikram Aggarwal</author>
   public class WifiResultParser : ResultParser
   {
      override public ParsedResult parse(ZXing.Result result)
      {
         String rawText = result.Text;
         if (!rawText.StartsWith("WIFI:"))
         {
            return null;
         }
         // Don't remove leading or trailing whitespace
         bool trim = false;
         String ssid = matchSinglePrefixedField("S:", rawText, ';', trim);
         if (string.IsNullOrEmpty(ssid))
         {
            return null;
         }
         String pass = matchSinglePrefixedField("P:", rawText, ';', trim);
         String type = matchSinglePrefixedField("T:", rawText, ';', trim);
         if (type == null)
         {
            type = "nopass";
         }

         return new WifiParsedResult(type, ssid, pass);
      }
   }
}