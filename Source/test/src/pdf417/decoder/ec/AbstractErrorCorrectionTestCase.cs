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

using NUnit.Framework;

/// <summary>
/// @author Sean Owen
/// </summary>
public abstract class AbstractErrorCorrectionTestCase
{
   public static void corrupt(int[] received, int howMany, Random random)
   {
      var corrupted = new System.Collections.BitArray(received.Length);
      // BitSet corrupted = new BitSet(received.Length);
      for (int j = 0; j < howMany; j++)
      {
         int location = random.Next(received.Length);
         if (corrupted[location])
         {
            j--;
         }
         else
         {
            corrupted[location] = true;
            received[location] = random.Next(929);
         }
      }
   }

   public static int[] erase(int[] received, int howMany, Random random)
   {
      var erased = new System.Collections.BitArray(received.Length);
      // BitSet erased = new BitSet(received.Length);
      int[] erasures = new int[howMany];
      int erasureOffset = 0;
      for (int j = 0; j < howMany; j++)
      {
         int location = random.Next(received.Length);
         if (erased[location])
         {
            j--;
         }
         else
         {
            erased[location] = true;
            received[location] = 0;
            erasures[erasureOffset++] = location;
         }
      }
      return erasures;
   }

   public static Random getRandom()
   {
      return new Random((int)DateTime.Now.Ticks);
      // return new SecureRandom(new byte[] { (byte)0xDE, (byte)0xAD, (byte)0xBE, (byte)0xEF });
   }

   public static void assertArraysEqual(int[] expected,
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