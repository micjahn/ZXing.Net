
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
	public class BarcodeFormatListAdapter : BaseAdapter<BarcodeFormat>
	{
		public BarcodeFormatListAdapter (Context context, BarcodeFormat[] formats)
		{
			this.Context = context;
			this.Formats = formats;
		}

		public Context Context { get;set; }
		public BarcodeFormat[] Formats { get; set; }

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView as TextView;

			if (view == null)
				view = LayoutInflater.FromContext(this.Context).Inflate(Android.Resource.Layout.SimpleListItem1, null) as TextView;

			view.Text = this[position].ToString();

			return view;
		}

		public override int Count { get { return Formats.Length; } }	

		public override BarcodeFormat this [int position] {	get { return this.Formats[position]; } }
	}
}

