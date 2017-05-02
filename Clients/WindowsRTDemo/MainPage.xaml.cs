using System;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;

namespace WindowsRT
{
   public sealed partial class MainPage : Page
   {
      private readonly MediaCapture _mediaCapture = new MediaCapture();
      private Result _result;

      public MainPage()
      {
         InitializeComponent();
      }

      protected override async void OnNavigatedTo(NavigationEventArgs e)
      {
         try
         {
            var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (cameras.Count < 1)
            {
               Error.Text = "No camera found, decoding static image";
               await DecodeStaticResource();
               return;
            }
            MediaCaptureInitializationSettings settings;
            if (cameras.Count == 1)
            {
               settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameras[0].Id }; // 0 => front, 1 => back
            }
            else
            {
               settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameras[1].Id }; // 0 => front, 1 => back
            }

            await _mediaCapture.InitializeAsync(settings);
            VideoCapture.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();

            while (_result == null)
            {
               var photoStorageFile = await KnownFolders.PicturesLibrary.CreateFileAsync("scan.jpg", CreationCollisionOption.GenerateUniqueName);
               await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), photoStorageFile);

               var stream = await photoStorageFile.OpenReadAsync();
               // initialize with 1,1 to get the current size of the image
               var writeableBmp = new WriteableBitmap(1, 1);
               writeableBmp.SetSource(stream);
               // and create it again because otherwise the WB isn't fully initialized and decoding
               // results in a IndexOutOfRange
               writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
               stream.Seek(0);
               writeableBmp.SetSource(stream);

               _result = ScanBitmap(writeableBmp);

               await photoStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            await _mediaCapture.StopPreviewAsync();
            VideoCapture.Visibility = Visibility.Collapsed;
            CaptureImage.Visibility = Visibility.Visible;
            ScanResult.Text = _result.Text;
         }
         catch (Exception ex)
         {
            Error.Text = ex.Message;
         }
      }

      private async System.Threading.Tasks.Task DecodeStaticResource()
      {
         var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\1.jpg");
         var stream = await file.OpenReadAsync();
         // initialize with 1,1 to get the current size of the image
         var writeableBmp = new WriteableBitmap(1, 1);
         writeableBmp.SetSource(stream);
         // and create it again because otherwise the WB isn't fully initialized and decoding
         // results in a IndexOutOfRange
         writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
         stream.Seek(0);
         writeableBmp.SetSource(stream);
         CaptureImage.Source = writeableBmp;
         VideoCapture.Visibility = Visibility.Collapsed;
         CaptureImage.Visibility = Visibility.Visible;

         _result = ScanBitmap(writeableBmp);
         if (_result != null)
         {
            ScanResult.Text += _result.Text;
         }
         return;
      }

      private Result ScanBitmap(WriteableBitmap writeableBmp)
      {
         var barcodeReader = new BarcodeReader
         {
            AutoRotate = true,
            Options = {TryHarder = true}
         };
         var result = barcodeReader.Decode(writeableBmp);

         if (result != null)
         {
            CaptureImage.Source = writeableBmp;
         }

         return result;
      }

      protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
      {
         await _mediaCapture.StopPreviewAsync();
      }
   }
}
