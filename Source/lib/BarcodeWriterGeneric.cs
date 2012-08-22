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

using System;
#if !(SILVERLIGHT || NETFX_CORE)
#if !UNITY
using System.Drawing;
#endif
#elif NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.Common;
using ZXing.Rendering;

namespace ZXing
{
   public class BarcodeWriterGeneric<TOutput> : IBarcodeWriterGeneric<TOutput>
   {
      private EncodingOptions options;

      public BarcodeFormat Format { get; set; }

      public EncodingOptions Options
      {
         get
         {
            return (options ?? (options = new EncodingOptions { Height = 100, Width = 100 }));
         }
         set
         {
            options = value;
         }
      }

      public IBarcodeRenderer<TOutput> Renderer { get; set; }

      public BitMatrix Encode(string contents)
      {
         var encoder = new MultiFormatWriter();
         var options = Options ?? new EncodingOptions {Height = 100, Width = 100};
         return encoder.encode(contents, Format, options.Width, options.Height, options.Hints);
      }

      public TOutput Write(string contents)
      {
         if (Renderer == null)
         {
            throw new InvalidOperationException("You have to set a renderer instance.");
         }

         var matrix = Encode(contents);

         return Renderer.Render(matrix, Format, contents);
      }
   }
}
