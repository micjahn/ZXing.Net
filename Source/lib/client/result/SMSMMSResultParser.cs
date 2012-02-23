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
   /// <summary> <p>Parses an "sms:" URI result, which specifies a number to SMS and optional
   /// "via" number. See <a href="http://gbiv.com/protocols/uri/drafts/draft-antti-gsm-sms-url-04.txt">
   /// the IETF draft</a> on this.</p>
   /// 
   /// <p>This actually also parses URIs starting with "mms:", "smsto:", "mmsto:", "SMSTO:", and
   /// "MMSTO:", and treats them all the same way, and effectively converts them to an "sms:" URI
   /// for purposes of forwarding to the platform.</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class SMSMMSResultParser : ResultParser
   {
      override public ParsedResult parse(ZXing.Result result)
      {
         String rawText = result.Text;
         if (rawText == null ||
             !(rawText.StartsWith("sms:") || rawText.StartsWith("SMS:") ||
               rawText.StartsWith("mms:") || rawText.StartsWith("MMS:")))
         {
            return null;
         }

         // Check up front if this is a URI syntax string with query arguments
         var nameValuePairs = parseNameValuePairs(rawText);
         String subject = null;
         String body = null;
         var querySyntax = false;
         if (nameValuePairs != null && nameValuePairs.Count != 0)
         {
            subject = nameValuePairs["subject"];
            body = nameValuePairs["body"];
            querySyntax = true;
         }

         // Drop sms, query portion
         //UPGRADE_WARNING: Method 'java.lang.String.indexOf' was converted to 'System.String.IndexOf' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
         var queryStart = rawText.IndexOf('?', 4);
         String smsURIWithoutQuery;
         // If it's not query syntax, the question mark is part of the subject or message
         if (queryStart < 0 || !querySyntax)
         {
            smsURIWithoutQuery = rawText.Substring(4);
         }
         else
         {
            smsURIWithoutQuery = rawText.Substring(4, (queryStart) - (4));
         }

         int lastComma = -1;
         int comma;
         var numbers = new List<String>(1);
         var vias = new List<String>(1);
         while ((comma = smsURIWithoutQuery.IndexOf(',', lastComma + 1)) > lastComma)
         {
            String numberPart = smsURIWithoutQuery.Substring(lastComma + 1, comma);
            addNumberVia(numbers, vias, numberPart);
            lastComma = comma;
         }
         addNumberVia(numbers, vias, smsURIWithoutQuery.Substring(lastComma + 1));

         return new SMSParsedResult(SupportClass.toStringArray(numbers),
                                    SupportClass.toStringArray(vias),
                                    subject,
                                    body);
      }

      private static void addNumberVia(ICollection<String> numbers,
                                       ICollection<String> vias,
                                       String numberPart)
      {
         int numberEnd = numberPart.IndexOf(';');
         if (numberEnd < 0)
         {
            numbers.Add(numberPart);
            vias.Add(null);
         }
         else
         {
            numbers.Add(numberPart.Substring(0, numberEnd));
            String maybeVia = numberPart.Substring(numberEnd + 1);
            String via;
            if (maybeVia.StartsWith("via="))
            {
               via = maybeVia.Substring(4);
            }
            else
            {
               via = null;
            }
            vias.Add(via);
         }
      }
   }
}