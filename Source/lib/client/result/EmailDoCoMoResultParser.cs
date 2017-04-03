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
   /// </summary>
   /// <author>Sean Owen</author>
   internal sealed class EmailDoCoMoResultParser : AbstractDoCoMoResultParser
   {
      private static readonly Regex ATEXT_ALPHANUMERIC = new Regex(@"\A(?:" + "[a-zA-Z0-9@.!#$%&'*+\\-/=?^_`{|}~]+" + @")\z"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
                                                                   , RegexOptions.Compiled);
#else
);
#endif

      public override ParsedResult parse(ZXing.Result result)
      {
         var rawText = result.Text;
         if (!rawText.StartsWith("MATMSG:"))
         {
            return null;
         }

         var tos = matchDoCoMoPrefixedField("TO:", rawText, true);
         if (tos == null)
         {
            return null;
         }
         for (var index = 0; index < tos.Length; index++)
         {
            var to = tos[index];
            if (!isBasicallyValidEmailAddress(to))
            {
               return null;
            }
         }
         var subject = matchSingleDoCoMoPrefixedField("SUB:", rawText, false);
         var body = matchSingleDoCoMoPrefixedField("BODY:", rawText, false);
         return new EmailAddressParsedResult(tos, null, null, subject, body);
      }

      /// <summary>
      /// This implements only the most basic checking for an email address's validity -- that it contains
      /// an '@' and contains no characters disallowed by RFC 2822. This is an overly lenient definition of
      /// validity. We want to generally be lenient here since this class is only intended to encapsulate what's
      /// in a barcode, not "judge" it.
      /// </summary>
      /// <param name="email">The email.</param>
      /// <returns>
      ///   <c>true</c> if it is basically a valid email address; otherwise, <c>false</c>.
      /// </returns>
      internal static bool isBasicallyValidEmailAddress(String email)
      {
         return email != null && ATEXT_ALPHANUMERIC.Match(email).Success && email.IndexOf('@') >= 0;
      }
   }
}