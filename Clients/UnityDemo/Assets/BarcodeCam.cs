using System.Threading;
using UnityDemo;
using UnityEngine;

using ZXing;

public class BarcodeCam : MonoBehaviour
{
   private WebCamTexture camTexture;
   private Thread qrThread;

   private Color32[] c;
   private byte[] dummy = new byte[0];
   private int W, H;
      
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

         try
         {
            // data for decoding is injected via Color32LuminanceSource above
            var result = barcodeReader.Decode(dummy, W, H);
            if (result != null)
               print(result.Text);
         }
         catch
         {
         }
      }
   }
}
