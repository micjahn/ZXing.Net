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

using System.Collections.Generic;

using ZXing.Common;
using ZXing.Multi.QrCode.Internal;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace ZXing.Multi.QrCode
{
    /// <summary>
    /// This implementation can detect and decode multiple QR Codes in an image.
    /// </summary>
    public sealed class QRCodeMultiReader : QRCodeReader, MultipleBarcodeReader
    {
        private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

        /// <summary>
        /// Decodes the multiple.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public Result[] decodeMultiple(BinaryBitmap image)
        {
            return decodeMultiple(image, null);
        }

        /// <summary>
        /// Decodes the multiple.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hints">The hints.</param>
        /// <returns></returns>
        public Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
        {
            var results = new List<Result>();
            var detectorResults = new MultiDetector(image.BlackMatrix).detectMulti(hints);
            foreach (DetectorResult detectorResult in detectorResults)
            {
                var decoderResult = getDecoder().decode(detectorResult.Bits, hints);
                if (decoderResult == null)
                    continue;

                var points = detectorResult.Points;
                // If the code was mirrored: swap the bottom-left and the top-right points.
                var data = decoderResult.Other as QRCodeDecoderMetaData;
                if (data != null)
                {
                    data.applyMirroredCorrection(points);
                }
                var result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.QR_CODE);
                var byteSegments = decoderResult.ByteSegments;
                if (byteSegments != null)
                {
                    result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
                }
                var ecLevel = decoderResult.ECLevel;
                if (ecLevel != null)
                {
                    result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, ecLevel);
                }
                if (decoderResult.StructuredAppend)
                {
                    result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE, decoderResult.StructuredAppendSequenceNumber);
                    result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_PARITY, decoderResult.StructuredAppendParity);
                }
                results.Add(result);
            }
            if (results.Count == 0)
            {
                return null;
            }
            results = ProcessStructuredAppend(results);
            return results.ToArray();
        }

        internal static List<Result> ProcessStructuredAppend(List<Result> results)
        {
            var newResults = new List<Result>();
            var saResults = new List<Result>();
            foreach (var result in results)
            {
                if (result.ResultMetadata.ContainsKey(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE))
                {
                    saResults.Add(result);
                }
                else
                {
                    newResults.Add(result);
                }
            }
            if (saResults.Count == 0)
            {
                return results;
            }
            // sort and concatenate the SA list items
            saResults.Sort(SaSequenceSort);
            var newText = new System.Text.StringBuilder();
            using (var newRawBytes = new System.IO.MemoryStream())
            using (var newByteSegment = new System.IO.MemoryStream())
            {
                foreach (Result saResult in saResults)
                {
                    newText.Append(saResult.Text);
                    byte[] saBytes = saResult.RawBytes;
                    newRawBytes.Write(saBytes, 0, saBytes.Length);
                    if (saResult.ResultMetadata.ContainsKey(ResultMetadataType.BYTE_SEGMENTS))
                    {
                        var byteSegments = (IEnumerable<byte[]>) saResult.ResultMetadata[ResultMetadataType.BYTE_SEGMENTS];
                        if (byteSegments != null)
                        {
                            foreach (byte[] segment in byteSegments)
                            {
                                newByteSegment.Write(segment, 0, segment.Length);
                            }
                        }
                    }
                }

                Result newResult = new Result(newText.ToString(), newRawBytes.ToArray(), NO_POINTS, BarcodeFormat.QR_CODE);
                if (newByteSegment.Length > 0)
                {
                    var byteSegmentList = new List<byte[]>();
                    byteSegmentList.Add(newByteSegment.ToArray());
                    newResult.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegmentList);
                }
                newResults.Add(newResult);
            }
            return newResults;
        }

        private static int SaSequenceSort(Result a, Result b)
        {
            var aNumber = (int) (a.ResultMetadata[ResultMetadataType.STRUCTURED_APPEND_SEQUENCE]);
            var bNumber = (int) (b.ResultMetadata[ResultMetadataType.STRUCTURED_APPEND_SEQUENCE]);
            return aNumber - bNumber;
        }
    }
}