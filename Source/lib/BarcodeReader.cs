using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.Common;

namespace ZXing
{
   /// <summary>
   /// A smart class to decode the barcode inside a bitmap object
   /// </summary>
   public class BarcodeReader : IBarcodeReader
   {
#if !SILVERLIGHT
      private static readonly Func<Bitmap, LuminanceSource> defaultCreateLuminanceSource =
         (bitmap) => new RGBLuminanceSource(bitmap, bitmap.Width, bitmap.Height);
#else
      private static readonly Func<WriteableBitmap, LuminanceSource> defaultCreateLuminanceSource =
         (bitmap) => new RGBLuminanceSource(bitmap, bitmap.PixelWidth, bitmap.PixelHeight);
#endif
      private static readonly Func<LuminanceSource, Binarizer> defaultCreateBinarizer =
         (luminanceSource) => new HybridBinarizer(luminanceSource);

      private Reader reader;
      private MultiFormatReader defaultReader;
      private readonly IDictionary<DecodeHintType, object> hints;
#if !SILVERLIGHT
      private Func<Bitmap, LuminanceSource> createLuminanceSource;
#else
      private Func<WriteableBitmap, LuminanceSource> createLuminanceSource;
#endif
      private Func<LuminanceSource, Binarizer> createBinarizer;
      private bool usePreviousState;

      /// <summary>
      /// Gets or sets the reader which should be used to find and decode the barcode.
      /// If null then MultiFormatReader is used
      /// </summary>
      /// <value>
      /// The reader.
      /// </value>
      public Reader Reader
      {
         get
         {
            return reader ?? (reader = defaultReader = new MultiFormatReader());
         }
         set
         {
            reader = value;
         }
      }

      /// <summary>
      /// Gets or sets a flag which cause a deeper look into the bitmap
      /// </summary>
      /// <value>
      ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
      /// </value>
      public bool TryHarder
      {
         get
         {
            if (hints.ContainsKey(DecodeHintType.TRY_HARDER))
               return (bool)hints[DecodeHintType.TRY_HARDER];
            return false;
         }
         set
         {
            if (value)
            {
               hints[DecodeHintType.TRY_HARDER] = true;
               usePreviousState = false;
            }
            else
            {
               if (hints.ContainsKey(DecodeHintType.TRY_HARDER))
               {
                  hints.Remove(DecodeHintType.TRY_HARDER);
                  usePreviousState = false;
               }
            }
         }
      }

      /// <summary>
      /// Gets or sets a method which is called if an important point is found
      /// </summary>
      /// <value>
      /// The result point callback.
      /// </value>
      public ResultPointCallback ResultPointCallback
      {
         get
         {
            if (hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
               return (ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
            return null;
         }
         set
         {
            if (value != null)
            {
               hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK] = value;
               usePreviousState = false;
            }
            else
            {
               if (hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
               {
                  hints.Remove(DecodeHintType.NEED_RESULT_POINT_CALLBACK);
                  usePreviousState = false;
               }
            }
         }
      }

#if !SILVERLIGHT
      /// <summary>
      /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
      /// If null then RGBLuminanceSource is used
      /// </summary>
      /// <value>
      /// The function to create a luminance source object.
      /// </value>
      public Func<Bitmap, LuminanceSource> CreateLuminanceSource
#else
      /// <summary>
      /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
      /// If null then RGBLuminanceSource is used
      /// </summary>
      /// <value>
      /// The function to create a luminance source object.
      /// </value>
      public Func<WriteableBitmap, LuminanceSource> CreateLuminanceSource
#endif
      {
         get
         {
            return createLuminanceSource ?? defaultCreateLuminanceSource;
         }
         set
         {
            createLuminanceSource = value;
         }
      }

      /// <summary>
      /// Optional: Gets or sets the function to create a binarizer object for a luminance source.
      /// If null then HybridBinarizer is used
      /// </summary>
      /// <value>
      /// The function to create a binarizer object.
      /// </value>
      public Func<LuminanceSource, Binarizer> CreateBinarizer
      {
         get
         {
            return createBinarizer ?? defaultCreateBinarizer;
         }
         set
         {
            createBinarizer = value;
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeReader()
      {
         hints = new Dictionary<DecodeHintType, object>();
         usePreviousState = false;
      }

#if !SILVERLIGHT
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      public Result Decode(Bitmap barcodeBitmap)
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      public Result Decode(WriteableBitmap barcodeBitmap)
#endif
      {
         if (barcodeBitmap == null)
            throw new ArgumentNullException("barcodeBitmap");

         Result result;
         var luminanceSource = CreateLuminanceSource(barcodeBitmap);
         var binarizer = CreateBinarizer(luminanceSource);
         var binaryBitmap = new BinaryBitmap(binarizer);

         if (usePreviousState && ReferenceEquals(Reader, defaultReader))
         {
            result = defaultReader.decodeWithState(binaryBitmap);
         }
         else
         {
            result = Reader.decode(binaryBitmap, hints);
            usePreviousState = true;
         }
         return result;
      }
   }
}
