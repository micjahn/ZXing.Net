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
   [Guid("BE201525-ABCD-4153-AFA7-55DAB7EC4621")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class EncodingOptions
   {
      internal readonly ZXing.Common.EncodingOptions wrappedEncodingOptions;

      /// <summary>
      /// Initializes a new instance of the <see cref="EncodingOptions"/> class.
      /// </summary>
      public EncodingOptions()
         : this(new ZXing.Common.EncodingOptions())
      {
      }

      internal EncodingOptions(ZXing.Common.EncodingOptions other)
      {
         wrappedEncodingOptions = other;
      }

      public int Height
      {
         get { return wrappedEncodingOptions.Height; }
         set { wrappedEncodingOptions.Height = value; }
      }

      /// <summary>
      /// Specifies the width of the barcode image
      /// </summary>
      public int Width
      {
         get { return wrappedEncodingOptions.Width; }
         set { wrappedEncodingOptions.Width = value; }
      }

      /// <summary>
      /// Don't put the content string into the output image.
      /// </summary>
      public bool PureBarcode
      {
         get { return wrappedEncodingOptions.PureBarcode; }
         set { wrappedEncodingOptions.PureBarcode = value; }
      }

      /// <summary>
      /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary
      /// by format; for example it controls margin before and after the barcode horizontally for
      /// most 1D formats.
      /// </summary>
      public int Margin
      {
         get { return wrappedEncodingOptions.Margin; }
         set { wrappedEncodingOptions.Margin = value; }
      }
   }
}
