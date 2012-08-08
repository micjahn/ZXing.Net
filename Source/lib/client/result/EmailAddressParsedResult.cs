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
using System.Text;

namespace ZXing.Client.Result
{
   /// <author>Sean Owen</author>
   public sealed class EmailAddressParsedResult : ParsedResult
   {
      public String EmailAddress { get; private set; }
      public String Subject { get; private set; }
      public String Body { get; private set; }
      public String MailtoURI { get; private set; }

      internal EmailAddressParsedResult(String emailAddress, String subject, String body, String mailtoURI)
         : base(ParsedResultType.EMAIL_ADDRESS)
      {
         EmailAddress = emailAddress;
         Subject = subject;
         Body = body;
         MailtoURI = mailtoURI;

         var result = new StringBuilder(30);
         maybeAppend(EmailAddress, result);
         maybeAppend(Subject, result);
         maybeAppend(Body, result);
         displayResult = result.ToString();
      }
   }
}