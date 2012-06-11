using System.Threading;

using UnityEngine;

using ZXing;

namespace UnityDemo
{
   public class BarcodeCam : MonoBehaviour
   {
      private WebCamTexture camTexture;
      private Thread qrThread;

      private Color32[] c;
      private int W, H, WxH;
      
      private Rect screenRect;

      private bool isQuit;

      void OnGUI()
      {
         GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
      }

      void OnEnable()
      {
         if (camTexture != null)
         {
            camTexture.Play();
            W = camTexture.width;
            H = camTexture.height;
            WxH = W * H;
         }
      }

      void OnDisable()
      {
         if (camTexture != null)
         {
            camTexture.Pause();
         }
      }

      void OnDestroy()
      {
         qrThread.Abort();
         camTexture.Stop();
      }

      // It's better to stop the thread by itself rather than abort it.
      void OnApplicationQuit()
      {
         isQuit = true;
      }

      void Start()
      {
         screenRect = new Rect(0, 0, Screen.width, Screen.height);

         camTexture = new WebCamTexture();
         OnEnable();

         qrThread = new Thread(DecodeQR);
         qrThread.Start();
      }

      void Update()
      {
         c = camTexture.GetPixels32();
      }

      void DecodeQR()
      {
         // create a reader with a custom luminance source
         var barcodeReader = new BarcodeReader(
            null,
            (rawRGB, width, height) => new Color32LuminanceSource(c, width, height),
            null);

         while (true)
         {
            if (isQuit)
               break;

            // data for decoding is injected via Color32LuminanceSource above
            var result = barcodeReader.Decode(null, W, H);
            if (result != null)
               print(result.Text);
         }
      }
   }
}
