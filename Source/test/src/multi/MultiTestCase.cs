/*
 * Copyright 2016 ZXing authors
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

using System.Drawing;
using System.IO;

using NUnit.Framework;

using ZXing.Common;
using ZXing.Common.Test;

namespace ZXing.Multi.Test
{
   public class MultiTestCase
   {
      [Test]
      public void testMulti()
      {
         // Very basic test for now
         var testBase = AbstractBlackBoxTestCase.buildTestBase("test/data/blackbox/multi-1");

         var source = new BitmapLuminanceSource((Bitmap)Bitmap.FromFile(Path.Combine(testBase, "1.png")));
         var bitmap = new BinaryBitmap(new HybridBinarizer(source));

         var reader = new GenericMultipleBarcodeReader(new MultiFormatReader());
         var results = reader.decodeMultiple(bitmap);
         Assert.IsNotNull(results);
         Assert.AreEqual(2, results.Length);

         Assert.AreEqual("031415926531", results[0].Text);
         Assert.AreEqual(BarcodeFormat.UPC_A, results[0].BarcodeFormat);

         Assert.AreEqual("www.airtable.com/jobs", results[1].Text);
         Assert.AreEqual(BarcodeFormat.QR_CODE, results[1].BarcodeFormat);
      }
   }
}