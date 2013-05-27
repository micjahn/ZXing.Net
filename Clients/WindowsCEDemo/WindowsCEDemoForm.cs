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

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ZXing;
using ZXing.Common;

namespace WindowsCEDemoForm
{
   public partial class WindowsCEDemoForm : Form
   {
      private readonly IBarcodeReader barcodeReader;

      public WindowsCEDemoForm()
      {
         InitializeComponent();

         barcodeReader = new BarcodeReader
            {
               AutoRotate = true,
               Options = new DecodingOptions
                  {
                     TryHarder = false,
                     PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.QR_CODE}
                  }
            };
      }

      private void btnSelectBarcodeImageFileForDecoding_Click(object sender, EventArgs e)
      {
         using (var openDlg = new OpenFileDialog())
         {
            openDlg.FileName = txtBarcodeImageFile.Text;
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
               txtBarcodeImageFile.Text = openDlg.FileName;
            }
         }
      }

      private void btnStartDecoding_Click(object sender, EventArgs e)
      {
         var fileName = txtBarcodeImageFile.Text;
         if (!File.Exists(fileName))
         {
            MessageBox.Show(String.Format("File not found:{0}", fileName), "Error",
               MessageBoxButtons.OK,
               MessageBoxIcon.Asterisk,
               MessageBoxDefaultButton.Button1);
            return;
         }
         using (Bitmap bitmp = new Bitmap(fileName))
         {
            Decode(bitmp);
         }
      }

      private void Decode(Bitmap image)
      {
         var timerStart = DateTime.Now;

         var result = barcodeReader.Decode(image);

         var timerStop = DateTime.Now;
         labDuration.Text = (timerStop - timerStart).TotalMilliseconds.ToString();/// ("0 ms");

         if (result == null)
         {
            txtContent.Text = "No barcode recognized";
         }
         else
         {
            txtType.Text = result.BarcodeFormat.ToString();
            txtContent.Text = result.Text;
         }
      }

      private Bitmap Encode(string text, BarcodeFormat format)
      {
         var writer = new BarcodeWriter { Format = format };
         return writer.Write(text);
      }

      private void txtBarcodeImageFile_TextChanged(object sender, EventArgs e)
      {
         var fileName = txtBarcodeImageFile.Text;
         if (File.Exists(fileName))
         {
            Image img = new Bitmap(fileName);
            picBarcode.Image = img;
         }
      }

      private void btnStartEncoding_Click(object sender, EventArgs e)
      {
         var format = BarcodeFormat.QR_CODE;
         if (!String.IsNullOrEmpty(txtType.Text))
         {
            try
            {
               format = (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), txtType.Text, true);
            }
            catch
            {
               txtType.Text = format.ToString();
            }
         }
         var bitmap = Encode(txtContent.Text, format);
         picBarcode.Image = bitmap;
      }
   }
}