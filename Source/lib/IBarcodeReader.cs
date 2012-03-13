using System;
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to decode the barcode inside a bitmap object
   /// </summary>
   public interface IBarcodeReader
   {
      /// <summary>
      /// Gets or sets a flag which cause a deeper look into the bitmap
      /// </summary>
      /// <value>
      ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
      /// </value>
      bool TryHarder { get; set; }

      /// <summary>
      /// Gets or sets a method which is called if an important point is found
      /// </summary>
      /// <value>
      /// The result point callback.
      /// </value>
      ResultPointCallback ResultPointCallback { get; set; }

#if !SILVERLIGHT
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result Decode(Bitmap barcodeBitmap);
#else
      /// <summary>
      /// Decodes the specified barcode bitmap.
      /// </summary>
      /// <param name="barcodeBitmap">The barcode bitmap.</param>
      /// <returns>the result data or null</returns>
      Result Decode(WriteableBitmap barcodeBitmap);
#endif
   }
}
