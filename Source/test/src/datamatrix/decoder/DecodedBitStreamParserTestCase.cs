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

using NUnit.Framework;

namespace ZXing.Datamatrix.Internal.Test
{
   /// <summary>
   /// <author>bbrown@google.com (Brian Brown)</author>
   /// </summary>
   [TestFixture]
   public sealed class DecodedBitStreamParserTestCase
   {
      [Test]
      public void testAsciiStandardDecode()
      {
         // ASCII characters 0-127 are encoded as the value + 1
         sbyte[] bytes = {(sbyte) ('a' + 1), (sbyte) ('b' + 1), (sbyte) ('c' + 1),
                    (sbyte) ('A' + 1), (sbyte) ('B' + 1), (sbyte) ('C' + 1)};
         String decodedString = DecodedBitStreamParser.decode(bytes).Text;
         Assert.AreEqual("abcABC", decodedString);
      }

      [Test]
      public void testAsciiDoubleDigitDecode()
      {
         // ASCII double digit (00 - 99) Numeric Value + 130
         sbyte[] bytes = {(sbyte)       -126 , (sbyte) (-125),
                    (sbyte) (-28), (sbyte) (-27)};
         String decodedString = DecodedBitStreamParser.decode(bytes).Text;
         Assert.AreEqual("00019899", decodedString);
      }

      // TODO(bbrown): Add test cases for each encoding type
      // TODO(bbrown): Add test cases for switching encoding types
   }
}