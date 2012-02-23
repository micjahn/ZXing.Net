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
using ErrorCorrectionLevel = ZXing.QrCode.Internal.ErrorCorrectionLevel;

namespace ZXing.Common
{

   /// <summary> <p>Encapsulates the result of decoding a matrix of bits. This typically
   /// applies to 2D barcode formats. For now it contains the raw bytes obtained,
   /// as well as a String interpretation of those bytes, if applicable.</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class DecoderResult
   {
      private sbyte[] rawBytes;
      private String text;
      private IList<sbyte[]> byteSegments;
      private String ecLevel;

      public sbyte[] RawBytes
      {
         get
         {
            return rawBytes;
         }

      }
      public String Text
      {
         get
         {
            return text;
         }

      }
      public IList<sbyte[]> ByteSegments
      {
         get
         {
            return byteSegments;
         }

      }
      public String ECLevel
      {
         get
         {
            return ecLevel;
         }

      }

      public DecoderResult(sbyte[] rawBytes, String text, IList<sbyte[]> byteSegments, String ecLevel)
      {
         if (rawBytes == null && text == null)
         {
            throw new ArgumentException();
         }
         this.rawBytes = rawBytes;
         this.text = text;
         this.byteSegments = byteSegments;
         this.ecLevel = ecLevel;
      }
   }
}