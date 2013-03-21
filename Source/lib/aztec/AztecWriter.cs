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

using System;
using System.Collections.Generic;
using System.Text;

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec
{
   /// <summary>
   /// Generates Aztec 2D barcodes.
   /// </summary>
   public sealed class AztecWriter : Writer
   {
      private static readonly Encoding DEFAULT_CHARSET;

      static AztecWriter()
      {
#if !(WindowsCE || SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE)
         DEFAULT_CHARSET = Encoding.GetEncoding("ISO-8859-1");
#elif WindowsCE
         try
         {
            DEFAULT_CHARSET = Encoding.GetEncoding("ISO-8859-1");
         }
         catch (PlatformNotSupportedException)
         {
            DEFAULT_CHARSET = Encoding.GetEncoding(1252);
         }
#else
         // not fully correct but what else
         DEFAULT_CHARSET = Encoding.GetEncoding("UTF-8");
#endif
      }

      /// <summary>
      /// Encode a barcode using the default settings.
      /// </summary>
      /// <param name="contents">The contents to encode in the barcode</param>
      /// <param name="format">The barcode format to generate</param>
      /// <param name="width">The preferred width in pixels</param>
      /// <param name="height">The preferred height in pixels</param>
      /// <returns>
      /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
      /// </returns>
      public BitMatrix encode(String contents, BarcodeFormat format, int width, int height)
      {
         return encode(contents, format, DEFAULT_CHARSET, Internal.Encoder.DEFAULT_EC_PERCENT);
      }

      /// <summary>
      /// </summary>
      /// <param name="contents">The contents to encode in the barcode</param>
      /// <param name="format">The barcode format to generate</param>
      /// <param name="width">The preferred width in pixels</param>
      /// <param name="height">The preferred height in pixels</param>
      /// <param name="hints">Additional parameters to supply to the encoder</param>
      /// <returns>
      /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
      /// </returns>
      public BitMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
      {
         Encoding charset = DEFAULT_CHARSET;
         int? eccPercent = null;
         if (hints != null)
         {
            if (hints.ContainsKey(EncodeHintType.CHARACTER_SET))
            {
               object charsetname = hints[EncodeHintType.CHARACTER_SET];
               if (charsetname != null)
               {
                  charset = Encoding.GetEncoding(charsetname.ToString());
               }
            }
            if (hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
            {
               object eccPercentObject = hints[EncodeHintType.ERROR_CORRECTION];
               if (eccPercentObject != null)
               {
                  eccPercent = Convert.ToInt32(eccPercentObject);
               }
            }
         }

         return encode(contents,
                       format,
                       charset,
                       eccPercent == null ? Internal.Encoder.DEFAULT_EC_PERCENT : eccPercent.Value);
      }

      private static BitMatrix encode(String contents, BarcodeFormat format, Encoding charset, int eccPercent)
      {
         if (format != BarcodeFormat.AZTEC)
         {
            throw new ArgumentException("Can only encode AZTEC code, but got " + format);
         }
         AztecCode aztec = Internal.Encoder.encode(charset.GetBytes(contents), eccPercent);
         return aztec.Matrix;
      }
   }
}