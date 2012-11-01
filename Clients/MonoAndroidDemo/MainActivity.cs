using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using ZXing;

namespace MonoAndroidDemo
{
	[Activity (Label = "Zxing.Net Demo", MainLauncher = true)]
	public class Activity1 : ListActivity
	{
		ArrayAdapter<string> adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var readers = new BarcodeFormat[] { 
				BarcodeFormat.AZTEC, BarcodeFormat.CODABAR, BarcodeFormat.CODE_128, BarcodeFormat.CODE_39,
				BarcodeFormat.CODE_93, BarcodeFormat.DATA_MATRIX, BarcodeFormat.EAN_13, BarcodeFormat.EAN_8,
				BarcodeFormat.ITF, BarcodeFormat.MAXICODE, BarcodeFormat.PDF_417, BarcodeFormat.QR_CODE,
				BarcodeFormat.RSS_14, BarcodeFormat.RSS_EXPANDED, BarcodeFormat.UPC_A, BarcodeFormat.UPC_E,
				BarcodeFormat.UPC_EAN_EXTENSION
			};

			var writers = new BarcodeFormat[] { 
				BarcodeFormat.UPC_A, BarcodeFormat.EAN_8, BarcodeFormat.EAN_13, BarcodeFormat.CODE_39,
				BarcodeFormat.CODE_128, BarcodeFormat.ITF, BarcodeFormat.CODABAR,
				BarcodeFormat.QR_CODE, BarcodeFormat.PDF_417
			};


			this.adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, new string[] { "Barcode Reading", "Barcode Writing" });

			this.ListAdapter = adapter;

			this.ListView.ItemClick += (sender, e) => {
				if (e.Position == 0)
				{
					FormatSelectionActivity.CurrentFormats = readers;
					FormatSelectionActivity.CurrentReadOnly = true;
					StartActivity(typeof(FormatSelectionActivity));
				}
				else
				{
					FormatSelectionActivity.CurrentFormats = writers;
					FormatSelectionActivity.CurrentReadOnly = false;
					StartActivity(typeof(FormatSelectionActivity));
				}
			};
		}
	}
}


