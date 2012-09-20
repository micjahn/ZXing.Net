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

#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
using System.Drawing;
#else
using UnityEngine;
#endif
#elif NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.Rendering;

namespace ZXing
{
#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<Bitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new BitmapRenderer();
      }
   }
#else
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<Color32[]>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new Color32Renderer();
      }
   }
#endif
#else
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<WriteableBitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeReader"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new WriteableBitmapRenderer();
      }
   }
#endif
}
