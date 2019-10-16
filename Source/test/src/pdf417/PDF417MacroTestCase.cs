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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417.Test
{
    public class PDF417MacroTestCase
    {
        [Test]
        public void TestMacroPdfCreation()
        {
            var writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.PDF_417,
                Options = new PDF417EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    ErrorCorrection = PDF417ErrorCorrectionLevel.L5,
                    Compact = false,
                    Margin = 0
                }
            };

            //writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, PDF417ErrorCorrectionLevel.L5);
            //writer.Options.Hints.Add(EncodeHintType.PDF417_COMPACTION, Compaction.TEXT);
            writer.Options.Hints.Add(EncodeHintType.PDF417_MACRO_META_DATA, new PDF417MacroMetadata()
            {
                SegmentIndex = 0,
                SegmentCount = 2,
                FileId = "HELLO.WORLD"
            });

            var reader = new BarcodeReader
            {
                Options =
                {
                    PureBarcode = true,
                    PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.PDF_417},
                    TryHarder = true,
                    ReturnCodabarStartEnd = true
                }
            };

            var matrix = new PDF417Writer().encode("Hello", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            Result result;

            using (Bitmap barcodeImg = writer.Write(matrix))
            {
                result = reader.Decode(barcodeImg);
                // Save the image
                barcodeImg.Save($@"{AppDomain.CurrentDomain.BaseDirectory}\macro-test.png", ImageFormat.Png);
            }

            using (Bitmap otherTestImg = (Bitmap)Bitmap.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}\php-generated-macro-test.png"))
            {
                result = reader.Decode(otherTestImg);
            }

            Assert.IsTrue(result != null);
        }
    }
}
