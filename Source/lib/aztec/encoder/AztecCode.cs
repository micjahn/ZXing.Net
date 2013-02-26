/*
 * Copyright 2013 ZXing authors
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

namespace ZXing.Aztec.Internal
{
   /// <summary>
   /// Aztec 2D code representation
   /// </summary>
   /// <author>Rustam Abdullaev</author>
   public sealed class AztecCode
   {
      /// <summary>
      /// Compact or full symbol indicator
      /// </summary>
      public bool isCompact { get; set; }

      /// <summary>
      /// Size in pixels (width and height)
      /// </summary>
      public int Size { get; set; }

      /// <summary>
      /// Number of levels
      /// </summary>
      public int Layers { get; set; }

      /// <summary>
      /// Number of data codewords
      /// </summary>
      public int CodeWords { get; set; }

      /// <summary>
      /// The symbol image
      /// </summary>
      public BitMatrix Matrix { get; set; }
   }
}