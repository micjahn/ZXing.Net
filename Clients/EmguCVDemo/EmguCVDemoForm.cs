using System;
using System.Drawing;
using System.Windows.Forms;

using Emgu.CV;

using ZXing;

namespace EmguCVDemo
{
   public partial class EmguCVDemoForm : Form
   {
      Capture capture;
      bool Capturing;
      Bitmap bimap;
      private readonly IBarcodeReader reader;

      public EmguCVDemoForm()
      {
         InitializeComponent();
         reader = new BarcodeReader();
      }

      private void captureButton_Click(object sender, EventArgs e)
      {
         if (capture == null)
         {
            try
            {
               capture = new Capture();
            }
            catch (NullReferenceException exception)
            {
               MessageBox.Show(exception.Message);
            }
            catch (TypeInitializationException exc)
            {
               MessageBox.Show(
                  "Attention: You have to copy all the assemblies and native libraries from an official release of EmguCV to the directory of the demo." +
                  Environment.NewLine + Environment.NewLine + exc);
            }
         }

         if (capture != null)
         {
            if (Capturing)
            {
               captureButton.Text = "Start Capturing";
               Application.Idle -= DoDecoding;
            }
            else
            {
               captureButton.Text = "Stop Capturing";
               Application.Idle += DoDecoding;
            }
            Capturing = !Capturing;
         }
      }

      protected override void OnClosed(EventArgs e)
      {
         if (capture != null)
            capture.Dispose();
      }

      private void DoDecoding(object sender, EventArgs args)
      {
         var timerStart = DateTime.Now.Ticks;

         var image = capture.QueryFrame();
         if (image != null)
         {
            bimap = image.ToBitmap();
            pictureBox1.Image = bimap;
            var result = reader.Decode(bimap);
            if (result != null)
            {
               txtContentWebCam.Text = result.Text;
               txtTypeWebCam.Text = result.BarcodeFormat.ToString();
            }
         }
         var timerStop = DateTime.Now.Ticks;
         labDuration.Text = new TimeSpan(timerStop - timerStart).Milliseconds.ToString("0 ms");
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         Close();
      }
   }
}
