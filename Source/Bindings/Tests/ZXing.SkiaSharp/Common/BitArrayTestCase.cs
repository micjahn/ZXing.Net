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
         Random r = new Random(0x0EADBEEF);
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
      public void testSetRange()
      {
         BitArray array = new BitArray(64);
         array.setRange(28, 36);
         Assert.IsFalse(array[27]);
         for (int i = 28; i < 36; i++)
         {
            Assert.IsTrue(array[i]);
         }
         Assert.IsFalse(array[36]);
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
      public void testFlip()
      {
         BitArray array = new BitArray(32);
         Assert.IsFalse(array[5]);
         array.flip(5);
         Assert.IsTrue(array[5]);
         array.flip(5);
         Assert.IsFalse(array[5]);
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

      [Test]
      public void testClone()
      {
         BitArray array = new BitArray(32);
         var clone = (BitArray) array.Clone();
         clone[0] = true;
         Assert.IsFalse(array[0]);
      }

      [Test]
      public void testEquals()
      {
         BitArray a = new BitArray(32);
         BitArray b = new BitArray(32);
         Assert.AreEqual(a, b);
         Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
         Assert.AreNotEqual(a, new BitArray(31));
         a[16] = true;
         Assert.AreNotEqual(a, b);
         Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
         b[16] = true;
         Assert.AreEqual(a, b);
         Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
      }

#if !SILVERLIGHT
      [Test]
      public void ReverseAlgorithmTest()
      {
         var oldBits = new[] { 128, 256, 512, 6453324, 50934953 };

         for (var size = 1; size < 160; size++)
         {
            var newBitsOriginal = reverseOriginal(oldBits, size);
            var newBitsNew = reverseNew(oldBits, size);

            if (!arrays_are_equal(newBitsOriginal, newBitsNew, size / 32 + 1))
            {
               System.Diagnostics.Trace.WriteLine(size);
               System.Diagnostics.Trace.WriteLine(BitsToString(oldBits, size));
               System.Diagnostics.Trace.WriteLine(BitsToString(newBitsOriginal, size));
               System.Diagnostics.Trace.WriteLine(BitsToString(newBitsNew, size));
            }
            Assert.IsTrue(arrays_are_equal(newBitsOriginal, newBitsNew, size / 32 + 1));
         }
      }

      [Test]
      public void ReverseSpeedTest()
      {
         var size = 140;
         var oldBits = new[] {128, 256, 512, 6453324, 50934953};
         var newBitsOriginal = reverseOriginal(oldBits, size);
         var newBitsNew = reverseNew(oldBits, size);

         System.Diagnostics.Trace.WriteLine(BitsToString(oldBits, size));
         System.Diagnostics.Trace.WriteLine(BitsToString(newBitsOriginal, size));
         System.Diagnostics.Trace.WriteLine(BitsToString(newBitsNew, size));

         Assert.IsTrue(arrays_are_equal(newBitsOriginal, newBitsNew, newBitsNew.Length));

         var startOld = DateTime.Now;
         for (int runs = 0; runs < 1000000; runs++)
         {
            reverseOriginal(oldBits, 140);
         }
         var endOld = DateTime.Now;

         var startNew = DateTime.Now;
         for (int runs = 0; runs < 1000000; runs++)
         {
            reverseNew(oldBits, 140);
         }
         var endNew = DateTime.Now;

         System.Diagnostics.Trace.WriteLine(endOld - startOld);
         System.Diagnostics.Trace.WriteLine(endNew - startNew);
      }

      /// <summary> Reverses all bits in the array.</summary>
      private int[] reverseOriginal(int[] oldBits, int oldSize)
      {
         int[] newBits = new int[oldBits.Length];
         int size = oldSize;
         for (int i = 0; i < size; i++)
         {
            if (bits_index(oldBits, size - i - 1))
            {
               newBits[i >> 5] |= 1 << (i & 0x1F);
            }
         }
         return newBits;
      }

      private bool bits_index(int[] bits, int i)
      {
         return (bits[i >> 5] & (1 << (i & 0x1F))) != 0;
      }

      /// <summary> Reverses all bits in the array.</summary>
      private int[] reverseNew(int[] oldBits, int oldSize)
      {
         // doesn't work if more ints are used as necessary
         int[] newBits = new int[oldBits.Length];
         var oldBitsLen = (int)Math.Ceiling(oldSize / 32f);
         var len = oldBitsLen - 1;
         for (var i = 0; i < oldBitsLen; i++)
         {
            var x = (long)oldBits[i];
            x = ((x >> 1) & 0x55555555u) | ((x & 0x55555555u) << 1);
            x = ((x >> 2) & 0x33333333u) | ((x & 0x33333333u) << 2);
            x = ((x >> 4) & 0x0f0f0f0fu) | ((x & 0x0f0f0f0fu) << 4);
            x = ((x >> 8) & 0x00ff00ffu) | ((x & 0x00ff00ffu) << 8);
            x = ((x >> 16) & 0xffffu) | ((x & 0xffffu) << 16);
            newBits[len - i] = (int)x;
         }
         if (oldSize != oldBitsLen * 32)
         {
            var leftOffset = oldBitsLen * 32 - oldSize;
            var mask = 1;
            for (var i = 0; i < 31 - leftOffset; i++ )
               mask = (mask << 1) | 1;
            var currentInt = (newBits[0] >> leftOffset) & mask;
            for (var i = 1; i < oldBitsLen; i++)
            {
               var nextInt = newBits[i];
               currentInt |= nextInt << (32 - leftOffset);
               newBits[i - 1] = currentInt;
               currentInt = (nextInt >> leftOffset) & mask;
            }
            newBits[oldBitsLen - 1] = currentInt;
         }
         return newBits;
      }

      private bool arrays_are_equal(int[] left, int[] right, int size)
      {
         for (var i = 0; i < size; i++)
         {
            if (left[i] != right[i])
               return false;
         }
         return true;
      }

      private string BitsToString(int[] bits, int size)
      {
         var result = new System.Text.StringBuilder(size);
         for (int i = 0; i < size; i++)
         {
            if ((i & 0x07) == 0)
            {
               result.Append(' ');
            }
            result.Append(bits_index(bits, i) ? 'X' : '.');
         }
         return result.ToString();
      }

      [Test]
      public void testBitArrayNet()
      {
         var netArray = new System.Collections.BitArray(140, false);
         var zxingArray = new BitArray(140);

         var netVal = netArray[100];
         var zxingVal = zxingArray[100];
         Assert.AreEqual(netVal, zxingVal);

         var startOld = DateTime.Now;
         for (int runs = 0; runs < 1000000; runs++)
         {
            netVal = netArray[100];
            netArray[100] = netVal;
         }
         var endOld = DateTime.Now;

         var startNew = DateTime.Now;
         for (int runs = 0; runs < 1000000; runs++)
         {
            zxingVal = zxingArray[100];
            zxingArray[100] = zxingVal;
         }
         var endNew = DateTime.Now;

         System.Diagnostics.Trace.WriteLine(endOld - startOld);
         System.Diagnostics.Trace.WriteLine(endNew - startNew);
      }
#endif

   }
}
