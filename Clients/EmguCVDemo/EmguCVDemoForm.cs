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
using System.Windows.Forms;

using Emgu.CV;

namespace EmguCVDemo
{
   /// <summary>
   /// See the README.txt for detailled information
   /// </summary>
   public partial class EmguCVDemoForm : Form
   {
      Capture capture;
      bool Capturing;
      private readonly IBarcodeReaderImage reader;

      public EmguCVDemoForm()
      {
         InitializeComponent();
         reader = new BarcodeReaderImage();
      }

      private void captureButton_Click(object sender, EventArgs e)
      {
         if (capture == null)
         {
            try
            {
               capture = new Capture();
            }
            catch (NullReferenceException exception)
            {
               MessageBox.Show(exception.Message);
            }
            catch (TypeInitializationException exc)
            {
               MessageBox.Show(
                  "Attention: You have to copy all the assemblies and native libraries from an official release of EmguCV to the directory of the demo." +
                  Environment.NewLine + Environment.NewLine + exc);
            }
         }

         if (capture != null)
         {
            if (Capturing)
            {
               captureButton.Text = "Start Capturing";
               Application.Idle -= DoDecoding;
            }
            else
            {
               captureButton.Text = "Stop Capturing";
               Application.Idle += DoDecoding;
            }
            Capturing = !Capturing;
         }
      }

      protected override void OnClosed(EventArgs e)
      {
         if (capture != null)
            capture.Dispose();
      }

      private void DoDecoding(object sender, EventArgs args)
      {
         var timerStart = DateTime.Now.Ticks;

         var image = capture.QueryFrame();
         if (image != null)
         {
            using(image)
            {
               // show it
               pictureBox1.Image = image.ToBitmap();
               // decode it
               var result = reader.Decode(image);
               // show result
               if (result != null)
               {
                  txtContentWebCam.Text = result.Text;
                  txtTypeWebCam.Text = result.BarcodeFormat.ToString();
               }
            }
         }
         var timerStop = DateTime.Now.Ticks;
         labDuration.Text = new TimeSpan(timerStop - timerStart).Milliseconds.ToString("0 ms");
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         Close();
      }
   }
}
