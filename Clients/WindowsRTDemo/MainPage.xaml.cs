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
               ScanResult.Text = "No camera found";
               return;
            }
            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameras[1].Id }; // 0 => front, 1 => back

            await _mediaCapture.InitializeAsync(settings);
            VideoCapture.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();

            while (_result == null)
            {
               var photoStorageFile = await KnownFolders.PicturesLibrary.CreateFileAsync("scan.jpg", CreationCollisionOption.GenerateUniqueName);
               await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), photoStorageFile);

               var writeableBmp = new WriteableBitmap(640, 360);
               writeableBmp.SetSource(await photoStorageFile.OpenReadAsync());

               var barcodeReader = new BarcodeReader
               {
                  TryHarder = true,
                  AutoRotate = true
               };
               _result = barcodeReader.Decode(writeableBmp);

               if (_result != null)
               {
                  CaptureImage.Source = writeableBmp;
               }

               await photoStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            await _mediaCapture.StopPreviewAsync();
            VideoCapture.Visibility = Visibility.Collapsed;
            CaptureImage.Visibility = Visibility.Visible;
            ScanResult.Text = _result.Text;
         }
         catch (Exception ex)
         {
            ScanResult.Text = ex.Message;
         }
      }

      protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
      {
         await _mediaCapture.StopPreviewAsync();
      }
   }
}
