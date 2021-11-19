/*
 * Copyright 2013 ZXing authors
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

namespace ZXing.PDF417
{
    /// <summary>
    /// PDF 417 result meta data.
    /// <author>Guenther Grau</author>
    /// </summary>
    public sealed class PDF417ResultMetadata
    {
        public PDF417ResultMetadata()
        {
            SegmentCount = -1;
            FileSize = -1;
            Timestamp = -1;
            Checksum = -1;
        }

        /// <summary>
        /// The Segment ID represents the segment of the whole file distributed over different symbols.
        /// </summary>
        public int SegmentIndex { get; set; }
        /// <summary>
        /// Is the same for each related PDF417 symbol
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// old optional data format as int array, always null
        /// </summary>
        [System.Obsolete("use dedicated already parsed fields")]
        public int[] OptionalData { get; set; }
        /// <summary>
        /// true if it is the last segment
        /// </summary>
        public bool IsLastSegment { get; set; }

        /// <summary>
        /// count of segments, -1 if not set
        /// </summary>
        public int SegmentCount { get; set; }

        /// <summary>
        /// sender
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// address
        /// </summary>
        public string Addressee { get; set; }

        /// <summary>
        /// Filename of the encoded file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// filesize in bytes of the encoded file
        /// returns filesize in bytes, -1 if not set
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 16-bit CRC checksum using CCITT-16
        /// returns crc checksum, -1 if not set
        /// </summary>
        public int Checksum { get; set; }

        /// <summary>
        /// unix epock timestamp, elapsed seconds since 1970-01-01
        /// returns elapsed seconds, -1 if not set
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// string represenation of that instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("SegmentIndex:{0};SegmentCount:{1};IsLastSegment:{2};FileSize:{3};Checksum:{4};Timestamp:{5};FileId:{6};FileName:{7};Sender:{8};Addressee:{9};", SegmentIndex, SegmentCount, IsLastSegment, FileSize, Checksum, Timestamp, FileId, FileName, Sender, Addressee);
        }

    }
}