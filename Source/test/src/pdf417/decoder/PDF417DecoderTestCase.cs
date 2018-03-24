/*
 * Copyright 2009 ZXing authors
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

namespace ZXing.PDF417.Internal.Test
{
   [TestFixture]
   public class PDF417DecoderTestCase
   {
      /// <summary>
      /// Tests the first sample given in ISO/IEC 15438:2015(E) - Annex H.4
      /// </summary>
      [Test]
      public void testStandardSample1()
      {
         PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
         int[] sampleCodes = {20, 928, 111, 100, 17, 53, 923, 1, 111, 104, 923, 3, 64, 416, 34, 923, 4, 258, 446, 67,
            // we should never reach these
            1000, 1000, 1000};

         DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata);

         Assert.AreEqual(0, resultMetadata.SegmentIndex);
         Assert.AreEqual("ARBX", resultMetadata.FileId);
         Assert.That(resultMetadata.IsLastSegment, Is.False);
         Assert.AreEqual(4, resultMetadata.SegmentCount);
         Assert.AreEqual("CEN BE", resultMetadata.Sender);
         Assert.AreEqual("ISO CH", resultMetadata.Addressee);

         //int[] optionalData = resultMetadata.OptionalData;
         //Assert.AreEqual(1, optionalData[0], "first element of optional array should be the first field identifier");
         //Assert.AreEqual(67, optionalData[optionalData.Length - 1], "last element of optional array should be the last codeword of the last field");
      }

      /// <summary>
      /// Tests the second given in ISO/IEC 15438:2015(E) - Annex H.4
      /// </summary>
      [Test]
      public void testStandardSample2()
      {
         PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
         int[] sampleCodes = {11, 928, 111, 103, 17, 53, 923, 1, 111, 104, 922,
            // we should never reach these
            1000, 1000, 1000};

         DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 2, resultMetadata);

         Assert.AreEqual(3, resultMetadata.SegmentIndex);
         Assert.AreEqual("ARBX", resultMetadata.FileId);
         Assert.That(resultMetadata.IsLastSegment, Is.True);
         Assert.AreEqual(4, resultMetadata.SegmentCount);
         Assert.That(resultMetadata.Addressee, Is.Null);
         Assert.That(resultMetadata.Sender, Is.Null);

         //int[] optionalData = resultMetadata.OptionalData;
         //Assert.AreEqual(1, optionalData[0], "first element of optional array should be the first field identifier");
         //Assert.AreEqual(104, optionalData[optionalData.Length - 1], "last element of optional array should be the last codeword of the last field");
      }

      [Test]
      public void testSampleWithFilename()
      {
         int[] sampleCodes = {23, 477, 928, 111, 100, 0, 252, 21, 86, 923, 0, 815, 251, 133, 12, 148, 537, 593,
            599, 923, 1, 111, 102, 98, 311, 355, 522, 920, 779, 40, 628, 33, 749, 267, 506, 213, 928, 465, 248,
            493, 72, 780, 699, 780, 493, 755, 84, 198, 628, 368, 156, 198, 809, 19, 113};
         PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

         DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 3, resultMetadata);

         Assert.AreEqual(0, resultMetadata.SegmentIndex);
         Assert.AreEqual("AAIMAVC ", resultMetadata.FileId);
         Assert.That(resultMetadata.IsLastSegment, Is.False);
         Assert.AreEqual(2, resultMetadata.SegmentCount);
         Assert.That(resultMetadata.Addressee, Is.Null);
         Assert.That(resultMetadata.Sender, Is.Null);
         Assert.AreEqual("filename.txt", resultMetadata.FileName);
      }

      [Test]
      public void testSampleWithNumericValues()
      {
         int[] sampleCodes = {25, 477, 928, 111, 100, 0, 252, 21, 86, 923, 2, 2, 0, 1, 0, 0, 0, 923, 5, 130, 923,
        6, 1, 500, 13, 0};
         PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();

         DecodedBitStreamParser.decodeMacroBlock(sampleCodes, 3, resultMetadata);

         Assert.AreEqual(0, resultMetadata.SegmentIndex);
         Assert.AreEqual("AAIMAVC ", resultMetadata.FileId);
         Assert.That(resultMetadata.IsLastSegment, Is.False);

         Assert.AreEqual(180980729000000L, resultMetadata.Timestamp);
         Assert.AreEqual(30, resultMetadata.FileSize);
         Assert.AreEqual(260013, resultMetadata.Checksum);
      }
   }
}