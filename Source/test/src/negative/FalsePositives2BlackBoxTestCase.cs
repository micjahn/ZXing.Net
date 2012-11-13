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

namespace ZXing.Negative.Test
{
   /// <summary>
   /// Additional random images with high contrast patterns which should not find any barcodes.
   ///
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public sealed class FalsePositives2BlackBoxTestCase : AbstractNegativeBlackBoxTestCase
   {
      public FalsePositives2BlackBoxTestCase()
         : base("test/data/blackbox/falsepositives-2")
      {
         addTest(4, 0.0f);
         addTest(4, 90.0f);
         addTest(4, 180.0f);
         addTest(4, 270.0f);
      }
   }
}
