/*
* Copyright 2008 ZXing authors
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
using System.Collections.Generic;

namespace ZXing.Client.Result
{
    /// <summary> Implements KDDI AU's address book format. See
    /// <a href="http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html">
    /// http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html</a>.
    /// (Thanks to Yuzo for translating!)
    /// 
    /// </summary>
    /// <author>  Sean Owen
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    sealed class AddressBookAUResultParser : ResultParser
    {
        override public ParsedResult parse(ZXing.Result result)
        {
            var rawText = result.Text;
            // MEMORY is mandatory; seems like a decent indicator, as does end-of-record separator CR/LF
            if (rawText == null || rawText.IndexOf("MEMORY") < 0 || rawText.IndexOf("\r\n") < 0)
            {
                return null;
            }

            // NAME1 and NAME2 have specific uses, namely written name and pronunciation, respectively.
            // Therefore we treat them specially instead of as an array of names.
            var name = matchSinglePrefixedField("NAME1:", rawText, '\r', true);
            var pronunciation = matchSinglePrefixedField("NAME2:", rawText, '\r', true);

            var phoneNumbers = matchMultipleValuePrefix("TEL", 3, rawText, true);
            var emails = matchMultipleValuePrefix("MAIL", 3, rawText, true);
            var note = matchSinglePrefixedField("MEMORY:", rawText, '\r', false);
            var address = matchSinglePrefixedField("ADD:", rawText, '\r', true);
            var addresses = address == null ? null : new[] { address };
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
                                               null,
                                               null,
                                               null,
                                               null,
                                               null);
        }

        private static String[] matchMultipleValuePrefix(String prefix, int max, String rawText, bool trim)
        {
            IList<string> values = null;
            for (int i = 1; i <= max; i++)
            {
                var value = matchSinglePrefixedField(prefix + i + ':', rawText, '\r', trim);
                if (value == null)
                {
                    break;
                }
                if (values == null)
                {
                    values = new List<string>();
                }
                values.Add(value);
            }
            if (values == null)
            {
                return null;
            }
            return SupportClass.toStringArray(values);
        }
    }
}