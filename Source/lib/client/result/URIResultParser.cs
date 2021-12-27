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
using System.Text.RegularExpressions;

namespace ZXing.Client.Result
{
    /// <summary>
    /// Tries to parse results that are a URI of some kind.
    /// </summary>
    /// <author>Sean Owen</author>
    sealed class URIResultParser : ResultParser
    {
        private static readonly Regex ALLOWED_URI_CHARS_PATTERN = new Regex("^[-._~:/?#\\[\\]@!$&'()*+,;=%A-Za-z0-9]+$"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || UNITY || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
            , RegexOptions.Compiled);
#else
);
#endif
        private static readonly Regex USER_IN_HOST = new Regex(":/*([^/@]+)@[^/]+"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || UNITY || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
            , RegexOptions.Compiled);
#else
);
#endif

        // See http://www.ietf.org/rfc/rfc2396.txt
        private static readonly Regex URL_WITH_PROTOCOL_PATTERN = new Regex("[a-zA-Z][a-zA-Z0-9+-.]+:"
#if !(NETFX_CORE || PORTABLE || UNITY || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
, RegexOptions.Compiled);
#else
);
#endif
        private static readonly Regex URL_WITHOUT_PROTOCOL_PATTERN = new Regex(
             "([a-zA-Z0-9\\-]+\\.){1,6}[a-zA-Z]{2,}" + // host name elements
             "(:\\d{1,5})?" + // maybe port
             "(/|\\?|$)" // query, path or nothing
#if !(NETFX_CORE || PORTABLE || UNITY || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
              , RegexOptions.Compiled);
#else
);
#endif

        override public ParsedResult parse(ZXing.Result result)
        {
            String rawText = result.Text;
            // We specifically handle the odd "URL" scheme here for simplicity and add "URI" for fun
            // Assume anything starting this way really means to be a URI
            if (rawText.StartsWith("URL:") || rawText.StartsWith("URI:"))
            {
                return new URIParsedResult(rawText.Substring(4).Trim(), null);
            }
            rawText = rawText.Trim();
            if (!isBasicallyValidURI(rawText) || isPossiblyMaliciousURI(rawText))
            {
                return null;
            }
            return new URIParsedResult(rawText, null);
        }

        /**
         * @return true if the URI contains suspicious patterns that may suggest it intends to
         *  mislead the user about its true nature. At the moment this looks for the presence
         *  of user/password syntax in the host/authority portion of a URI which may be used
         *  in attempts to make the URI's host appear to be other than it is. Example:
         *  http://yourbank.com@phisher.com  This URI connects to phisher.com but may appear
         *  to connect to yourbank.com at first glance.
         */
        internal static bool isPossiblyMaliciousURI(String uri)
        {
            return !ALLOWED_URI_CHARS_PATTERN.Match(uri).Success || USER_IN_HOST.Match(uri).Success;
        }

        internal static bool isBasicallyValidURI(String uri)
        {
            if (uri.IndexOf(" ") >= 0)
            {
                // Quick hack check for a common case
                return false;
            }
            var m = URL_WITH_PROTOCOL_PATTERN.Match(uri);
            if (m.Success && m.Index == 0)
            {
                // match at start only
                return true;
            }
            m = URL_WITHOUT_PROTOCOL_PATTERN.Match(uri);
            return m.Success && m.Index == 0;
        }
    }
}