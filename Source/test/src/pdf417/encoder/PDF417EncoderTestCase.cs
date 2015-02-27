/*
 * Copyright (C) 2014 ZXing authors
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

using System.Text;

using NUnit.Framework;

namespace ZXing.PDF417.Internal.Test
{
   public sealed class PDF417EncoderTestCase
   {
      [Test]
      public void testEncodeAuto()
      {
         var encoded = PDF417HighLevelEncoder.encodeHighLevel(
            "ABCD", Compaction.AUTO, Encoding.UTF8, false);
         Assert.AreEqual("\u039f\u001A\u0385ABCD", encoded);
      }

      [Test]
      public void testEncodeText()
      {
         var encoded = PDF417HighLevelEncoder.encodeHighLevel(
            "ABCD", Compaction.TEXT, Encoding.UTF8, false);
         Assert.AreEqual("Ο\u001A\u0001?", encoded);
      }

      [Test]
      public void testEncodeNumeric()
      {
         var encoded = PDF417HighLevelEncoder.encodeHighLevel(
            "1234", Compaction.NUMERIC, Encoding.UTF8, false);
         Assert.AreEqual("\u039f\u001A\u0386\f\u01b2", encoded);
      }

      [Test]
      public void testEncodeByte()
      {
         var encoded = PDF417HighLevelEncoder.encodeHighLevel(
            "abcd", Compaction.BYTE, Encoding.UTF8, false);
         Assert.AreEqual("\u039f\u001A\u0385abcd", encoded);
      }
   }
}