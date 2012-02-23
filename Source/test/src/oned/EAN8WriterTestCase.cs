/*
 * Copyright 2009 ZXing authors
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
using ZXing.Common;

namespace ZXing.OneD.Test
{
   /// <summary>
   /// <author>Ari Pollak</author>
   /// </summary>
   [TestFixture]
   public sealed class EAN8WriterTestCase
   {
      [Test]
      public void testEncode()
      {
         String testStr = "0001010001011010111101111010110111010101001110111001010001001011100101000";
         BitMatrix result = new EAN8Writer().encode("96385074", BarcodeFormat.EAN_8, testStr.Length, 0);
         for (int i = 0; i < testStr.Length; i++)
         {
            Assert.AreEqual(testStr[i] == '1', result[i, 0], "Element " + i);
         }
      }
   }
}