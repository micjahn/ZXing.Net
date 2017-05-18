/*
 * Copyright 2017 ZXing.Net authors
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
using System.IO;

using Microsoft.AspNetCore.Razor.TagHelpers;

using ZXing;
using ZXing.QrCode;

namespace ASP.NetCoreDemo.Helper
{
   [HtmlTargetElement("barcode")]
   public class BarcodeTagHelper : TagHelper
   {
      public enum OutputFormat
      {
         PNG,
      }

      public override void Process(TagHelperContext context, TagHelperOutput output)
      {
         var content = context.AllAttributes["content"]?.Value.ToString();
         var alt = context.AllAttributes["alt"]?.Value.ToString();
         var width = Convert.ToInt32(context.AllAttributes["width"] == null ? "500" : context.AllAttributes["width"].Value.ToString());
         var height = Convert.ToInt32(context.AllAttributes["height"] == null ? "500" : context.AllAttributes["height"].Value.ToString());
         var margin = Convert.ToInt32(context.AllAttributes["margin"] == null ? "5" : context.AllAttributes["margin"].Value.ToString());
         var barcodeformat = BarcodeFormat.QR_CODE;
         var outputformat = OutputFormat.PNG;

         if (context.AllAttributes["barcodeformat"] != null)
         {
            if (!Enum.TryParse(context.AllAttributes["barcodeformat"].Value.ToString(), true, out barcodeformat))
            {
               barcodeformat = BarcodeFormat.QR_CODE;
            }
         }

         if (context.AllAttributes["outputformat"] != null)
         {
            if (!Enum.TryParse(context.AllAttributes["outputformat"].Value.ToString(), true, out outputformat))
            {
               outputformat = OutputFormat.PNG;
            }
         }

         if (String.IsNullOrEmpty(content))
            return;

         var qrWriter = new ZXing.BarcodeWriterPixelData
         {
            Format = barcodeformat,
            Options = new QrCodeEncodingOptions { Height = height, Width = width, Margin = margin }
         };


         var pixelData = qrWriter.Write(content);

         // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
         // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
         // the System.Drawing.Bitmap class is provided by the CoreCompat.System.Drawing package
         using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
         using (var ms = new MemoryStream())
         {
            // lock the data area for fast access
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
               System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
               // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
               System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                  pixelData.Pixels.Length);
            }
            finally
            {
               bitmap.UnlockBits(bitmapData);
            }
            // save to stream as PNG
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            output.TagName = "img";
            output.Attributes.Clear();
            output.Attributes.Add("width", width);
            output.Attributes.Add("height", height);
            output.Attributes.Add("alt", alt);
            output.Attributes.Add("src",
               String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
         }
      }
   }
}
