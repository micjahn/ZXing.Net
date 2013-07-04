/*
 * Copyright 2013
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

namespace ZXing.Datamatrix.Encoder
{
   /// <summary>
   /// Enumeration for encodation types
   /// </summary>
   public sealed class Encodation
   {
      public const int ASCII = 0;
      public const int C40 = 1;
      public const int TEXT = 2;
      public const int X12 = 3;
      public const int EDIFACT = 4;
      public const int BASE256 = 5;
   }
}
