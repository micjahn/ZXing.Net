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

using System.Collections.Generic;
using System.Drawing;

using ZXing;

namespace CommandLineDecoder
{
   internal sealed class Config
   {
      public IDictionary<DecodeHintType, object> Hints { get; set; }
      public bool TryHarder { get; set; }
      public bool PureBarcode { get; set; }
      public bool ProductsOnly { get; set; }
      public bool DumpResults { get; set; }
      public bool DumpBlackPoint { get; set; }
      public bool Multi { get; set; }
      public bool Brief { get; set; }
      public bool Recursive { get; set; }
      public int[] Crop { get; set; }
      public int Threads { get; set; }
      public bool AutoRotate { get; set; }
      public Bitmap BitmapFromClipboard { get; set; }

      public Config()
      {
         Threads = 1;
      }
   }
}