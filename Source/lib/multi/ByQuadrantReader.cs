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

using System.Collections.Generic;

namespace ZXing.Multi
{
   /// <summary>
   /// This class attempts to decode a barcode from an image, not by scanning the whole image,
   /// but by scanning subsets of the image. This is important when there may be multiple barcodes in
   /// an image, and detecting a barcode may find parts of multiple barcode and fail to decode
   /// (e.g. QR Codes). Instead this scans the four quadrants of the image -- and also the center
   /// 'quadrant' to cover the case where a barcode is found in the center.
   /// </summary>
   /// <seealso cref="GenericMultipleBarcodeReader" />
   public sealed class ByQuadrantReader : Reader
   {
      private readonly Reader @delegate;

      /// <summary>
      /// Initializes a new instance of the <see cref="ByQuadrantReader"/> class.
      /// </summary>
      /// <param name="delegate">The @delegate.</param>
      public ByQuadrantReader(Reader @delegate)
      {
         this.@delegate = @delegate;
      }

      /// <summary>
      /// Locates and decodes a barcode in some format within an image.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      public Result decode(BinaryBitmap image)
      {
         return decode(image, null);
      }

      /// <summary>
      /// Locates and decodes a barcode in some format within an image. This method also accepts
      /// hints, each possibly associated to some data, which may help the implementation decode.
      /// </summary>
      /// <param name="image">image of barcode to decode</param>
      /// <param name="hints">passed as a <see cref="IDictionary{TKey, TValue}"/> from <see cref="DecodeHintType"/>
      /// to arbitrary data. The
      /// meaning of the data depends upon the hint type. The implementation may or may not do
      /// anything with these hints.</param>
      /// <returns>
      /// String which the barcode encodes
      /// </returns>
      public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         int width = image.Width;
         int height = image.Height;
         int halfWidth = width/2;
         int halfHeight = height/2;

         // No need to call makeAbsolute as results will be relative to original top left here
         var result = @delegate.decode(image.crop(0, 0, halfWidth, halfHeight), hints);
         if (result != null)
            return result;

         result = @delegate.decode(image.crop(halfWidth, 0, halfWidth, halfHeight), hints);
         if (result != null)
         {
            makeAbsolute(result.ResultPoints, halfWidth, 0);
            return result;
         }

         result = @delegate.decode(image.crop(0, halfHeight, halfWidth, halfHeight), hints);
         if (result != null)
         {
            makeAbsolute(result.ResultPoints, 0, halfHeight);
            return result;
         }

         result = @delegate.decode(image.crop(halfWidth, halfHeight, halfWidth, halfHeight), hints);
         if (result != null)
         {
            makeAbsolute(result.ResultPoints, halfWidth, halfHeight);
            return result;
         }

         int quarterWidth = halfWidth/2;
         int quarterHeight = halfHeight/2;
         var center = image.crop(quarterWidth, quarterHeight, halfWidth, halfHeight);
         result = @delegate.decode(center, hints);
         if (result != null)
         {
            makeAbsolute(result.ResultPoints, quarterWidth, quarterHeight);
         }
         return result;
      }

      /// <summary>
      /// Resets any internal state the implementation has after a decode, to prepare it
      /// for reuse.
      /// </summary>
      public void reset()
      {
         @delegate.reset();
      }

      private static void makeAbsolute(ResultPoint[] points, int leftOffset, int topOffset)
      {
         if (points != null)
         {
            for (int i = 0; i < points.Length; i++)
            {
               ResultPoint relative = points[i];
               points[i] = new ResultPoint(relative.X + leftOffset, relative.Y + topOffset);
            }
         }
      }
   }
}
