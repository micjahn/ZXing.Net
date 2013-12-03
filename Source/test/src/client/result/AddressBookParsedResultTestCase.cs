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
using System.Collections.Generic;
using NUnit.Framework;

namespace ZXing.Client.Result.Test
{
   /// <summary>
   /// Tests <see cref="AddressBookParsedResult" />.
   ///
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class AddressBookParsedResultTestCase
   {
      [Test]
      public void testAddressBookDocomo()
      {
         doTest("MECARD:N:Sean Owen;;", null, new String[] { "Sean Owen" }, null, null, null, null, null, null, null, null);
         doTest("MECARD:NOTE:ZXing Team;N:Sean Owen;URL:google.com;EMAIL:srowen@example.org;;",
             null, new String[] { "Sean Owen" }, null, null, new String[] { "srowen@example.org" }, null, null,
              new String[] { "google.com" }, null, "ZXing Team");
      }

      [Test]
      public void testAddressBookAU()
      {
         doTest("MEMORY:foo\r\nNAME1:Sean\r\nTEL1:+12125551212\r\n",
             null, new String[] { "Sean" }, null, null, null, new String[] { "+12125551212" }, null, null, null, "foo");
      }

      [Test]
      public void testVCard()
      {
         doTest("BEGIN:VCARD\r\nADR;HOME:123 Main St\r\nVERSION:2.1\r\nN:Owen;Sean\r\nEND:VCARD",
                null, new String[] { "Sean Owen" }, null, new String[] { "123 Main St" }, null, null, null, null, null, null);
      }

      [Test]
      public void testVCardFullN()
      {
         doTest("BEGIN:VCARD\r\nVERSION:2.1\r\nN:Owen;Sean;T;Mr.;Esq.\r\nEND:VCARD",
                null, new String[] {"Mr. Sean T Owen Esq."}, null, null, null, null, null, null, null, null);
      }

      [Test]
      public void testVCardFullN2()
      {
         doTest("BEGIN:VCARD\r\nVERSION:2.1\r\nN:Owen;Sean;;;\r\nEND:VCARD",
                null, new String[] {"Sean Owen"}, null, null, null, null, null, null, null, null);
      }

      [Test]
      public void testVCardFullN3()
      {
         doTest("BEGIN:VCARD\r\nVERSION:2.1\r\nN:;Sean;;;\r\nEND:VCARD",
                null, new String[] {"Sean"}, null, null, null, null, null, null, null, null);
      }

      [Test]
      public void testVCardCaseInsensitive()
      {
         doTest("begin:vcard\r\nadr;HOME:123 Main St\r\nVersion:2.1\r\nn:Owen;Sean\r\nEND:VCARD",
                null, new String[] { "Sean Owen" }, null, new String[] { "123 Main St" }, null, null, null, null, null, null);
      }

      [Test]
      public void testEscapedVCard()
      {
         doTest("BEGIN:VCARD\r\nADR;HOME:123\\;\\\\ Main\\, St\\nHome\r\nVERSION:2.1\r\nN:Owen;Sean\r\nEND:VCARD",
                null, new String[] { "Sean Owen" }, null, new String[] { "123;\\ Main, St\nHome" }, null, null, null, null, null, null);
      }

      [Test]
      public void testBizcard()
      {
         doTest("BIZCARD:N:Sean;X:Owen;C:Google;A:123 Main St;M:+12125551212;E:srowen@example.org;",
             null, new String[] { "Sean Owen" }, null, new String[] { "123 Main St" }, new String[] { "srowen@example.org" },
             new String[] { "+12125551212" }, "Google", null, null, null);
      }

      [Test]
      public void testSeveralAddresses()
      {
         doTest("MECARD:N:Foo Bar;ORG:Company;TEL:5555555555;EMAIL:foo.bar@xyz.com;ADR:City, 10001;" +
                "ADR:City, 10001;NOTE:This is the memo.;;",
                null, new String[] { "Foo Bar" }, null, new String[] { "City, 10001", "City, 10001" },
                new String[] { "foo.bar@xyz.com" },
                new String[] { "5555555555" }, "Company", null, null, "This is the memo.");
      }

      [Test]
      public void testQuotedPrintable()
      {
         doTest("BEGIN:VCARD\r\nADR;HOME;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:;;" +
                "=38=38=20=4C=79=6E=62=72=6F=6F=6B=0D=0A=43=\r\n" +
                "=4F=20=36=39=39=\r\n" +
                "=39=39;;;\r\nEND:VCARD",
                null, null, null, new String[] { "88 Lynbrook\r\nCO 69999" },
                null, null, null, null, null, null);
      }

      [Test]
      public void testVCardEscape()
      {
         doTest("BEGIN:VCARD\r\nNOTE:foo\\nbar\r\nEND:VCARD",
                null, null, null, null, null, null, null, null, null, "foo\nbar");
         doTest("BEGIN:VCARD\r\nNOTE:foo\\;bar\r\nEND:VCARD",
                null, null, null, null, null, null, null, null, null, "foo;bar");
         doTest("BEGIN:VCARD\r\nNOTE:foo\\\\bar\r\nEND:VCARD",
                null, null, null, null, null, null, null, null, null, "foo\\bar");
         doTest("BEGIN:VCARD\r\nNOTE:foo\\,bar\r\nEND:VCARD",
                null, null, null, null, null, null, null, null, null, "foo,bar");
      }

      private static void doTest(String contents,
                                 String title,
                                 String[] names,
                                 String pronunciation,
                                 String[] addresses,
                                 String[] emails,
                                 String[] phoneNumbers,
                                 String org,
                                 String[] urls,
                                 String birthday,
                                 String note)
      {
         ZXing.Result fakeResult = new ZXing.Result(contents, null, null, BarcodeFormat.QR_CODE);
         ParsedResult result = ResultParser.parseResult(fakeResult);
         Assert.AreEqual(ParsedResultType.ADDRESSBOOK, result.Type);
         AddressBookParsedResult addressResult = (AddressBookParsedResult)result;
         Assert.AreEqual(title, addressResult.Title);
         Assert.IsTrue(AreEqual(names, addressResult.Names));
         Assert.AreEqual(pronunciation, addressResult.Pronunciation);
         Assert.IsTrue(AreEqual(addresses, addressResult.Addresses));
         Assert.IsTrue(AreEqual(emails, addressResult.Emails));
         Assert.IsTrue(AreEqual(phoneNumbers, addressResult.PhoneNumbers));
         Assert.AreEqual(org, addressResult.Org);
         Assert.IsTrue(AreEqual(urls, addressResult.URLs));
         Assert.AreEqual(birthday, addressResult.Birthday);
         Assert.AreEqual(note, addressResult.Note);
      }

      internal static bool AreEqual<T>(IList<T> left, IList<T> right)
      {
         if (left == null)
            return right == null;
         if (right == null)
            return false;
         if (left.Count != right.Count)
            return false;

         foreach (var leftItem in left)
         {
            var found = false;
            foreach (var rightItem in right)
            {
               if (Equals(rightItem, leftItem))
               {
                  found = true;
                  break;
               }
            }
            if (!found)
               return false;
         }
         foreach (var rightItem in right)
         {
            var found = false;
            foreach (var leftItem in left)
            {
               if (Equals(rightItem, leftItem))
               {
                  found = true;
                  break;
               }
            }
            if (!found)
               return false;
         }
         return true;
      }
   }
}