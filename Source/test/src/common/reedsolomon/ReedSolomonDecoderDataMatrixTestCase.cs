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

namespace ZXing.Common.ReedSolomon
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// <author>sanfordsquires</author>
   /// </summary>
   [TestFixture]
   public sealed class ReedSolomonDecoderDataMatrixTestCase : AbstractReedSolomonTestCase
   {
      private static int[] DM_CODE_TEST = { 142, 164, 186 };
      private static int[] DM_CODE_TEST_WITH_EC = { 142, 164, 186, 114, 25, 5, 88, 102 };
      private static int DM_CODE_ECC_BYTES = DM_CODE_TEST_WITH_EC.Length - DM_CODE_TEST.Length;
      private static int DM_CODE_CORRECTABLE = DM_CODE_ECC_BYTES / 2;

      private ReedSolomonDecoder dmRSDecoder = new ReedSolomonDecoder(GenericGF.DATA_MATRIX_FIELD_256);

      [Test]
      public void testNoError()
      {
         int[] received = new int[DM_CODE_TEST_WITH_EC.Length];
         Array.Copy(DM_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
         // no errors
         Assert.IsTrue(checkQRRSDecode(received, true));
      }

      [Test]
      public void testOneError()
      {
         int[] received = new int[DM_CODE_TEST_WITH_EC.Length];
         Random random = getRandom();
         for (int i = 0; i < received.Length; i++)
         {
            Array.Copy(DM_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
            received[i] = random.Next(256);
            Assert.IsTrue(checkQRRSDecode(received, true));
         }
      }

      [Test]
      public void testMaxErrors()
      {
         int[] received = new int[DM_CODE_TEST_WITH_EC.Length];
         Random random = getRandom();
         foreach (int test in DM_CODE_TEST)
         { // # iterations is kind of arbitrary
            Array.Copy(DM_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
            corrupt(received, DM_CODE_CORRECTABLE, random);
            Assert.IsTrue(checkQRRSDecode(received, true));
         }
      }

      [Test]
      public void testTooManyErrors()
      {
         int[] received = new int[DM_CODE_TEST_WITH_EC.Length];
         Array.Copy(DM_CODE_TEST_WITH_EC, 0, received, 0, received.Length);
         Random random = getRandom();
         corrupt(received, DM_CODE_CORRECTABLE + 1, random);
         if (checkQRRSDecode(received, false))
            Assert.Fail("Should not have decoded");
      }

      private bool checkQRRSDecode(int[] received, bool checkResult)
      {
         var result = dmRSDecoder.decode(received, DM_CODE_ECC_BYTES);
         if (checkResult)
         {
            for (int i = 0; i < DM_CODE_TEST.Length; i++)
            {
               Assert.AreEqual(received[i], DM_CODE_TEST[i]);
            }
         }

         return result;
      }
   }
}
