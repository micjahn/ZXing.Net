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
   /// Tests <see cref="ISBNParsedResult" />.
   ///
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class ISBNParsedResultTestCase
   {
      [Test]
      public void testISBN()
      {
         doTest("9784567890123");
      }

      private static void doTest(String contents)
      {
         ZXing.Result fakeResult = new ZXing.Result(contents, null, null, BarcodeFormat.EAN_13);
         ParsedResult result = ResultParser.parseResult(fakeResult);
         Assert.AreEqual(ParsedResultType.ISBN, result.Type);
         ISBNParsedResult isbnResult = (ISBNParsedResult)result;
         Assert.AreEqual(contents, isbnResult.ISBN);
      }
   }
}