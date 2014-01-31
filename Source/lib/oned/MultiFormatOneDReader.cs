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

using System.Collections.Generic;

using ZXing.Common;
using ZXing.OneD.RSS;
using ZXing.OneD.RSS.Expanded;

namespace ZXing.OneD
{
   /// <summary>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class MultiFormatOneDReader : OneDReader
   {
      private readonly IList<OneDReader> readers;

      /// <summary>
      /// Initializes a new instance of the <see cref="MultiFormatOneDReader"/> class.
      /// </summary>
      /// <param name="hints">The hints.</param>
      public MultiFormatOneDReader(IDictionary<DecodeHintType, object> hints)
      {
         var possibleFormats = hints == null || !hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? null :
             (IList<BarcodeFormat>)hints[DecodeHintType.POSSIBLE_FORMATS];
         readers = new List<OneDReader>();
         if (possibleFormats != null)
         {
            if (possibleFormats.Contains(BarcodeFormat.All_1D) || 
                possibleFormats.Contains(BarcodeFormat.EAN_13) ||
                possibleFormats.Contains(BarcodeFormat.UPC_A) ||
                possibleFormats.Contains(BarcodeFormat.EAN_8) ||
                possibleFormats.Contains(BarcodeFormat.UPC_E))
            {
               readers.Add(new MultiFormatUPCEANReader(hints));
            }
            if (possibleFormats.Contains(BarcodeFormat.MSI))
            {
               // MSI needs to be activated explicit
               bool useMsiCheckDigit = (hints.ContainsKey(DecodeHintType.ASSUME_MSI_CHECK_DIGIT)
                                          ? (bool)hints[DecodeHintType.ASSUME_MSI_CHECK_DIGIT]
                                          : false);
               readers.Add(new MSIReader(useMsiCheckDigit));
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_39) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               bool useCode39CheckDigit = hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT) &&
                                          (bool) hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT];
               bool useCode39ExtendedMode = hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE) &&
                                            (bool) hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
               readers.Add(new Code39Reader(useCode39CheckDigit, useCode39ExtendedMode));
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_93) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new Code93Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_128) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new Code128Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.ITF) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new ITFReader());
            }
            if (possibleFormats.Contains(BarcodeFormat.CODABAR) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new CodaBarReader());
            }
            if (possibleFormats.Contains(BarcodeFormat.RSS_14) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new RSS14Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.RSS_EXPANDED) || possibleFormats.Contains(BarcodeFormat.All_1D))
            {
               readers.Add(new RSSExpandedReader());
            }
         }
         if (readers.Count == 0)
         {
            bool useCode39CheckDigit = hints != null && hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT) &&
                                       (bool) hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT];
            bool useCode39ExtendedMode = hints != null && hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE) &&
                                         (bool) hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
            // MSI needs to be activated explicit

            readers.Add(new MultiFormatUPCEANReader(hints));
            readers.Add(new Code39Reader(useCode39CheckDigit, useCode39ExtendedMode));
            readers.Add(new CodaBarReader());
            readers.Add(new Code93Reader());
            readers.Add(new Code128Reader());
            readers.Add(new ITFReader());
            readers.Add(new RSS14Reader());
            readers.Add(new RSSExpandedReader());
         }
      }

      /// <summary>
      ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
      /// an image.</p>
      /// </summary>
      /// <param name="rowNumber">row number from top of the row</param>
      /// <param name="row">the black/white pixel data of the row</param>
      /// <param name="hints">decode hints</param>
      /// <returns>
      ///   <see cref="Result"/>containing encoded string and start/end of barcode or null, if an error occurs or barcode cannot be found
      /// </returns>
      override public Result decodeRow(int rowNumber,
                              BitArray row,
                              IDictionary<DecodeHintType, object> hints)
      {
         foreach (OneDReader reader in readers)
         {
            var result = reader.decodeRow(rowNumber, row, hints);
            if (result != null)
               return result;
         }

         return null;
      }

      /// <summary>
      /// Resets any internal state the implementation has after a decode, to prepare it
      /// for reuse.
      /// </summary>
      public override void reset()
      {
         foreach (Reader reader in readers)
         {
            reader.reset();
         }
      }
   }
}