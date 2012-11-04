using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using ZXing;

namespace MonoTouchDemo
{
	public class ImageDisplayElement : Element, IElementSizing
	{
		public ImageDisplayElement (UIImage img, float maxHeight) : base(null)
		{
			this.MaxHeight = maxHeight;
			this.Image = img;
		}
		
		public float MaxHeight { get;set; }
		
		
		public UIImage Image { get;set; }
		
		
		protected override NSString CellKey 
		{
			get { return new NSString("ImageDisplayElement"); }
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
			
			iv.Image = this.Image;
			
			return cell;
		}
		
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return this.MaxHeight;
		}
	}
}

