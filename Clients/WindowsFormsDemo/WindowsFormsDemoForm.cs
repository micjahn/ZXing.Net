using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ZXing;
using ZXing.Common;
using System.Drawing.Imaging;

namespace WindowsFormsDemo
{
   public partial class WindowsFormsDemoForm : Form
   {
      private WebCam wCam;
      private Timer webCamTimer;
      private readonly IBarcodeReader barcodeReader;
      private IList<ResultPoint> resultPoints;

      public WindowsFormsDemoForm()
      {
         InitializeComponent();
         barcodeReader = new BarcodeReader();
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
                                            txtContent.Text = result.Text;
                                         };
         resultPoints = new List<ResultPoint>();
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

         Decode((Bitmap) Bitmap.FromFile(fileName));
      }

      private void Decode(Bitmap image)
      {
         var timerStart = DateTime.Now.Ticks;

         barcodeReader.TryHarder = false;
         resultPoints.Clear();
         var result = barcodeReader.Decode(image);
         if (result == null)
         {
            barcodeReader.TryHarder = true;
            result = barcodeReader.Decode(image);
         }
         var timerStop = DateTime.Now.Ticks;
         if (result == null)
         {
            txtContent.Text = "No barcode recognized";
         }
         labDuration.Text = new TimeSpan(timerStop - timerStart).Milliseconds.ToString("0 ms");

         if (result != null && resultPoints.Count > 0)
         {
            var rect = new Rectangle((int)resultPoints[0].X, (int)resultPoints[0].Y, 1, 1);
            foreach (var point in resultPoints)
            {
               if (point.X < rect.Left)
                  rect = new Rectangle((int)point.X, rect.Y, rect.Width + rect.X - (int)point.X, rect.Height);
               if (point.X > rect.Right)
                  rect = new Rectangle(rect.X, rect.Y, rect.Width + (int)point.X - rect.X, rect.Height);
               if (point.Y < rect.Top)
                  rect = new Rectangle(rect.X, (int)point.Y, rect.Width, rect.Height + rect.Y - (int)point.Y);
               if (point.Y > rect.Bottom)
                  rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)point.Y - rect.Y);
            }
            using (var g = picBarcode.CreateGraphics())
            {
               g.DrawRectangle(Pens.Green, rect);
            }
         }
      }

      private void txtBarcodeImageFile_TextChanged(object sender, EventArgs e)
      {
         var fileName = txtBarcodeImageFile.Text;
         if (File.Exists(fileName))
            picBarcode.Load(fileName);
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
         var imageSource = new RGBLuminanceSource(bitmap);
         var binarizer = new HybridBinarizer(imageSource);
         var binaryBitmap = new BinaryBitmap(binarizer);
         var reader = new MultiFormatReader();
         var result = reader.decode(binaryBitmap);
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
            var encoder = new MultiFormatWriter();
            var bitMatrix = encoder.encode(txtEncoderContent.Text, (BarcodeFormat)cmbEncoderType.SelectedItem, picEncodedBarCode.Width, picEncodedBarCode.Height);
            picEncodedBarCode.Image = bitMatrix.ToBitmap((BarcodeFormat)cmbEncoderType.SelectedItem, txtEncoderContent.Text);
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
               dlg.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
               if (dlg.ShowDialog(this) != DialogResult.OK)
                  return;
               fileName = dlg.FileName;
            }
            var bmp = (Bitmap) picEncodedBarCode.Image;
            bmp.Save(fileName, ImageFormat.Png);
         }
      }

      private void btnEncodeDecode_Click(object sender, EventArgs e)
      {
         if (picEncodedBarCode.Image != null)
         {
            tabCtrlMain.SelectedTab = tabPageDecoder;
            picBarcode.Image = picEncodedBarCode.Image;
            Decode((Bitmap)picEncodedBarCode.Image);
         }
      }
   }
}
