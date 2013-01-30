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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Tasks;

using ZXing;
using ZXing.Common;

namespace WindowsPhoneDemo
{
   public partial class MainPage
   {
      private readonly PhotoChooserTask photoChooserTask;
      private readonly BackgroundWorker scannerWorker;

      private DispatcherTimer timer;
      private PhotoCameraLuminanceSource luminance;
      private IBarcodeReader reader;
      private PhotoCamera photoCamera;
      private readonly WriteableBitmap dummyBitmap = new WriteableBitmap(1, 1);

      // Konstruktor
      public MainPage()
      {
         InitializeComponent();

         // prepare Photo Chooser Task for the open button
         photoChooserTask = new PhotoChooserTask();
         photoChooserTask.Completed += (s, e) => { if (e.TaskResult == TaskResult.OK) ProcessImage(e); };

         // prepare the backround worker thread for the image processing
         scannerWorker = new BackgroundWorker();
         scannerWorker.DoWork += scannerWorker_DoWork;
         scannerWorker.RunWorkerCompleted += scannerWorker_RunWorkerCompleted;

         foreach (var x in typeof(BarcodeFormat).GetFields())
         {
            if (x.IsLiteral)
            {
               BarcodeType.Items.Add(x.GetValue(null));
            }
         }

         // open the default barcode which should be displayed when the app starts
         var uri = new Uri("/images/35.png", UriKind.Relative);
         var imgSource = new BitmapImage(uri);
         BarcodeImage.Source = imgSource;
         imgSource.ImageOpened += (s, e) => scannerWorker.RunWorkerAsync(new WriteableBitmap((BitmapImage) s));
      }

      void scannerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
      {
         // processing the result of the background scanning
         if (e.Cancelled)
         {
            BarcodeContent.Text = "Cancelled.";
         }
         else if (e.Error != null)
         {
            BarcodeContent.Text = e.Error.Message;
         }
         else
         {
            var result = (Result) e.Result;
            DisplayResult(result);
         }
      }

      static void scannerWorker_DoWork(object sender, DoWorkEventArgs e)
      {
         // scanning for a barcode
         e.Result = new BarcodeReader().Decode((WriteableBitmap) e.Argument);
      }

      private void ProcessImage(PhotoResult e)
      {
         // setting the image in the display and start scanning in the background
         var bmp = new BitmapImage();
         bmp.SetSource(e.ChosenPhoto);
         BarcodeImage.Source = bmp;
         scannerWorker.RunWorkerAsync(new WriteableBitmap(bmp));
      }

      private void OpenImageButton_Click(object sender, RoutedEventArgs e)
      {
         if (timer != null)
         {
            timer.Stop();
            BarcodeImage.Visibility = System.Windows.Visibility.Visible;
            previewRect.Visibility = System.Windows.Visibility.Collapsed;
         }

         photoChooserTask.Show();
      }

      private void CameraButton_Click(object sender, RoutedEventArgs e)
      {
         if (photoCamera == null)
         {
            photoCamera = new PhotoCamera();
            photoCamera.Initialized += OnPhotoCameraInitialized;
            previewVideo.SetSource(photoCamera);

            CameraButtons.ShutterKeyHalfPressed += (o, arg) => photoCamera.Focus();
         }

         if (timer == null)
         {
            timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            if (photoCamera.IsFocusSupported)
            {
               photoCamera.AutoFocusCompleted += (o, arg) => { if (arg.Succeeded) ScanPreviewBuffer(); };
               timer.Tick += (o, arg) => { try { photoCamera.Focus(); } catch (Exception ) { } };
            }
            else
            {
               timer.Tick += (o, arg) => ScanPreviewBuffer();
            }
         }

         BarcodeImage.Visibility = System.Windows.Visibility.Collapsed;
         previewRect.Visibility = System.Windows.Visibility.Visible;
         timer.Start();
      }

      protected override void OnNavigatedFrom(NavigationEventArgs e)
      {
         base.OnNavigatedFrom(e);

         if (timer != null)
         {
            timer.Stop();
            timer = null;
         }
         photoCamera = null;
      }

      private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
      {
         var width = Convert.ToInt32(photoCamera.PreviewResolution.Width);
         var height = Convert.ToInt32(photoCamera.PreviewResolution.Height);

         Dispatcher.BeginInvoke(() =>
         {
            previewTransform.Rotation = photoCamera.Orientation;
            // create a luminance source which gets its values directly from the camera
            // the instance is returned directly to the reader
            luminance = new PhotoCameraLuminanceSource(width, height);
            reader = new BarcodeReader(null, bmp => luminance, null);
         });
      }

      private void ScanPreviewBuffer()
      {
         if (luminance == null)
            return;

         photoCamera.GetPreviewBufferY(luminance.PreviewBufferY);
         // use a dummy writeable bitmap because the luminance values are written directly to the luminance buffer
         var result = reader.Decode(dummyBitmap);
         Dispatcher.BeginInvoke(() => DisplayResult(result));
      }

      private void DisplayResult(Result result)
      {
         if (result != null)
         {
            BarcodeType.SelectedItem = result.BarcodeFormat;
            BarcodeContent.Text = result.Text;
         }
         else
         {
            BarcodeType.SelectedItem = null;
            BarcodeContent.Text = "No barcode found.";
         }
      }

      private void GenerateButton_Click(object sender, RoutedEventArgs e)
      {
         if (BarcodeType.SelectedItem == null)
            return;

         IBarcodeWriter writer = new BarcodeWriter
                                    {
                                       Format = (BarcodeFormat) BarcodeType.SelectedItem,
                                       Options = new EncodingOptions
                                                    {
                                                       Height = 480,
                                                       Width = 640
                                                    }
                                    };
         var bmp = writer.Write(BarcodeContent.Text);
         BarcodeImage.Source = bmp;
      }
   }
}