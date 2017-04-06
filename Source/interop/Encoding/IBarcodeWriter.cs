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
   [Guid("917B963C-D041-4551-9666-6FE43BD19E20")]
   [InterfaceType(ComInterfaceType.InterfaceIsDual)]
   public interface IBarcodeWriter
   {
      Common.BarcodeFormat Format { get; set; }

      /// <summary>
      /// Gets or sets the options container for the encoding and renderer process.
      /// </summary>
      EncodingOptions Options { get; set; }

      PixelData Write(string contents);

      void WritePngToFile(string contents, string fileName);
   }
}
