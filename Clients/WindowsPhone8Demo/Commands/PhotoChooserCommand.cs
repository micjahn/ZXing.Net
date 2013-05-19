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

using Microsoft.Devices;
using Microsoft.Phone.Tasks;
using System;
using System.Windows.Input;
using WindowsPhone8Demo.Extensions;
using WindowsPhone8Demo.ViewModels;

namespace WindowsPhone8Demo.Commands
{
    public class PhotoChooserCommand : ICommand
    {
        private readonly PhotoChooserTask _photoChooserTask;
        private AsyncPictureDecoderExtension _asyncPictureDecoder;
        private readonly MainViewModel _viewModel;
        
        public PhotoChooserCommand(object parameter)
        {
            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += PhotoChooserTaskOnCompleted;

            _viewModel = (MainViewModel) parameter;
        }

        private void PhotoChooserTaskOnCompleted(object sender, PhotoResult photoResult)
        {
            _asyncPictureDecoder = new AsyncPictureDecoderExtension(_viewModel, photoResult);
        }

        // ICommand Implementation
        public bool CanExecute(object parameter)
        {
            // Determine if Camera is supported on the device.
            return Camera.IsCameraTypeSupported(CameraType.Primary) || Camera.IsCameraTypeSupported(CameraType.FrontFacing);
        }

        public void Execute(object parameter)
        {
            _viewModel.ActivityMessage = string.Empty;
            _viewModel.Processing = true;

            _photoChooserTask.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
