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

namespace com.google.zxing.client.result
{

   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class AddressBookParsedResult : ParsedResult
   {
      public String[] Names { get; private set; }

      /// <summary> In Japanese, the name is written in kanji, which can have multiple readings. Therefore a hint
      /// is often provided, called furigana, which spells the name phonetically.
      /// 
      /// </summary>
      /// <returns> The pronunciation of the getNames() field, often in hiragana or katakana.
      /// </returns>
      public String Pronunciation { get; private set; }
      public String[] PhoneNumbers { get; private set; }
      public String[] Emails { get; private set; }
      public String Note { get; private set; }
      public String[] Addresses { get; private set; }
      public String Title { get; private set; }
      public String Org { get; private set; }
      public String URL { get; private set; }
      /// <returns> birthday formatted as yyyyMMdd (e.g. 19780917)
      /// </returns>
      public String Birthday { get; private set; }

      override public String DisplayResult
      {
         get
         {
            var result = new StringBuilder(100);
            maybeAppend(Names, result);
            maybeAppend(Pronunciation, result);
            maybeAppend(Title, result);
            maybeAppend(Org, result);
            maybeAppend(Addresses, result);
            maybeAppend(PhoneNumbers, result);
            maybeAppend(Emails, result);
            maybeAppend(URL, result);
            maybeAppend(Birthday, result);
            maybeAppend(Note, result);
            return result.ToString();
         }

      }

      public AddressBookParsedResult(String[] names, String pronunciation, String[] phoneNumbers, String[] emails, String note, String[] addresses, String org, String birthday, String title, String url)
         : base(ParsedResultType.ADDRESSBOOK)
      {
         Names = names;
         Pronunciation = pronunciation;
         PhoneNumbers = phoneNumbers;
         Emails = emails;
         Note = note;
         Addresses = addresses;
         Org = org;
         Birthday = birthday;
         Title = title;
         URL = url;
      }
   }
}