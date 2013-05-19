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

using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Devices;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using WindowsPhone8Demo.Annotations;
using ZXing;

namespace WindowsPhone8Demo.ViewModels
{
    public class CaptureViewModel : INotifyPropertyChanged
    {
        private VideoBrush _videoBrush;
        private CompositeTransform _compositeTransform;
        private PhotoCaptureDevice _photoCaptureDevice;
        private Size _captureResolution;
        private ObservableCollection<Result> _results;

        public CaptureViewModel()
        {
            InitializeAndGo();
        }

        #region Methods
        private async void InitializeAndGo()
        {
            Results = new ObservableCollection<Result>();

            CaptureResolution = await GetBestCaptureResolution();
            await InitializePhotoCaptureDevice(CaptureResolution);

            await StartCapturingAsync();

            while (true)
            {
                Results.Add(await GetBarcodeAsync());
            }
        }

        private async Task<Result> GetBarcodeAsync()
        {
            await PhotoCaptureDevice.FocusAsync();
            return await DetectBarcodeAsync();
        }

        private async Task<Result> DetectBarcodeAsync()
        {
            var width = (int)CaptureResolution.Width;
            var height = (int)CaptureResolution.Height;
            var previewBuffer = new byte[width * height];

            PhotoCaptureDevice.GetPreviewBufferY(previewBuffer);

            var barcodeReader = new BarcodeReader();
            barcodeReader.TryHarder = true;
            //barcodeReader.TryInverted = true;
            //barcodeReader.AutoRotate = true;

            var result = barcodeReader.Decode(previewBuffer, width, height, RGBLuminanceSource.BitmapFormat.Gray8);
            return result;
        }

        private async Task StartCapturingAsync()
        {
            CameraCaptureSequence sequence = PhotoCaptureDevice.CreateCaptureSequence(1);
            var memoryStream = new MemoryStream();
            sequence.Frames[0].CaptureStream = memoryStream.AsOutputStream();

            PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Off);
            PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.SceneMode, CameraSceneMode.Macro);

            await PhotoCaptureDevice.PrepareCaptureSequenceAsync(sequence);
        }

        private async Task<Size> GetBestCaptureResolution()
        {
            // The last size in the AvailableCaptureResolutions is the lowest available
            var captureResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back);
            var previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back);

            Size resolution = await Task.Factory.StartNew(() => captureResolutions.Last(
                c => (c.Width > 1000.0 || c.Height > 1000.0) && previewResolutions.Any(p => (c.Width / c.Height).Equals(p.Width / p.Height))));
            return resolution;
        }

        private async Task InitializePhotoCaptureDevice(Size size)
        {
            PhotoCaptureDevice = await PhotoCaptureDevice.OpenAsync(CameraSensorLocation.Back, size);

            CompositeTransform = new CompositeTransform();
            CompositeTransform.CenterX = .5;
            CompositeTransform.CenterY = .5;
            CompositeTransform.Rotation = PhotoCaptureDevice.SensorRotationInDegrees - 90;

            VideoBrush = new VideoBrush();
            VideoBrush.RelativeTransform = CompositeTransform;
            VideoBrush.Stretch = Stretch.Fill;
            // IMPORTANT: You need to add a namespace Microsoft.Devices to be able to 
            VideoBrush.SetSource(PhotoCaptureDevice);
        }
        #endregion

        #region Properties
        public VideoBrush VideoBrush
        {
            get { return _videoBrush; }
            set
            {
                if (Equals(value, _videoBrush)) return;
                _videoBrush = value;
                OnPropertyChanged();
            }
        }

        public PhotoCaptureDevice PhotoCaptureDevice
        {
            get { return _photoCaptureDevice; }
            set
            {
                if (Equals(value, _photoCaptureDevice)) return;
                _photoCaptureDevice = value;
                OnPropertyChanged();
            }
        }

        public CompositeTransform CompositeTransform
        {
            get { return _compositeTransform; }
            set
            {
                if (Equals(value, _compositeTransform)) return;
                _compositeTransform = value;
                OnPropertyChanged();
            }
        }

        public Size CaptureResolution
        {
            get { return _captureResolution; }
            set
            {
                if (value.Equals(_captureResolution)) return;
                _captureResolution = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Result> Results
        {
            get { return _results; }
            set
            {
                if (Equals(value, _results)) return;
                _results = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INPC
        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

