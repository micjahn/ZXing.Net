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
using com.google.zxing.aztec;
using com.google.zxing.maxicode;
using MultiFormatOneDReader = com.google.zxing.oned.MultiFormatOneDReader;
using PDF417Reader = com.google.zxing.pdf417.PDF417Reader;
using QRCodeReader = com.google.zxing.qrcode.QRCodeReader;
using DataMatrixReader = com.google.zxing.datamatrix.DataMatrixReader;

namespace com.google.zxing
{
   /// <summary> MultiFormatReader is a convenience class and the main entry point into the library for most uses.
   /// By default it attempts to decode all barcode formats that the library supports. Optionally, you
   /// can provide a hints object to request different behavior, for example only decoding QR codes.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>  dswitkin@google.com (Daniel Switkin)
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class MultiFormatReader : Reader
   {
      private IDictionary<DecodeHintType, object> hints;
      private IList<Reader> readers;

      /// <summary> This version of decode honors the intent of Reader.decode(BinaryBitmap) in that it
      /// passes null as a hint to the decoders. However, that makes it inefficient to call repeatedly.
      /// Use setHints() followed by decodeWithState() for continuous scan applications.
      /// 
      /// </summary>
      /// <param name="image">The pixel data to decode
      /// </param>
      /// <returns> The contents of the image
      /// </returns>
      /// <throws>  ReaderException Any errors which occurred </throws>
      public Result decode(BinaryBitmap image)
      {
         Hints = null;
         return decodeInternal(image);
      }

      /// <summary> Decode an image using the hints provided. Does not honor existing state.
      /// 
      /// </summary>
      /// <param name="image">The pixel data to decode
      /// </param>
      /// <param name="hints">The hints to use, clearing the previous state.
      /// </param>
      /// <returns> The contents of the image
      /// </returns>
      /// <throws>  ReaderException Any errors which occurred </throws>
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         Hints = hints;
         return decodeInternal(image);
      }

      /// <summary> Decode an image using the state set up by calling setHints() previously. Continuous scan
      /// clients will get a <b>large</b> speed increase by using this instead of decode().
      /// 
      /// </summary>
      /// <param name="image">The pixel data to decode
      /// </param>
      /// <returns> The contents of the image
      /// </returns>
      /// <throws>  ReaderException Any errors which occurred </throws>
      public Result decodeWithState(BinaryBitmap image)
      {
         // Make sure to set up the default state so we don't crash
         if (readers == null)
         {
            Hints = null;
         }
         return decodeInternal(image);
      }

      /// <summary> This method adds state to the MultiFormatReader. By setting the hints once, subsequent calls
      /// to decodeWithState(image) can reuse the same set of readers without reallocating memory. This
      /// is important for performance in continuous scan clients.
      /// 
      /// </summary>
      /// <param name="hints">The set of hints to use for subsequent calls to decode(image)
      /// </param>
      public IDictionary<DecodeHintType, object> Hints
      {
         set
         {
            hints = value;

            var tryHarder = value != null && value.ContainsKey(DecodeHintType.TRY_HARDER);
            var formats = value == null ? null : (IList<BarcodeFormat>)value[DecodeHintType.POSSIBLE_FORMATS];
            readers = new List<Reader>();

            if (formats != null)
            {
               bool addOneDReader =
                  formats.Contains(BarcodeFormat.UPC_A) ||
                  formats.Contains(BarcodeFormat.UPC_E) ||
                  formats.Contains(BarcodeFormat.EAN_13) ||
                  formats.Contains(BarcodeFormat.EAN_8) ||
                  //formats.Contains(BarcodeFormat.CODABAR) ||
                  formats.Contains(BarcodeFormat.CODE_39) ||
                  formats.Contains(BarcodeFormat.CODE_93) ||
                  formats.Contains(BarcodeFormat.CODE_128) ||
                  formats.Contains(BarcodeFormat.ITF) ||
                  formats.Contains(BarcodeFormat.RSS_14) ||
                  formats.Contains(BarcodeFormat.RSS_EXPANDED);

               // Put 1D readers upfront in "normal" mode
               if (addOneDReader && !tryHarder)
               {
                  readers.Add(new MultiFormatOneDReader(value));
               }
               if (formats.Contains(BarcodeFormat.QR_CODE))
               {
                  readers.Add(new QRCodeReader());
               }
               if (formats.Contains(BarcodeFormat.DATA_MATRIX))
               {
                  readers.Add(new DataMatrixReader());
               }
               if (formats.Contains(BarcodeFormat.AZTEC))
               {
                  readers.Add(new AztecReader());
               }
               if (formats.Contains(BarcodeFormat.PDF_417))
               {
                  readers.Add(new PDF417Reader());
               }
               if (formats.Contains(BarcodeFormat.MAXICODE))
               {
                  readers.Add(new MaxiCodeReader());
               }
               // At end in "try harder" mode
               if (addOneDReader && tryHarder)
               {
                  readers.Add(new MultiFormatOneDReader(value));
               }
            }

            if (readers.Count == 0)
            {
               if (!tryHarder)
               {
                  readers.Add(new MultiFormatOneDReader(value));
               }
               readers.Add(new QRCodeReader());
               readers.Add(new DataMatrixReader());
               readers.Add(new AztecReader());
               readers.Add(new PDF417Reader());
               readers.Add(new MaxiCodeReader());

               if (tryHarder)
               {
                  readers.Add(new MultiFormatOneDReader(value));
               }
            }
         }

      }

      public void reset()
      {
         if (readers != null)
         {
            foreach (var reader in readers)
            {
               reader.reset();
            }
         }
      }

      private Result decodeInternal(BinaryBitmap image)
      {
         if (readers != null)
         {
            foreach (var reader in readers)
            {
               try
               {
                  return reader.decode(image, hints);
               }
               catch (ReaderException)
               {
                  // continue
               }
            }
         }

         throw ReaderException.Instance;
      }
   }
}