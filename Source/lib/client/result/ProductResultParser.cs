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
using ZXing.OneD;

namespace ZXing.Client.Result
{
   /// <summary>
   /// Parses strings of digits that represent a UPC code.
   /// </summary>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   internal sealed class ProductResultParser : ResultParser
   {
      // Treat all UPC and EAN variants as UPCs, in the sense that they are all product barcodes.
      public override ParsedResult parse(ZXing.Result result)
      {
         BarcodeFormat format = result.BarcodeFormat;
         if (!(format == BarcodeFormat.UPC_A || format == BarcodeFormat.UPC_E ||
               format == BarcodeFormat.EAN_8 || format == BarcodeFormat.EAN_13))
         {
            return null;
         }
         // Really neither of these should happen:
         String rawText = result.Text;
         if (rawText == null)
         {
            return null;
         }

         if (!isStringOfDigits(rawText, rawText.Length))
         {
            return null;
         }
         // Not actually checking the checksum again here    

         String normalizedProductID;
         // Expand UPC-E for purposes of searching
         if (format == BarcodeFormat.UPC_E && rawText.Length == 8)
         {
            normalizedProductID = UPCEReader.convertUPCEtoUPCA(rawText);
         }
         else
         {
            normalizedProductID = rawText;
         }

         return new ProductParsedResult(rawText, normalizedProductID);
      }
   }
}