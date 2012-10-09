/*
 * Copyright 2010 ZXing authors
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
   /// The class contains all information about the Aztec code which was found
   /// </summary>
   public class AztecDetectorResult : DetectorResult
   {
      /// <summary>
      /// Gets a value indicating whether this Aztec code is compact.
      /// </summary>
      /// <value>
      ///   <c>true</c> if compact; otherwise, <c>false</c>.
      /// </value>
      public bool Compact { get; private set; }
      /// <summary>
      /// Gets the nb datablocks.
      /// </summary>
      public int NbDatablocks { get; private set; }
      /// <summary>
      /// Gets the nb layers.
      /// </summary>
      public int NbLayers { get; private set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="AztecDetectorResult"/> class.
      /// </summary>
      /// <param name="bits">The bits.</param>
      /// <param name="points">The points.</param>
      /// <param name="compact">if set to <c>true</c> [compact].</param>
      /// <param name="nbDatablocks">The nb datablocks.</param>
      /// <param name="nbLayers">The nb layers.</param>
      public AztecDetectorResult(BitMatrix bits,
                                 ResultPoint[] points,
                                 bool compact,
                                 int nbDatablocks,
                                 int nbLayers)
         : base(bits, points)
      {
         Compact = compact;
         NbDatablocks = nbDatablocks;
         NbLayers = nbLayers;
      }
   }
}