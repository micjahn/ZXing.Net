using System;
using System.Collections.Generic;
#if !SILVERLIGHT
#if !UNITY
using System.Drawing;
#endif
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.Common;

namespace ZXing
{
   /// <summary>
   /// A smart class to decode the barcode inside a bitmap object
   /// </summary>
   public class BarcodeReader : IBarcodeReader, IMultipleBarcodeReader
   {
#if !SILVERLIGHT
#if !UNITY
      private static readonly Func<Bitmap, LuminanceSource> defaultCreateLuminanceSource =
         (bitmap) => new RGBLuminanceSource(bitmap);
#else
      private static readonly Func<byte[], int, int, LuminanceSource> defaultCreateLuminanceSource =
         (rawRGB, width, height) => new RGBLuminanceSource(rawRGB, width, height);
#endif
#else
      private static readonly Func<WriteableBitmap, LuminanceSource> defaultCreateLuminanceSource =
         (bitmap) => new RGBLuminanceSource(bitmap);
#endif
      private static readonly Func<LuminanceSource, Binarizer> defaultCreateBinarizer =
         (luminanceSource) => new HybridBinarizer(luminanceSource);

      private Reader reader;
      private readonly IDictionary<DecodeHintType, object> hints;
#if !SILVERLIGHT
#if !UNITY
      private Func<Bitmap, LuminanceSource> createLuminanceSource;
#else
      private Func<byte[], int, int, LuminanceSource> createLuminanceSource;
#endif
#else
      private readonly Func<WriteableBitmap, LuminanceSource> createLuminanceSource;
#endif
      private readonly Func<LuminanceSource, Binarizer> createBinarizer;
      private bool usePreviousState;

      /// <summary>
      /// Gets the reader which should be used to find and decode the barcode.
      /// </summary>
      /// <value>
      /// The reader.
      /// </value>
      public Reader Reader
      {
         get
         {
            return reader ?? (reader = new MultiFormatReader());
         }
      }

      /// <summary>
      /// Gets or sets a method which is called if an important point is found
      /// </summary>
      /// <value>
      /// The result point callback.
      /// </value>
      public event Action<ResultPoint> ResultPointFound
      {
         add
         {
            if (!hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
            {
               ResultPointCallback callback = resultPoint =>
                                                 {
                                                    if (explicitResultPointFound != null)
                                                       explicitResultPointFound(resultPoint);
                                                 };
               hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK] = callback;
            }
            explicitResultPointFound += value;
            usePreviousState = false;
         }
         remove
         {
            explicitResultPointFound -= value;
            if (explicitResultPointFound == null)
               hints.Remove(DecodeHintType.NEED_RESULT_POINT_CALLBACK);
            usePreviousState = false;
         }
      }

      private event Action<ResultPoint> explicitResultPointFound;

      /// <summary>
      /// event is executed if a result was found via decode
      /// </summary>
      public event Action<Result> ResultFound;

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
      /// Image is a pure monochrome image of a barcode. Doesn't matter what it maps to;
      /// use {@link Boolean#TRUE}.
      /// </summary>
      /// <value>
      ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
      /// </value>
      public bool PureBarcode
      {
         get
         {
            if (hints.ContainsKey(DecodeHintType.PURE_BARCODE))
               return (bool)hints[DecodeHintType.PURE_BARCODE];
            return false;
         }
         set
         {
            if (value)
            {
               hints[DecodeHintType.PURE_BARCODE] = true;
               usePreviousState = false;
            }
            else
            {
               if (hints.ContainsKey(DecodeHintType.PURE_BARCODE))
               {
                  hints.Remove(DecodeHintType.PURE_BARCODE);
                  usePreviousState = false;
               }
            }
         }
      }

      /// <summary>
      /// Specifies what character encoding to use when decoding, where applicable (type String)
      /// </summary>
      /// <value>
      /// The character set.
      /// </value>
      public string CharacterSet
      {
         get
         {
            if (hints.ContainsKey(DecodeHintType.CHARACTER_SET))
               return (string)hints[DecodeHintType.CHARACTER_SET];
            return null;
         }
         set
         {
            if (value != null)
            {
               hints[DecodeHintType.CHARACTER_SET] = value;
               usePreviousState = false;
            }
            else
            {
               if (hints.ContainsKey(DecodeHintType.CHARACTER_SET))
               {
                  hints.Remove(DecodeHintType.CHARACTER_SET);
                  usePreviousState = false;
               }
            }
         }
      }

