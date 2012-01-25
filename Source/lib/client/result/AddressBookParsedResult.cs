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
      private String[] names;
      private String pronunciation;
      private String[] phoneNumbers;
      private String[] phoneTypes;
      private String[] emails;
      private String[] emailTypes;
      private String instantMessenger;
      private String note;
      private String[] addresses;
      private String[] addressTypes;
      private String org;
      private String birthday;
      private String title;
      private String url;

      public AddressBookParsedResult(String[] names,
                                     String pronunciation,
                                     String[] phoneNumbers,
                                     String[] phoneTypes,
                                     String[] emails,
                                     String[] emailTypes,
                                     String instantMessenger,
                                     String note,
                                     String[] addresses,
                                     String[] addressTypes,
                                     String org,
                                     String birthday,
                                     String title,
                                     String url)
         : base(ParsedResultType.ADDRESSBOOK)
      {
         this.names = names;
         this.pronunciation = pronunciation;
         this.phoneNumbers = phoneNumbers;
         this.phoneTypes = phoneTypes;
         this.emails = emails;
         this.emailTypes = emailTypes;
         this.instantMessenger = instantMessenger;
         this.note = note;
         this.addresses = addresses;
         this.addressTypes = addressTypes;
         this.org = org;
         this.birthday = birthday;
         this.title = title;
         this.url = url;
      }

      public String[] Names
      {
         get { return names; }
      }

      /// <summary>
      /// In Japanese, the name is written in kanji, which can have multiple readings. Therefore a hint
      /// is often provided, called furigana, which spells the name phonetically.
      /// </summary>
      /// <return>The pronunciation of the getNames() field, often in hiragana or katakana.</return>
      public String Pronunciation
      {
         get { return pronunciation; }
      }

      public String[] PhoneNumbers
      {
         get { return phoneNumbers; }
      }

      /// <return>optional descriptions of the type of each phone number. It could be like "HOME", but,
      /// there is no guaranteed or standard format.</return>
      public String[] PhoneTypes
      {
         get { return phoneTypes; }
      }

      public String[] Emails
      {
         get { return emails; }
      }

      /// <return>optional descriptions of the type of each e-mail. It could be like "WORK", but,
      /// there is no guaranteed or standard format.</return>
      public String[] EmailTypes
      {
         get { return emailTypes; }
      }

      public String InstantMessenger
      {
         get { return instantMessenger; }
      }

      public String Note
      {
         get { return note; }
      }

      public String[] Addresses
      {
         get { return addresses; }
      }

      /// <return>optional descriptions of the type of each e-mail. It could be like "WORK", but,
      /// there is no guaranteed or standard format.</return>
      public String[] AddressTypes
      {
         get { return addressTypes; }
      }

      public String Title
      {
         get { return title; }
      }

      public String Org
      {
         get { return org; }
      }

      public String URL
      {
         get { return url; }
      }

      /// <return>birthday formatted as yyyyMMdd (e.g. 19780917)</return>
      public String Birthday
      {
         get { return birthday; }
      }

      override public String DisplayResult
      {
         get
         {
            var result = new StringBuilder(100);
            maybeAppend(names, result);
            maybeAppend(pronunciation, result);
            maybeAppend(title, result);
            maybeAppend(org, result);
            maybeAppend(addresses, result);
            maybeAppend(phoneNumbers, result);
            maybeAppend(emails, result);
            maybeAppend(instantMessenger, result);
            maybeAppend(url, result);
            maybeAppend(birthday, result);
            maybeAppend(note, result);
            return result.ToString();
         }
      }
   }
}