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
   /// Implements the "MATMSG" email message entry format.
   /// 
   /// Supported keys: TO, SUB, BODY
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class EmailDoCoMoResultParser : AbstractDoCoMoResultParser
   {
      private static Regex ATEXT_ALPHANUMERIC = new Regex("^[a-zA-Z0-9@.!#$%&'*+\\-/=?^_`{|}~]+$"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE)
         , RegexOptions.Compiled);
#else
);
#endif

      override public ParsedResult parse(ZXing.Result result)
      {
         String rawText = result.Text;
         if (!rawText.StartsWith("MATMSG:"))
         {
            return null;
         }
         String[] rawTo = matchDoCoMoPrefixedField("TO:", rawText, true);
         if (rawTo == null)
         {
            return null;
         }
         String to = rawTo[0];
         if (!isBasicallyValidEmailAddress(to))
         {
            return null;
         }
         String subject = matchSingleDoCoMoPrefixedField("SUB:", rawText, false);
         String body = matchSingleDoCoMoPrefixedField("BODY:", rawText, false);

         return new EmailAddressParsedResult(to, subject, body, "mailto:" + to);
      }

      /**
       * This implements only the most basic checking for an email address's validity -- that it contains
       * an '@' and contains no characters disallowed by RFC 2822. This is an overly lenient definition of
       * validity. We want to generally be lenient here since this class is only intended to encapsulate what's
       * in a barcode, not "judge" it.
       */

      internal static bool isBasicallyValidEmailAddress(String email)
      {
         return email != null && ATEXT_ALPHANUMERIC.Match(email).Success && email.IndexOf('@') >= 0;
      }
   }
}