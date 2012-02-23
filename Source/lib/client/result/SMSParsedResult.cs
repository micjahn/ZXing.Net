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
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class SMSParsedResult : ParsedResult
   {
      private String[] numbers;
      private String[] vias;
      private String subject;
      private String body;

      public SMSParsedResult(String number,
                             String via,
                             String subject,
                             String body)
         : base(ParsedResultType.SMS)
      {
         this.numbers = new [] { number };
         this.vias = new [] { via };
         this.subject = subject;
         this.body = body;
      }

      public SMSParsedResult(String[] numbers,
                             String[] vias,
                             String subject,
                             String body)
         : base(ParsedResultType.SMS)
      {
         this.numbers = numbers;
         this.vias = vias;
         this.subject = subject;
         this.body = body;
      }

      public String getSMSURI()
      {
         var result = new StringBuilder();
         result.Append("sms:");
         bool first = true;
         for (int i = 0; i < numbers.Length; i++)
         {
            if (first)
            {
               first = false;
            }
            else
            {
               result.Append(',');
            }
            result.Append(numbers[i]);
            if (vias != null && vias[i] != null)
            {
               result.Append(";via=");
               result.Append(vias[i]);
            }
         }
         bool hasBody = body != null;
         bool hasSubject = subject != null;
         if (hasBody || hasSubject)
         {
            result.Append('?');
            if (hasBody)
            {
               result.Append("body=");
               result.Append(body);
            }
            if (hasSubject)
            {
               if (hasBody)
               {
                  result.Append('&');
               }
               result.Append("subject=");
               result.Append(subject);
            }
         }
         return result.ToString();
      }

      public String[] Numbers
      {
         get
         {
            return numbers;
         }

      }

      public String[] Vias
      {
         get
         {
            return vias;
         }

      }

      public String Subject
      {
         get
         {
            return subject;
         }

      }

      public String Body
      {
         get
         {
            return body;
         }

      }

      override public String DisplayResult
      {
         get
         {
            var result = new StringBuilder(100);
            maybeAppend(numbers, result);
            maybeAppend(subject, result);
            maybeAppend(body, result);
            return result.ToString();
         }
      }
   }
}