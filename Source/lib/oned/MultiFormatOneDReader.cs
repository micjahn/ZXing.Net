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

using com.google.zxing.common;
using com.google.zxing.oned.rss;
using com.google.zxing.oned.rss.expanded;

namespace com.google.zxing.oned
{
   /// <summary>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class MultiFormatOneDReader : OneDReader
   {
      private OneDReader[] readers;

      public MultiFormatOneDReader(IDictionary<DecodeHintType, object> hints)
      {
         var possibleFormats = hints == null || !hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? null :
             (IList<BarcodeFormat>)hints[DecodeHintType.POSSIBLE_FORMATS];
         bool useCode39CheckDigit = hints != null && hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT) &&
             hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT] != null;
         var readers = new List<OneDReader>();
         if (possibleFormats != null)
         {
            if (possibleFormats.Contains(BarcodeFormat.EAN_13) ||
                possibleFormats.Contains(BarcodeFormat.UPC_A) ||
                possibleFormats.Contains(BarcodeFormat.EAN_8) ||
                possibleFormats.Contains(BarcodeFormat.UPC_E))
            {
               readers.Add(new MultiFormatUPCEANReader(hints));
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_39))
            {
               readers.Add(new Code39Reader(useCode39CheckDigit));
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_93))
            {
               readers.Add(new Code93Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.CODE_128))
            {
               readers.Add(new Code128Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.ITF))
            {
               readers.Add(new ITFReader());
            }
            if (possibleFormats.Contains(BarcodeFormat.CODABAR))
            {
               readers.Add(new CodaBarReader());
            }
            if (possibleFormats.Contains(BarcodeFormat.RSS_14))
            {
               readers.Add(new RSS14Reader());
            }
            if (possibleFormats.Contains(BarcodeFormat.RSS_EXPANDED))
            {
               readers.Add(new RSSExpandedReader());
            }
         }
         if (readers.Count == 0)
         {
            readers.Add(new MultiFormatUPCEANReader(hints));
            readers.Add(new Code39Reader());
            //readers.Add(new CodaBarReader());
            readers.Add(new Code93Reader());
            readers.Add(new Code128Reader());
            readers.Add(new ITFReader());
            readers.Add(new RSS14Reader());
            readers.Add(new RSSExpandedReader());
         }
         this.readers = readers.ToArray();
      }

      override public Result decodeRow(int rowNumber,
                              BitArray row,
                              IDictionary<DecodeHintType, object> hints)
      {
         foreach (OneDReader reader in readers)
         {
            try
            {
               return reader.decodeRow(rowNumber, row, hints);
            }
            catch (ReaderException re)
            {
               // continue
            }
         }

         throw NotFoundException.Instance;
      }

      public override void reset()
      {
         foreach (Reader reader in readers)
         {
            reader.reset();
         }
      }
   }
}