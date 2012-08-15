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

namespace ZXing.QrCode.Internal.Test
{
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   [TestFixture]
   public sealed class ErrorCorrectionLevelTestCase
   {
      [Test]
      public void testForBits()
      {
         Assert.AreEqual(ErrorCorrectionLevel.M, ErrorCorrectionLevel.forBits(0));
         Assert.AreEqual(ErrorCorrectionLevel.L, ErrorCorrectionLevel.forBits(1));
         Assert.AreEqual(ErrorCorrectionLevel.H, ErrorCorrectionLevel.forBits(2));
         Assert.AreEqual(ErrorCorrectionLevel.Q, ErrorCorrectionLevel.forBits(3));
         try
         {
            ErrorCorrectionLevel.forBits(4);
            throw new AssertionException("Should have thrown an exception");
         }
         catch (ArgumentException )
         {
            // good
         }
      }
   }
}
