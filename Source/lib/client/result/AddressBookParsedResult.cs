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
   /// Represents a parsed result that encodes contact information, like that in an address book entry.
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class AddressBookParsedResult : ParsedResult
   {
      private readonly String[] names;
      private readonly String[] nicknames;
      private readonly String pronunciation;
      private readonly String[] phoneNumbers;
      private readonly String[] phoneTypes;
      private readonly String[] emails;
      private readonly String[] emailTypes;
      private readonly String instantMessenger;
      private readonly String note;
      private readonly String[] addresses;
      private readonly String[] addressTypes;
      private readonly String org;
      private readonly String birthday;
      private readonly String title;
      private readonly String[] urls;
      private readonly String[] geo;

      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="names"></param>
      /// <param name="phoneNumbers"></param>
      /// <param name="phoneTypes"></param>
      /// <param name="emails"></param>
      /// <param name="emailTypes"></param>
      /// <param name="addresses"></param>
      /// <param name="addressTypes"></param>
      public AddressBookParsedResult(String[] names,
                                 String[] phoneNumbers,
                                 String[] phoneTypes,
                                 String[] emails,
                                 String[] emailTypes,
                                 String[] addresses,
                                 String[] addressTypes)
         : this(names,
           null,
           null,
           phoneNumbers,
           phoneTypes,
           emails,
           emailTypes,
           null,
           null,
           addresses,
           addressTypes,
           null,
           null,
           null,
           null,
           null)
      {
      }

      /// <summary>
      /// initializing constructor
      /// </summary>
      /// <param name="names"></param>
      /// <param name="nicknames"></param>
      /// <param name="pronunciation"></param>
      /// <param name="phoneNumbers"></param>
      /// <param name="phoneTypes"></param>
      /// <param name="emails"></param>
      /// <param name="emailTypes"></param>
      /// <param name="instantMessenger"></param>
      /// <param name="note"></param>
      /// <param name="addresses"></param>
      /// <param name="addressTypes"></param>
      /// <param name="org"></param>
      /// <param name="birthday"></param>
      /// <param name="title"></param>
      /// <param name="urls"></param>
      /// <param name="geo"></param>
      public AddressBookParsedResult(String[] names,
                                     String[] nicknames,
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
                                     String[] urls,
                                     String[] geo)
         : base(ParsedResultType.ADDRESSBOOK)
      {
         if (phoneNumbers != null && phoneTypes != null && phoneNumbers.Length != phoneTypes.Length)
         {
            throw new ArgumentException("Phone numbers and types lengths differ");
         }
         if (emails != null && emailTypes != null && emails.Length != emailTypes.Length)
         {
            throw new ArgumentException("Emails and types lengths differ");
         }
         if (addresses != null && addressTypes != null && addresses.Length != addressTypes.Length)
         {
            throw new ArgumentException("Addresses and types lengths differ");
         }

         this.names = names;
         this.nicknames = nicknames;
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
         this.urls = urls;
         this.geo = geo;

         displayResultValue = getDisplayResult();
      }

      /// <summary>
      /// the names
      /// </summary>
      public String[] Names
      {
         get { return names; }
      }

      /// <summary>
      /// the nicknames
      /// </summary>
      public String[] Nicknames
      {
         get { return nicknames; }
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

      /// <summary>
      /// the phone numbers
      /// </summary>
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

      /// <summary>
      /// the e-mail addresses
      /// </summary>
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

      /// <summary>
      /// the instant messenger addresses
      /// </summary>
      public String InstantMessenger
      {
         get { return instantMessenger; }
      }

      /// <summary>
      /// the note field
      /// </summary>
      public String Note
      {
         get { return note; }
      }

      /// <summary>
      /// the addresses
      /// </summary>
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

      /// <summary>
      /// the title
      /// </summary>
      public String Title
      {
         get { return title; }
      }

      // the organisation
      public String Org
      {
         get { return org; }
      }

      /// <summary>
      /// the urls
      /// </summary>
      public String[] URLs
      {
         get { return urls; }
      }

      /// <return>birthday formatted as yyyyMMdd (e.g. 19780917)</return>
      public String Birthday
      {
         get { return birthday; }
      }

      /// <return>a location as a latitude/longitude pair</return>
      public String[] Geo
      {
         get { return geo; }
      }

      private String getDisplayResult()
      {
         var result = new StringBuilder(100);
         maybeAppend(names, result);
         maybeAppend(nicknames, result);
         maybeAppend(pronunciation, result);
         maybeAppend(title, result);
         maybeAppend(org, result);
         maybeAppend(addresses, result);
         maybeAppend(phoneNumbers, result);
         maybeAppend(emails, result);
         maybeAppend(instantMessenger, result);
         maybeAppend(urls, result);
         maybeAppend(birthday, result);
         maybeAppend(geo, result);
         maybeAppend(note, result);
         return result.ToString();
      }
   }
}