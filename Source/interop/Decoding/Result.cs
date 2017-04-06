/*
 * Copyright 2017 ZXing.Net authors
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
using System.Runtime.InteropServices;

using ZXing.Interop.Common;

namespace ZXing.Interop.Decoding
{
   /// <summary>
   /// Encapsulates the result of decoding a barcode within an image.
   /// </summary>
   [ComVisible(true)]
   [Guid("6A7AC019-6108-474E-9806-E36F5409EE66")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public sealed class Result : IResult
   {
      /// <returns>raw text encoded by the barcode, if applicable, otherwise <code>null</code></returns>
      public String Text { get; private set; }

      /// <returns>raw bytes encoded by the barcode, if applicable, otherwise <code>null</code></returns>
      public byte[] RawBytes { get; private set; }

      /// <returns>
      /// points related to the barcode in the image. These are typically points
      /// identifying finder patterns or the corners of the barcode. The exact meaning is
      /// specific to the type of barcode that was decoded.
      /// </returns>
      public ResultPoint[] ResultPoints { get; private set; }

      /// <returns>{@link BarcodeFormat} representing the format of the barcode that was decoded</returns>
      public Common.BarcodeFormat BarcodeFormat { get; private set; }

      /// <returns>
      /// {@link Hashtable} mapping {@link ResultMetadataType} keys to values. May be
      /// <code>null</code>. This contains optional metadata about what was detected about the barcode,
      /// like orientation.
      /// </returns>
      public ResultMetadataItem[] ResultMetadata { get; private set; }

      /// <summary>
      /// Gets the timestamp.
      /// </summary>
      public long Timestamp { get; private set; }

      /// <summary>
      /// how many bits of <see cref="RawBytes"/> are valid; typically 8 times its length
      /// </summary>
      public int NumBits { get; private set; }

      internal Result(ZXing.Result result)
      {
         if (result != null)
         {
            Text = result.Text;
            RawBytes = result.RawBytes;
            ResultPoints = result.ResultPoints.ToInteropResultPoints();
            BarcodeFormat = result.BarcodeFormat.ToInterop();
            if (result.ResultMetadata != null)
            {
               ResultMetadata = new ResultMetadataItem[result.ResultMetadata.Count];
               var index = 0;
               foreach (var item in result.ResultMetadata)
               {
                  ResultMetadata[index] = new ResultMetadataItem {Key = item.Key.ToInterop(), Value = item.Value != null ? item.Value.ToString() : null};
                  index++;
               }
            }
            Timestamp = result.Timestamp;
            NumBits = result.NumBits;
         }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override String ToString()
      {
         if (Text == null)
         {
            return "[" + RawBytes.Length + " bytes]";
         }
         return Text;
      }
   }

   [ComVisible(true)]
   [Guid("E6B8E5FD-E301-416B-9620-BA86FB5EA1B7")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class ResultMetadataItem
   {
      public ResultMetadataType Key { get; internal set; }
      public string Value { get; internal set; }

      public override string ToString()
      {
         return Key + ":" + (Value ?? String.Empty);
      }
   }

   /// <summary>
   /// Represents some type of metadata about the result of the decoding that the decoder
   /// wishes to communicate back to the caller.
   /// </summary>
   /// <author>Sean Owen</author>
   [ComVisible(true)]
   [Guid("7089C95E-18C4-4A67-B0F1-FC1D6D14EDD2")]
   public enum ResultMetadataType
   {
      /// <summary>
      /// Unspecified, application-specific metadata. Maps to an unspecified {@link Object}.
      /// </summary>
      OTHER,

      /// <summary>
      /// Denotes the likely approximate orientation of the barcode in the image. This value
      /// is given as degrees rotated clockwise from the normal, upright orientation.
      /// For example a 1D barcode which was found by reading top-to-bottom would be
      /// said to have orientation "90". This key maps to an {@link Integer} whose
      /// value is in the range [0,360).
      /// </summary>
      ORIENTATION,

      /// <summary>
      /// <p>2D barcode formats typically encode text, but allow for a sort of 'byte mode'
      /// which is sometimes used to encode binary data. While {@link Result} makes available
      /// the complete raw bytes in the barcode for these formats, it does not offer the bytes
      /// from the byte segments alone.</p>
      /// <p>This maps to a {@link java.util.List} of byte arrays corresponding to the
      /// raw bytes in the byte segments in the barcode, in order.</p>
      /// </summary>
      BYTE_SEGMENTS,

      /// <summary>
      /// Error correction level used, if applicable. The value type depends on the
      /// format, but is typically a String.
      /// </summary>
      ERROR_CORRECTION_LEVEL,

      /// <summary>
      /// For some periodicals, indicates the issue number as an {@link Integer}.
      /// </summary>
      ISSUE_NUMBER,

      /// <summary>
      /// For some products, indicates the suggested retail price in the barcode as a
      /// formatted {@link String}.
      /// </summary>
      SUGGESTED_PRICE,

      /// <summary>
      /// For some products, the possible country of manufacture as a {@link String} denoting the
      /// ISO country code. Some map to multiple possible countries, like "US/CA".
      /// </summary>
      POSSIBLE_COUNTRY,

      /// <summary>
      /// For some products, the extension text
      /// </summary>
      UPC_EAN_EXTENSION,

      /// <summary>
      /// If the code format supports structured append and
      /// the current scanned code is part of one then the
      /// sequence number is given with it.
      /// </summary>
      STRUCTURED_APPEND_SEQUENCE,

      /// <summary>
      /// If the code format supports structured append and
      /// the current scanned code is part of one then the
      /// parity is given with it.
      /// </summary>
      STRUCTURED_APPEND_PARITY,

      /// <summary>
      /// PDF417-specific metadata
      /// </summary>
      PDF417_EXTRA_METADATA,

      /// <summary>
      /// Aztec-specific metadata
      /// </summary>
      AZTEC_EXTRA_METADATA
   }

   internal static class ResultMetadataTypeExtensions
   {
      public static ResultMetadataType ToInterop(this ZXing.ResultMetadataType metadataType)
      {
         switch (metadataType)
         {
            case ZXing.ResultMetadataType.AZTEC_EXTRA_METADATA:
               return ResultMetadataType.AZTEC_EXTRA_METADATA;
            case ZXing.ResultMetadataType.BYTE_SEGMENTS:
               return ResultMetadataType.BYTE_SEGMENTS;
            case ZXing.ResultMetadataType.ERROR_CORRECTION_LEVEL:
               return ResultMetadataType.ERROR_CORRECTION_LEVEL;
            case ZXing.ResultMetadataType.ISSUE_NUMBER:
               return ResultMetadataType.ISSUE_NUMBER;
            case ZXing.ResultMetadataType.ORIENTATION:
               return ResultMetadataType.ORIENTATION;
            case ZXing.ResultMetadataType.OTHER:
               return ResultMetadataType.OTHER;
            case ZXing.ResultMetadataType.PDF417_EXTRA_METADATA:
               return ResultMetadataType.PDF417_EXTRA_METADATA;
            case ZXing.ResultMetadataType.POSSIBLE_COUNTRY:
               return ResultMetadataType.POSSIBLE_COUNTRY;
            case ZXing.ResultMetadataType.STRUCTURED_APPEND_PARITY:
               return ResultMetadataType.STRUCTURED_APPEND_PARITY;
            case ZXing.ResultMetadataType.STRUCTURED_APPEND_SEQUENCE:
               return ResultMetadataType.STRUCTURED_APPEND_SEQUENCE;
            case ZXing.ResultMetadataType.SUGGESTED_PRICE:
               return ResultMetadataType.SUGGESTED_PRICE;
            case ZXing.ResultMetadataType.UPC_EAN_EXTENSION:
               return ResultMetadataType.UPC_EAN_EXTENSION;
            default:
               return ResultMetadataType.OTHER;
         }
      }
   }
}