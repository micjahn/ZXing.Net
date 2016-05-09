/*
 * Copyright 2006 Jeremias Maerki
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
   /// Tests the SymbolInfo class.
   /// </summary>
   [TestFixture]
   public sealed class SymbolInfoTestCase
   {
      [Test]
      public void testSymbolInfo()
      {
         SymbolInfo info = SymbolInfo.lookup(3);
         Assert.AreEqual(5, info.errorCodewords);
         Assert.AreEqual(8, info.matrixWidth);
         Assert.AreEqual(8, info.matrixHeight);
         Assert.AreEqual(10, info.getSymbolWidth());
         Assert.AreEqual(10, info.getSymbolHeight());

         info = SymbolInfo.lookup(3, SymbolShapeHint.FORCE_RECTANGLE);
         Assert.AreEqual(7, info.errorCodewords);
         Assert.AreEqual(16, info.matrixWidth);
         Assert.AreEqual(6, info.matrixHeight);
         Assert.AreEqual(18, info.getSymbolWidth());
         Assert.AreEqual(8, info.getSymbolHeight());

         info = SymbolInfo.lookup(9);
         Assert.AreEqual(11, info.errorCodewords);
         Assert.AreEqual(14, info.matrixWidth);
         Assert.AreEqual(6, info.matrixHeight);
         Assert.AreEqual(32, info.getSymbolWidth());
         Assert.AreEqual(8, info.getSymbolHeight());

         info = SymbolInfo.lookup(9, SymbolShapeHint.FORCE_SQUARE);
         Assert.AreEqual(12, info.errorCodewords);
         Assert.AreEqual(14, info.matrixWidth);
         Assert.AreEqual(14, info.matrixHeight);
         Assert.AreEqual(16, info.getSymbolWidth());
         Assert.AreEqual(16, info.getSymbolHeight());

         try
         {
            SymbolInfo.lookup(1559);
            throw new AssertionException("There's no rectangular symbol for more than 1558 data codewords");
         }
         catch (ArgumentException iae)
         {
            //expected
         }
         try
         {
            SymbolInfo.lookup(50, SymbolShapeHint.FORCE_RECTANGLE);
            throw new AssertionException("There's no rectangular symbol for 50 data codewords");
         }
         catch (ArgumentException iae)
         {
            //expected
         }

         info = SymbolInfo.lookup(35);
         Assert.AreEqual(24, info.getSymbolWidth());
         Assert.AreEqual(24, info.getSymbolHeight());

         Dimension fixedSize = new Dimension(26, 26);
         info = SymbolInfo.lookup(35,
                                  SymbolShapeHint.FORCE_NONE, fixedSize, fixedSize, false);
         Assert.AreEqual(26, info.getSymbolWidth());
         Assert.AreEqual(26, info.getSymbolHeight());

         info = SymbolInfo.lookup(45,
                                  SymbolShapeHint.FORCE_NONE, fixedSize, fixedSize, false);
         Assert.IsNull(info);

         Dimension minSize = fixedSize;
         Dimension maxSize = new Dimension(32, 32);

         info = SymbolInfo.lookup(35,
                                  SymbolShapeHint.FORCE_NONE, minSize, maxSize, false);
         Assert.AreEqual(26, info.getSymbolWidth());
         Assert.AreEqual(26, info.getSymbolHeight());

         info = SymbolInfo.lookup(40,
                                  SymbolShapeHint.FORCE_NONE, minSize, maxSize, false);
         Assert.AreEqual(26, info.getSymbolWidth());
         Assert.AreEqual(26, info.getSymbolHeight());

         info = SymbolInfo.lookup(45,
                                  SymbolShapeHint.FORCE_NONE, minSize, maxSize, false);
         Assert.AreEqual(32, info.getSymbolWidth());
         Assert.AreEqual(32, info.getSymbolHeight());

         info = SymbolInfo.lookup(63,
                                  SymbolShapeHint.FORCE_NONE, minSize, maxSize, false);
         Assert.IsNull(info);
      }
   }
}