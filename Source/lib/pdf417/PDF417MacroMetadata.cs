
namespace ZXing.PDF417
{
    /// <summary>
    /// PDF417 Macro meta data.
    /// </summary>
    public sealed class PDF417MacroMetadata
    {
        /// <summary>
        /// The Segment ID represents the segment of the whole file distributed over different symbols.
        /// </summary>
        public int SegmentIndex { get; set; }

        /// <summary>
        /// Is the same for each related PDF417 symbol
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// true if it is the last segment
        /// </summary>
        public bool IsLastSegment { get; set; }

        /// <summary>
        /// count of segments, -1 if not set
        /// </summary>
        public int SegmentCount { get; set; }


        public string Sender { get; set; }

        public string Addressee { get; set; }

        /// <summary>
        /// Filename of the encoded file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// filesize in bytes of the encoded file
        /// returns filesize in bytes, -1 if not set
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 16-bit CRC checksum using CCITT-16
        /// returns crc checksum, -1 if not set
        /// </summary>
        public int? Checksum { get; set; }

        /// <summary>
        /// unix epock timestamp, elapsed seconds since 1970-01-01
        /// returns elapsed seconds, -1 if not set
        /// </summary>
        public long? Timestamp { get; set; }

    }
}
