/*
 * Copyright 2012 ZXing authors
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
using NUnit.Framework;

namespace ZXing.Common.Test
{
   [TestFixture]
   public class StringUtilsTestCase
   {
      [Test]
      public void testShortShiftJIS_1()
      {
         // ÈáëÈ≠ö
         doTest(new byte[] {0x8b, 0xe0, 0x8b, 0x9b,}, "SJIS");
      }

      [Test]
      public void testShortISO88591_1()
      {
         // b√•d
         doTest(new byte[] {0x62, 0xe5, 0x64,}, "ISO-8859-1");
      }

      [Test]
      public void testMixedShiftJIS_1()
      {
         // Hello Èáë!
         doTest(new byte[]
                   {
                      0x48, 0x65, 0x6c, 0x6c, 0x6f,
                      0x20, 0x8b, 0xe0, 0x21,
                   },
                "SJIS");
      }

      private static void doTest(byte[] bytes, String charsetName)
      {
         var charset = Encoding.GetEncoding(charsetName);
         String guessedName = StringUtils.guessEncoding(bytes, null);
         var guessedEncoding = Encoding.GetEncoding(guessedName);
         Assert.AreEqual(charset, guessedEncoding);
      }

      /**
       * Utility for printing out a string in given encoding as a Java statement, since it's better
       * to write that into the Java source file rather than risk character encoding issues in the 
       * source file itself
       */
      public static void main(String[] args)
      {
         var text = args[0];
         var charset = Encoding.GetEncoding(args[1]);
         var declaration = new StringBuilder();
         declaration.Append("new byte[] { ");
         foreach (byte b in charset.GetBytes(text))
         {
            declaration.Append("(byte) 0x");
            declaration.Append((b & 0xFF).ToString("X"));
            declaration.Append(", ");
         }
         declaration.Append('}');
         Console.WriteLine(declaration);
      }
   }
}