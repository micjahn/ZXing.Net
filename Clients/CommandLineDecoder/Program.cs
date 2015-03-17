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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using ZXing;

namespace CommandLineDecoder
{
   class Program
   {
      [STAThread]
      static void Main(string[] args)
      {
         if (args.Length == 0)
         {
            printUsage();
            return;
         }

         Config config = new Config();
         Inputs inputs = new Inputs();

         foreach (var arg in args)
         {
            if ("--try_harder".Equals(arg))
            {
               config.TryHarder = true;
            }
            else if ("--pure_barcode".Equals(arg))
            {
               config.PureBarcode = true;
            }
            else if ("--products_only".Equals(arg))
            {
               config.ProductsOnly = true;
            }
            else if ("--dump_results".Equals(arg))
            {
               config.DumpResults = true;
            }
            else if ("--dump_black_point".Equals(arg))
            {
               config.DumpBlackPoint = true;
            }
            else if ("--multi".Equals(arg))
            {
               config.Multi = true;
            }
            else if ("--brief".Equals(arg))
            {
               config.Brief = true;
            }
            else if ("--recursive".Equals(arg))
            {
               config.Recursive = true;
            }
            else if ("--autorotate".Equals(arg))
            {
               config.AutoRotate = true;
            }
            else if (arg.StartsWith("--crop"))
            {
               int[] crop = new int[4];
               String[] tokens = arg.Substring(7).Split(',');
               for (int i = 0; i < crop.Length; i++)
               {
                  crop[i] = int.Parse(tokens[i]);
               }
               config.Crop = crop;
            }
            else if (arg.StartsWith("--threads") && arg.Length >= 10)
            {
               int threadsCount = int.Parse(arg.Substring(10));
               if (threadsCount > 1)
               {
                  config.Threads = threadsCount;
               }
            }
            else if ("--get_from_clipboard".Equals(arg))
            {
               config.BitmapFromClipboard = (Bitmap)Clipboard.GetImage();
               if (config.BitmapFromClipboard == null)
               {
                  Console.Error.WriteLine("There is no image in the clipboard.");
                  Environment.ExitCode = 1;
                  return;
               }
               // Dummy
               inputs.addInput(".");
            }
            else if (arg.StartsWith("-"))
            {
               Console.Error.WriteLine("Unknown command line option " + arg);
               printUsage();
               return;
            }
         }

         config.Hints = buildHints(config);

         foreach (var arg in args)
         {
            if (!arg.StartsWith("--"))
            {
               addArgumentToInputs(arg, config, inputs);
            }
         }

         var threads = new Dictionary<Thread, DecodeThread>(Math.Min(config.Threads, inputs.getInputCount()));
         var decodeObjects = new List<DecodeThread>();
         for (int x = 0; x < config.Threads; x++)
         {
            var decodeThread = new DecodeThread(config, inputs);
            decodeObjects.Add(decodeThread);
            var thread = new Thread(decodeThread.run);
            threads.Add(thread, decodeThread);
            thread.Start();
         }

         int successful = 0;
         foreach (var thread in threads.Keys)
         {
            thread.Join();
            successful += threads[thread].getSuccessful();
         }

         var completeResult = String.Empty;
         foreach (var decodeObject in decodeObjects)
         {
            completeResult += decodeObject.ResultString;
            completeResult += Environment.NewLine;
         }
         Clipboard.SetText(completeResult);

         int total = inputs.getInputCount();
         if (total > 1)
         {
            Console.Out.WriteLine("\nDecoded " + successful + " files out of " + total +
                " successfully (" + (successful * 100 / total) + "%)\n");
         }
      }

      // Build all the inputs up front into a single flat list, so the threads can atomically pull
      // paths/URLs off the queue.
      private static void addArgumentToInputs(String argument, Config config, Inputs inputs)
      {
         if (Directory.Exists(argument))
         {
            foreach (var singleFile in Directory.EnumerateFiles(argument))
            {
               String filename = singleFile.ToLower(CultureInfo.InvariantCulture);
               // Skip hidden files and directories (e.g. svn stuff).)
               if (filename.StartsWith("."))
               {
                  continue;
               }
               // Skip text files and the results of dumping the black point.
               if (filename.EndsWith(".txt") || filename.Contains(".mono.png"))
               {
                  continue;
               }
               inputs.addInput(singleFile);
            }
            // Recurse on nested directories if requested, otherwise skip them.
            if (config.Recursive)
            {
               foreach (var dirName in Directory.EnumerateDirectories(argument))
               {
                  addArgumentToInputs(dirName, config, inputs);
               }
            }
         }
         else
         {
            inputs.addInput(argument);
         }
      }

      // Manually turn on all formats, even those not yet considered production quality.
      private static IDictionary<DecodeHintType, object> buildHints(Config config)
      {
         var hints = new Dictionary<DecodeHintType, Object>();
         var vector = new List<BarcodeFormat>(8)
                    {
                       BarcodeFormat.UPC_A,
                       BarcodeFormat.UPC_E,
                       BarcodeFormat.EAN_13,
                       BarcodeFormat.EAN_8,
                       BarcodeFormat.RSS_14,
                       BarcodeFormat.RSS_EXPANDED
                    };
         if (!config.ProductsOnly)
         {
            vector.Add(BarcodeFormat.CODE_39);
            vector.Add(BarcodeFormat.CODE_93);
            vector.Add(BarcodeFormat.CODE_128);
            vector.Add(BarcodeFormat.ITF);
            vector.Add(BarcodeFormat.QR_CODE);
            vector.Add(BarcodeFormat.DATA_MATRIX);
            vector.Add(BarcodeFormat.AZTEC);
            vector.Add(BarcodeFormat.PDF_417);
            vector.Add(BarcodeFormat.CODABAR);
            vector.Add(BarcodeFormat.MAXICODE);
         }
         hints[DecodeHintType.POSSIBLE_FORMATS] = vector;
         if (config.TryHarder)
         {
            hints[DecodeHintType.TRY_HARDER] = true;
         }
         if (config.PureBarcode)
         {
            hints[DecodeHintType.PURE_BARCODE] = true;
         }
         return hints;
      }

      private static void printUsage()
      {
         Console.Out.WriteLine("Decode barcode images using the ZXing library\n");
         Console.Out.WriteLine("usage: CommandLineRunner { file | dir | url } [ options ]");
         Console.Out.WriteLine("  --try_harder: Use the TRY_HARDER hint, default is normal (mobile) mode");
         Console.Out.WriteLine("  --pure_barcode: Input image is a pure monochrome barcode image, not a photo");
         Console.Out.WriteLine("  --products_only: Only decode the UPC and EAN families of barcodes");
         Console.Out.WriteLine("  --dump_results: Write the decoded contents to input.txt");
         Console.Out.WriteLine("  --dump_black_point: Compare black point algorithms as input.mono.png");
         Console.Out.WriteLine("  --multi: Scans image for multiple barcodes");
         Console.Out.WriteLine("  --brief: Only output one line per file, omitting the contents");
         Console.Out.WriteLine("  --recursive: Descend into subdirectories");
         Console.Out.WriteLine("  --crop=left,top,width,height: Only examine cropped region of input image(s)");
         Console.Out.WriteLine("  --threads=n: The number of threads to use while decoding");
         Console.Out.WriteLine("  --get_from_clipboard: Get the image from the clipboard instead loading from a file");
      }
   }
}
