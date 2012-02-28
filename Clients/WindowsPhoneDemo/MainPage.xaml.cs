using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

using ZXing;
using ZXing.Common;

namespace WindowsPhoneDemo
{
   public partial class MainPage
   {
      private readonly PhotoChooserTask photoChooserTask;
      private readonly CameraCaptureTask cameraCaptureTask;
      private readonly BackgroundWorker scannerWorker;

      // Konstruktor
      public MainPage()
      {
         InitializeComponent();

         // prepare Photo Chooser Task for the open button
         photoChooserTask = new PhotoChooserTask();
         photoChooserTask.Completed += (s, e) => { if (e.TaskResult == TaskResult.OK) ProcessImage(e); };

         // prepare Camera Capture Task for the camera button
         cameraCaptureTask = new CameraCaptureTask();
         cameraCaptureTask.Completed += (s, e) => { if (e.TaskResult == TaskResult.OK) ProcessImage(e); };

         // prepare the backround worker thread for the image processing
         scannerWorker = new BackgroundWorker();
         scannerWorker.DoWork += scannerWorker_DoWork;
         scannerWorker.RunWorkerCompleted += scannerWorker_RunWorkerCompleted;

         // open the default barcode which should be displayed when the app starts
         var uri = new Uri("/images/35.png", UriKind.Relative);
         var imgSource = new BitmapImage(uri);
         BarcodeImage.Source = imgSource;
         imgSource.ImageOpened += (s, e) =>
                                     {
                                        var bmp = (BitmapImage) s;
                                        scannerWorker.RunWorkerAsync(new WriteableBitmap(bmp));
                                     };
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
            if (result != null)
            {
               BarcodeType.Text = result.BarcodeFormat.ToString();
               BarcodeContent.Text = result.Text;
            }
            else
            {
               BarcodeType.Text = String.Empty;
               BarcodeContent.Text = "No barcode found.";
            }
         }
      }

      static void scannerWorker_DoWork(object sender, DoWorkEventArgs e)
      {
         // scanning for a barcode
         var bmp = (WriteableBitmap) e.Argument;
         var imageSource = new RGBLuminanceSource(bmp, bmp.PixelWidth, bmp.PixelHeight);
         var binarizer = new HybridBinarizer(imageSource);
         var binaryBitmap = new BinaryBitmap(binarizer);
         var reader = new MultiFormatReader();
         e.Result = reader.decode(binaryBitmap);
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
         photoChooserTask.Show();
      }

      private void CameraButton_Click(object sender, RoutedEventArgs e)
      {
         cameraCaptureTask.Show();
      }
   }
}