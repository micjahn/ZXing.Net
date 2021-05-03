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
    /// <p>For WPA2 enterprise(EAP), strings will be of the form:</p>
    /// <p>{@code WIFI:T:WPA2-EAP;S:[network SSID];H:[hidden?];E:[EAP method];PH2:[Phase 2 method];A:[anonymous identity];I:[username];P:[password];;}</p>
    /// <p>"EAP method" can e.g.be "TTLS" or "PWD" or one of the other fields in <a href = "https://developer.android.com/reference/android/net/wifi/WifiEnterpriseConfig.Eap.html"> WifiEnterpriseConfig.Eap </a> and "Phase 2 method" can e.g.be "MSCHAPV2" or any of the other fields in <a href = "https://developer.android.com/reference/android/net/wifi/WifiEnterpriseConfig.Phase2.html"> WifiEnterpriseConfig.Phase2 </a></p>
    /// </summary>
    /// <author>Vikram Aggarwal</author>
    /// <author>Sean Owen</author>
    /// <author>Steffen Kie�</author>
    public class WifiResultParser : ResultParser
    {
        /// <summary>
        /// attempt to parse the raw result to the specific type
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public override ParsedResult parse(ZXing.Result result)
        {
            var rawText = result.Text;
            if (!rawText.StartsWith("WIFI:"))
            {
                return null;
            }
            rawText = rawText.Substring("WIFI:".Length);
            var ssid = matchSinglePrefixedField("S:", rawText, ';', false);
            if (string.IsNullOrEmpty(ssid))
            {
                return null;
            }
            var pass = matchSinglePrefixedField("P:", rawText, ';', false);
            var type = matchSinglePrefixedField("T:", rawText, ';', false) ?? "nopass";

            // Unfortunately, in the past, H: was not just used for boolean 'hidden', but 'phase 2 method'.
            // To try to retain backwards compatibility, we set one or the other based on whether the string
            // is 'true' or 'false':
            bool hidden = false;
            String phase2Method = matchSinglePrefixedField("PH2:", rawText, ';', false);
            String hValue = matchSinglePrefixedField("H:", rawText, ';', false);
            if (hValue != null)
            {
                // If PH2 was specified separately, or if the value is clearly boolean, interpret it as 'hidden'
                if (phase2Method != null || String.Compare("true", hValue, StringComparison.OrdinalIgnoreCase) == 0 || String.Compare("false", hValue, StringComparison.OrdinalIgnoreCase) == 0)
                {
#if WindowsCE
                    try { hidden = Boolean.Parse(hValue); } catch { }
#else
                    Boolean.TryParse(hValue, out hidden);
#endif
                }
                else
                {
                    phase2Method = hValue;
                }
            }

            var identity = matchSinglePrefixedField("I:", rawText, ';', false);
            var anonymousIdentity = matchSinglePrefixedField("A:", rawText, ';', false);
            var eapMethod = matchSinglePrefixedField("E:", rawText, ';', false);

            return new WifiParsedResult(type, ssid, pass, hidden, identity, anonymousIdentity, eapMethod, phase2Method);
        }
    }
}