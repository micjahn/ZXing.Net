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
   public abstract class AbstractReedSolomonTestCase
   {
      private static Random random = new Random((int)DateTime.Now.Ticks);

      internal static void corrupt(int[] received, int howMany, Random random)
      {
         var corrupted = new System.Collections.BitArray(received.Length);
         //BitSet corrupted = new BitSet(received.Length);
         for (int j = 0; j < howMany; j++)
         {
            int location = random.Next(received.Length);
            if (corrupted[location])
            {
               j--;
            }
            else
            {
               corrupted.Set(location, true);
               received[location] = (received[location] + 1 + random.Next(255)) & 0xFF;
            }
         }
      }

      internal static void doTestQRCodeEncoding(int[] dataBytes, int[] expectedECBytes)
      {
         int[] toEncode = new int[dataBytes.Length + expectedECBytes.Length];
         Array.Copy(dataBytes, 0, toEncode, 0, dataBytes.Length);
         new ReedSolomonEncoder(GenericGF.QR_CODE_FIELD_256).encode(toEncode, expectedECBytes.Length);
         assertArraysEqual(dataBytes, 0, toEncode, 0, dataBytes.Length);
         assertArraysEqual(expectedECBytes, 0, toEncode, dataBytes.Length, expectedECBytes.Length);
      }

      internal static Random getRandom()
      {
         //return new SecureRandom(new sbyte[] { (byte)0xDE, (byte)0xAD, (byte)0xBE, (byte)0xEF });
         return new Random((int)DateTime.Now.Ticks);
      }

      static void assertArraysEqual(int[] expected,
                                    int expectedOffset,
                                    int[] actual,
                                    int actualOffset,
                                    int length)
      {
         for (int i = 0; i < length; i++)
         {
            Assert.AreEqual(expected[expectedOffset + i], actual[actualOffset + i]);
         }
      }
   }
}
