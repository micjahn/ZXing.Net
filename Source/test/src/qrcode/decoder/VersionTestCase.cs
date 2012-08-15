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
   public sealed class VersionTestCase
   {
      [Test]
      public void testVersionForNumber()
      {
         try
         {
            Version.getVersionForNumber(0);
            throw new AssertionException("Should have thrown an exception");
         }
         catch (ArgumentException )
         {
            // good
         }
         for (int i = 1; i <= 40; i++)
         {
            checkVersion(Version.getVersionForNumber(i), i, 4 * i + 17);
         }
      }

      private static void checkVersion(Version version, int number, int dimension)
      {
         Assert.IsNotNull(version);
         Assert.AreEqual(number, version.VersionNumber);
         Assert.IsNotNull(version.AlignmentPatternCenters);
         if (number > 1)
         {
            Assert.IsTrue(version.AlignmentPatternCenters.Length > 0);
         }
         Assert.AreEqual(dimension, version.DimensionForVersion);
         Assert.IsNotNull(version.getECBlocksForLevel(ErrorCorrectionLevel.H));
         Assert.IsNotNull(version.getECBlocksForLevel(ErrorCorrectionLevel.L));
         Assert.IsNotNull(version.getECBlocksForLevel(ErrorCorrectionLevel.M));
         Assert.IsNotNull(version.getECBlocksForLevel(ErrorCorrectionLevel.Q));
         Assert.IsNotNull(version.buildFunctionPattern());
      }

      [Test]
      public void testGetProvisionalVersionForDimension()
      {
         for (int i = 1; i <= 40; i++)
         {
            Assert.AreEqual(i, Version.getProvisionalVersionForDimension(4 * i + 17).VersionNumber);
         }
      }

      [Test]
      public void testDecodeVersionInformation()
      {
         // Spot check
         doTestVersion(7, 0x07C94);
         doTestVersion(12, 0x0C762);
         doTestVersion(17, 0x1145D);
         doTestVersion(22, 0x168C9);
         doTestVersion(27, 0x1B08E);
         doTestVersion(32, 0x209D5);
      }

      private static void doTestVersion(int expectedVersion, int mask)
      {
         Version version = Version.decodeVersionInformation(mask);
         Assert.IsNotNull(version);
         Assert.AreEqual(expectedVersion, version.VersionNumber);
      }
   }
}