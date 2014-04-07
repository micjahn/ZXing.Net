/*
 * Copyright 2013 ZXing.Net authors
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
   /// <summary>
   /// <author>Sean Owen</author>
   /// </summary>
   public sealed class MSIBlackBox1TestCase : AbstractBlackBoxTestCase
   {
      public MSIBlackBox1TestCase()
         : base("test/data/blackbox/msi-1", new MSIReader(), BarcodeFormat.MSI)
      {
         addTest(5, 5, 0.0f);
         addTest(5, 5, 180.0f);
      }
   }
}
