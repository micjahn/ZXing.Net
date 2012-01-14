/*
* Copyright 2008 ZXing authors
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

namespace com.google.zxing.client.result.optional
{
   /// <summary> <p>Represents a record in an NDEF message. This class only supports certain types
   /// of records -- namely, non-chunked records, where ID length is omitted, and only
   /// "short records".</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class NDEFRecord
   {
      private readonly int header;

      internal bool MessageBegin
      {
         get
         {
            return (header & 0x80) != 0;
         }

      }
      internal bool MessageEnd
      {
         get
         {
            return (header & 0x40) != 0;
         }

      }

      internal String Type { get; private set; }
      internal sbyte[] Payload { get; private set; }
      internal int TotalRecordLength { get; private set; }

      private const int SUPPORTED_HEADER_MASK = 0x3F; // 0 0 1 1 1 111 (the bottom 6 bits matter)
      private const int SUPPORTED_HEADER = 0x11; // 0 0 0 1 0 001

      public const String TEXT_WELL_KNOWN_TYPE = "T";
      public const String URI_WELL_KNOWN_TYPE = "U";
      public const String SMART_POSTER_WELL_KNOWN_TYPE = "Sp";
      public const String ACTION_WELL_KNOWN_TYPE = "act";


      private NDEFRecord(int header, String type, sbyte[] payload, int totalRecordLength)
      {
         this.header = header;
         Type = type;
         Payload = payload;
         TotalRecordLength = totalRecordLength;
      }

      internal static NDEFRecord readRecord(sbyte[] bytes, int offset)
      {
         var header = bytes[offset] & 0xFF;

         // Does header match what we support in the bits we care about?
         // XOR figures out where we differ, and if any of those are in the mask, fail
         if (((header ^ SUPPORTED_HEADER) & SUPPORTED_HEADER_MASK) != 0)
         {
            return null;
         }
         var typeLength = bytes[offset + 1] & 0xFF;

         var payloadLength = bytes[offset + 2] & 0xFF;

         var type = AbstractNDEFResultParser.bytesToString(bytes, offset + 3, typeLength, "US-ASCII");

         var payload = new sbyte[payloadLength];
         Array.Copy(bytes, offset + 3 + typeLength, payload, 0, payloadLength);

         return new NDEFRecord(header, type, payload, 3 + typeLength + payloadLength);
      }
   }
}