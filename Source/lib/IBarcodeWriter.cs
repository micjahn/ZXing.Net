/*
 * Copyright 2012 ZXing.Net authors
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

using ZXing.Common;

#if MONOTOUCH
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif
#endif

namespace ZXing
{
   /// <summary>
   /// Interface for a smart class to encode some content into a barcode
   /// </summary>
#if UNITY
   [System.CLSCompliant(false)]
#endif
   public interface IBarcodeWriter
   {
      /// <summary>
      /// Encodes the specified contents.
      /// </summary>
      /// <param name="contents">The contents.</param>
      /// <returns></returns>
      BitMatrix Encode(string contents);

#if MONOTOUCH
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      UIImage Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      UIImage Write(BitMatrix matrix);
#endif

#if MONOANDROID
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      Android.Graphics.Bitmap Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      Android.Graphics.Bitmap Write(BitMatrix matrix);
#endif

#if UNITY
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      [System.CLSCompliant(false)]
      UnityEngine.Color32[] Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      [System.CLSCompliant(false)]
      UnityEngine.Color32[] Write(BitMatrix matrix);
#endif

#if SILVERLIGHT
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      System.Windows.Media.Imaging.WriteableBitmap Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      System.Windows.Media.Imaging.WriteableBitmap Write(BitMatrix matrix);
#endif

#if NETFX_CORE
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      Windows.UI.Xaml.Media.Imaging.WriteableBitmap Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      Windows.UI.Xaml.Media.Imaging.WriteableBitmap Write(BitMatrix matrix);
#endif

#if (NET46 || NET45 || NET40 || NET35 || NET20) && !UNITY
      /// <summary>
      /// Creates a visual representation of the contents
      /// </summary>
      System.Drawing.Bitmap Write(string contents);
      /// <summary>
      /// Returns a rendered instance of the barcode which is given by a BitMatrix.
      /// </summary>
      System.Drawing.Bitmap Write(BitMatrix matrix);
#endif
   }
}
