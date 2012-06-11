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

namespace ZXing
{
   public class BaseLuminanceSource : LuminanceSource
   {
      protected byte[] luminances;

      protected BaseLuminanceSource(int width, int height)
         : base(width, height)
      {
         luminances = new byte[width * height];
      }

      protected BaseLuminanceSource(byte[] luminanceArray, int width, int height)
         : base(width, height)
      {
         luminances = new byte[width * height];
         Buffer.BlockCopy(luminanceArray, 0, luminances, 0, width * height);
      }

      override public byte[] getRow(int y, byte[] row)
      {
         int width = Width;
         if (row == null || row.Length < width)
         {
            row = new byte[width];
         }
         for (int i = 0; i < width; i++)
            row[i] = (byte)(luminances[y * width + i] - 128);
         return row;
      }

      public override byte[] Matrix
      {
         get { return luminances; }
      }

      public override LuminanceSource rotateCounterClockwise()
      {
         var rotatedLuminances = new byte[Width * Height];
         var newWidth = Height;
         var newHeight = Width;
         for (var yold = 0; yold < Height; yold++)
         {
            for (var xold = 0; xold < Width; xold++)
            {
               var ynew = xold;
               var xnew = newWidth - yold - 1;
               rotatedLuminances[ynew * newWidth + xnew] = luminances[yold * Width + xold];
            }
         }
         luminances = rotatedLuminances;
         Height = newHeight;
         Width = newWidth;
         return this;
      }

      public override bool RotateSupported
      {
         get
         {
            return true;
         }
      }
   }
}
