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
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServiceDemo
{
   static class Program
   {
      /// <summary>
      /// Der Haupteinstiegspunkt für die Anwendung.
      /// </summary>
      static void Main(string[] args)
      {
         if (!Environment.UserInteractive)
         {
            var ServicesToRun = new ServiceBase[]
                                             {
                                                new BarcodeScannerService()
                                             };
            ServiceBase.Run(ServicesToRun);
         }
         else
         {
            try
            {
               EnableConsole();
               var service = new BarcodeScannerService();
               service.StartForeground(args);
            }
            catch (Exception exc)
            {
               Console.WriteLine(exc.Message);
               var innerExc = exc.InnerException;
               while (innerExc != null)
               {
                  Console.WriteLine(innerExc.Message);
                  innerExc = innerExc.InnerException;
               }
            }
         }
      }

      private const int ATTACH_PARENT_PROCESS = -1;

      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
      private static extern bool AttachConsole(int dwProcessId);

      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
      private static extern bool AllocConsole();

      public static void EnableConsole()
      {
         if (!AttachConsole(ATTACH_PARENT_PROCESS))
         {
            AllocConsole();
         }
      }
   }
}
