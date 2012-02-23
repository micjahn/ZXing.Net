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

namespace ZXing.Common.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class BitArrayTestCase
   {

      [Test]
      public void testGetSet()
      {
         BitArray array = new BitArray(33);
         for (int i = 0; i < 33; i++)
         {
            Assert.IsFalse(array[i]);
            array[i] = true;
            Assert.IsTrue(array[i]);
         }
      }

      [Test]
      public void testGetNextSet1()
      {
         BitArray array = new BitArray(32);
         for (int i = 0; i < array.Size; i++)
         {
            Assert.AreEqual(32, array.getNextSet(i));
         }
         array = new BitArray(33);
         for (int i = 0; i < array.Size; i++)
         {
            Assert.AreEqual(33, array.getNextSet(i));
         }
      }

      [Test]
      public void testGetNextSet2()
      {
         BitArray array = new BitArray(33);
         array[31] = true;
         for (int i = 0; i < array.Size; i++)
         {
            Assert.AreEqual(i <= 31 ? 31 : 33, array.getNextSet(i));
         }
         array = new BitArray(33);
         array[32] = true;
         for (int i = 0; i < array.Size; i++)
         {
            Assert.AreEqual(32, array.getNextSet(i));
         }
      }

      [Test]
      public void testGetNextSet3()
      {
         BitArray array = new BitArray(63);
         array[31] = true;
         array[32] = true;
         for (int i = 0; i < array.Size; i++)
         {
            int expected;
            if (i <= 31)
            {
               expected = 31;
            }
            else if (i == 32)
            {
               expected = 32;
            }
            else
            {
               expected = 63;
            }
            Assert.AreEqual(expected, array.getNextSet(i));
         }
      }

      [Test]
      public void testGetNextSet4()
      {
         BitArray array = new BitArray(63);
         array[33] = true;
         array[40] = true;
         for (int i = 0; i < array.Size; i++)
         {
            int expected;
            if (i <= 33)
            {
               expected = 33;
            }
            else if (i <= 40)
            {
               expected = 40;
            }
            else
            {
               expected = 63;
            }
            Assert.AreEqual(expected, array.getNextSet(i));
         }
      }

      [Test]
      public void testGetNextSet5()
      {
         Random r = new Random((int)DateTime.Now.Ticks);
         for (int i = 0; i < 10; i++)
         {
            BitArray array = new BitArray(1 + r.Next(100));
            int numSet = r.Next(20);
            for (int j = 0; j < numSet; j++)
            {
               array[r.Next(array.Size)] = true;
            }
            int numQueries = r.Next(20);
            for (int j = 0; j < numQueries; j++)
            {
               int query = r.Next(array.Size);
               int expected = query;
               while (expected < array.Size && !array[expected])
               {
                  expected++;
               }
               int actual = array.getNextSet(query);
               if (actual != expected)
               {
                  array.getNextSet(query);
               }
               Assert.AreEqual(expected, actual);
            }
         }
      }


      [Test]
      public void testSetBulk()
      {
         BitArray array = new BitArray(64);
         array.setBulk(32, -65536);
         for (int i = 0; i < 48; i++)
         {
            Assert.IsFalse(array[i]);
         }
         for (int i = 48; i < 64; i++)
         {
            Assert.IsTrue(array[i]);
         }
      }

      [Test]
      public void testClear()
      {
         BitArray array = new BitArray(32);
         for (int i = 0; i < 32; i++)
         {
            array[i] = true;
         }
         array.clear();
         for (int i = 0; i < 32; i++)
         {
            Assert.IsFalse(array[i]);
         }
      }

      [Test]
      public void testGetArray()
      {
         BitArray array = new BitArray(64);
         array[0] = true;
         array[63] = true;
         var ints = array.Array;
         Assert.AreEqual(1, ints[0]);
         Assert.AreEqual(Int32.MinValue, ints[1]);
      }

      [Test]
      public void testIsRange()
      {
         BitArray array = new BitArray(64);
         Assert.IsTrue(array.isRange(0, 64, false));
         Assert.IsFalse(array.isRange(0, 64, true));
         array[32] = true;
         Assert.IsTrue(array.isRange(32, 33, true));
         array[31] = true;
         Assert.IsTrue(array.isRange(31, 33, true));
         array[34] = true;
         Assert.IsFalse(array.isRange(31, 35, true));
         for (int i = 0; i < 31; i++)
         {
            array[i] = true;
         }
         Assert.IsTrue(array.isRange(0, 33, true));
         for (int i = 33; i < 64; i++)
         {
            array[i] = true;
         }
         Assert.IsTrue(array.isRange(0, 64, true));
         Assert.IsFalse(array.isRange(0, 64, false));
      }
   }
}
