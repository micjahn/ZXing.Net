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

using System.Runtime.InteropServices;

namespace ZXing.Interop.Encoding
{
   [ComVisible(true)]
   [Guid("8AFD3EC9-207D-4CDE-933F-28C188F734CC")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class QrCodeEncodingOptions : EncodingOptions
   {
      internal readonly QrCode.QrCodeEncodingOptions wrappedQrEncodingOptions;

      /// <summary>
      /// Initializes a new instance of the <see cref="QrCodeEncodingOptions"/> class.
      /// </summary>
      public QrCodeEncodingOptions()
         : this(new QrCode.QrCodeEncodingOptions())
      {
      }

      internal QrCodeEncodingOptions(QrCode.QrCodeEncodingOptions other)
         : base(other)
      {
         wrappedQrEncodingOptions = other;
      }

      public ErrorCorrectionLevel ErrorCorrection
      {
         get { return wrappedQrEncodingOptions.ErrorCorrection.ToInterop(); }
         set { wrappedQrEncodingOptions.ErrorCorrection = value.ToZXing(); }
      }

      /// <summary>
      /// Specifies what character encoding to use where applicable (type {@link String})
      /// </summary>
      public string CharacterSet
      {
         get { return wrappedQrEncodingOptions.CharacterSet; }
         set { wrappedQrEncodingOptions.CharacterSet = value; }
      }

      /// <summary>
      /// Explicitly disables ECI segment when generating QR Code
      /// That is against the specification of QR Code but some
      /// readers have problems if the charset is switched from
      /// ISO-8859-1 (default) to UTF-8 with the necessary ECI segment.
      /// If you set the property to true you can use UTF-8 encoding
      /// and the ECI segment is omitted.
      /// </summary>
      public bool DisableECI
      {
         get { return wrappedQrEncodingOptions.DisableECI; }
         set { wrappedQrEncodingOptions.DisableECI = value; }
      }

      /// <summary>
      /// Specifies the exact version of QR code to be encoded. An integer, range 1 to 40. If the data specified
      /// cannot fit within the required version, a WriterException will be thrown.
      /// </summary>
      public int QrVersion
      {
         get { return wrappedQrEncodingOptions.QrVersion.GetValueOrDefault(0); }
         set { wrappedQrEncodingOptions.QrVersion = (value == 0 ? (int?)null : value); }

      }
   }

   [ComVisible(true)]
   [Guid("F31A7AE6-A0F5-4D27-949E-98A2759C848B")]
   public enum ErrorCorrectionLevel
   {
      /// <summary> L = ~7% correction</summary>
      L,

      /// <summary> M = ~15% correction</summary>
      M,

      /// <summary> Q = ~25% correction</summary>
      Q,

      /// <summary> H = ~30% correction</summary>
      H,
   }

   internal static class ErrorCorrectionLevelExtensions
   {
      public static ErrorCorrectionLevel ToInterop(this QrCode.Internal.ErrorCorrectionLevel other)
      {
         if (other == null)
            return ErrorCorrectionLevel.L;
         if (other == QrCode.Internal.ErrorCorrectionLevel.H)
            return ErrorCorrectionLevel.H;
         if (other == QrCode.Internal.ErrorCorrectionLevel.L)
            return ErrorCorrectionLevel.L;
         if (other == QrCode.Internal.ErrorCorrectionLevel.M)
            return ErrorCorrectionLevel.M;
         if (other != QrCode.Internal.ErrorCorrectionLevel.Q)
            return ErrorCorrectionLevel.Q;
         return ErrorCorrectionLevel.L;
      }
      public static QrCode.Internal.ErrorCorrectionLevel ToZXing(this ErrorCorrectionLevel other)
      {
         switch (other)
         {
            case ErrorCorrectionLevel.H:
               return QrCode.Internal.ErrorCorrectionLevel.H;
            case ErrorCorrectionLevel.L:
               return QrCode.Internal.ErrorCorrectionLevel.L;
            case ErrorCorrectionLevel.M:
               return QrCode.Internal.ErrorCorrectionLevel.M;
            case ErrorCorrectionLevel.Q:
               return QrCode.Internal.ErrorCorrectionLevel.Q;
            default:
               return QrCode.Internal.ErrorCorrectionLevel.L;
         }
      }
   }
}
