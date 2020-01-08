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
    /// <summary>
    /// Encapsulates the result of one test over a batch of black-box images.
    /// </summary>
    internal sealed class TestResult
    {
        public TestResult(int mustPassCount, int tryHarderCount, int maxMisreads, int maxTryHarderMisreads, float rotation)
        {
            MustPassCount = mustPassCount;
            TryHarderCount = tryHarderCount;
            MaxMisreads = maxMisreads;
            MaxTryHarderMisreads = maxTryHarderMisreads;
            Rotation = rotation;
        }

        public int MustPassCount { get; private set; }

        public int TryHarderCount { get; private set; }

        public int MaxMisreads { get; private set; }

        public int MaxTryHarderMisreads { get; private set; }

        public float Rotation { get; private set; }
    }
}