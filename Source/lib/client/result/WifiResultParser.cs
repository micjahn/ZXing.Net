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
   /// <p>Parses a WIFI configuration string. Strings will be of the form:</p>
   /// <p>{@code WIFI:T:[network type];S:[network SSID];P:[network password];H:[hidden?];;}</p>
   /// <p>The fields can appear in any order. Only "S:" is required.</p>
   /// </summary>
   /// <author>Vikram Aggarwal</author>
   /// <author>Sean Owen</author>
   public class WifiResultParser : ResultParser
   {
      override public ParsedResult parse(ZXing.Result result)
      {
         var rawText = result.Text;
         if (!rawText.StartsWith("WIFI:"))
         {
            return null;
         }
         var ssid = matchSinglePrefixedField("S:", rawText, ';', false);
         if (string.IsNullOrEmpty(ssid))
         {
            return null;
         }
         var pass = matchSinglePrefixedField("P:", rawText, ';', false);
         var type = matchSinglePrefixedField("T:", rawText, ';', false) ?? "nopass";

         bool hidden = false;
#if WindowsCE
         try { hidden = Boolean.Parse(matchSinglePrefixedField("H:", rawText, ';', false)); } catch { }
#else
         Boolean.TryParse(matchSinglePrefixedField("H:", rawText, ';', false), out hidden);
#endif

         return new WifiParsedResult(type, ssid, pass, hidden);
      }
   }
}