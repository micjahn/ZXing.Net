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
   /// Tests <see cref="SMSParsedResult" />.
   ///
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class SMSMMSParsedResultTestCase
   {
      [Test]
      public void testSMS()
      {
         doTest("sms:+15551212", "+15551212", null, null, null);
         doTest("sms:+15551212?subject=foo&body=bar", "+15551212", "foo", "bar", null);
         doTest("sms:+15551212;via=999333", "+15551212", null, null, "999333");
      }

      [Test]
      public void testMMS()
      {
         doTest("mms:+15551212", "+15551212", null, null, null);
         doTest("mms:+15551212?subject=foo&body=bar", "+15551212", "foo", "bar", null);
         doTest("mms:+15551212;via=999333", "+15551212", null, null, "999333");
      }

      private static void doTest(String contents,
                                 String number,
                                 String subject,
                                 String body,
                                 String via)
      {
         doTest(contents, new String[] { number }, subject, body, new String[] { via });
      }

      private static void doTest(String contents,
                                 String[] numbers,
                                 String subject,
                                 String body,
                                 String[] vias)
      {
         ZXing.Result fakeResult = new ZXing.Result(contents, null, null, BarcodeFormat.QR_CODE);
         ParsedResult result = ResultParser.parseResult(fakeResult);
         Assert.AreEqual(ParsedResultType.SMS, result.Type);
         SMSParsedResult smsResult = (SMSParsedResult)result;
         Assert.IsTrue(AddressBookParsedResultTestCase.AreEqual(numbers, smsResult.Numbers));
         Assert.AreEqual(subject, smsResult.Subject);
         Assert.AreEqual(body, smsResult.Body);
         Assert.IsTrue(AddressBookParsedResultTestCase.AreEqual(vias, smsResult.Vias));
      }
   }
}