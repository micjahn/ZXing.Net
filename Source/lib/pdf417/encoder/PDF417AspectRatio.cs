/*
 * Copyright ZXing Authors in part
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// defines the aspect ratio of the image
    /// </summary>
    public enum PDF417AspectRatio
    {
        /// <summary>
        /// ratio 1
        /// </summary>
        A1 = 1,
        /// <summary>
        /// ratio 2
        /// </summary>
        A2,
        /// <summary>
        /// ratio 3
        /// </summary>
        A3,
        /// <summary>
        /// ratio 4
        /// </summary>
        A4,
        /// <summary>
        /// automatic selection
        /// </summary>
        AUTO
    }
}
