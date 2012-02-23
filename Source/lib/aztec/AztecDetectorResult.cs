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

namespace ZXing.Aztec
{
   public class AztecDetectorResult : DetectorResult
   {
      private bool compact;
      private int nbDatablocks;
      private int nbLayers;

      public AztecDetectorResult(BitMatrix bits,
                                 ResultPoint[] points,
                                 bool compact,
                                 int nbDatablocks,
                                 int nbLayers)
         : base(bits, points)
      {
         this.compact = compact;
         this.nbDatablocks = nbDatablocks;
         this.nbLayers = nbLayers;
      }

      public int getNbLayers()
      {
         return nbLayers;
      }

      public int getNbDatablocks()
      {
         return nbDatablocks;
      }

      public bool isCompact()
      {
         return compact;
      }
   }
}