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

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec.Test
{
   public sealed class DecoderTest
   {
      private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

      [Test]
      public void testDecodeTooManyErrors()
      {
         var matrix = BitMatrix.parse(""
                                      + "X X . X . . . X X . . . X . . X X X . X . X X X X X . \n"
                                      + "X X . . X X . . . . . X X . . . X X . . . X . X . . X \n"
                                      + "X . . . X X . . X X X . X X . X X X X . X X . . X . . \n"
                                      + ". . . . X . X X . . X X . X X . X . X X X X . X . . X \n"
                                      + "X X X . . X X X X X . . . . . X X . . . X . X . X . X \n"
                                      + "X X . . . . . . . . X . . . X . X X X . X . . X . . . \n"
                                      + "X X . . X . . . . . X X . . . . . X . . . . X . . X X \n"
                                      + ". . . X . X . X . . . . . X X X X X X . . . . . . X X \n"
                                      + "X . . . X . X X X X X X . . X X X . X . X X X X X X . \n"
                                      + "X . . X X X . X X X X X X X X X X X X X . . . X . X X \n"
                                      + ". . . . X X . . . X . . . . . . . X X . . . X X . X . \n"
                                      + ". . . X X X . . X X . X X X X X . X . . X . . . . . . \n"
                                      + "X . . . . X . X . X . X . . . X . X . X X . X X . X X \n"
                                      + "X . X . . X . X . X . X . X . X . X . . . . . X . X X \n"
                                      + "X . X X X . . X . X . X . . . X . X . X X X . . . X X \n"
                                      + "X X X X X X X X . X . X X X X X . X . X . X . X X X . \n"
                                      + ". . . . . . . X . X . . . . . . . X X X X . . . X X X \n"
                                      + "X X . . X . . X . X X X X X X X X X X X X X . . X . X \n"
                                      + "X X X . X X X X . . X X X X . . X . . . . X . . X X X \n"
                                      + ". . . . X . X X X . . . . X X X X . . X X X X . . . . \n"
                                      + ". . X . . X . X . . . X . X X . X X . X . . . X . X . \n"
                                      + "X X . . X . . X X X X X X X . . X . X X X X X X X . . \n"
                                      + "X . X X . . X X . . . . . X . . . . . . X X . X X X . \n"
                                      + "X . . X X . . X X . X . X . . . . X . X . . X . . X . \n"
                                      + "X . X . X . . X . X X X X X X X X . X X X X . . X X . \n"
                                      + "X X X X . . . X . . X X X . X X . . X . . . . X X X . \n"
                                      + "X X . X . X . . . X . X . . . . X X . X . . X X . . . \n",
                                      "X ", ". ");
         var r = new AztecDetectorResult(matrix, NO_POINTS, true, 16, 4);
         Assert.That(new Decoder().decode(r), Is.Null);
      }

      [Test]
      public void testDecodeTooManyErrors2()
      {
         var matrix = BitMatrix.parse(""
                                      + ". X X . . X . X X . . . X . . X X X . . . X X . X X . \n"
                                      + "X X . X X . . X . . . X X . . . X X . X X X . X . X X \n"
                                      + ". . . . X . . . X X X . X X . X X X X . X X . . X . . \n"
                                      + "X . X X . . X . . . X X . X X . X . X X . . . . . X . \n"
                                      + "X X . X . . X . X X . . . . . X X . . . . . X . . . X \n"
                                      + "X . . X . . . . . . X . . . X . X X X X X X X . . . X \n"
                                      + "X . . X X . . X . . X X . . . . . X . . . . . X X X . \n"
                                      + ". . X X X X . X . . . . . X X X X X X . . . . . . X X \n"
                                      + "X . . . X . X X X X X X . . X X X . X . X X X X X X . \n"
                                      + "X . . X X X . X X X X X X X X X X X X X . . . X . X X \n"
                                      + ". . . . X X . . . X . . . . . . . X X . . . X X . X . \n"
                                      + ". . . X X X . . X X . X X X X X . X . . X . . . . . . \n"
                                      + "X . . . . X . X . X . X . . . X . X . X X . X X . X X \n"
                                      + "X . X . . X . X . X . X . X . X . X . . . . . X . X X \n"
                                      + "X . X X X . . X . X . X . . . X . X . X X X . . . X X \n"
                                      + "X X X X X X X X . X . X X X X X . X . X . X . X X X . \n"
                                      + ". . . . . . . X . X . . . . . . . X X X X . . . X X X \n"
                                      + "X X . . X . . X . X X X X X X X X X X X X X . . X . X \n"
                                      + "X X X . X X X X . . X X X X . . X . . . . X . . X X X \n"
                                      + ". . X X X X X . X . . . . X X X X . . X X X . X . X . \n"
                                      + ". . X X . X . X . . . X . X X . X X . . . . X X . . . \n"
                                      + "X . . . X . X . X X X X X X . . X . X X X X X . X . . \n"
                                      + ". X . . . X X X . . . . . X . . . . . X X X X X . X . \n"
                                      + "X . . X . X X X X . X . X . . . . X . X X . X . . X . \n"
                                      + "X . . . X X . X . X X X X X X X X . X X X X . . X X . \n"
                                      + ". X X X X . . X . . X X X . X X . . X . . . . X X X . \n"
                                      + "X X . . . X X . . X . X . . . . X X . X . . X . X . X \n",
                                      "X ", ". ");
         var r = new AztecDetectorResult(matrix, NO_POINTS, true, 16, 4);
         Assert.That(new Decoder().decode(r), Is.Null);
      }
   }
}