/*
 * Copyright 2011 ZXing authors
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
using System.Drawing.Imaging;
using System.IO;
using System.Net;

using ZXing;
using ZXing.Client.Result;
using ZXing.Common;

namespace CommandLineDecoder
{
   /// <summary>
   /// One of a pool of threads which pulls images off the Inputs queue and decodes them in parallel.
   /// @see CommandLineRunner
   /// </summary>
   internal sealed class DecodeThread
   {
      private int successful;
      private readonly Config config;
      private readonly Inputs inputs;
      public string ResultString { get; private set; }

      public DecodeThread(Config config, Inputs inputs)
      {
         this.config = config;
         this.inputs = inputs;
      }

      public void run()
      {
         ResultString = String.Empty;
         while (true)
         {
            String input = inputs.getNextInput();
            if (input == null)
            {
               break;
            }

            if (config.BitmapFromClipboard != null || File.Exists(input))
            {
               try
               {
                  if (config.Multi)
                  {
                     Result[] results = decodeMulti(new Uri(Path.GetFullPath(input)), input, config.Hints);
                     if (results != null)
                     {
                        successful++;
                        if (config.DumpResults)
                        {
                           dumpResultMulti(input, results);
                        }
                     }
                  }
                  else
                  {
                     Result result = decode(new Uri(Path.GetFullPath(input)), input, config.Hints);
                     if (result != null)
                     {
                        successful++;
                        if (config.DumpResults)
                        {
                           dumpResult(input, result);
                        }
                     }
                  }
               }
               catch (IOException exc)
               {
                  Console.WriteLine(exc.ToString());
               }
            }
            else
            {
               try
               {
                  var tempFile = Path.GetTempFileName();
                  var uri = new Uri(input);
                  var client = new WebClient();
                  client.DownloadFile(uri, tempFile);
                  try
                  {
                     Result result = decode(new Uri(tempFile), input, config.Hints);
                     if (result != null)
                     {
                        successful++;
                        if (config.DumpResults)
                        {
                           dumpResult(input, result);
                        }
                     }
                  }
                  finally
                  {
                     File.Delete(tempFile);
                  }
               }
               catch (Exception exc)
               {
                  Console.WriteLine(exc.ToString());
               }
            }
         }
      }

      public int getSuccessful()
      {
         return successful;
      }

      private static void dumpResult(string input, Result result)
      {
         int pos = input.LastIndexOf('.');
         if (pos > 0)
         {
            input = input.Substring(0, pos);
         }
         using (var stream = File.CreateText(input + ".txt"))
         {
            stream.Write(result.Text);
         }
      }

      private static void dumpResultMulti(String input, Result[] results)
      {
         int pos = input.LastIndexOf('.');
         if (pos > 0)
         {
            input = input.Substring(0, pos);
         }
         using (var stream = File.CreateText(input + ".txt"))
         {
            foreach (var result in results)
            {
               stream.WriteLine(result.Text);
            }
         }
      }

      private Result decode(Uri uri, string originalInput, IDictionary<DecodeHintType, object> hints)
      {
         Bitmap image = null;
         try
         {
            if (originalInput == ".")
            {
               image = config.BitmapFromClipboard;
            }
            else
            {
               image = (Bitmap)Bitmap.FromFile(uri.LocalPath);
            }
         }
         catch (Exception)
         {
            throw new FileNotFoundException("Resource not found: " + uri);
         }

         if (image == null)
            return null;

         using (image)
         {
            return decode(uri, image, originalInput, hints);
         }
      }

      private Result decode(Uri uri, Bitmap image, string originalInput, IDictionary<DecodeHintType, object> hints)
      {
         LuminanceSource source;
         if (config.Crop == null)
         {
            source = new BitmapLuminanceSource(image);
         }
         else
         {
            int[] crop = config.Crop;
            source = new BitmapLuminanceSource(image).crop(crop[0], crop[1], crop[2], crop[3]);
         }
         if (config.DumpBlackPoint)
         {
            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
            dumpBlackPoint(uri, image, bitmap, source);
         }
         var reader = new BarcodeReader {AutoRotate = config.AutoRotate};
         foreach (var entry in hints)
            reader.Options.Hints.Add(entry.Key, entry.Value);
         Result result = reader.Decode(source);
         if (result != null)
         {
            if (config.Brief)
            {
               Console.Out.WriteLine(uri + ": Success");
            }
            else
            {
               ParsedResult parsedResult = ResultParser.parseResult(result);
               var resultString = originalInput + " (format: " + result.BarcodeFormat + ", type: " + parsedResult.Type + "):" + Environment.NewLine;
               for (int i = 0; i < result.ResultPoints.Length; i++)
               {
                  ResultPoint rp = result.ResultPoints[i];
                  Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
               }
               resultString += "Raw result:" + Environment.NewLine + result.Text + Environment.NewLine;
               resultString += "Parsed result:" + Environment.NewLine + parsedResult.DisplayResult + Environment.NewLine;

               Console.Out.WriteLine(resultString);
               ResultString = resultString;
            }
         }
         else
         {
            var resultString = originalInput + ": No barcode found";
            Console.Out.WriteLine(resultString);
            ResultString = resultString;
         }
         return result;
      }

      private Result[] decodeMulti(Uri uri, string originalInput, IDictionary<DecodeHintType, object> hints)
      {
         Bitmap image;
         try
         {
            image = (Bitmap)Bitmap.FromFile(uri.LocalPath);
         }
         catch (Exception)
         {
            throw new FileNotFoundException("Resource not found: " + uri);
         }

         using (image)
         {
            LuminanceSource source;
            if (config.Crop == null)
            {
               source = new BitmapLuminanceSource(image);
            }
            else
            {
               int[] crop = config.Crop;
               source = new BitmapLuminanceSource(image).crop(crop[0], crop[1], crop[2], crop[3]);
            }
            if (config.DumpBlackPoint)
            {
               var bitmap = new BinaryBitmap(new HybridBinarizer(source));
               dumpBlackPoint(uri, image, bitmap, source);
            }

            var reader = new BarcodeReader {AutoRotate = config.AutoRotate};
            foreach (var entry in hints)
               reader.Options.Hints.Add(entry.Key, entry.Value);
            Result[] results = reader.DecodeMultiple(source);
            if (results != null && results.Length > 0)
            {
               if (config.Brief)
               {
                  Console.Out.WriteLine(uri + ": Success");
               }
               else
               {
                  foreach (var result in results)
                  {
                     ParsedResult parsedResult = ResultParser.parseResult(result);
                     var resultString = originalInput + " (format: " + result.BarcodeFormat + ", type: " + parsedResult.Type + "):" + Environment.NewLine;
                     for (int i = 0; i < result.ResultPoints.Length; i++)
                     {
                        ResultPoint rp = result.ResultPoints[i];
                        Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
                     }
                     resultString += "Raw result:" + Environment.NewLine + result.Text + Environment.NewLine;
                     resultString += "Parsed result:" + Environment.NewLine + parsedResult.DisplayResult + Environment.NewLine;

                     Console.Out.WriteLine(resultString);
                     ResultString = resultString;
                  }
               }
               return results;
            }
            else
            {
               var resultString = originalInput + ": No barcode found";
               Console.Out.WriteLine(resultString);
               ResultString = resultString;
            }
            return null;
         }
      }

      /**
       * Writes out a single PNG which is three times the width of the input image, containing from left
       * to right: the original image, the row sampling monochrome version, and the 2D sampling
       * monochrome version.
       */
      private static void dumpBlackPoint(Uri uri, Bitmap image, BinaryBitmap bitmap, LuminanceSource luminanceSource)
      {
         // TODO: Update to compare different Binarizer implementations.
         String inputName = uri.LocalPath;
         if (inputName.Contains(".mono.png"))
         {
            return;
         }

         // Use the current working directory for URLs
         String resultName = inputName;
         int pos;
         if ("http".Equals(uri.Scheme))
         {
            pos = resultName.LastIndexOf('/');
            if (pos > 0)
            {
               resultName = '.' + resultName.Substring(pos);
            }
         }
         pos = resultName.LastIndexOf('.');
         if (pos > 0)
         {
            resultName = resultName.Substring(0, pos);
         }
         resultName += ".mono.png";

         int width = bitmap.Width;
         int height = bitmap.Height;
         int stride = width * 4;
         var result = new Bitmap(stride, height, PixelFormat.Format32bppArgb);
         var offset = 0;

         // The original image
         for (int indexH = 0; indexH < height; indexH++)
         {
            for (int indexW = 0; indexW < width; indexW++)
            {
               result.SetPixel(indexW, indexH, image.GetPixel(indexW, indexH));
            }
         }

         // Row sampling
         BitArray row = new BitArray(width);
         offset += width;
         for (int y = 0; y < height; y++)
         {
            row = bitmap.getBlackRow(y, row);
            if (row == null)
            {
               // If fetching the row failed, draw a red line and keep going.
               for (int x = 0; x < width; x++)
               {
                  result.SetPixel(offset + x, y, Color.Red);
               }
               continue;
            }

            for (int x = 0; x < width; x++)
            {
               result.SetPixel(offset + x, y, row[x] ? Color.Black : Color.White);
            }
         }

         // 2D sampling
         offset += width;
         BitMatrix matrix = bitmap.BlackMatrix;
         for (int y = 0; y < height; y++)
         {
            for (int x = 0; x < width; x++)
            {
               result.SetPixel(offset + x, y, matrix[x, y] ? Color.Black : Color.White);
            }
         }

         offset += width;
         var luminanceMatrix = luminanceSource.Matrix;
         for (int y = 0; y < height; y++)
         {
            for (int x = 0; x < width; x++)
            {
               result.SetPixel(offset + x, y, Color.FromArgb(luminanceMatrix[y * width + x], luminanceMatrix[y * width + x], luminanceMatrix[y * width + x]));
            }
         }
         result.Save(resultName, ImageFormat.Png);
      }
   }
}