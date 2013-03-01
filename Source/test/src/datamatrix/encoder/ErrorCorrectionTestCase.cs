/*
 * Copyright 2006 Jeremias Maerki.
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

using ZXing.Datamatrix.Encoder;

namespace ZXing.Datamatrix.Test
{
   /// <summary>
   /// Tests for the ECC200 error correction.
   /// </summary>
   [TestFixture]
   public sealed class ErrorCorrectionTestCase
   {
      [Test]
      public void testRS()
      {
         //Sample from Annexe R in ISO/IEC 16022:2000(E)
         char[] cw = { (char)142, (char)164, (char)186 };
         SymbolInfo symbolInfo = SymbolInfo.lookup(3);
         String s = ErrorCorrection.encodeECC200(String.Join("", cw), symbolInfo);
         Assert.AreEqual("142 164 186 114 25 5 88 102", HighLevelEncodeTestCase.visualize(s));

         //"A" encoded (ASCII encoding + 2 padding characters)
         cw = new char[] { (char)66, (char)129, (char)70 };
         s = ErrorCorrection.encodeECC200(String.Join("", cw), symbolInfo);
         Assert.AreEqual("66 129 70 138 234 82 82 95", HighLevelEncodeTestCase.visualize(s));
      }
   }
}
