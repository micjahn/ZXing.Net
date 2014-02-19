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
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using ZXing;

namespace SimpleWindowsPhone8Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera _photoCamera;

        private BarcodeReader _reader;

        private PhotoCameraLuminanceSource _luminance;

        private ObservableCollection<string> _matches;

        private bool _isInitialized;

        public MainPage()
        {
            InitializeComponent();

            _matches = new ObservableCollection<string>();
            _matchesList.ItemsSource = _matches;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this._photoCamera = new PhotoCamera();
            _photoCamera.Initialized += OnPhotoCameraInitialized;
            _previewVideo.SetSource(_photoCamera);

            CameraButtons.ShutterKeyHalfPressed += (o, arg) => _photoCamera.Focus();

            _photoCamera.AutoFocusCompleted += PhotoCameraAutoFocusCompleted;
            
            base.OnNavigatedTo(e);
        }

        void PhotoCameraAutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            ScanPreviewBuffer();
        }
  
        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);

            this._luminance = new PhotoCameraLuminanceSource(width, height);
            this._reader = new BarcodeReader();
            _reader.Options.TryHarder = true;

            Dispatcher.BeginInvoke(() => _previewTransform.Rotation = _photoCamera.Orientation);

            _photoCamera.Resolution = _photoCamera.AvailableResolutions.First();

            _isInitialized = true;
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var result = _reader.Decode(_luminance);
                if (result != null)
                {
                    Dispatcher.BeginInvoke(() => DisplayResult(string.Format("{0} - {1}", result.BarcodeFormat, result.Text)));
                }
            }
            catch
            {
            }
        }

        private void DisplayResult(string text)
        {
            if (!_matches.Contains(text))
                _matches.Add(text);
        }

        private void PreviewRect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_isInitialized)
            {
                _photoCamera.Focus();
            }
        }
    }
}