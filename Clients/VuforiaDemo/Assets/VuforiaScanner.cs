//
// based upon the following solution, modified to work with ZXing.Net
// http://whydoidoit.com/2012/07/18/unity-vuforia-zxing-barcode-scanning-in-ar-games/
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using ZXing;
using ZXing.Client.Result;

[AddComponentMenu("System/VuforiaScanner")]
public class VuforiaScanner : MonoBehaviour
{
   public QCARBehaviour vuforia;
   public GameObject target;

   public static event Action<ParsedResult, string> Scanned = delegate { };
   public static event Action<string> ScannedQRCode = delegate { };
   public static event Action<string> ScannedBarCode = delegate { };

   bool decoding;
   bool init;
   BarcodeReader barcodeReader = new BarcodeReader();

   void Update()
   {
      if (vuforia == null)
         return;
      if (vuforia.enabled && !init)
      {
         //Wait 1/4 seconds for the device to initialize (otherwise it seems to crash sometimes)
         init = true;
         Loom.QueueOnMainThread(() =>
         {
            init = CameraDevice.Instance.SetFrameFormat(Image.PIXEL_FORMAT.RGB888, true);
         }, 0.25f);
      }
      if (vuforia.enabled && CameraDevice.Instance != null && !decoding)
      {
         var image = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.RGB888);
         if (image != null)
         {
            decoding = true;

            Loom.RunAsync(() =>
            {
               try
               {
                  var data = barcodeReader.Decode(image.Pixels, image.BufferWidth, image.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                  if (data != null)
                  {
                     Loom.QueueOnMainThread(() =>
                     {
                        if (data.BarcodeFormat == BarcodeFormat.QR_CODE)
                        {
                           ScannedQRCode(data.Text);
                           if (target != null)
                           {
                              target.SendMessage("ScannedQRCode", data.Text, SendMessageOptions.DontRequireReceiver);
                           }
                        }
                        if (data.BarcodeFormat != BarcodeFormat.QR_CODE)
                        {
                           ScannedBarCode(data.Text);
                           if (target != null)
                           {
                              target.SendMessage("ScannedBarCode", data.Text, SendMessageOptions.DontRequireReceiver);
                           }

                        }
                        var parsedResult = ResultParser.parseResult(data);
                        if (target != null)
                        {
                           target.SendMessage("Scanned", parsedResult, SendMessageOptions.DontRequireReceiver);
                        }
                        Scanned(parsedResult, data.Text);
                     });
                  }
               }
               finally
               {
                  decoding = false;
               }
            });
         }
      }
   }
}