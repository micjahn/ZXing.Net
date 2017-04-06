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
 
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZXing.Interop.Common
{
   [System.Flags]
   [ComVisible(true)]
   [Guid("AB573F22-4F28-494C-BEF5-47940DC94924")]
   public enum BarcodeFormat
   {
      /// <summary>Aztec 2D barcode format.</summary>
      AZTEC = 1,

      /// <summary>CODABAR 1D format.</summary>
      CODABAR = 2,

      /// <summary>Code 39 1D format.</summary>
      CODE_39 = 4,

      /// <summary>Code 93 1D format.</summary>
      CODE_93 = 8,

      /// <summary>Code 128 1D format.</summary>
      CODE_128 = 16,

      /// <summary>Data Matrix 2D barcode format.</summary>
      DATA_MATRIX = 32,

      /// <summary>EAN-8 1D format.</summary>
      EAN_8 = 64,

      /// <summary>EAN-13 1D format.</summary>
      EAN_13 = 128,

      /// <summary>ITF (Interleaved Two of Five) 1D format.</summary>
      ITF = 256,

      /// <summary>MaxiCode 2D barcode format.</summary>
      MAXICODE = 512,

      /// <summary>PDF417 format.</summary>
      PDF_417 = 1024,

      /// <summary>QR Code 2D barcode format.</summary>
      QR_CODE = 2048,

      /// <summary>RSS 14</summary>
      RSS_14 = 4096,

      /// <summary>RSS EXPANDED</summary>
      RSS_EXPANDED = 8192,

      /// <summary>UPC-A 1D format.</summary>
      UPC_A = 16384,

      /// <summary>UPC-E 1D format.</summary>
      UPC_E = 32768,

      /// <summary>UPC/EAN extension format. Not a stand-alone format.</summary>
      UPC_EAN_EXTENSION = 65536,

      /// <summary>MSI</summary>
      MSI = 131072,

      /// <summary>Plessey</summary>
      PLESSEY = 262144,

      /// <summary>Intelligent Mail barcode</summary>
      IMB = 524288,

      /// <summary>
      /// UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 | ITF | RSS_14 | RSS_EXPANDED
      /// without MSI (to many false-positives) and IMB (not enough tested, and it looks more like a 2D)
      /// </summary>
      All_1D = UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 | ITF | RSS_14 | RSS_EXPANDED
   }

   internal static class BarcodeFormatExtensions
   {
      public static ZXing.BarcodeFormat ToZXing(this BarcodeFormat format)
      {
         switch (format)
         {
            case BarcodeFormat.AZTEC:
               return ZXing.BarcodeFormat.AZTEC;
            case BarcodeFormat.All_1D:
               return ZXing.BarcodeFormat.All_1D;
            case BarcodeFormat.CODABAR:
               return ZXing.BarcodeFormat.CODABAR;
            case BarcodeFormat.CODE_128:
               return ZXing.BarcodeFormat.CODE_128;
            case BarcodeFormat.CODE_39:
               return ZXing.BarcodeFormat.CODE_39;
            case BarcodeFormat.CODE_93:
               return ZXing.BarcodeFormat.CODE_93;
            case BarcodeFormat.DATA_MATRIX:
               return ZXing.BarcodeFormat.DATA_MATRIX;
            case BarcodeFormat.EAN_13:
               return ZXing.BarcodeFormat.EAN_13;
            case BarcodeFormat.EAN_8:
               return ZXing.BarcodeFormat.EAN_8;
            case BarcodeFormat.IMB:
               return ZXing.BarcodeFormat.IMB;
            case BarcodeFormat.ITF:
               return ZXing.BarcodeFormat.ITF;
            case BarcodeFormat.MAXICODE:
               return ZXing.BarcodeFormat.MAXICODE;
            case BarcodeFormat.MSI:
               return ZXing.BarcodeFormat.MSI;
            case BarcodeFormat.PDF_417:
               return ZXing.BarcodeFormat.PDF_417;
            case BarcodeFormat.PLESSEY:
               return ZXing.BarcodeFormat.PLESSEY;
            case BarcodeFormat.QR_CODE:
               return ZXing.BarcodeFormat.QR_CODE;
            case BarcodeFormat.RSS_14:
               return ZXing.BarcodeFormat.RSS_14;
            case BarcodeFormat.RSS_EXPANDED:
               return ZXing.BarcodeFormat.RSS_EXPANDED;
            case BarcodeFormat.UPC_A:
               return ZXing.BarcodeFormat.UPC_A;
            case BarcodeFormat.UPC_E:
               return ZXing.BarcodeFormat.UPC_E;
            case BarcodeFormat.UPC_EAN_EXTENSION:
               return ZXing.BarcodeFormat.UPC_EAN_EXTENSION;
            default:
               return ZXing.BarcodeFormat.QR_CODE;
         }
      }

      public static IList<ZXing.BarcodeFormat> ToZXing(this IList<BarcodeFormat> formate)
      {
         if (formate == null)
            return null;

         var result = new List<ZXing.BarcodeFormat>();

         foreach (var format in formate)
         {
            result.Add(format.ToZXing());
         }

         return result;
      }

      public static BarcodeFormat ToInterop(this ZXing.BarcodeFormat format)
      {
         switch (format)
         {
            case ZXing.BarcodeFormat.AZTEC:
               return BarcodeFormat.AZTEC;
            case ZXing.BarcodeFormat.All_1D:
               return BarcodeFormat.All_1D;
            case ZXing.BarcodeFormat.CODABAR:
               return BarcodeFormat.CODABAR;
            case ZXing.BarcodeFormat.CODE_128:
               return BarcodeFormat.CODE_128;
            case ZXing.BarcodeFormat.CODE_39:
               return BarcodeFormat.CODE_39;
            case ZXing.BarcodeFormat.CODE_93:
               return BarcodeFormat.CODE_93;
            case ZXing.BarcodeFormat.DATA_MATRIX:
               return BarcodeFormat.DATA_MATRIX;
            case ZXing.BarcodeFormat.EAN_13:
               return BarcodeFormat.EAN_13;
            case ZXing.BarcodeFormat.EAN_8:
               return BarcodeFormat.EAN_8;
            case ZXing.BarcodeFormat.IMB:
               return BarcodeFormat.IMB;
            case ZXing.BarcodeFormat.ITF:
               return BarcodeFormat.ITF;
            case ZXing.BarcodeFormat.MAXICODE:
               return BarcodeFormat.MAXICODE;
            case ZXing.BarcodeFormat.MSI:
               return BarcodeFormat.MSI;
            case ZXing.BarcodeFormat.PDF_417:
               return BarcodeFormat.PDF_417;
            case ZXing.BarcodeFormat.PLESSEY:
               return BarcodeFormat.PLESSEY;
            case ZXing.BarcodeFormat.QR_CODE:
               return BarcodeFormat.QR_CODE;
            case ZXing.BarcodeFormat.RSS_14:
               return BarcodeFormat.RSS_14;
            case ZXing.BarcodeFormat.RSS_EXPANDED:
               return BarcodeFormat.RSS_EXPANDED;
            case ZXing.BarcodeFormat.UPC_A:
               return BarcodeFormat.UPC_A;
            case ZXing.BarcodeFormat.UPC_E:
               return BarcodeFormat.UPC_E;
            case ZXing.BarcodeFormat.UPC_EAN_EXTENSION:
               return BarcodeFormat.UPC_EAN_EXTENSION;
            default:
               return BarcodeFormat.QR_CODE;
         }
      }

      public static BarcodeFormat[] ToInterop(this IList<ZXing.BarcodeFormat> formate)
      {
         if (formate == null)
            return null;

         var result = new BarcodeFormat[formate.Count];
         var index = 0;
         foreach (var format in formate)
         {
            result[index] = format.ToInterop();
            index++;
         }

         return result;
      }
   }
}
