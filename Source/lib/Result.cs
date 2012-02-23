/*
* Copyright 2007 ZXing authors
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

namespace ZXing
{
   /// <summary> <p>Encapsulates the result of decoding a barcode within an image.</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class Result
   {
      private String text;
      private sbyte[] rawBytes;
      private ResultPoint[] resultPoints;
      private BarcodeFormat format;
      private IDictionary<ResultMetadataType, object> resultMetadata;
      private long timestamp;

      public Result(String text,
                    sbyte[] rawBytes,
                    ResultPoint[] resultPoints,
                    BarcodeFormat format)
         : this(text, rawBytes, resultPoints, format, DateTime.Now.Ticks)
      {
      }

      public Result(String text, sbyte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format, long timestamp)
      {
         if (text == null && rawBytes == null)
         {
            throw new ArgumentException("Text and bytes are null");
         }
         this.text = text;
         this.rawBytes = rawBytes;
         this.resultPoints = resultPoints;
         this.format = format;
         this.resultMetadata = null;
         this.timestamp = timestamp;
      }

      /// <returns> raw text encoded by the barcode, if applicable, otherwise <code>null</code>
      /// </returns>
      public String Text
      {
         get
         {
            return text;
         }

      }
      /// <returns> raw bytes encoded by the barcode, if applicable, otherwise <code>null</code>
      /// </returns>
      public sbyte[] RawBytes
      {
         get
         {
            return rawBytes;
         }

      }
      /// <returns> points related to the barcode in the image. These are typically points
      /// identifying finder patterns or the corners of the barcode. The exact meaning is
      /// specific to the type of barcode that was decoded.
      /// </returns>
      public ResultPoint[] ResultPoints
      {
         get
         {
            return resultPoints;
         }

      }
      /// <returns> {@link BarcodeFormat} representing the format of the barcode that was decoded
      /// </returns>
      public BarcodeFormat BarcodeFormat
      {
         get
         {
            return format;
         }

      }
      /// <returns> {@link Hashtable} mapping {@link ResultMetadataType} keys to values. May be
      /// <code>null</code>. This contains optional metadata about what was detected about the barcode,
      /// like orientation.
      /// </returns>
      public IDictionary<ResultMetadataType, object> ResultMetadata
      {
         get
         {
            return resultMetadata;
         }

      }

      public void putMetadata(ResultMetadataType type, Object value)
      {
         if (resultMetadata == null)
         {
            resultMetadata = new Dictionary<ResultMetadataType, object>();
         }
         resultMetadata[type] = value;
      }

      public void putAllMetadata(IDictionary<ResultMetadataType, object> metadata)
      {
         if (metadata != null)
         {
            if (resultMetadata == null)
            {
               resultMetadata = metadata;
            }
            else
            {
               foreach (var entry in metadata)
                  resultMetadata[entry.Key] = entry.Value;
            }
         }
      }

      public void addResultPoints(ResultPoint[] newPoints)
      {
         var oldPoints = resultPoints;
         if (oldPoints == null)
         {
            resultPoints = newPoints;
         }
         else if (newPoints != null && newPoints.Length > 0)
         {
            var allPoints = new ResultPoint[oldPoints.Length + newPoints.Length];
            Array.Copy(oldPoints, 0, allPoints, 0, oldPoints.Length);
            Array.Copy(newPoints, 0, allPoints, oldPoints.Length, newPoints.Length);
            resultPoints = allPoints;
         }
      }

      public long getTimestamp()
      {
         return timestamp;
      }

      public override String ToString()
      {
         if (text == null)
         {
            return "[" + rawBytes.Length + " bytes]";
         }
         return text;
      }
   }
}