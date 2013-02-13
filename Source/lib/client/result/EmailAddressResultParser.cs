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
   /// <summary> Represents a result that encodes an e-mail address, either as a plain address
   /// like "joe@example.org" or a mailto: URL like "mailto:joe@example.org".
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class EmailAddressResultParser : ResultParser
   {
      override public ParsedResult parse(ZXing.Result result)
      {
         String rawText = result.Text;
         if (rawText == null)
         {
            return null;
         }
         String emailAddress;
         if (rawText.ToLower().StartsWith("mailto:"))
         {
            // If it starts with mailto:, assume it is definitely trying to be an email address
            emailAddress = rawText.Substring(7);
            int queryStart = emailAddress.IndexOf('?');
            if (queryStart >= 0)
            {
               emailAddress = emailAddress.Substring(0, queryStart);
            }
            emailAddress = urlDecode(emailAddress);
            var nameValues = parseNameValuePairs(rawText);
            String subject = null;
            String body = null;
            if (nameValues != null)
            {
               if (emailAddress.Length == 0)
               {
                  emailAddress = nameValues["to"];
               }
               subject = nameValues["subject"];
               body = nameValues["body"];
            }
            return new EmailAddressParsedResult(emailAddress, subject, body, rawText);
         }

         if (!EmailDoCoMoResultParser.isBasicallyValidEmailAddress(rawText))
         {
            return null;
         }
         emailAddress = rawText;
         return new EmailAddressParsedResult(emailAddress, null, null, "mailto:" + emailAddress);
      }
   }
}