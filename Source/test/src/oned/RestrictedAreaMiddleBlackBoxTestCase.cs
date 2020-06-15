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

using ZXing.Common.Test;

namespace ZXing.OneD.Test
{
    public sealed class RestrictedAreaMiddleBlackBoxTestCase : AbstractBlackBoxTestCase
    {
        public RestrictedAreaMiddleBlackBoxTestCase():base("test/data/blackbox/restricted-area/mid", new MultiFormatReader(), null)
        {
            addTest(0, 0, 3, 3, 0.0f, new RestrictedScanningArea(0.03f, 0.1f));
            addTest(0, 0, 3, 3, 180.0f, new RestrictedScanningArea(0.03f, 0.1f));
            addTest(3, 3, 0, 0, 0.0f, new RestrictedScanningArea(0.52f, 0.56f));
            addTest(0, 0, 3, 3, 0.0f, new RestrictedScanningArea(0.78f, 0.88f));
            addTest(0, 0, 3, 3, 180.0f, new RestrictedScanningArea(0.78f, 0.88f));

            addTest(3, 3, 0, 0, 180.0f, new RestrictedScanningArea(0.5f, 0.56f));
        }
    }
}