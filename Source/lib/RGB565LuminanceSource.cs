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

namespace ZXing
{
   /// <summary>
   /// 
   /// </summary>
   public class RGB565LuminanceSource :BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RGB565LuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected RGB565LuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="RGB565LuminanceSource"/> class.
      /// </summary>
      /// <param name="rgb565RawData">The RGB565 raw data.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      public RGB565LuminanceSource(byte[] rgb565RawData, int width, int height)
         : base(width, height)
      {
         CalculateLuminance(rgb565RawData);
      }

      private void CalculateLuminance(byte[] rgb565RawData)
      {
         var luminanceIndex = 0;
         for (var index = 0; index < rgb565RawData.Length; index += 2, luminanceIndex++)
         {
            var byte1 = rgb565RawData[index];
            var byte2 = rgb565RawData[index + 1];
            // cheap, not fully accurate conversion
            var r = (byte1 & 0x1f);
            var g = ((byte2 & 0x07) | ((byte1 & 0xe0) >> 5));
            var b = ((byte2 & 0xf8) >> 3);

            luminances[luminanceIndex] = (byte)((r + g + g + b) >> 2);
         }
      }

      /// <summary>
      /// Should create a new luminance source with the right class type.
      /// The method is used in methods crop and rotate.
      /// </summary>
      /// <param name="newLuminances">The new luminances.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns></returns>
      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new RGB565LuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}
