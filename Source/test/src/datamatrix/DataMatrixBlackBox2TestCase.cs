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

namespace ZXing.Datamatrix.Test
{
   /// <summary>
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   /// </summary>
   public sealed class DataMatrixBlackBox2TestCase : AbstractBlackBoxTestCase
   {
      public DataMatrixBlackBox2TestCase()
         : base("test/data/blackbox/datamatrix-2", new MultiFormatReader(), BarcodeFormat.DATA_MATRIX)
      {
         addTest(8, 8, 0, 1, 0.0f);
         addTest(14, 14, 0, 1, 90.0f);
         addTest(14, 14, 0, 1, 180.0f);
         addTest(13, 13, 0, 1, 270.0f);
      }
   }
}
