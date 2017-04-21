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
   /// <summary>
   /// Represents a parsed result that encodes an email message including recipients, subject and body text.
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class EmailAddressParsedResult : ParsedResult
   {
      /// <summary>
      /// the email address
      /// </summary>
      public String EmailAddress
      {
         get
         {
            return Tos == null || Tos.Length == 0 ? null : Tos[0];
         }
      }
      /// <summary>
      /// the TOs
      /// </summary>
      public String[] Tos { get; private set; }
      /// <summary>
      /// the CCs
      /// </summary>
      public String[] CCs { get; private set; }
      /// <summary>
      /// the BCCs
      /// </summary>
      public String[] BCCs { get; private set; }
      /// <summary>
      /// the subject
      /// </summary>
      public String Subject { get; private set; }
      /// <summary>
      /// the body
      /// </summary>
      public String Body { get; private set; }
      /// <summary>
      /// the mailto: uri
      /// </summary>
      [Obsolete("deprecated without replacement")]
      public String MailtoURI { get { return "mailto:"; } }

      internal EmailAddressParsedResult(String to)
         : this(new[] {to}, null, null, null, null)
      {
      }

      internal EmailAddressParsedResult(String[] tos,
                               String[] ccs,
                               String[] bccs,
                               String subject,
                               String body)
         : base(ParsedResultType.EMAIL_ADDRESS)
      {
         Tos = tos;
         CCs = ccs;
         BCCs = bccs;
         Subject = subject;
         Body = body;

         var result = new StringBuilder(30);
         maybeAppend(Tos, result);
         maybeAppend(CCs, result);
         maybeAppend(BCCs, result);
         maybeAppend(Subject, result);
         maybeAppend(Body, result);
         displayResultValue = result.ToString();
      }
   }
}