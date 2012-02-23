/*
 * Copyright 2009 ZXing authors
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

namespace ZXing.OneD.RSS
{
   public sealed class FinderPattern
   {
      public int Value { get; private set; }
      public int[] StartEnd { get; private set; }
      public ResultPoint[] ResultPoints { get; private set; }

      public FinderPattern(int value, int[] startEnd, int start, int end, int rowNumber)
      {
         Value = value;
         StartEnd = startEnd;
         ResultPoints = new ResultPoint[]
                                {
                                   new ResultPoint(start, rowNumber),
                                   new ResultPoint(end, rowNumber),
                                };
      }
   }
}