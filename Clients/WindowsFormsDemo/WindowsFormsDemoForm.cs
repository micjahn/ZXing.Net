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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using WindowsFormsDemo.ImageFilters;

using ZXing;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.PDF417.Internal;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using Timer = System.Windows.Forms.Timer;

namespace WindowsFormsDemo
{
   public partial class WindowsFormsDemoForm : Form
   {
      private WebCam wCam;
      private Timer webCamTimer;
      private readonly BarcodeReader barcodeReader;
      private readonly IList<ResultPoint> resultPoints;
      private readonly IList<Result> lastResults;
      private EncodingOptions EncodingOptions { get; set; }
      private Type Renderer { get; set; }
      private bool TryMultipleBarcodes { get; set; }
      private bool TryOnlyMultipleQRCodes { get; set; }

      public WindowsFormsDemoForm()
      {
         InitializeComponent();
         barcodeReader = new BarcodeReader
         {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions {TryHarder = true}
         };
         barcodeReader.ResultPointFound += point =>
         {
            if (point == null)
               resultPoints.Clear();
            else
               resultPoints.Add(point);
         };
         barcodeReader.ResultFound += result =>
         {
            txtType.Text = result.BarcodeFormat.ToString();
            txtContent.Text += result.Text + Environment.NewLine;
            lastResults.Add(result);
            var parsedResult = ResultParser.parseResult(result);
            if (parsedResult != null)
            {
               btnExtendedResult.Visible = !(parsedResult is TextParsedResult);
                                               txtContent.Text += "\r\n\r\nParsed result:\r\n" + parsedResult.DisplayResult + Environment.NewLine + Environment.NewLine;
            }
            else
            {
               btnExtendedResult.Visible = false;
            }
         };
         resultPoints = new List<ResultPoint>();
         lastResults = new List<Result>();
         Renderer = typeof (BitmapRenderer);
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         foreach (var format in MultiFormatWriter.SupportedWriters)
            cmbEncoderType.Items.Add(format);
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void btnSelectBarcodeImageFileForDecoding_Click(object sender, EventArgs e)
      {
         using (var openDlg = new OpenFileDialog())
         {
            openDlg.FileName = txtBarcodeImageFile.Text;
            openDlg.Multiselect = false;
            openDlg.Filter = "PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|TIFF Files (*.tif)|*.tif|JPG Files (*.jpg)|*.jpg|PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
            if (openDlg.ShowDialog(this) == DialogResult.OK)
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
            MessageBox.Show(this, String.Format("File not found: {0}", fileName), "Error", MessageBoxButtons.OK,
               MessageBoxIcon.Error);
            return;
         }

         if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
         {
            if (TryOnlyMultipleQRCodes)
               Decode(PdfSupport.GetBitmapsFromPdf(fileName), TryMultipleBarcodes, new List<BarcodeFormat> { BarcodeFormat.QR_CODE });
            else
               Decode(PdfSupport.GetBitmapsFromPdf(fileName), TryMultipleBarcodes, null);
         }
         else
         {
            using (var bitmap = (Bitmap) Bitmap.FromFile(fileName))
            {
               if (TryOnlyMultipleQRCodes)
                  Decode(new[] { bitmap }, TryMultipleBarcodes, new List<BarcodeFormat> {BarcodeFormat.QR_CODE});
               else
                  Decode(new[] { bitmap }, TryMultipleBarcodes, null);
            }
         }
      }

      private void Decode(IEnumerable<Bitmap> bitmaps, bool tryMultipleBarcodes, IList<BarcodeFormat> possibleFormats)
      {
         resultPoints.Clear();
         lastResults.Clear();
         txtContent.Text = String.Empty;

         var timerStart = DateTime.Now.Ticks;
         IList<Result> results = null;
         var previousFormats = barcodeReader.Options.PossibleFormats;
         if (possibleFormats != null)
            barcodeReader.Options.PossibleFormats = possibleFormats;

         foreach (var bitmap in bitmaps)
         {
            if (tryMultipleBarcodes)
               results = barcodeReader.DecodeMultiple(bitmap);
            else
            {
               var result = barcodeReader.Decode(bitmap);
               if (result != null)
               {
                  if (results == null)
                  {
                     results = new List<Result>();
                  }
                  results.Add(result);
               }
            }
         }
         var timerStop = DateTime.Now.Ticks;

         barcodeReader.Options.PossibleFormats = previousFormats;

         if (results == null)
         {
            txtContent.Text = "No barcode recognized";
         }
         labDuration.Text = new TimeSpan(timerStop - timerStart).ToString();

         if (results != null)
         {
            foreach (var result in results)
            {
               if (result.ResultPoints.Length > 0)
               {
                  var offsetX = picBarcode.SizeMode == PictureBoxSizeMode.CenterImage
                     ? (picBarcode.Width - picBarcode.Image.Width)/2 :
                     0;
                  var offsetY = picBarcode.SizeMode == PictureBoxSizeMode.CenterImage
                     ? (picBarcode.Height - picBarcode.Image.Height) / 2 :
                     0;
                  var rect = new Rectangle((int)result.ResultPoints[0].X + offsetX, (int)result.ResultPoints[0].Y + offsetY, 1, 1);
                  foreach (var point in result.ResultPoints)
                  {
                     if (point.X + offsetX < rect.Left)
                        rect = new Rectangle((int)point.X + offsetX, rect.Y, rect.Width + rect.X - (int)point.X - offsetX, rect.Height);
                     if (point.X + offsetX > rect.Right)
                        rect = new Rectangle(rect.X, rect.Y, rect.Width + (int)point.X - (rect.X - offsetX), rect.Height);
                     if (point.Y + offsetY < rect.Top)
                        rect = new Rectangle(rect.X, (int)point.Y + offsetY, rect.Width, rect.Height + rect.Y - (int)point.Y - offsetY);
                     if (point.Y + offsetY > rect.Bottom)
                        rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)point.Y - rect.Y - offsetY);
                  }
                  using (var g = picBarcode.CreateGraphics())
                  {
                     g.DrawRectangle(Pens.Green, rect);
                  }
               }
            }
         }
      }

