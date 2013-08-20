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
using System.Windows.Forms;

using ZXing.Common;
using ZXing.Rendering;

namespace WindowsFormsDemo
{
   public partial class EncodingOptionsForm : Form
   {
      public EncodingOptions Options
      {
         get
         {
            return (EncodingOptions)propOptions.SelectedObject;
         }
         set
         {
            propOptions.SelectedObject = value;
         }
      }

      public Type Renderer
      {
         get { return (Type)cmbRenderer.SelectedItem; }
         set { cmbRenderer.SelectedItem = value; }
      }

      public EncodingOptionsForm()
      {
         InitializeComponent();

         cmbRenderer.Items.Add(typeof (BitmapRenderer));
         cmbRenderer.Items.Add(typeof (CustomBitmapRenderer));
         cmbRenderer.SelectedItem = typeof (BitmapRenderer);
      }
   }
}
