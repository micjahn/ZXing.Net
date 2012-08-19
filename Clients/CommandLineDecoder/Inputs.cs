/*
 * Copyright 2012 ZXing.Net authors
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

using System;
using System.Collections.Generic;

namespace CommandLineDecoder
{
   /// <summary>
   /// Represents the collection of all images files/URLs to decode.
   /// </summary>
   internal sealed class Inputs
   {
      private readonly List<String> inputs = new List<String>(10);
      private int position;

      public void addInput(String pathOrUrl)
      {
         lock (inputs)
         {
            inputs.Add(pathOrUrl);
         }
      }

      public String getNextInput()
      {
         lock (inputs)
         {
            if (position < inputs.Count)
            {
               String result = inputs[position];
               position++;
               return result;
            }
            else
            {
               return null;
            }
         }
      }

      public int getInputCount()
      {
         return inputs.Count;
      }
   }
}