      private void txtBarcodeImageFile_TextChanged(object sender, EventArgs e)
      {
         var fileName = txtBarcodeImageFile.Text;
         if (File.Exists(fileName))
         {
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
               picBarcode.Image = null;
               foreach (var bitmap in PdfSupport.GetBitmapsFromPdf(fileName))
               {
                  picBarcode.Image = bitmap;
                  break;
               }
            }
            else
            {
               picBarcode.Load(fileName);
            }
         }
      }

      private void btnDecodeWebCam_Click(object sender, EventArgs e)
      {
         if (wCam == null)
         {
            wCam = new WebCam {Container = picWebCam};

            wCam.OpenConnection();

            webCamTimer = new Timer();
            webCamTimer.Tick += webCamTimer_Tick;
            webCamTimer.Interval = 200;
            webCamTimer.Start();
         }
         else
         {
            webCamTimer.Stop();
            webCamTimer = null;
            wCam.Dispose();
            wCam = null;
         }
      }

      void webCamTimer_Tick(object sender, EventArgs e)
      {
         var bitmap = wCam.GetCurrentImage();
         if (bitmap == null)
            return;
         var reader = new BarcodeReader();
         var result = reader.Decode(bitmap);
         if (result != null)
         {
            txtTypeWebCam.Text = result.BarcodeFormat.ToString();
            txtContentWebCam.Text = result.Text;
         }
      }

      private void btnEncode_Click(object sender, EventArgs e)
      {
         try
         {
            var writer = new BarcodeWriter
            {
               Format = (BarcodeFormat) cmbEncoderType.SelectedItem,
               Options = EncodingOptions ?? new EncodingOptions
                         {
                            Height = picEncodedBarCode.Height,
                            Width = picEncodedBarCode.Width
                         },
                  Renderer = (IBarcodeRenderer<Bitmap>)Activator.CreateInstance(Renderer)
            };
            picEncodedBarCode.Image = writer.Write(txtEncoderContent.Text);
         }
         catch (Exception exc)
         {
            MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void btnEncoderSave_Click(object sender, EventArgs e)
      {
         if (picEncodedBarCode.Image != null)
         {
            var fileName = String.Empty;
            using (var dlg = new SaveFileDialog())
            {
               dlg.DefaultExt = "png";
               dlg.Filter = "PNG Files (*.png)|*.png|SVG Files (*.svg)|*.svg|BMP Files (*.bmp)|*.bmp|TIFF Files (*.tif)|*.tif|JPG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";
               if (dlg.ShowDialog(this) != DialogResult.OK)
                  return;
               fileName = dlg.FileName;
            }
            var extension = Path.GetExtension(fileName).ToLower();
            var bmp = (Bitmap)picEncodedBarCode.Image;
            switch (extension)
            {
               case ".bmp":
                  bmp.Save(fileName, ImageFormat.Bmp);
                  break;
               case ".jpeg":
               case ".jpg":
                  bmp.Save(fileName, ImageFormat.Jpeg);
                  break;
               case ".tiff":
               case ".tif":
                  bmp.Save(fileName, ImageFormat.Tiff);
                  break;
               case ".svg":
               {
                  var writer = new BarcodeWriterSvg
                  {
                     Format = (BarcodeFormat) cmbEncoderType.SelectedItem,
                     Options = EncodingOptions ?? new EncodingOptions
                               {
                                  Height = picEncodedBarCode.Height,
                                  Width = picEncodedBarCode.Width
                               }
                  };
                  var svgImage = writer.Write(txtEncoderContent.Text);
                  File.WriteAllText(fileName, svgImage.Content, System.Text.Encoding.UTF8);
               }
                  break;
               default:
                  bmp.Save(fileName, ImageFormat.Png);
                  break;
            }
         }
      }

      private void btnEncodeDecode_Click(object sender, EventArgs e)
      {
         if (picEncodedBarCode.Image != null)
         {
            tabCtrlMain.SelectedTab = tabPageDecoder;
            picBarcode.Image = picEncodedBarCode.Image;
            var pureBarcodeSetting = barcodeReader.Options.PureBarcode;
            try
            {
               barcodeReader.Options.PureBarcode = true;
               Decode(new []{(Bitmap) picEncodedBarCode.Image}, false, null);
            }
            finally
            {
               barcodeReader.Options.PureBarcode = pureBarcodeSetting;
            }
         }
      }

      private void btnEncodeOptions_Click(object sender, EventArgs e)
      {
         if (cmbEncoderType.SelectedItem == null)
         {
            MessageBox.Show(this, "Please select a barcode format first.", "Error", MessageBoxButtons.OK,
               MessageBoxIcon.Error);
            return;
         }
         try
         {
            EncodingOptions options;
            switch ((BarcodeFormat) cmbEncoderType.SelectedItem)
            {
               case BarcodeFormat.QR_CODE:
                  options = EncodingOptions as ZXing.QrCode.QrCodeEncodingOptions ??
                            new ZXing.QrCode.QrCodeEncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width,
                               ErrorCorrection = ErrorCorrectionLevel.L
                            };
                  break;
               case BarcodeFormat.PDF_417:
                  options = EncodingOptions as ZXing.PDF417.PDF417EncodingOptions ??
                            new ZXing.PDF417.PDF417EncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width,
                               ErrorCorrection = PDF417ErrorCorrectionLevel.L0
                            };
                  break;
               case BarcodeFormat.DATA_MATRIX:
                  options = EncodingOptions as ZXing.Datamatrix.DatamatrixEncodingOptions ??
                            new ZXing.Datamatrix.DatamatrixEncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width,
                               SymbolShape = ZXing.Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE
                            };
                  break;
               case BarcodeFormat.AZTEC:
                  options = EncodingOptions as ZXing.Aztec.AztecEncodingOptions ??
                            new ZXing.Aztec.AztecEncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width,
                            };
                  break;
               case BarcodeFormat.CODE_128:
                  options = EncodingOptions as ZXing.OneD.Code128EncodingOptions ??
                            new ZXing.OneD.Code128EncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width,
                            };
                  break;
               default:
                  options = EncodingOptions ??
                            new EncodingOptions
                            {
                               Height = picEncodedBarCode.Height,
                               Width = picEncodedBarCode.Width
                            };
                  break;
            }
            var dlg = new EncodingOptionsForm
            {
               Options = options,
               Renderer = Renderer
            };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
               EncodingOptions = dlg.Options;
               Renderer = dlg.Renderer;
            }
         }
         catch (Exception exc)
         {
            MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void btnDecodingOptions_Click(object sender, EventArgs e)
      {
         using (var dlg = new DecodingOptionsForm(barcodeReader, TryMultipleBarcodes, TryOnlyMultipleQRCodes))
         {
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
               TryMultipleBarcodes = dlg.MultipleBarcodes;
               TryOnlyMultipleQRCodes = dlg.MultipleBarcodesOnlyQR;
            }
         }
      }

      private void btnExtendedResult_Click(object sender, EventArgs e)
      {
         if (lastResults.Count < 1)
            return;
         var parsedResult = ResultParser.parseResult(lastResults[0]);
         using (var dlg = new ExtendedResultForm())
         {
            dlg.Result = parsedResult;
            dlg.ShowDialog(this);
         }
      }

      private void btnScreenCapture_Click(object sender, EventArgs e)
      {
         try
         {
            Visible = false;
            Thread.Sleep(1000);
            picBarcode.Image = ScreenCapture.CaptureScreen();
            Visible = true;
            Decode(new []{(Bitmap)picBarcode.Image}, false, null);
         }
         catch (Exception exc)
         {
            MessageBox.Show(this, exc.ToString(), "Error by capturing", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         finally
         {
            Visible = true;
         }
      }

      private void chkImageScaling_CheckedChanged(object sender, EventArgs e)
      {
         picEncodedBarCode.SizeMode = chkImageScaling.Checked
            ? PictureBoxSizeMode.StretchImage
            : PictureBoxSizeMode.CenterImage;
      }

      private void chkScaleDecodingImage_CheckedChanged(object sender, EventArgs e)
      {
         picBarcode.SizeMode = chkScaleDecodingImage.Checked
            ? PictureBoxSizeMode.StretchImage
            : PictureBoxSizeMode.CenterImage;
      }

      private void button1_Click(object sender, EventArgs e)
      {
         Bitmap img = new Bitmap(picBarcode.Image);
         picBarcode.Image = MedianFilter.Filter(img, 3, 0, false);
      }
   }
}
