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

using UnityEngine;

using ZXing;

namespace UnityDemo
{
   public class Color32LuminanceSource : BaseLuminanceSource
   {
      public Color32LuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      public Color32LuminanceSource(Color32[] color32s, int width, int height)
         : base(width, height)
      {
         SetPixels(color32s);
      }

      public void SetPixels(Color32[] color32s)
      {
         var z = 0;

         for (var y = Height - 1; y >= 0; y--)
         {
            // This is flipped vertically because the Color32 array from Unity is reversed vertically,
            // it means that the top most row of the image would be the bottom most in the array.
            for (var x = 0; x < Width; x++)
            {
               var color32 = color32s[y*Width + x];
               // Calculate luminance cheaply, favoring green.
               luminances[z++] = (byte)((
                  color32.r +
                  color32.g + color32.g +
                  color32.b) >> 2);
            }
         }
      }
   }
}
