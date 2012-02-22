using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.google.zxing;
using com.google.zxing.common;

namespace WindowsFormsDemo
{
   public partial class WindowsFormsDemoForm : Form
   {
      public WindowsFormsDemoForm()
      {
         InitializeComponent();
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
            MessageBox.Show(this, String.Format("File not found: {0}", fileName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         var timerStart = DateTime.Now.Ticks;
         var image = (Bitmap)Bitmap.FromFile(fileName);
         var imageSource = new RGBLuminanceSource(image, image.Width, image.Height);
         var binarizer = new HybridBinarizer(imageSource);
         var binaryBitmap = new BinaryBitmap(binarizer);
         var reader = new MultiFormatReader();
         var result = reader.decode(binaryBitmap);
         var timerStop = DateTime.Now.Ticks;
         if (result == null)
         {
            txtContent.Text = "No barcode recognized";
         }
         else
         {
            txtType.Text = result.BarcodeFormat.ToString();
            txtContent.Text = result.Text;
         }
         labDuration.Text = new TimeSpan(timerStop - timerStart).Milliseconds.ToString("0 ms");
      }

      private void txtBarcodeImageFile_TextChanged(object sender, EventArgs e)
      {
         var fileName = txtBarcodeImageFile.Text;
         if (File.Exists(fileName))
            picBarcode.Load(fileName);
      }
   }
}
