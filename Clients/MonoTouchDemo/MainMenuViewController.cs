using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

using ZXing;

namespace MonoTouchDemo
{
	public class MainMenuViewController : DialogViewController
	{
		public MainMenuViewController () : base(UITableViewStyle.Grouped, new RootElement(""), false)
		{
		}

		EncoderViewController encoderViewController = null;
		DecoderViewController decoderViewController = null;

		public override void ViewDidLoad ()
		{
			this.Root = new RootElement("Zxing.Net Demo") 
			{
				new Section("Barcode Reading"),
				new Section("Barcode Writing")
			};

			var readers = new BarcodeFormat[] { 
				BarcodeFormat.AZTEC, BarcodeFormat.CODABAR, BarcodeFormat.CODE_128, BarcodeFormat.CODE_39,
				BarcodeFormat.CODE_93, BarcodeFormat.DATA_MATRIX, BarcodeFormat.EAN_13, BarcodeFormat.EAN_8,
				BarcodeFormat.ITF, BarcodeFormat.MAXICODE, BarcodeFormat.PDF_417, BarcodeFormat.QR_CODE,
				BarcodeFormat.RSS_14, BarcodeFormat.RSS_EXPANDED, BarcodeFormat.UPC_A, BarcodeFormat.UPC_E,
				BarcodeFormat.UPC_EAN_EXTENSION
			};

			foreach (var fmt in readers)
			{
				this.Root[0].Add(new StyledStringElement(fmt.ToString(), () => {

					decoderViewController = new DecoderViewController("Read: " + fmt.ToString(), fmt);
					this.NavigationController.PushViewController(decoderViewController, true);

				}) { Accessory = UITableViewCellAccessory.DisclosureIndicator });
			}

			var writers = new BarcodeFormat[] { 
				BarcodeFormat.UPC_A, BarcodeFormat.EAN_8, BarcodeFormat.EAN_13, BarcodeFormat.CODE_39,
				BarcodeFormat.CODE_128, BarcodeFormat.ITF, BarcodeFormat.CODABAR,
				BarcodeFormat.QR_CODE, BarcodeFormat.PDF_417
			};

			foreach (var fmt in writers)
			{
				this.Root[1].Add(new StyledStringElement(fmt.ToString(), () => {

					encoderViewController = new EncoderViewController("Write: " + fmt.ToString(), fmt);
					this.NavigationController.PushViewController(encoderViewController, true);

				}) { Accessory = UITableViewCellAccessory.DisclosureIndicator });
			}

		}

	}
}

