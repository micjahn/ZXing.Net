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

namespace com.google.zxing.client.result.optional
{
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class NDEFSmartPosterParsedResult : ParsedResult
   {
      public String Title { get; private set; }
      public String URI { get; private set; }
      public int Action { get; private set; }

      override public String DisplayResult
      {
         get
         {
            return Title == null ? URI : Title + '\n' + URI;
         }
      }

      public const int ACTION_UNSPECIFIED = -1;
      public const int ACTION_DO = 0;
      public const int ACTION_SAVE = 1;
      public const int ACTION_OPEN = 2;

      internal NDEFSmartPosterParsedResult(int action, String uri, String title)
         : base(ParsedResultType.NDEF_SMART_POSTER)
      {
         Action = action;
         URI = uri;
         Title = title;
      }
   }
}