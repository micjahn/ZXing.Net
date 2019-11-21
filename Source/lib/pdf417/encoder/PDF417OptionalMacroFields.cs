/*
 * Copyright 2011 ZXing authors
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

namespace ZXing.PDF417.Internal
{

    /// <summary>
    /// Macro PDF417 optional fields.
    /// </summary>
    /// <remarks>The values are set to their field designator.</remarks>
    internal enum PDF417OptionalMacroFields
    {
        /// <summary>
        /// The file name.
        /// Field designator: 0
        /// </summary>
        FileName = 0x0,

        /// <summary>
        /// The segment count field can contain values from 1 to 99,999.
        /// Field designator: 1
        /// </summary>
        SegmentCount = 0x1,

        /// <summary>
        /// The time stamp of the source file expressed in Unix time.
        /// Field designator: 2
        /// </summary>
        TimeStamp = 0x2,

        /// <summary>
        /// The sender.
        /// Field designator: 3
        /// </summary>
        Sender = 0x3,

        /// <summary>
        /// The addressee.
        /// Field designator: 4
        /// </summary>
        Addressee = 0x4,

        /// <summary>
        /// The file size in bytes.
        /// Field designator: 5
        /// </summary>
        FileSize = 0x5,

        /// <summary>
        /// The 16-bit CRC checksum using the CCITT-16 polynomial.
        /// Field designator: 6
        /// </summary>
        Checksum = 0x6

    }
}
