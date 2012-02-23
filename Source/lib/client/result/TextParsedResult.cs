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
   /// <summary> A simple result type encapsulating a string that has no further
   /// interpretation.
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class TextParsedResult : ParsedResult
   {
      private String text;
      private String language;

      public TextParsedResult(String text, String language)
         : base(ParsedResultType.TEXT)
      {
         this.text = text;
         this.language = language;
      }

      public String Text
      {
         get { return text; }
      }

      public String Language
      {
         get { return language; }
      }

      override public String DisplayResult
      {
         get { return text; }
      }
   }
}