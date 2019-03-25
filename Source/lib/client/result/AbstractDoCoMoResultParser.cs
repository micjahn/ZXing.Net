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
    /// <summary> <p>See
    /// <a href="http://www.nttdocomo.co.jp/english/service/imode/make/content/barcode/about/s2.html">
    /// DoCoMo's documentation</a> about the result types represented by subclasses of this class.</p>
    /// 
    /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
    /// on exception-based mechanisms during parsing.</p>
    /// 
    /// </summary>
    /// <author>  Sean Owen
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    abstract class AbstractDoCoMoResultParser : ResultParser
    {
        internal static String[] matchDoCoMoPrefixedField(String prefix, String rawText, bool trim)
        {
            return matchPrefixedField(prefix, rawText, ';', trim);
        }

        internal static String matchSingleDoCoMoPrefixedField(String prefix, String rawText, bool trim)
        {
            return matchSinglePrefixedField(prefix, rawText, ';', trim);
        }
    }
}