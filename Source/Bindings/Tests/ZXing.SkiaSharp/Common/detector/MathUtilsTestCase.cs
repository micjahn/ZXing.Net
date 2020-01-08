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

using NUnit.Framework;

namespace ZXing.Common.Detector.Test
{
   [TestFixture]
   public class MathUtilsTestCase
   {
      private static float EPSILON = 1.0E-8f;

      [Test]
      public void testRound()
      {
         Assert.That(MathUtils.round(-1.0f), Is.EqualTo(-1));
         Assert.That(MathUtils.round(0.0f), Is.EqualTo(0));
         Assert.That(MathUtils.round(1.0f), Is.EqualTo(1));

         Assert.That(MathUtils.round(1.9f), Is.EqualTo(2));
         Assert.That(MathUtils.round(2.1f), Is.EqualTo(2));

         Assert.That(MathUtils.round(2.5f), Is.EqualTo(3));

         Assert.That(MathUtils.round(-1.9f), Is.EqualTo(-2));
         Assert.That(MathUtils.round(-2.1f), Is.EqualTo(-2));

         Assert.That(MathUtils.round(-2.5f), Is.EqualTo(-3)); // This differs from Math.round()

         // doesn't work like java
         //Assert.That(MathUtils.round(int.MaxValue), Is.EqualTo(int.MaxValue));
         Assert.That(MathUtils.round(int.MinValue), Is.EqualTo(int.MinValue));

         Assert.That(MathUtils.round(float.PositiveInfinity), Is.EqualTo(int.MaxValue));
         Assert.That(MathUtils.round(float.NegativeInfinity), Is.EqualTo(int.MinValue));

         Assert.That(MathUtils.round(float.NaN), Is.EqualTo(0));
      }

      [Test]
      public void testDistance()
      {
         Assert.AreEqual((float)System.Math.Sqrt(8.0), MathUtils.distance(1.0f, 2.0f, 3.0f, 4.0f), EPSILON);
         Assert.AreEqual(0.0f, MathUtils.distance(1.0f, 2.0f, 1.0f, 2.0f), EPSILON);

         Assert.AreEqual((float)System.Math.Sqrt(8.0), MathUtils.distance(1, 2, 3, 4), EPSILON);
         Assert.AreEqual(0.0f, MathUtils.distance(1, 2, 1, 2), EPSILON);
      }

      [Test]
      public void testSum()
      {
         Assert.That(MathUtils.sum(new int[] { }), Is.EqualTo(0));
         Assert.That(MathUtils.sum(new int[] { 1 }), Is.EqualTo(1));
         Assert.That(MathUtils.sum(new int[] { 1, 3 }), Is.EqualTo(4));
         Assert.That(MathUtils.sum(new int[] { -1, 1 }), Is.EqualTo(0));
      }
   }
}
