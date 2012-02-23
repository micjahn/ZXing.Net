/*
 * Copyright 2008 ZXing authors
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

namespace ZXing.Common.Test
{
   internal sealed class TestResult
   {
      private int mustPassCount;
      private int tryHarderCount;
      private int maxMisreads;
      private int maxTryHarderMisreads;
      private float rotation;

      public TestResult(int mustPassCount, int tryHarderCount, int maxMisreads, int maxTryHarderMisreads, float rotation)
      {
         this.mustPassCount = mustPassCount;
         this.tryHarderCount = tryHarderCount;
         this.maxMisreads = maxMisreads;
         this.maxTryHarderMisreads = maxTryHarderMisreads;
         this.rotation = rotation;
      }

      public int MustPassCount
      {
         get { return mustPassCount; }
      }

      public int TryHarderCount
      {
         get { return tryHarderCount; }
      }

      public int MaxMisreads
      {
         get { return maxMisreads; }
      }

      public int MaxTryHarderMisreads
      {
         get { return maxTryHarderMisreads; }
      }

      public float Rotation
      {
         get { return rotation; }
      }
   }
}