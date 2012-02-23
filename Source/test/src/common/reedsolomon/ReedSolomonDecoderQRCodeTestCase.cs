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

namespace ZXing.Common.ReedSolomon.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class ReedSolomonDecoderQRCodeTestCase : AbstractReedSolomonTestCase
   {
      /// <summary> See ISO 18004, Appendix I, from which this example is taken.</summary>
      private static int[] QR_CODE_TEST =
      { 0x10, 0x20, 0x0C, 0x56, 0x61, 0x80, 0xEC, 0x11, 0xEC,
        0x11, 0xEC, 0x11, 0xEC, 0x11, 0xEC, 0x11 };
      private static int[] QR_CODE_TEST_WITH_EC =
      { 0x10, 0x20, 0x0C, 0x56, 0x61, 0x80, 0xEC, 0x11, 0xEC,
        0x11, 0xEC, 0x11, 0xEC, 0x11, 0xEC, 0x11, 0xA5, 0x24,
        0xD4, 0xC1, 0xED, 0x36, 0xC7, 0x87, 0x2C, 0x55 };
      private static int QR_CODE_ECC_BYTES = QR_CODE_TEST_WITH_EC.Length - QR_CODE_TEST.Length;
      private static int QR_CODE_CORRECTABLE = QR_CODE_ECC_BYTES / 2;

      private ReedSolomonDecoder qrRSDecoder = new ReedSolomonDecoder(GenericGF.QR_CODE_FIELD_256);

      [Test]
      public void testNoError()
      {
         int[] received = new int[QR_CODE_TEST_WITH_EC.Length];
         Array.Copy(QR_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
         // no errors
         Assert.IsTrue(checkQRRSDecode(received, true));
      }

      [Test]
      public void testOneError()
      {
         int[] received = new int[QR_CODE_TEST_WITH_EC.Length];
         Random random = getRandom();
         for (int i = 0; i < received.Length; i++)
         {
            Array.Copy(QR_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
            received[i] = random.Next(256);
            Assert.IsTrue(checkQRRSDecode(received, true));
         }
      }

      [Test]
      public void testMaxErrors()
      {
         int[] received = new int[QR_CODE_TEST_WITH_EC.Length];
         Random random = getRandom();
         foreach (int test in QR_CODE_TEST)
         { // # iterations is kind of arbitrary
            Array.Copy(QR_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
            corrupt(received, QR_CODE_CORRECTABLE, random);
            Assert.IsTrue(checkQRRSDecode(received, true));
         }
      }

      [Test]
      public void testTooManyErrors()
      {
         int[] received = new int[QR_CODE_TEST_WITH_EC.Length];
         Array.Copy(QR_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
         Random random = getRandom();
         corrupt(received, QR_CODE_CORRECTABLE + 1, random);
         if (checkQRRSDecode(received, false))
            Assert.Fail("Should not have decoded");
      }

      private bool checkQRRSDecode(int[] received, bool checkResult)
      {
         var result = qrRSDecoder.decode(received, QR_CODE_ECC_BYTES);
         if (checkResult)
         {
            for (int i = 0; i < QR_CODE_TEST.Length; i++)
            {
               Assert.AreEqual(received[i], QR_CODE_TEST[i]);
            }
         }

         return result;
      }
   }
}
