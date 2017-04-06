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
   [Guid("87B6F992-5EFB-4444-9ADE-96E86AAD3613")]
   [ClassInterface(ClassInterfaceType.AutoDual)]
   public class AztecEncodingOptions : EncodingOptions
   {
      internal readonly Aztec.AztecEncodingOptions wrappedAztecEncodingOptions;

      /// <summary>
      /// Initializes a new instance of the <see cref="AztecEncodingOptions"/> class.
      /// </summary>
      public AztecEncodingOptions()
         : this(new Aztec.AztecEncodingOptions())
      {
      }

      internal AztecEncodingOptions(Aztec.AztecEncodingOptions other)
         : base(other)
      {
         wrappedAztecEncodingOptions = other;
      }
      
      /// <summary>
      /// Representing the minimal percentage of error correction words. 
      /// Note: an Aztec symbol should have a minimum of 25% EC words.
      /// </summary>
      public int ErrorCorrection
      {
         get { return wrappedAztecEncodingOptions.ErrorCorrection.GetValueOrDefault(0); }
         set { wrappedAztecEncodingOptions.ErrorCorrection = value == 0 ? (int?) null : value; }
      }

      /// <summary>
      /// Specifies the required number of layers for an Aztec code:
      /// a negative number (-1, -2, -3, -4) specifies a compact Aztec code
      /// 0 indicates to use the minimum number of layers (the default)
      /// a positive number (1, 2, .. 32) specifies a normal (non-compact) Aztec code
      /// </summary>
      public int Layers
      {
         get { return wrappedAztecEncodingOptions.Layers.GetValueOrDefault(0); }
         set { wrappedAztecEncodingOptions.Layers = value == 0 ? (int?)null : value; }
      }
   }
}
