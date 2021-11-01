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
using System.Linq;
using NUnit.Framework;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417.Test
{
    public class PDF417MacroTestCase
    {
        [Test]
        public void TestMacroPdfCreation()
        {
            List<Result> results = new List<Result>();

            var writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.PDF_417,
                Options = new PDF417EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    ErrorCorrection = PDF417ErrorCorrectionLevel.L5,
                    Compact = false,
                    Margin = 1
                }
            };

            writer.Options.Hints.Add(EncodeHintType.PDF417_MACRO_META_DATA, new PDF417MacroMetadata()
            {
                SegmentIndex = 0,
                SegmentCount = 2,
                FileId = "HELLO.WORLD",
                FileName = "Bar.code",
                Sender = "From",
                Addressee = "To",
                FileSize = 9001,
                Checksum = 300,
                Timestamp = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond
            });

            var reader = new BarcodeReader
            {
                Options =
                {
                    PureBarcode = true,
                    PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.PDF_417},
                    TryHarder = true,
                    //ReturnCodabarStartEnd = true
                }
            };

            var matrix1 = new PDF417Writer().encode("Hello", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            // Barcode 1 of 2
            using (Bitmap barcodeImg = writer.Write(matrix1))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            writer.Options.Hints[EncodeHintType.PDF417_MACRO_META_DATA] = new PDF417MacroMetadata()
            {
                SegmentIndex = 1,
                SegmentCount = 2,
                FileId = "HELLO.WORLD"
            };

            var matrix2 = new PDF417Writer().encode(" World", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            // Barcode 2 of 2
            using (Bitmap barcodeImg = writer.Write(matrix2))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            Assert.IsTrue(
                (
                    from r in results
                    where r != null
                       && r.ResultMetadata.ContainsKey(ResultMetadataType.PDF417_EXTRA_METADATA) == true
                       && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "214341448538674521119"
                       // && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "HELLO.WORLD" <- encoded with TEXT compaction, isn't spec conform
                       // fileId has to be between 0 and 899 and is directly encoded as codeword
                    select r
                ).Count() == 2
            );

            writer.Options.Hints[EncodeHintType.PDF417_MACRO_META_DATA] = new PDF417MacroMetadata()
            {
                SegmentIndex = 1,
                SegmentCount = 2,
                FileId = "023" // file id has to be a sequence of numbers between 000 and 899, otherwise TEXT compaction will be done
            };

            var matrix3 = new PDF417Writer().encode(" World", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            results.Clear();
            using (Bitmap barcodeImg = writer.Write(matrix3))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            Assert.IsTrue(
                (
                    from r in results
                    where r != null
                       && r.ResultMetadata.ContainsKey(ResultMetadataType.PDF417_EXTRA_METADATA) == true
                       && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "023"
                    select r
                ).Count() == 1
            );

            writer.Options.Hints[EncodeHintType.PDF417_MACRO_META_DATA] = new PDF417MacroMetadata()
            {
                SegmentIndex = 1,
                SegmentCount = 2,
                FileId = "023899257" // file id has to be a sequence of numbers between 000 and 899, otherwise TEXT compaction will be done
            };

            matrix3 = new PDF417Writer().encode(" World", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            results.Clear();
            using (Bitmap barcodeImg = writer.Write(matrix3))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            Assert.IsTrue(
                (
                    from r in results
                    where r != null
                       && r.ResultMetadata.ContainsKey(ResultMetadataType.PDF417_EXTRA_METADATA) == true
                       && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "023899257"
                    select r
                ).Count() == 1
            );

            writer.Options.Hints[EncodeHintType.PDF417_MACRO_META_DATA] = new PDF417MacroMetadata()
            {
                SegmentIndex = 1,
                SegmentCount = 2,
                FileId = "023899257ABC" // file id has to be a sequence of numbers between 000 and 899, otherwise TEXT compaction will be done
            };

            matrix3 = new PDF417Writer().encode(" World", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            results.Clear();
            using (Bitmap barcodeImg = writer.Write(matrix3))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            Assert.IsTrue(
                (
                    from r in results
                    where r != null
                       && r.ResultMetadata.ContainsKey(ResultMetadataType.PDF417_EXTRA_METADATA) == true
                       && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "840063249272157840032"
                    select r
                ).Count() == 1
            );

            writer.Options.Hints[EncodeHintType.PDF417_MACRO_META_DATA] = new PDF417MacroMetadata()
            {
                SegmentIndex = 1,
                SegmentCount = 2,
                FileId = "02389925" // file id has to be a sequence of numbers between 000 and 899, otherwise TEXT compaction will be done
            };

            matrix3 = new PDF417Writer().encode(" World", BarcodeFormat.PDF_417, writer.Options.Width, writer.Options.Height, writer.Options.Hints);

            results.Clear();
            using (Bitmap barcodeImg = writer.Write(matrix3))
            {
                var result = reader.Decode(barcodeImg);
                results.Add(result);
            }

            Assert.IsTrue(
                (
                    from r in results
                    where r != null
                       && r.ResultMetadata.ContainsKey(ResultMetadataType.PDF417_EXTRA_METADATA) == true
                       && ((PDF417ResultMetadata)r.ResultMetadata[ResultMetadataType.PDF417_EXTRA_METADATA]).FileId == "840063249272179"
                    select r
                ).Count() == 1
            );
        }
    }
}
