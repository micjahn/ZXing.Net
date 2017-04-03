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
   /// A simple result type encapsulating a URI that has no further interpretation.
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class URIParsedResult : ParsedResult
   {
      private static readonly Regex USER_IN_HOST = new Regex(":/*([^/@]+)@[^/]+"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
         , RegexOptions.Compiled);
#else
);
#endif

      public String URI { get; private set; }

      public String Title { get; private set; }

      /// <returns> true if the URI contains suspicious patterns that may suggest it intends to
      /// mislead the user about its true nature. At the moment this looks for the presence
      /// of user/password syntax in the host/authority portion of a URI which may be used
      /// in attempts to make the URI's host appear to be other than it is. Example:
      /// http://yourbank.com@phisher.com  This URI connects to phisher.com but may appear
      /// to connect to yourbank.com at first glance.
      /// </returns>
      public bool PossiblyMaliciousURI { get; private set; }

      public URIParsedResult(String uri, String title)
         : base(ParsedResultType.URI)
      {
         URI = massageURI(uri);
         Title = title;
         PossiblyMaliciousURI = USER_IN_HOST.Match(URI).Success;

         var result = new System.Text.StringBuilder(30);
         maybeAppend(Title, result);
         maybeAppend(URI, result);
         displayResultValue = result.ToString();
      }

      /// <summary> Transforms a string that represents a URI into something more proper, by adding or canonicalizing
      /// the protocol.
      /// </summary>
      private static String massageURI(String uri)
      {
         int protocolEnd = uri.IndexOf(':');
         if (protocolEnd < 0 || isColonFollowedByPortNumber(uri, protocolEnd))
         {
            // No protocol, or found a colon, but it looks like it is after the host, so the protocol is still missing,
            // so assume http
            uri = "http://" + uri;
         }
         return uri;
      }
      private static bool isColonFollowedByPortNumber(String uri, int protocolEnd)
      {
         int start = protocolEnd + 1;
         int nextSlash = uri.IndexOf('/', start);
         if (nextSlash < 0)
         {
            nextSlash = uri.Length;
         }
         return ResultParser.isSubstringOfDigits(uri, start, nextSlash - start);
      }
   }
}