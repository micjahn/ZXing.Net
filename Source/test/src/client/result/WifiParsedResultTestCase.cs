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

using NUnit.Framework;

namespace ZXing.Client.Result.Test
{
   /// <summary>
   /// Tests <see cref="WifiParsedResult" />.
   ///
   /// <author>Vikram Aggarwal</author>
   /// </summary>
   [TestFixture]
   public sealed class WifiParsedResultTestCase
   {

      [Test]
      public void testNoPassword()
      {
         doTest("WIFI:S:NoPassword;P:;T:;;", "NoPassword", null, "nopass");
         doTest("WIFI:S:No Password;P:;T:;;", "No Password", null, "nopass");
      }

      [Test]
      public void testWep()
      {
         doTest("WIFI:S:TenChars;P:0123456789;T:WEP;;", "TenChars", "0123456789", "WEP");
         doTest("WIFI:S:TenChars;P:abcde56789;T:WEP;;", "TenChars", "abcde56789", "WEP");
         // Non hex should not fail at this level
         doTest("WIFI:S:TenChars;P:hellothere;T:WEP;;", "TenChars", "hellothere", "WEP");

         // Escaped semicolons
         doTest("WIFI:S:Ten\\;\\;Chars;P:0123456789;T:WEP;;", "Ten;;Chars", "0123456789", "WEP");
         // Escaped colons
         doTest("WIFI:S:Ten\\:\\:Chars;P:0123456789;T:WEP;;", "Ten::Chars", "0123456789", "WEP");

         // TODO(vikrama) Need a test for SB as well.
      }

      /// <summary>
      /// Put in checks for the length of the password for wep.
      /// </summary>
      [Test]
      public void testWpa()
      {
         doTest("WIFI:S:TenChars;P:wow;T:WPA;;", "TenChars", "wow", "WPA");
         doTest("WIFI:S:TenChars;P:space is silent;T:WPA;;", "TenChars", "space is silent", "WPA");
         doTest("WIFI:S:TenChars;P:hellothere;T:WEP;;", "TenChars", "hellothere", "WEP");

         // Escaped semicolons
         doTest("WIFI:S:TenChars;P:hello\\;there;T:WEP;;", "TenChars", "hello;there", "WEP");
         // Escaped colons
         doTest("WIFI:S:TenChars;P:hello\\:there;T:WEP;;", "TenChars", "hello:there", "WEP");
      }

      [Test]
      public void testEscape()
      {
         doTest("WIFI:T:WPA;S:test;P:my_password\\\\;;", "test", "my_password\\", "WPA");
      }

      /// <summary>
      /// Given the string contents for the barcode, check that it matches our expectations
      /// </summary>
      private static void doTest(String contents,
                                 String ssid,
                                 String password,
                                 String type)
      {
         var fakeResult = new ZXing.Result(contents, null, null, BarcodeFormat.QR_CODE);
         var result = ResultParser.parseResult(fakeResult);

         // Ensure it is a wifi code
         Assert.AreEqual(ParsedResultType.WIFI, result.Type);
         var wifiResult = (WifiParsedResult)result;

         Assert.AreEqual(ssid, wifiResult.Ssid);
         Assert.AreEqual(password, wifiResult.Password);
         Assert.AreEqual(type, wifiResult.NetworkEncryption);
      }
   }
}