// define USE_ORIGINAL_IMPLEMENTATION if the original implementation from Jonas Follesø should be used
// otherwise use a ZXing.Net standard implementation
// #define USE_ORIGINAL_IMPLEMENTATION

using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections.ObjectModel;

using Microsoft.Devices;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;


namespace ScannerDemo
{
    public partial class MainPage
    {
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<string> _matches;

        private PhotoCameraLuminanceSource _luminance;
        private PhotoCamera _photoCamera;
#if USE_ORIGINAL_IMPLEMENTATION
        private Reader _reader;
#else
        private WriteableBitmap _dummy;
        private IBarcodeReader _reader;
#endif
        
        public MainPage()
        {            
            InitializeComponent();

            _matches = new ObservableCollection<string>();
            _matchesList.ItemsSource = _matches;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();

#if !USE_ORIGINAL_IMPLEMENTATION
            _dummy = new WriteableBitmap(1, 1);
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _photoCamera = new PhotoCamera();
            _photoCamera.Initialized += OnPhotoCameraInitialized;            
            _previewVideo.SetSource(_photoCamera);

            CameraButtons.ShutterKeyHalfPressed += (o, arg) => _photoCamera.Focus();

            base.OnNavigatedTo(e);
        }

        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            
            _luminance = new PhotoCameraLuminanceSource(width, height);
#if USE_ORIGINAL_IMPLEMENTATION
            _reader = new QRCodeReader();
#else
            _reader = new BarcodeReader(null, p => _luminance, null);
#endif

            Dispatcher.BeginInvoke(() => {
                _previewTransform.Rotation = _photoCamera.Orientation;
                _timer.Start();
            });
        }
 
        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
#if USE_ORIGINAL_IMPLEMENTATION
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                var result = _reader.decode(binBitmap);
#else
                var result = _reader.Decode(_dummy);
#endif
                if (result != null)
                {
                    Dispatcher.BeginInvoke(() => DisplayResult(result.Text));
                }
            }
            catch
            {
            }            
        }

        private void DisplayResult(string text)
        {
            if(!_matches.Contains(text))
                _matches.Add(text);            
        }
    }
}