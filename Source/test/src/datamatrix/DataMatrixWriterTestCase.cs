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
using System.Collections.Generic;

using NUnit.Framework;

using ZXing.Datamatrix.Encoder;

namespace ZXing.Datamatrix.Test
{
   /// <summary>
   /// @author satorux@google.com (Satoru Takabayashi) - creator
   /// @author dswitkin@google.com (Daniel Switkin) - ported and expanded from C++
   /// </summary>
   [TestFixture]
   public sealed class DataMatrixWriterTestCase
   {
      [Test]
      public void testDataMatrixImageWriter()
      {
         var writer = new DataMatrixWriter();

         var hints = new Dictionary<EncodeHintType, Object>
                        {{EncodeHintType.DATA_MATRIX_SHAPE, SymbolShapeHint.FORCE_SQUARE}};

         const int bigEnough = 64;
         var matrix = writer.encode("Hello Google", BarcodeFormat.DATA_MATRIX, bigEnough, bigEnough, hints);

         Assert.IsNotNull(matrix);
         Assert.IsTrue(bigEnough >= matrix.Width);
         Assert.IsTrue(bigEnough >= matrix.Height);
      }

      [Test]
      public void testDataMatrixWriter()
      {
         var writer = new DataMatrixWriter();

         var hints = new Dictionary<EncodeHintType, Object>
                        {{EncodeHintType.DATA_MATRIX_SHAPE, SymbolShapeHint.FORCE_SQUARE}};

         const int bigEnough = 14;
         var matrix = writer.encode("Hello Me", BarcodeFormat.DATA_MATRIX, bigEnough, bigEnough, hints);

         Assert.IsNotNull(matrix);
         Assert.AreEqual(bigEnough, matrix.Width);
         Assert.AreEqual(bigEnough, matrix.Height);
      }

      [Test]
      public void testDataMatrixTooSmall()
      {
         // The DataMatrix will not fit in this size, so the matrix should come back bigger
         const int tooSmall = 8;
         var writer = new DataMatrixWriter();
         var matrix = writer.encode("http://www.google.com/", BarcodeFormat.DATA_MATRIX, tooSmall, tooSmall, null);

         Assert.IsNotNull(matrix);
         Assert.IsTrue(tooSmall < matrix.Width);
         Assert.IsTrue(tooSmall < matrix.Height);
      }
   }
}