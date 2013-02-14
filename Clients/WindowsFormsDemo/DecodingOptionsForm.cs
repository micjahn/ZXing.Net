/*
 * Copyright 2013 ZXing.Net authors
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

using ZXing;

namespace WindowsFormsDemo
{
   public partial class DecodingOptionsForm : Form
   {
      private readonly BarcodeReader reader;

      public DecodingOptionsForm(BarcodeReader reader)
      {
         this.reader = reader;
         InitializeComponent();
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         chkTryInverted.Checked = reader.TryInverted;
         chkTryHarder.Checked = reader.TryHarder;
         chkAutoRotate.Checked = reader.AutoRotate;
         chkPureBarcode.Checked = reader.PureBarcode;
      }

      private void btnOk_Click(object sender, EventArgs e)
      {
         reader.TryInverted = chkTryInverted.Checked;
         reader.TryHarder = chkTryHarder.Checked;
         reader.AutoRotate = chkAutoRotate.Checked;
         reader.PureBarcode = chkPureBarcode.Checked;
         Close();
      }
   }
}
