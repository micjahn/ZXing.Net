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
            var b5 = byte1 & 0x1F;
            var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
            var r5 = (byte2 >> 2) & 0x1F;
            var r8 = (r5 * 527 + 23) >> 6;
            var g8 = (g5 * 527 + 23) >> 6;
            var b8 = (b5 * 527 + 23) >> 6;

            luminances[luminanceIndex] = (byte)(0.3 * r8 + 0.59 * g8 + 0.11 * b8 + 0.01);
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
