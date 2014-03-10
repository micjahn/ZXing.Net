/*
 * Copyright 2014 ZXing authors
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

using ZXing.Common;

namespace ZXing.OneD.Test
{
   public class Code128WriterTestCase
   {
      private const String FNC1 = "11110101110";
      private const String FNC2 = "11110101000";
      private const String FNC3 = "10111100010";
      private const String FNC4 = "10111101110";
      private const String START_CODE_B = "11010010000";
      private const String QUIET_SPACE = "00000";
      private const String STOP = "1100011101011";

      private Writer writer;

      [SetUp]
      public void setup()
      {
         writer = new Code128Writer();
      }

      [Test]
      public void testEncodeWithFunc3()
      {
         const string toEncode = "\u00f3" + "123";
         //                                                       "1"            "2"             "3"          check digit 51
         var expected = QUIET_SPACE + START_CODE_B + FNC3 + "10011100110" + "11001110010" + "11001011100" + "11101000110" + STOP + QUIET_SPACE;

         var result = writer.encode(toEncode, BarcodeFormat.CODE_128, 0, 0);

         var actual = matrixToString(result);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void testEncodeWithFunc2()
      {
         const string toEncode = "\u00f2" + "123";
         //                                                       "1"            "2"             "3"          check digit 56
         var expected = QUIET_SPACE + START_CODE_B + FNC2 + "10011100110" + "11001110010" + "11001011100" + "11100010110" + STOP + QUIET_SPACE;

         var result = writer.encode(toEncode, BarcodeFormat.CODE_128, 0, 0);

         var actual = matrixToString(result);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void testEncodeWithFunc1()
      {
         const string toEncode = "\u00f1" + "123";
         //                                                       "1"            "2"             "3"          check digit 61
         var expected = QUIET_SPACE + START_CODE_B + FNC1 + "10011100110" + "11001110010" + "11001011100" + "11001000010" + STOP + QUIET_SPACE;

         var result = writer.encode(toEncode, BarcodeFormat.CODE_128, 0, 0);

         var actual = matrixToString(result);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void testEncodeWithFunc4()
      {
         var toEncode = "\u00f4" + "123";
         //                                                       "1"            "2"             "3"          check digit 59
         var expected = QUIET_SPACE + START_CODE_B + FNC4 + "10011100110" + "11001110010" + "11001011100" + "11100011010" + STOP + QUIET_SPACE;

         var result = writer.encode(toEncode, BarcodeFormat.CODE_128, 0, 0);

         var actual = matrixToString(result);

         Assert.AreEqual(expected, actual);
      }

      private String matrixToString(BitMatrix result)
      {
         var builder = new StringBuilder(result.Width);
         for (int i = 0; i < result.Width; i++)
         {
            builder.Append(result[i, 0] ? '1' : '0');
         }
         return builder.ToString();
      }
   }
}