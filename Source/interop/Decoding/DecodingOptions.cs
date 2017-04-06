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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ZXing.Interop.Common;

namespace ZXing.Interop.Decoding
{
   /// <summary>
   /// Defines an container for encoder options
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [Guid("24BE4318-BF09-4542-945D-3A9BF1DF5682")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class DecodingOptions
   {
      internal readonly ZXing.Common.DecodingOptions wrappedDecodingOptions;
      internal readonly BarcodeFormatCollection formatCollection;

      /// <summary>
      /// Gets or sets a flag which cause a deeper look into the bitmap
      /// </summary>
      /// <value>
      ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
      /// </value>
      public bool TryHarder
      {
         get { return wrappedDecodingOptions.TryHarder; }
         set { wrappedDecodingOptions.TryHarder = value; }
      }

      /// <summary>
      /// Image is a pure monochrome image of a barcode.
      /// </summary>
      /// <value>
      ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
      /// </value>
      public bool PureBarcode
      {
         get { return wrappedDecodingOptions.PureBarcode; }
         set { wrappedDecodingOptions.PureBarcode = value; }
      }

      /// <summary>
      /// Specifies what character encoding to use when decoding, where applicable (type String)
      /// </summary>
      /// <value>
      /// The character set.
      /// </value>
      public string CharacterSet
      {
         get { return wrappedDecodingOptions.CharacterSet; }
         set { wrappedDecodingOptions.CharacterSet = value; }
      }

      /// <summary>
      /// Image is known to be of one of a few possible formats.
      /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
      /// </summary>
      /// <value>
      /// The possible formats.
      /// </value>
      public IBarcodeFormatCollection PossibleFormats
      {
         get { return formatCollection; }
      }

      /// <summary>
      /// if Code39 could be detected try to use extended mode for full ASCII character set
      /// </summary>
      public bool UseCode39ExtendedMode
      {
         get { return wrappedDecodingOptions.UseCode39ExtendedMode; }
         set { wrappedDecodingOptions.UseCode39ExtendedMode = value; }
      }

      /// <summary>
      /// Don't fail if a Code39 is detected but can't be decoded in extended mode.
      /// Return the raw Code39 result instead. Maps to <see cref="bool" />.
      /// </summary>
      public bool UseCode39RelaxedExtendedMode
      {
         get { return wrappedDecodingOptions.UseCode39RelaxedExtendedMode; }
         set { wrappedDecodingOptions.UseCode39RelaxedExtendedMode = value; }
      }

      /// <summary>
      /// Assume Code 39 codes employ a check digit. Maps to <see cref="bool" />.
      /// </summary>
      /// <value>
      ///   <c>true</c> if it should assume a Code 39 check digit; otherwise, <c>false</c>.
      /// </value>
      public bool AssumeCode39CheckDigit
      {
         get { return wrappedDecodingOptions.AssumeCode39CheckDigit; }
         set { wrappedDecodingOptions.AssumeCode39CheckDigit = value; }
      }

      /// <summary>
      /// If true, return the start and end digits in a Codabar barcode instead of stripping them. They
      /// are alpha, whereas the rest are numeric. By default, they are stripped, but this causes them
      /// to not be. Doesn't matter what it maps to; use <see cref="bool" />.
      /// </summary>
      public bool ReturnCodabarStartEnd
      {
         get { return wrappedDecodingOptions.ReturnCodabarStartEnd; }
         set { wrappedDecodingOptions.ReturnCodabarStartEnd = value; }
      }

      /// <summary>
      /// Assume the barcode is being processed as a GS1 barcode, and modify behavior as needed.
      /// For example this affects FNC1 handling for Code 128 (aka GS1-128).
      /// </summary>
      /// <value>
      ///   <c>true</c> if it should assume GS1; otherwise, <c>false</c>.
      /// </value>
      public bool AssumeGS1
      {
         get { return wrappedDecodingOptions.AssumeGS1; }
         set { wrappedDecodingOptions.AssumeGS1 = value; }
      }

      /// <summary>
      /// Assume MSI codes employ a check digit. Maps to <see cref="bool" />.
      /// </summary>
      /// <value>
      ///   <c>true</c> if it should assume a MSI check digit; otherwise, <c>false</c>.
      /// </value>
      public bool AssumeMSICheckDigit
      {
         get { return wrappedDecodingOptions.AssumeMSICheckDigit; }
         set { wrappedDecodingOptions.AssumeMSICheckDigit = value; }

      }

      /// <summary>
      /// Allowed lengths of encoded data -- reject anything else. Maps to an int[].
      /// </summary>
      public int[] AllowedLengths
      {
         get { return wrappedDecodingOptions.AllowedLengths; }
         set { wrappedDecodingOptions.AllowedLengths = value; }
      }

      /// <summary>
      /// Allowed extension lengths for EAN or UPC barcodes. Other formats will ignore this.
      /// Maps to an int[] of the allowed extension lengths, for example [2], [5], or [2, 5].
      /// If it is optional to have an extension, do not set this hint. If this is set,
      /// and a UPC or EAN barcode is found but an extension is not, then no result will be returned
      /// at all.
      /// </summary>
      public int[] AllowedEANExtensions
      {
         get { return wrappedDecodingOptions.AllowedEANExtensions; }
         set { wrappedDecodingOptions.AllowedEANExtensions = value; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DecodingOptions"/> class.
      /// </summary>
      public DecodingOptions()
      {
         wrappedDecodingOptions = new ZXing.Common.DecodingOptions();
         formatCollection = new BarcodeFormatCollection(wrappedDecodingOptions);
      }

      internal DecodingOptions(ZXing.Common.DecodingOptions other)
      {
         wrappedDecodingOptions = other;
         formatCollection = new BarcodeFormatCollection(wrappedDecodingOptions);
      }
   }

   [ComVisible(true)]
   [Guid("5BEBE0B0-5B8A-4897-B2FE-6EA3795BA2B5")]
   [InterfaceType(ComInterfaceType.InterfaceIsDual)]
   public interface IBarcodeFormatCollection
   {
      void Add(Common.BarcodeFormat format);
      void Remove(Common.BarcodeFormat format);
      void Clear();
   }

   internal class BarcodeFormatCollection : IBarcodeFormatCollection
   {
      private readonly ZXing.Common.DecodingOptions options;

      public BarcodeFormatCollection(ZXing.Common.DecodingOptions options)
      {
         this.options = options;
      }

      public void Add(Common.BarcodeFormat format)
      {
         if (options.PossibleFormats == null)
            options.PossibleFormats = new List<ZXing.BarcodeFormat>();
         options.PossibleFormats.Add(format.ToZXing());
      }

      public void Remove(Common.BarcodeFormat format)
      {
         if (options.PossibleFormats != null)
         {
            options.PossibleFormats.Remove(format.ToZXing());
            if (options.PossibleFormats.Count == 0)
               options.PossibleFormats = null;
         }
      }

      public void Clear()
      {
         options.PossibleFormats = null;
      }
   }
}
