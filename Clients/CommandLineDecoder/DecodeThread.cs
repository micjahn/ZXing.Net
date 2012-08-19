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

using ZXing;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Multi;

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

      public DecodeThread(Config config, Inputs inputs)
      {
         this.config = config;
         this.inputs = inputs;
      }

      public void run()
      {
         while (true)
         {
            String input = inputs.getNextInput();
            if (input == null)
            {
               break;
            }

            if (File.Exists(input))
            {
               try
               {
                  if (config.Multi)
                  {
                     Result[] results = decodeMulti(new Uri(input), config.Hints);
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
                     Result result = decode(new Uri(input), config.Hints);
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
               catch (IOException e)
               {
               }
            }
            else
            {
               try
               {
                  if (decode(new Uri(input), config.Hints) != null)
                  {
                     successful++;
                  }
               }
               catch (Exception e)
               {
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

      private Result decode(Uri uri, IDictionary<DecodeHintType, object> hints)
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

         try
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
            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
            if (config.DumpBlackPoint)
            {
               dumpBlackPoint(uri, image, bitmap);
            }
            Result result = new MultiFormatReader().decode(bitmap, hints);
            if (result != null)
            {
               if (config.Brief)
               {
                  Console.Out.WriteLine(uri + ": Success");
               }
               else
               {
                  ParsedResult parsedResult = ResultParser.parseResult(result);
                  Console.Out.WriteLine(uri + " (format: " + result.BarcodeFormat + ", type: " +
                                        parsedResult.Type + "):\nRaw result:\n" + result.Text + "\nParsed result:\n" +
                                        parsedResult.DisplayResult);

                  Console.Out.WriteLine("Found " + result.ResultPoints.Length + " result points.");
                  for (int i = 0; i < result.ResultPoints.Length; i++)
                  {
                     ResultPoint rp = result.ResultPoints[i];
                     Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
                  }
               }
            }
            else
            {
               Console.Out.WriteLine(uri + ": No barcode found");
            }
            return result;
         }
         catch (NotFoundException nfe)
         {
            Console.Out.WriteLine(uri + ": No barcode found");
            return null;
         }
      }

      private Result[] decodeMulti(Uri uri, IDictionary<DecodeHintType, object> hints)
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

         try
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
            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
            if (config.DumpBlackPoint)
            {
               dumpBlackPoint(uri, image, bitmap);
            }

            MultiFormatReader multiFormatReader = new MultiFormatReader();
            GenericMultipleBarcodeReader reader = new GenericMultipleBarcodeReader(
                multiFormatReader);
            Result[] results = reader.decodeMultiple(bitmap, hints);
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
                     Console.Out.WriteLine(uri + " (format: "
                                           + result.BarcodeFormat + ", type: "
                                           + parsedResult.Type + "):\nRaw result:\n"
                                           + result.Text + "\nParsed result:\n"
                                           + parsedResult.DisplayResult);
                     Console.Out.WriteLine("Found " + result.ResultPoints.Length + " result points.");
                     for (int i = 0; i < result.ResultPoints.Length; i++)
                     {
                        ResultPoint rp = result.ResultPoints[i];
                        Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
                     }
                  }
               }
               return results;
            }
            else
            {
               Console.Out.WriteLine(uri + ": No barcode found");
            }
         }
         catch (NotFoundException nfe)
         {
            Console.Out.WriteLine(uri + ": No barcode found");
         }
         return null;
      }

      /**
       * Writes out a single PNG which is three times the width of the input image, containing from left
       * to right: the original image, the row sampling monochrome version, and the 2D sampling
       * monochrome version.
       */
      private static void dumpBlackPoint(Uri uri, Bitmap image, BinaryBitmap bitmap)
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
         int stride = width * 3;
         var result = new Bitmap(stride, height, PixelFormat.Format32bppArgb);

         // The original image
         for (int indexH = 0; indexH < height; indexH++)
         {
            for (int indexW = 0; indexW < width; indexW++)
            {
               result.SetPixel(indexW, indexH * width, image.GetPixel(indexW, indexH));
            }
         }

         // Row sampling
         BitArray row = new BitArray(width);
         int offset;
         for (int y = 0; y < height; y++)
         {
            offset = y * stride + width;
            try
            {
               row = bitmap.getBlackRow(y, row);
            }
            catch (NotFoundException nfe)
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
         try
         {
            for (int y = 0; y < height; y++)
            {
               BitMatrix matrix = bitmap.BlackMatrix;
               offset = y * stride + width * 2;
               for (int x = 0; x < width; x++)
               {
                  result.SetPixel(offset + x, y, matrix[x, y] ? Color.Black : Color.White);
               }
            }
         }
         catch (NotFoundException nfe)
         {
         }

         result.Save(resultName, ImageFormat.Png);
      }
   }
}