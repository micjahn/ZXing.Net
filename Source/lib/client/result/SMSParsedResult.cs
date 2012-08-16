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
using System.Text;

namespace ZXing.Client.Result
{
   /// <author>Sean Owen</author>
   public sealed class SMSParsedResult : ParsedResult
   {
      public SMSParsedResult(String number,
                             String via,
                             String subject,
                             String body)
         : this(new[] { number }, new[] { via }, subject, body)
      {
      }

      public SMSParsedResult(String[] numbers,
                             String[] vias,
                             String subject,
                             String body)
         : base(ParsedResultType.SMS)
      {
         Numbers = numbers;
         Vias = vias;
         Subject = subject;
         Body = body;
         SMSURI = getSMSURI();

         var result = new StringBuilder(100);
         maybeAppend(Numbers, result);
         maybeAppend(Subject, result);
         maybeAppend(Body, result);
         displayResultValue = result.ToString();
      }

      private String getSMSURI()
      {
         var result = new StringBuilder();
         result.Append("sms:");
         bool first = true;
         for (int i = 0; i < Numbers.Length; i++)
         {
            if (first)
            {
               first = false;
            }
            else
            {
               result.Append(',');
            }
            result.Append(Numbers[i]);
            if (Vias != null && Vias[i] != null)
            {
               result.Append(";via=");
               result.Append(Vias[i]);
            }
         }
         bool hasBody = Body != null;
         bool hasSubject = Subject != null;
         if (hasBody || hasSubject)
         {
            result.Append('?');
            if (hasBody)
            {
               result.Append("body=");
               result.Append(Body);
            }
            if (hasSubject)
            {
               if (hasBody)
               {
                  result.Append('&');
               }
               result.Append("subject=");
               result.Append(Subject);
            }
         }
         return result.ToString();
      }

      public String[] Numbers { get; private set; }

      public String[] Vias { get; private set; }

      public String Subject { get; private set; }

      public String Body { get; private set; }

      public String SMSURI { get; private set; }
   }
}