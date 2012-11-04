using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using ZXing;

namespace MonoTouchDemo
{
	public class BarcodeElement : Element, IElementSizing
	{
		public BarcodeElement (BarcodeFormat format, string value, float height) : base(null)
		{
			this.MaxHeight = height;
			this.Format = format;
			this.Value = value;
		}

		string value =  string.Empty;
		BarcodeFormat format = BarcodeFormat.QR_CODE;
		UIImage barcodeImg = null;


		public float MaxHeight { get;set; }


		public string Value
		{ 
			get { return value; }
			set { this.value = value; this.barcodeImg = null; }
		}

		public BarcodeFormat Format 
		{ 
			get { return this.format; }
			set { this.format = value; this.barcodeImg = null; }
		}

		protected override NSString CellKey 
		{
			get { return new NSString("BarcodeElement"); }
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			tableView.DeselectRow(path, true);

			var av = new UIAlertView("Save to Album?", "Would you like to save this barcode to your photo album?", null, "No", "Yes");
			av.Clicked += (sender, e) => {

				if (e.ButtonIndex == 1)
					this.barcodeImg.SaveToPhotosAlbum(null);
			};
			av.Show();
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell(this.CellKey);
			UIImageView iv = null;

			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Default, this.CellKey);

				iv = new UIImageView(new RectangleF(10f, 5f, tv.Bounds.Width - 20f, this.MaxHeight - 10f));
				iv.Tag = 101;
				iv.ContentMode = UIViewContentMode.ScaleAspectFit;

				cell.AddSubview(iv);

			}
			else
			{
				iv = cell.ViewWithTag(101) as UIImageView;

			}

			if (this.barcodeImg == null)
			{
				var writer = new BarcodeWriter() { Format = this.format };
				this.barcodeImg = writer.Write(this.value);
			}

			iv.Image = this.barcodeImg;

			return cell;
		}

		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return this.MaxHeight;
		}
	}
}

