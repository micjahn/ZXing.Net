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
using System.Drawing.Imaging;

using ZXing;
using ZXing.Common;

namespace CommandLineEncoder
{
   /// <summary>
   /// Command line utility for encoding barcodes.
   /// <author>Sean Owen</author>
   /// </summary>
   class Program
   {
      private const BarcodeFormat DEFAULT_BARCODE_FORMAT = BarcodeFormat.QR_CODE;
      private static readonly ImageFormat DEFAULT_IMAGE_FORMAT = ImageFormat.Png;
      private const String DEFAULT_OUTPUT_FILE = "out";
      private const int DEFAULT_WIDTH = 300;
      private const int DEFAULT_HEIGHT = 300;

      [STAThread]
      static void Main(string[] args)
      {
         if (args.Length == 0)
         {
            printUsage();
            return;
         }

         AppDomain.CurrentDomain.UnhandledException +=
            (sender, exc) => Console.Error.WriteLine(exc.ExceptionObject.ToString());

         var barcodeFormat = DEFAULT_BARCODE_FORMAT;
         var imageFormat = DEFAULT_IMAGE_FORMAT;
         var outFileString = DEFAULT_OUTPUT_FILE;
         var width = DEFAULT_WIDTH;
         var height = DEFAULT_HEIGHT;
         var clipboard = false;
         foreach (var arg in args)
         {
            if (arg.StartsWith("--barcode_format"))
            {
               barcodeFormat = (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), arg.Split('=')[1].Trim());
            }
            else if (arg.StartsWith("--image_format"))
            {
               var format = arg.Split('=')[1].ToLower();
               switch (format)
               {
                  case "bmp":
                     imageFormat = ImageFormat.Bmp;
                     break;
                  case "emf":
                     imageFormat = ImageFormat.Emf;
                     break;
                  case "gif":
                     imageFormat = ImageFormat.Gif;
                     break;
                  case "icon":
                     imageFormat = ImageFormat.Icon;
                     break;
                  case "jpeg":
                  case "jpg":
                     imageFormat = ImageFormat.Jpeg;
                     break;
                  case "png":
                     imageFormat = ImageFormat.Png;
                     break;
                  case "tiff":
                     imageFormat = ImageFormat.Tiff;
                     break;
                  case "wmf":
                     imageFormat = ImageFormat.Wmf;
                     break;
                  default:
                     throw new ArgumentException("Image format isn't supported.", arg);
               }
            }
            else if (arg.StartsWith("--output"))
            {
               outFileString = arg.Split('=')[1];
            }
            else if (arg.StartsWith("--width"))
            {
               width = int.Parse(arg.Split('=')[1]);
            }
            else if (arg.StartsWith("--height"))
            {
               height = int.Parse(arg.Split('=')[1]);
            }
            else if (arg.StartsWith("--copy_to_clipboard"))
            {
               clipboard = true;
            }
         }

         if (DEFAULT_OUTPUT_FILE.Equals(outFileString))
         {
            outFileString += '.' + imageFormat.ToString();
         }

         String contents = null;
         foreach (var arg in args)
         {
            if (!arg.StartsWith("--"))
            {
               contents = arg;
               break;
            }
         }

         if (contents == null)
         {
            printUsage();
            return;
         }

         var barcodeWriter = new BarcodeWriter
                                {
                                   Format = barcodeFormat,
                                   Options = new EncodingOptions
                                                {
                                                   Height = height,
                                                   Width = width
                                                }
                                };
         var bitmap = barcodeWriter.Write(contents);
         if (clipboard)
         {
            System.Windows.Forms.Clipboard.SetImage(bitmap);
         }
         else
         {
            bitmap.Save(outFileString, imageFormat);
         }
      }

      private static void printUsage()
      {
         Console.Out.WriteLine("Encodes barcode images using the ZXing.Net library\n");
         Console.Out.WriteLine("usage: CommandLineEncoder [ options ] content_to_encode");
         Console.Out.WriteLine("  --barcode_format=format: Format to encode, from BarcodeFormat class. " +
                               "Supported formats: QR_CODE, EAN_8, EAN_13, UPC_A, CODE_39, CODE_128, ITF, PDF_417, CODABAR. Defaults to QR_CODE.");
         Console.Out.WriteLine("  --image_format=format: image output format, such as PNG, JPG, GIF, BMP, TIFF. Defaults to PNG");
         Console.Out.WriteLine("  --output=filename: File to write to. Defaults to out.png");
         Console.Out.WriteLine("  --width=pixels: Image width. Defaults to 300");
         Console.Out.WriteLine("  --height=pixels: Image height. Defaults to 300");
         Console.Out.WriteLine("  --copy_to_clipboard: Copy the image to the clipboard instead saving to a file");
      }
   }
}
