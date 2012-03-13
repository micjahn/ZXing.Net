using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ZXing;

namespace SilverlightDemo
{
   public partial class MainPage : UserControl
   {
      private readonly IBarcodeReader reader = new BarcodeReader();
      private WriteableBitmap currentBarcode;

      public MainPage()
      {
         InitializeComponent();

         foreach (var format in MultiFormatWriter.SupportedWriters)
            cmbEncoderType.Items.Add(format);
         cmbEncoderType.SelectedIndex = 0;
      }

      private void btnDecoderOpen_Click(object sender, RoutedEventArgs e)
      {
         var dlg = new OpenFileDialog();
         dlg.Filter = "PNG (*.png)|*.png";
         if (dlg.ShowDialog().GetValueOrDefault(false))
         {
            try
            {
               currentBarcode = new WriteableBitmap(0, 0);
               using (var stream = dlg.File.OpenRead())
               {
                  currentBarcode.SetSource(stream);
               }
               imgDecoderBarcode.Source = currentBarcode;
            }
            catch (Exception exc)
            {
               txtDecoderContent.Text = exc.Message;
            }
         }
      }

      private void btnDecoderDecode_Click(object sender, RoutedEventArgs e)
      {
         if (currentBarcode == null)
            return;

         var result = reader.Decode(currentBarcode);
         if (result != null)
         {
            txtDecoderType.Text = result.BarcodeFormat.ToString();
            txtDecoderContent.Text = result.Text;
         }
         else
         {
            txtDecoderType.Text = String.Empty;
            txtDecoderContent.Text = "No barcode found.";
         }
      }

      private void btnEncoderEncode_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            var encoder = new MultiFormatWriter();
            var bitMatrix = encoder.encode(txtEncoderContent.Text, (BarcodeFormat)cmbEncoderType.SelectedItem, (int)imgEncoderBarcode.Width, (int)imgEncoderBarcode.Height);
            imgEncoderBarcode.Source = bitMatrix.ToBitmap((BarcodeFormat)cmbEncoderType.SelectedItem, txtEncoderContent.Text);
         }
         catch (Exception exc)
         {
            MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK);
         }
      }
   }
}
