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

using System.ComponentModel;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using WindowsPhone8Demo.Models;
using WindowsPhone8Demo.ViewModels;
using ZXing;

namespace WindowsPhone8Demo.Extensions
{
    public class AsyncPictureDecoderExtension
    {
        private BackgroundWorker _worker;
        private readonly MainViewModel _viewModel;
        private BitmapImage _image;
        private BarcodeReader _reader;

        public AsyncPictureDecoderExtension(MainViewModel viewModel, PhotoResult photoResult)
        {
            _viewModel = viewModel;

            InitializeDecoder(photoResult);
        }

        private void InitializeDecoder(PhotoResult photoResult)
        {
            _image = new BitmapImage();

            _reader = new BarcodeReader();

            _worker = new BackgroundWorker();
            _worker.DoWork += WorkerOnDoWork;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;

            TryGetPhoto(photoResult);
        }

        private void TryGetPhoto(PhotoResult photoResult)
        {
            if (photoResult.TaskResult == TaskResult.OK)
            {
                _reader.Options.TryHarder = false;
                _reader.TryInverted = false;
                _reader.AutoRotate = false;

                _image.SetSource(photoResult.ChosenPhoto);
                _worker.RunWorkerAsync(new WriteableBitmap(_image));
            }
            else
            {
                SetActivityMessage(photoResult.TaskResult.ToString(), false);
            }
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            doWorkEventArgs.Result = _reader.Decode((WriteableBitmap)doWorkEventArgs.Argument);
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Cancelled)
            {
                SetActivityMessage("Canceled", false);
            }
            else if (runWorkerCompletedEventArgs.Error != null)
            {
                SetActivityMessage(runWorkerCompletedEventArgs.Error.Message, false);
            }
            else
            {
                var result = (Result)runWorkerCompletedEventArgs.Result;
                if (result != null)
                {
                    AddBarcodeToResults(result);
                }
                else
                {
                    if (!_reader.Options.TryHarder)
                    {
                        SetActivityMessage("TryHarder", true);
                        _reader.Options.TryHarder = true;
                        _worker.RunWorkerAsync(new WriteableBitmap(_image));
                    }
                    else if (_reader.Options.TryHarder && !_reader.AutoRotate)
                    {
                        SetActivityMessage(".TryHarder & .AutoRotate", true);
                        _reader.AutoRotate = true;
                        _worker.RunWorkerAsync(new WriteableBitmap(_image));
                    }
                    else if (_reader.Options.TryHarder && _reader.AutoRotate)
                    {
                        SetActivityMessage(".TryHarder & .AutoRotate & .TryInverted", true);
                        _reader.TryInverted = true;
                        _worker.RunWorkerAsync(new WriteableBitmap(_image));
                    }
                    else
                    {
                        SetActivityMessage("No barcode detected", false);
                    }
                }
            }
        }

        private void AddBarcodeToResults(Result result)
        {
            SetActivityMessage("Barcode decoded", false);
            BarcodeResult.AddToResultCollection(result, _viewModel);
            
        }

        private void SetActivityMessage(string msg, bool processing)
        {
            _viewModel.ActivityMessage = msg;
            _viewModel.Processing = processing;
        }
    }
}