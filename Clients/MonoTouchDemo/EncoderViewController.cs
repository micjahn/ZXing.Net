using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace MonoTouchDemo
{
	public class EncoderViewController : DialogViewController
	{
		public EncoderViewController (string title, ZXing.BarcodeFormat format) : base(UITableViewStyle.Grouped, new RootElement(title), true)
		{
			this.Root.UnevenRows = true;

			this.Format = format;
		}

		public ZXing.BarcodeFormat Format { get;set; }

		public override void ViewDidLoad ()
		{
			this.Root.Add(new Section() { 

				new EntryElement("Value:", "Barcode Value to Generate", ""),
				new StyledStringElement("Generate Barcode", () => {

					string val = string.Empty;

					this.InvokeOnMainThread(() => {
						var ee = this.Root[0][0] as EntryElement;
						ee.FetchValue();
						val = ee.Value;
					});

					var writer = new ZXing.BarcodeWriter() { Format = this.Format };

					this.InvokeOnMainThread(() => {

						this.Root[1].Clear();

						try
						{
							var img = writer.Write(val);							
							this.Root[1].Add(new BarcodeElement(this.Format, val, 160f));
						}
						catch (Exception ex)
						{
							this.Root[1].Add(new StyledMultilineElement(null, ex.ToString()));
						}
					});
				})
			});

			this.Root.Add(new Section() {



			});




		}
	}
}

