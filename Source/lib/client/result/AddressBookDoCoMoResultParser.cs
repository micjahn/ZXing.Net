/*
* Copyright 2007 ZXing authors
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
    /// <summary> Implements the "MECARD" address book entry format.
    /// 
    /// Supported keys: N, SOUND, TEL, EMAIL, NOTE, ADR, BDAY, URL, plus ORG
    /// Unsupported keys: TEL-AV, NICKNAME
    /// 
    /// Except for TEL, multiple values for keys are also not supported;
    /// the first one found takes precedence.
    /// 
    /// Our understanding of the MECARD format is based on this document:
    /// 
    /// http://www.mobicode.org.tw/files/OMIA%20Mobile%20Bar%20Code%20Standard%20v3.2.1.doc 
    /// 
    /// </summary>
    /// <author>  Sean Owen
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    sealed class AddressBookDoCoMoResultParser : AbstractDoCoMoResultParser
    {
        override public ParsedResult parse(ZXing.Result result)
        {
            String rawText = result.Text;
            if (rawText == null || !rawText.StartsWith("MECARD:"))
            {
                return null;
            }
            String[] rawName = matchDoCoMoPrefixedField("N:", rawText, true);
            if (rawName == null)
            {
                return null;
            }
            String name = parseName(rawName[0]);
            String pronunciation = matchSingleDoCoMoPrefixedField("SOUND:", rawText, true);
            String[] phoneNumbers = matchDoCoMoPrefixedField("TEL:", rawText, true);
            String[] emails = matchDoCoMoPrefixedField("EMAIL:", rawText, true);
            String note = matchSingleDoCoMoPrefixedField("NOTE:", rawText, false);
            String[] addresses = matchDoCoMoPrefixedField("ADR:", rawText, true);
            String birthday = matchSingleDoCoMoPrefixedField("BDAY:", rawText, true);
            if (!isStringOfDigits(birthday, 8))
            {
                // No reason to throw out the whole card because the birthday is formatted wrong.
                birthday = null;
            }
            String[] urls = matchDoCoMoPrefixedField("URL:", rawText, true);

            // Although ORG may not be strictly legal in MECARD, it does exist in VCARD and we might as well
            // honor it when found in the wild.
            String org = matchSingleDoCoMoPrefixedField("ORG:", rawText, true);

            return new AddressBookParsedResult(maybeWrap(name),
                                               null,
                                               pronunciation,
                                               phoneNumbers,
                                               null,
                                               emails,
                                               null,
                                               null,
                                               note,
                                               addresses,
                                               null,
                                               org,
                                               birthday,
                                               null,
                                               urls,
                                               null);
        }

        private static String parseName(String name)
        {
            int comma = name.IndexOf(',');
            if (comma >= 0)
            {
                // Format may be last,first; switch it around
                return name.Substring(comma + 1) + ' ' + name.Substring(0, comma);
            }
            return name;
        }
    }
}