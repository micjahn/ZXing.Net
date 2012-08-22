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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using AForge.Video;
using ZXing;

namespace AForgeDemo
{
   public partial class AForgeDemoForm : Form
   {
      private struct Device
      {
         public int Index;
         public string Name;
         public override string ToString()
         {
            return Name;
         }
      }

      private readonly CameraDevices camDevices;
      private Bitmap currentBitmapForDecoding;
      private readonly Thread decodingThread;
      private Result currentResult;
      private readonly Pen resultRectPen;

      public AForgeDemoForm()
      {
         InitializeComponent();

         camDevices = new CameraDevices();

         decodingThread = new Thread(DecodeBarcode);
         decodingThread.Start();

         pictureBox1.Paint += pictureBox1_Paint;
         resultRectPen = new Pen(Color.Green, 10);
      }

      void pictureBox1_Paint(object sender, PaintEventArgs e)
      {
         if (currentResult == null)
            return;

         if (currentResult.ResultPoints != null && currentResult.ResultPoints.Length > 0)
         {
            var resultPoints = currentResult.ResultPoints;
            var rect = new Rectangle((int)resultPoints[0].X, (int)resultPoints[0].Y, 1, 1);
            foreach (var point in resultPoints)
            {
               if (point.X < rect.Left)
                  rect = new Rectangle((int)point.X, rect.Y, rect.Width + rect.X - (int)point.X, rect.Height);
               if (point.X > rect.Right)
                  rect = new Rectangle(rect.X, rect.Y, rect.Width + (int)point.X - rect.X, rect.Height);
               if (point.Y < rect.Top)
                  rect = new Rectangle(rect.X, (int)point.Y, rect.Width, rect.Height + rect.Y - (int)point.Y);
               if (point.Y > rect.Bottom)
                  rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)point.Y - rect.Y);
            }
            using (var g = pictureBox1.CreateGraphics())
            {
               g.DrawRectangle(resultRectPen, rect);
            }
         }
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         LoadDevicesToCombobox();
      }

      protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
      {
         base.OnClosing(e);
         if (!e.Cancel)
         {
            decodingThread.Abort();
            if (camDevices.Current != null)
            {
               camDevices.Current.NewFrame -= Current_NewFrame;
               if (camDevices.Current.IsRunning)
               {
                  camDevices.Current.SignalToStop();
               }
            }
         }
      }

      private void LoadDevicesToCombobox()
      {
         cmbDevice.Items.Clear();
         for (var index = 0; index < camDevices.Devices.Count; index++)
         {
            cmbDevice.Items.Add(new Device { Index = index, Name = camDevices.Devices[index].Name });
         }
      }

      private void cmbDevice_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (camDevices.Current != null)
         {
            camDevices.Current.NewFrame -= Current_NewFrame;
            if (camDevices.Current.IsRunning)
            {
               camDevices.Current.SignalToStop();
            }
         }

         camDevices.SelectCamera(((Device)(cmbDevice.SelectedItem)).Index);
         camDevices.Current.NewFrame += Current_NewFrame;
         camDevices.Current.Start();
      }

      private void Current_NewFrame(object sender, NewFrameEventArgs eventArgs)
      {
         if (IsDisposed)
         {
            return;
         }

         try
         {
            if (currentBitmapForDecoding == null)
            {
               currentBitmapForDecoding = (Bitmap)eventArgs.Frame.Clone();
            }
            Invoke(new Action<Bitmap>(ShowFrame), eventArgs.Frame.Clone());
         }
         catch (ObjectDisposedException)
         {
            // not sure, why....
         }
      }

      private void ShowFrame(Bitmap frame)
      {
         if (pictureBox1.Width < frame.Width)
         {
            pictureBox1.Width = frame.Width;
         }
         if (pictureBox1.Height < frame.Height)
         {
            pictureBox1.Height = frame.Height;
         }
         pictureBox1.Image = frame;
      }

      private void DecodeBarcode()
      {
         var reader = new BarcodeReader();
         while (true)
         {
            if (currentBitmapForDecoding != null)
            {
               var result = reader.Decode(currentBitmapForDecoding);
               if (result != null)
               {
                  Invoke(new Action<Result>(ShowResult), result);
               }
               currentBitmapForDecoding.Dispose();
               currentBitmapForDecoding = null;
            }
            Thread.Sleep(200);
         }
      }

      private void ShowResult(Result result)
      {
         currentResult = result;
         txtBarcodeFormat.Text = result.BarcodeFormat.ToString();
         txtContent.Text = result.Text;
      }
   }
}
