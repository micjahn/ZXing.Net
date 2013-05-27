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
      public bool MultipleBarcodes
      {
         get { return chkMultipleDecode.Checked; }
      }
      public bool MultipleBarcodesOnlyQR
      {
         get { return chkMultipleDecodeOnlyQR.Checked; }
      }

      public DecodingOptionsForm(BarcodeReader reader, bool multipleBarcodes, bool multipleBarcodesOnlyQR)
      {
         this.reader = reader;
         InitializeComponent();

         chkMultipleDecode.Checked = multipleBarcodes;
         chkMultipleDecodeOnlyQR.Checked = multipleBarcodesOnlyQR;
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         chkTryInverted.Checked = reader.TryInverted;
         chkTryHarder.Checked = reader.Options.TryHarder;
         chkAutoRotate.Checked = reader.AutoRotate;
         chkPureBarcode.Checked = reader.Options.PureBarcode;
      }

      private void btnOk_Click(object sender, EventArgs e)
      {
         reader.TryInverted = chkTryInverted.Checked;
         reader.Options.TryHarder = chkTryHarder.Checked;
         reader.AutoRotate = chkAutoRotate.Checked;
         reader.Options.PureBarcode = chkPureBarcode.Checked;
         Close();
      }
   }
}
