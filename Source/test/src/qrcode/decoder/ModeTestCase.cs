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

namespace ZXing.QrCode.Internal.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class ModeTestCase
   {
      [Test]
      public void testForBits()
      {
         Assert.AreEqual(Mode.TERMINATOR, Mode.forBits(0x00));
         Assert.AreEqual(Mode.NUMERIC, Mode.forBits(0x01));
         Assert.AreEqual(Mode.ALPHANUMERIC, Mode.forBits(0x02));
         Assert.AreEqual(Mode.BYTE, Mode.forBits(0x04));
         Assert.AreEqual(Mode.KANJI, Mode.forBits(0x08));
         try
         {
            Mode.forBits(0x10);
            throw new AssertionException("Should have thrown an exception");
         }
         catch (ArgumentException )
         {
            // good
         }
      }

      [Test]
      public void testCharacterCount()
      {
         // Spot check a few values
         Assert.AreEqual(10, Mode.NUMERIC.getCharacterCountBits(Version.getVersionForNumber(5)));
         Assert.AreEqual(12, Mode.NUMERIC.getCharacterCountBits(Version.getVersionForNumber(26)));
         Assert.AreEqual(14, Mode.NUMERIC.getCharacterCountBits(Version.getVersionForNumber(40)));
         Assert.AreEqual(9, Mode.ALPHANUMERIC.getCharacterCountBits(Version.getVersionForNumber(6)));
         Assert.AreEqual(8, Mode.BYTE.getCharacterCountBits(Version.getVersionForNumber(7)));
         Assert.AreEqual(8, Mode.KANJI.getCharacterCountBits(Version.getVersionForNumber(8)));
      }
   }
}