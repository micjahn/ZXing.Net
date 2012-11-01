
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
	[Activity (Label = "Barcode")]			
	public class FormatSelectionActivity : ListActivity
	{
		public static BarcodeFormat[] CurrentFormats= null;
		public static bool CurrentReadOnly = true;

		BarcodeFormatListAdapter adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.Title = CurrentReadOnly ? "Barcode Reading" : "Barcode Writing";

			adapter = new BarcodeFormatListAdapter(this, CurrentFormats);

			this.ListAdapter = adapter;

			this.ListView.ItemClick += (sender, e) => {

				if (CurrentReadOnly)
				{
					DecoderActivity.CurrentFormat = adapter[e.Position];
					StartActivity(typeof(DecoderActivity));
				}
				else
				{
					EncoderActivity.CurrentFormat = adapter[e.Position];
					StartActivity(typeof(EncoderActivity));
				}
			};

		}
	}
}

