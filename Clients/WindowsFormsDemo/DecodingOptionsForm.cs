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
using System.Collections.Generic;
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

         foreach (var val in Enum.GetValues(typeof (BarcodeFormat)))
         {
            var valBarcode = (BarcodeFormat) val;
            if (valBarcode == BarcodeFormat.PLESSEY)
               continue;
            var selectedByDefault = valBarcode != BarcodeFormat.MSI &&
                                    valBarcode != BarcodeFormat.IMB;
            if (reader.Options.PossibleFormats != null)
            {
               selectedByDefault = reader.Options.PossibleFormats.Contains(valBarcode);
            }
            dataGridViewBarcodeFormats.Rows.Add(selectedByDefault, val.ToString());
         }
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
         reader.Options.PossibleFormats = new List<BarcodeFormat>();

         foreach (DataGridViewRow row in dataGridViewBarcodeFormats.Rows)
         {
            if (((bool) (row.Cells[0].Value)))
            {
               reader.Options.PossibleFormats.Add(
                  (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), row.Cells[1].Value.ToString()));
            }
         }
         
         Close();
      }
   }
}
