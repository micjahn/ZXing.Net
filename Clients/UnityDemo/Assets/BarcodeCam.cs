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

using System.Threading;

using UnityEngine;

using ZXing;
using ZXing.QrCode;

public class BarcodeCam : MonoBehaviour
{
   // Texture for encoding test
   public Texture2D encoded;

   private WebCamTexture camTexture;
   private Thread qrThread;

   private Color32[] c;
   private int W, H;
      
   private Rect screenRect;

   private bool isQuit;

   public string LastResult;
   private bool shouldEncodeNow;

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
      encoded = new Texture2D(256, 256);
      LastResult = "http://www.google.com";
      shouldEncodeNow = true;

      screenRect = new Rect(0, 0, Screen.width, Screen.height);

      camTexture = new WebCamTexture();
      camTexture.requestedHeight = Screen.height; // 480;
      camTexture.requestedWidth = Screen.width; //640;
      OnEnable();

      qrThread = new Thread(DecodeQR);
      qrThread.Start();
   }

   void Update()
   {
      if (c == null)
      {
         c = camTexture.GetPixels32();
      }

      // encode the last found
      var textForEncoding = LastResult;
      if (shouldEncodeNow && 
          textForEncoding != null)
      {
         var color32 = Encode(textForEncoding, encoded.width, encoded.height);
         encoded.SetPixels32(color32);
         encoded.Apply();
         shouldEncodeNow = false;
      }
   }

   void DecodeQR()
   {
      // create a reader with a custom luminance source
      var barcodeReader = new BarcodeReader {AutoRotate = false, TryHarder = false};

      while (true)
      {
         if (isQuit)
            break;

         try
         {
            // decode the current frame
            var result = barcodeReader.Decode(c, W, H);
            if (result != null)
            {
               LastResult = result.Text;
               shouldEncodeNow = true;
               print(result.Text);
            }

            // Sleep a little bit and set the signal to get the next frame
            Thread.Sleep(200);
            c = null;
         }
         catch
         {
         }
      }
   }

   private static Color32[] Encode(string textForEncoding, int width, int height)
   {
      var writer = new BarcodeWriter
      {
         Format = BarcodeFormat.QR_CODE,
         Options = new QrCodeEncodingOptions
         {
            Height = height,
            Width = width
         }
      };
      return writer.Write(textForEncoding);
   }
}
