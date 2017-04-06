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
   [Guid("9EDD06B8-92B7-48CF-A390-B96A27433127")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class PDF417EncodingOptions : EncodingOptions
   {
      internal readonly PDF417.PDF417EncodingOptions wrappedPDF417EncodingOptions;

      /// <summary>
      /// Initializes a new instance of the <see cref="PDF417EncodingOptions"/> class.
      /// </summary>
      public PDF417EncodingOptions()
         : this(new PDF417.PDF417EncodingOptions())
      {
      }

      internal PDF417EncodingOptions(PDF417.PDF417EncodingOptions other)
         : base(other)
      {
         wrappedPDF417EncodingOptions = other;
      }
      
      /// <summary>
      /// Specifies whether to use compact mode for PDF417 (type <see cref="bool" />).
      /// </summary>
      public bool Compact
      {
         get { return wrappedPDF417EncodingOptions.Compact; }
         set { wrappedPDF417EncodingOptions.Compact = value; }
      }

      /// <summary>
      /// Specifies what compaction mode to use for PDF417 (type
      /// <see cref="Compaction" />).
      /// </summary>
      public Compaction Compaction
      {
         get { return wrappedPDF417EncodingOptions.Compaction.ToInterop(); }
         set { wrappedPDF417EncodingOptions.Compaction = value.ToZXing(); }
      }

      /// <summary>
      /// Specifies the minimum and maximum number of rows and columns for PDF417 (type
      /// <see cref="Dimensions" />).
      /// </summary>
      public Dimensions Dimensions
      {
         get { return wrappedPDF417EncodingOptions.Dimensions.ToInterop(); }
         set { wrappedPDF417EncodingOptions.Dimensions = value.ToZXing(); }
      }

      /// <summary>
      /// Specifies what degree of error correction to use
      /// </summary>
      public PDF417ErrorCorrectionLevel ErrorCorrection
      {
         get { return wrappedPDF417EncodingOptions.ErrorCorrection.ToInterop(); }
         set { wrappedPDF417EncodingOptions.ErrorCorrection = value.ToZXing(); }
      }

      /// <summary>
      /// Specifies what character encoding to use where applicable (type {@link String})
      /// </summary>
      public string CharacterSet
      {
         get { return wrappedPDF417EncodingOptions.CharacterSet; }
         set { wrappedPDF417EncodingOptions.CharacterSet = value; }
      }

      /// <summary>
      /// Explicitly disables ECI segment when generating PDF417 Code
      /// That is against the specification but some
      /// readers have problems if the charset is switched from
      /// CP437 (default) to UTF-8 with the necessary ECI segment.
      /// If you set the property to true you can use different encodings
      /// and the ECI segment is omitted.
      /// </summary>
      public bool DisableECI
      {
         get { return wrappedPDF417EncodingOptions.DisableECI; }
         set { wrappedPDF417EncodingOptions.DisableECI = value; }
      }
   }

   [ComVisible(true)]
   [Guid("C6555153-F9E9-4FDC-AC74-AFD397727881")]
   public enum Compaction
   {
      /// <summary>
      /// 
      /// </summary>
      AUTO,

      /// <summary>
      /// 
      /// </summary>
      TEXT,

      /// <summary>
      /// 
      /// </summary>
      BYTE,

      /// <summary>
      /// 
      /// </summary>
      NUMERIC
   }

   internal static class CompactionExtensions
   {
      public static Compaction ToInterop(this PDF417.Internal.Compaction other)
      {
         switch (other)
         {
            case PDF417.Internal.Compaction.BYTE:
               return Compaction.BYTE;
            case PDF417.Internal.Compaction.NUMERIC:
               return Compaction.NUMERIC;
            case PDF417.Internal.Compaction.TEXT:
               return Compaction.TEXT;
            case PDF417.Internal.Compaction.AUTO:
            default:
               return Compaction.AUTO;
         }
      }
      public static PDF417.Internal.Compaction ToZXing(this Compaction other)
      {
         switch (other)
         {
            case Compaction.BYTE:
               return PDF417.Internal.Compaction.BYTE;
            case Compaction.NUMERIC:
               return PDF417.Internal.Compaction.NUMERIC;
            case Compaction.TEXT:
               return PDF417.Internal.Compaction.TEXT;
            case Compaction.AUTO:
            default:
               return PDF417.Internal.Compaction.AUTO;
         }
      }
   }

   [ComVisible(true)]
   [Guid("00198D03-9216-4A80-B318-4F4E65F06805")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public sealed class Dimensions
   {
      /// <summary>
      /// Gets the min cols.
      /// </summary>
      public int MinCols { get; private set; }

      /// <summary>
      /// Gets the max cols.
      /// </summary>
      public int MaxCols { get; private set; }

      /// <summary>
      /// Gets the min rows.
      /// </summary>
      public int MinRows { get; private set; }

      /// <summary>
      /// Gets the max rows.
      /// </summary>
      public int MaxRows { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="Dimensions"/> class.
      /// </summary>
      /// <param name="minCols">The min cols.</param>
      /// <param name="maxCols">The max cols.</param>
      /// <param name="minRows">The min rows.</param>
      /// <param name="maxRows">The max rows.</param>
      public Dimensions(int minCols, int maxCols, int minRows, int maxRows)
      {
         MinCols = minCols;
         MaxCols = maxCols;
         MinRows = minRows;
         MaxRows = maxRows;
      }
   }

   internal static class DimensionsExtensions
   {
      public static Dimensions ToInterop(this PDF417.Internal.Dimensions other)
      {
         if (other == null)
            return null;
         return new Dimensions(other.MinCols, other.MaxCols, other.MinRows, other.MaxRows);
      }
      public static PDF417.Internal.Dimensions ToZXing(this Dimensions other)
      {
         if (other == null)
            return null;
         return new PDF417.Internal.Dimensions(other.MinCols, other.MaxCols, other.MinRows, other.MaxRows);
      }
   }

   [ComVisible(true)]
   [Guid("F8730D38-7CAE-47E4-BEF7-7B76C6C4DE7D")]
   public enum PDF417ErrorCorrectionLevel
   {
      L0 = 0,
      L1,
      L2,
      L3,
      L4,
      L5,
      L6,
      L7,
      L8
   }

   internal static class PDF417ErrorCorrectionLevelExtensions
   {
      public static PDF417ErrorCorrectionLevel ToInterop(this PDF417.Internal.PDF417ErrorCorrectionLevel other)
      {
         switch (other)
         {
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L1:
               return PDF417ErrorCorrectionLevel.L1;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L2:
               return PDF417ErrorCorrectionLevel.L2;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L3:
               return PDF417ErrorCorrectionLevel.L3;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L4:
               return PDF417ErrorCorrectionLevel.L4;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L5:
               return PDF417ErrorCorrectionLevel.L5;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L6:
               return PDF417ErrorCorrectionLevel.L6;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L7:
               return PDF417ErrorCorrectionLevel.L7;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L8:
               return PDF417ErrorCorrectionLevel.L8;
            case PDF417.Internal.PDF417ErrorCorrectionLevel.L0:
            default:
               return PDF417ErrorCorrectionLevel.L0;
         }
      }
      public static PDF417.Internal.PDF417ErrorCorrectionLevel ToZXing(this PDF417ErrorCorrectionLevel other)
      {
         switch (other)
         {
            case PDF417ErrorCorrectionLevel.L1:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L1;
            case PDF417ErrorCorrectionLevel.L2:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L2;
            case PDF417ErrorCorrectionLevel.L3:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L3;
            case PDF417ErrorCorrectionLevel.L4:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L4;
            case PDF417ErrorCorrectionLevel.L5:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L5;
            case PDF417ErrorCorrectionLevel.L6:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L6;
            case PDF417ErrorCorrectionLevel.L7:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L7;
            case PDF417ErrorCorrectionLevel.L8:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L8;
            case PDF417ErrorCorrectionLevel.L0:
            default:
               return PDF417.Internal.PDF417ErrorCorrectionLevel.L0;
         }
      }
   }
}

