using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NetCoreDemo.Controllers
{
   public enum BarcodeFormats
   {
      AZTEC = 1,
      CODABAR = 2,
      CODE_39 = 4,
      CODE_93 = 8,
      CODE_128 = 16,
      DATA_MATRIX = 32,
      EAN_8 = 64,
      EAN_13 = 128,
      ITF = 256,
      MAXICODE = 512,
      PDF_417 = 1024,
      QR_CODE = 2048,
      UPC_A = 16384,
      UPC_E = 32768,
      MSI = 131072,
      PLESSEY = 262144,
      IMB = 524288,
   }
   public class HomeController : Controller
   {
      public IActionResult Index()
      {
         return View();
      }

      public IActionResult Error()
      {
         return View();
      }

      public IActionResult Encode()
      {
         if (this.Request.HasFormContentType &&
             this.Request.Form.ContainsKey("content"))
         {
            ViewData["BarcodeContent"] = this.Request.Form["content"];
            ViewData["BarcodeFormat"] = this.Request.Form["barcodeformat"];
            ViewData["OutputFormat"] = this.Request.Form["outputformat"];
            ViewData["BarcodeWidth"] = this.Request.Form["width"];
            ViewData["BarcodeHeight"] = this.Request.Form["height"];
            ViewData["BarcodeMargin"] = this.Request.Form["margin"];
            
            ViewData["Message"] = "Here is your barcode";
         }
         else
         {
            ViewData["Message"] = "Input content, create barcode...";
         }

         return View();
      }

      public IActionResult Decode()
      {
         ViewData["Message"] = "Your decoding page.";

         return View();
      }

      public IActionResult DecodeFiles()
      {
         ViewData["Message"] = "Your decoding page.";

         ViewData["TextAreaResult"] = "The result.";

         return View("Decode");
      }

      [HttpPost("DecodeFiles")]
      public async Task<IActionResult> DecodeFiles(ICollection<IFormFile> files)
      {
         ViewData["TextAreaResult"] = "No result.";

         foreach (var formFile in files)
         {
            if (formFile.Length > 0)
            {
               using (var stream = formFile.OpenReadStream())
               {
                  try
                  {
                     using (var image = ImageSharp.Image.Load(stream))
                     {
                        var reader = new ZXing.ImageSharp.BarcodeReader();
                        var result = reader.Decode(image);
                        ViewData["TextAreaResult"] = "ImageSharp: " + result?.Text;
                     }
                  }
                  catch (Exception exc)
                  {
                     ViewData["TextAreaResult"] = "ImageSharp: " + exc.Message;
                  }

                  try
                  {
                     stream.Seek(0, SeekOrigin.Begin);
                     using (var openCVImage = OpenCvSharp.Mat.FromStream(stream, OpenCvSharp.ImreadModes.GrayScale))
                     {
                        var openCVReader = new ZXing.OpenCV.BarcodeReader();
                        var openCVResult = openCVReader.Decode(openCVImage);
                        ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "OpenCV: " + openCVResult?.Text;
                     }
                  }
                  catch (Exception exc)
                  {
                     ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "OpenCV: " + exc.Message;
                  }

                  try
                  {
                     stream.Seek(0, SeekOrigin.Begin);
                     using (var coreCompatImage = (System.Drawing.Bitmap) System.Drawing.Bitmap.FromStream(stream))
                     {
                        var coreCompatReader = new ZXing.CoreCompat.System.Drawing.BarcodeReader();
                        var coreCompatResult = coreCompatReader.Decode(coreCompatImage);
                        ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "CoreCompat.System.Drawing: " + coreCompatResult?.Text;
                     }
                  }
                  catch (Exception exc)
                  {
                     ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "CoreCompat.System.Drawing: " + exc.Message;
                  }

                  try
                  {
                     stream.Seek(0, SeekOrigin.Begin);
                     using (var magickImage = new ImageMagick.MagickImage(stream))
                     {
                        var magickReader = new ZXing.Magick.BarcodeReader();
                        var magickResult = magickReader.Decode(magickImage);
                        ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "Magick: " + magickResult?.Text;
                     }
                  }
                  catch (Exception exc)
                  {
                     ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "Magick: " + exc.Message;
                  }

                  try
                  {
                     stream.Seek(0, SeekOrigin.Begin);
                     using (var skiaImage = SkiaSharp.SKBitmap.Decode(stream))
                     {
                        var skiaReader = new ZXing.SkiaSharp.BarcodeReader();
                        var skiaResult = skiaReader.Decode(skiaImage);
                        ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "SkiaSharp: " + skiaResult?.Text;
                     }
                  }
                  catch (Exception exc)
                  {
                     ViewData["TextAreaResult"] = ViewData["TextAreaResult"] + "SkiaSharp: " + exc.Message;
                  }
               }
            }
         }

         ViewData["Message"] = "Your decoding page.";

         return View("Decode");
      }
   }
}
