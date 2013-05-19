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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsPhone8Demo.Annotations;
using WindowsPhone8Demo.Commands;
using ZXing;
using ZXing.Common;

namespace WindowsPhone8Demo.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        public ICommand PhotoChooserCommandOnClick { get; private set; }
        public ICommand CameraCaptureTaskOnClick { get; private set; }
        
        private bool _processing;
        private ObservableCollection<Result> _results;
        private string _activityMessage;
        private ObservableCollection<string> _barcodeFormats;
        private string _valueToBeEncoded;
        private ImageSource _imageSource;
        private string _selectedBarcodeFormat;

        public MainViewModel()
        {
            PhotoChooserCommandOnClick = new PhotoChooserCommand(this);
            CameraCaptureTaskOnClick = new CameraCaptureCommand(this);
            Results = new ObservableCollection<Result>();
            Processing = false;

            BarcodeFormats = new ObservableCollection<string>();
            var values = Enum.GetValues(typeof (BarcodeFormat));
            foreach (var value in values)
            {
                BarcodeFormats.Add(Enum.GetName(typeof (BarcodeFormat), value).ToString());
            }

            ValueToBeEncoded = "http://zxingnet.codeplex.com";
        }

        #region Methods
        public void BarcodeFormatOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Processing = true;
            try
            {
                var selectedBarcodeFormat = e.AddedItems[0];
                var format = new BarcodeFormat();
                BarcodeFormat.TryParse(selectedBarcodeFormat.ToString(), out format);

                IBarcodeWriter writer = new BarcodeWriter
                {
                    Format = format
                };
                var bmp = writer.Write(ValueToBeEncoded);
                ImageSource = bmp;
            }
            catch (Exception ex)
            {
                ActivityMessage = ex.Message;
            }
            Processing = false;
        }
        #endregion
        #region Properties
        public string SelectedBarcodeFormat
        {
            get { return _selectedBarcodeFormat; }
            set
            {
                if (value == _selectedBarcodeFormat) return;
                _selectedBarcodeFormat = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (Equals(value, _imageSource)) return;
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        public string ValueToBeEncoded
        {
            get { return _valueToBeEncoded; }
            set
            {
                if (value == _valueToBeEncoded) return;
                _valueToBeEncoded = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> BarcodeFormats
        {
            get { return _barcodeFormats; }
            set
            {
                if (Equals(value, _barcodeFormats)) return;
                _barcodeFormats = value;
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

        public bool Processing
        {
            get { return _processing; }
            set
            {
                if (value.Equals(_processing)) return;
                _processing = value;
                OnPropertyChanged();
            }
        }

        public string ActivityMessage
        {
            get { return _activityMessage; }
            set
            {
                if (value == _activityMessage) return;
                _activityMessage = value;
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