      /// <summary>
      /// Image is known to be of one of a few possible formats.
      /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
      /// </summary>
      /// <value>
      /// The possible formats.
      /// </value>
      public IList<BarcodeFormat> PossibleFormats
      {
         get
         {
            if (hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS))
               return (IList<BarcodeFormat>)hints[DecodeHintType.POSSIBLE_FORMATS];
            return null;
         }
         set
         {
            if (value != null)
            {
               hints[DecodeHintType.POSSIBLE_FORMATS] = value;
               usePreviousState = false;
            }
            else
            {
               if (hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS))
               {
                  hints.Remove(DecodeHintType.POSSIBLE_FORMATS);
                  usePreviousState = false;
               }
            }
         }
      }

#if !SILVERLIGHT
#if !UNITY
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
      public Func<byte[], int, int, LuminanceSource> CreateLuminanceSource
#endif
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
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeReader()
         : this(new MultiFormatReader(), defaultCreateLuminanceSource, defaultCreateBinarizer)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
      /// If null then MultiFormatReader is used</param>
      /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
      /// If null then RGBLuminanceSource is used</param>
      /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
      /// If null then HybridBinarizer is used</param>
      public BarcodeReader(Reader reader,
#if !SILVERLIGHT
#if !UNITY
         Func<Bitmap, LuminanceSource> createLuminanceSource,
#else
         Func<byte[], int, int, LuminanceSource> createLuminanceSource,
#endif
#else
         Func<WriteableBitmap, LuminanceSource> createLuminanceSource,
#endif
         Func<LuminanceSource, Binarizer> createBinarizer
         )
      {
         this.reader = reader ?? new MultiFormatReader();
         this.createLuminanceSource = createLuminanceSource ?? defaultCreateLuminanceSource;
         this.createBinarizer = createBinarizer ?? defaultCreateBinarizer;
         hints = new Dictionary<DecodeHintType, object>();
         usePreviousState = false;
      }

#if !SILVERLIGHT
#if !UNITY
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
      /// <param name="rawRGB">raw bytes of the image in RGB order</param>
      /// <param name="width"></param>
      /// <param name="height"></param>
      /// <returns>
      /// the result data or null
      /// </returns>
      public Result Decode(byte[] rawRGB, int width, int height)
#endif
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      public Result Decode(WriteableBitmap barcodeBitmap)
#endif
      {
#if !UNITY
         if (barcodeBitmap == null)
            throw new ArgumentNullException("barcodeBitmap");
#else
         if (rawRGB == null)
            throw new ArgumentNullException("rawRGB");
#endif

         var result = default(Result);
#if !UNITY
         var luminanceSource = CreateLuminanceSource(barcodeBitmap);
#else
         var luminanceSource = CreateLuminanceSource(rawRGB, width, height);
#endif
         var binarizer = CreateBinarizer(luminanceSource);
         var binaryBitmap = new BinaryBitmap(binarizer);
         var multiformatReader = Reader as MultiFormatReader;
         var rotationCount = 0;

         for (; rotationCount < 4; rotationCount++)
         {
            if (usePreviousState && multiformatReader != null)
            {
               result = multiformatReader.decodeWithState(binaryBitmap);
            }
            else
            {
               result = Reader.decode(binaryBitmap, hints);
               usePreviousState = true;
            }
            
            if (result != null ||
                !luminanceSource.RotateSupported)
               break;
            binaryBitmap = new BinaryBitmap(CreateBinarizer(luminanceSource.rotateCounterClockwise()));
         }

         if (result != null)
         {
            if (result.ResultMetadata == null)
            {
               result.putMetadata(ResultMetadataType.ORIENTATION, rotationCount*90);
            }
            else if (!result.ResultMetadata.ContainsKey(ResultMetadataType.ORIENTATION))
            {
               result.ResultMetadata[ResultMetadataType.ORIENTATION] = rotationCount*90;
            }
            else
            {
               // perhaps the core decoder rotates the image already (can happen if TryHarder is specified)
               result.ResultMetadata[ResultMetadataType.ORIENTATION] = ((int)(result.ResultMetadata[ResultMetadataType.ORIENTATION]) + rotationCount * 90) % 360;
            }

            if (ResultFound != null)
               ResultFound(result);
         }

         return result;
      }


#if !SILVERLIGHT
#if !UNITY
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      public Result[] DecodeMultiple(Bitmap barcodeBitmap)
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="rawRGB">raw bytes of the image in RGB order</param>
      /// <param name="width"></param>
      /// <param name="height"></param>
      /// <returns>
      /// the result data or null
      /// </returns>
      public Result[] DecodeMultiple(byte[] rawRGB, int width, int height)
#endif
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      public Result[] DecodeMultiple(WriteableBitmap barcodeBitmap)
#endif
      {
         throw new NotImplementedException();
      }
   }
}
