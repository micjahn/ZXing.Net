/*
 * Copyright 2020 ZXing authors
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

using ZXing.SkiaSharp.Test.Common;

namespace ZXing.SkiaSharp.Test.oned
{
    public sealed class Code39BlackBox1TestCase : SkiaBarcodeBlackBoxTestCase
    {
        public Code39BlackBox1TestCase()
           : base("../../../../../test/data/blackbox/code39-1", BarcodeFormat.CODE_39)
        {
            addTest(4, 4, 0.0f);
            addTest(4, 4, 180.0f);
        }
    }
}
