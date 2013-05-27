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
using System.Collections;
using System.Configuration.Install;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

using ZXing;
using ZXing.Common;

namespace ServiceDemo
{
   public partial class BarcodeScannerService : ServiceBase
   {
      private readonly IBarcodeReader barcodeReader;

      public BarcodeScannerService()
      {
         InitializeComponent();

         barcodeReader = new BarcodeReader
            {
               AutoRotate = true,
               Options = new DecodingOptions
                  {
                     TryHarder = true
                  }
            };
      }

      public void StartForeground(string[] args)
      {
         if (args.Length > 0)
         {
            switch (args[0])
            {
               case "/install":
               case "-install":
               case "--install":
                  {
                     var directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Barcodes");
                     if (args.Length > 1)
                     {
                        directory = Path.GetFullPath(args[1]);
                     }
                     if (!Directory.Exists(directory))
                        throw new ArgumentException(String.Format("The barcode directory {0} doesn't exists.", directory));
                     
                     var transactedInstaller = new TransactedInstaller();
                     var serviceInstaller = new ServiceInstaller();
                     transactedInstaller.Installers.Add(serviceInstaller);
                     var ctx = new InstallContext();
                     ctx.Parameters["assemblypath"] = String.Format("{0} \"{1}\"", Assembly.GetExecutingAssembly().Location, directory);
                     transactedInstaller.Context = ctx;
                     transactedInstaller.Install(new Hashtable());

                     Console.WriteLine("The service is installed. Barcode images have to be placed into the directory {0}.", directory);
                  }
                  return;
               case "/uninstall":
               case "-uninstall":
               case "--uninstall":
                  {
                     var transactedInstaller = new TransactedInstaller();
                     var serviceInstaller = new ServiceInstaller();
                     transactedInstaller.Installers.Add(serviceInstaller);
                     var ctx = new InstallContext();
                     ctx.Parameters["assemblypath"] = String.Format("{0}", Assembly.GetExecutingAssembly().Location);
                     transactedInstaller.Context = ctx;
                     transactedInstaller.Uninstall(null);

                     Console.WriteLine("The service is uninstalled.");
                  }
                  return;
               default:
                  if (args[0][0] != '/' &&
                      args[0][0] != '-')
                     throw new ArgumentException(String.Format("The argument {0} isn't supported.", args[0]));
                  break;
            }
         }

         OnStart(args);

         Console.ReadLine();
      }

      protected override void OnStart(string[] args)
      {
         var directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Barcodes");
         if (args.Length > 0)
         {
            directory = Path.GetFullPath(args[0]);
         }

         Console.WriteLine("Waiting for barcode images: {0}", directory);
         EventLog.WriteEntry(String.Format("Waiting for barcode images: {0}", directory));

         fileWatcher.Path = directory;
      }

      protected override void OnStop()
      {
      }

      private void fileWatcher_Created(object sender, FileSystemEventArgs e)
      {
         if (Path.GetExtension(e.FullPath) == ".txt")
            return;
         DecodeBarcode(e.FullPath);
      }

      private void DecodeBarcode(string fileName)
      {
         try
         {
            Console.WriteLine("Decoding image: {0}", fileName);

            var barcodeImageFile = fileName;
            var barcodeResultFile = fileName + ".txt";
            using (var resultWriter = new StreamWriter(barcodeResultFile, false, Encoding.UTF8))
            using (var bitmap = (Bitmap)Bitmap.FromFile(barcodeImageFile))
            {
               try
               {
                  var result = barcodeReader.Decode(bitmap);

                  Console.WriteLine("Result: {0}", result.Text);

                  resultWriter.WriteLine("RESULT:{0}", result.Text);
                  resultWriter.WriteLine("FORMAT: {0}", result.BarcodeFormat);
                  foreach (var metaData in result.ResultMetadata)
                  {
                     resultWriter.WriteLine("METADATA:{0}:{1}", metaData.Key, metaData.Value);
                  }
               }
               catch (Exception innerExc)
               {
                  Console.WriteLine("Exception: {0}", innerExc.Message);

                  resultWriter.Write(innerExc.ToString());
               }
            }
         }
         catch (Exception exc)
         {
            Console.WriteLine("Exception: {0}", exc.Message);
         }
      }
   }
}
