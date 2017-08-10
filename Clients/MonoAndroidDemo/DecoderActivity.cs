
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using ZXing;

namespace MonoAndroidDemo
{
   [Activity (Label = "Read:")]			
   public class DecoderActivity : Activity
   {
      public static BarcodeFormat CurrentFormat = BarcodeFormat.QR_CODE;

      Button buttonLoadSample;
      Button buttonChooseExisting;
      Button buttonTakeNew;
      ImageView imageViewBarcode;
      TextView textViewResults;

      Java.IO.File _file;

      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);

         this.SetContentView(Resource.Layout.DecoderActivityLayout);

         this.Title = "Read: " + CurrentFormat.ToString();
         
         this.buttonLoadSample = this.FindViewById<Button>(Resource.Id.buttonDecoderAppSample);
         this.buttonChooseExisting = this.FindViewById<Button>(Resource.Id.buttonDecoderChooseExisting);
         this.buttonTakeNew = this.FindViewById<Button>(Resource.Id.buttonDecoderTakeNew);
         this.imageViewBarcode = this.FindViewById<ImageView>(Resource.Id.imageViewDecoderBarcode);
         this.textViewResults = this.FindViewById<TextView>(Resource.Id.textViewDecoderResult);


         this.buttonLoadSample.Click += (sender, e) => {
            int r = 0;
            switch (CurrentFormat.ToString().ToLower())
            {
            case "aztec":
               r = Resource.Drawable.aztec;
               break;
            case "codabar":
               r = Resource.Drawable.codabar;
               break;
            case "code_128":
               r = Resource.Drawable.code_128;
               break;
            case "code_39":
               r = Resource.Drawable.code_39;
               break;
            case "code_93":
               r = Resource.Drawable.code_93;
               break;
            case "data_matrix":
               r = Resource.Drawable.data_matrix;
               break;
            case "ean_13":
               r = Resource.Drawable.ean_13;
               break;
            case "ean_8":
               r = Resource.Drawable.ean_8;
               break;
            case "itf":
               r = Resource.Drawable.itf;
               break;
            case "pdf_417":
               r = Resource.Drawable.pdf_417;
               break;
            case "qr_code":
               r = Resource.Drawable.qr_code;
               break;
            case "upc_a":
               r = Resource.Drawable.upc_a;
               break;
            case "upc_e":
               r = Resource.Drawable.upc_e;
               break;
            default: r = Resource.Drawable.aztec;
               break;
            }

            var d = Resources.GetDrawable(r);
            var bmp = ((Android.Graphics.Drawables.BitmapDrawable)d).Bitmap;

            this.imageViewBarcode.SetImageBitmap(bmp);
            Decode (bmp);
         };

         this.buttonChooseExisting.Click += (sender, e) => {
            var imageIntent = new Intent ();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser (imageIntent, "Choose an Existing Barcode Photo"), 101);
         };

         this.buttonTakeNew.Click += (sender, e) => {
            var intent = new Intent (Android.Provider.MediaStore.ActionImageCapture);

            var availableActivities = this.PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            
            if (availableActivities != null && availableActivities.Count > 0) 
            {              
               var dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "Barcode Reader");     
               
               if (!dir.Exists ())
                  dir.Mkdirs ();  
            
               _file = new Java.IO.File (dir, String.Format ("zxingreader{0}.jpg", Guid.NewGuid ()));     
               
               intent.PutExtra (Android.Provider.MediaStore.ExtraOutput, Android.Net.Uri.FromFile (_file));                    
               StartActivityForResult (intent, 102);
            }
         };
      }

      protected override void OnActivityResult (int requestCode, Android.App.Result resultCode, Android.Content.Intent data)
      {
         base.OnActivityResult (requestCode, resultCode, data);

         if (resultCode == Android.App.Result.Ok) {

            if (requestCode == 101)
            {
               //Choose existing
               var bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);
               this.imageViewBarcode.SetImageBitmap(bitmap);
               Decode (bitmap);
            }
            else if (requestCode == 102)
            {
               //Take New picture

               // make it available in the gallery
               var mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
               var contentUri = Android.Net.Uri.FromFile(_file);
               mediaScanIntent.SetData (contentUri);
               this.SendBroadcast (mediaScanIntent);             
               
               // display in ImageView
               var bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap (ContentResolver, contentUri);
               this.imageViewBarcode.SetImageBitmap (bitmap);   

               Decode (bitmap);
            }
         }
      }

      void Decode (Android.Graphics.Bitmap image)
      {
         try 
         {
            var reader = new BarcodeReader ();
            reader.Options.PossibleFormats = new List<BarcodeFormat> () { CurrentFormat };
            reader.Options.TryHarder = true;
            var result = reader.Decode (image);
            if (result != null)
            {
               this.textViewResults.Text = "Result: " + result.Text;
            }
            else
            {
               this.textViewResults.Text = "No barcode found";
            }
         } catch (Exception ex) 
         {
            this.textViewResults.Text = ex.ToString();
         }
      }
   }
}

