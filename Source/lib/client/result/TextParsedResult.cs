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
   /// <summary>
   /// A simple result type encapsulating a string that has no further interpretation.
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class TextParsedResult : ParsedResult
   {
      public TextParsedResult(String text, String language)
         : base(ParsedResultType.TEXT)
      {
         Text = text;
         Language = language;
         displayResultValue = text;
      }

      public String Text { get; private set; }

      public String Language { get; private set; }
   }
}