using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using ZXing;

namespace MonoTouchDemo
{
	public class DecoderViewController : DialogViewController
	{
		public DecoderViewController (string title, BarcodeFormat format) : base(UITableViewStyle.Grouped, new RootElement(title), true)
		{
			this.Root.UnevenRows = true;
			this.Format = format;
		}

		public BarcodeFormat Format { get;set; } 

		public override void ViewDidLoad ()
		{
			this.Root.Add(new Section() {

				new StyledStringElement("Load App's Sample Photo", () => {
					Decode(UIImage.FromFile("BarcodeSamples/" + this.Format.ToString().ToLower() + ".png"));
				}),
				new StyledStringElement("Choose Existing Photo", () => {
					var ipc = new UIImagePickerController();
					ipc.FinishedPickingImage += (sender, e) => {
						Decode(e.Image);

						this.InvokeOnMainThread(() => this.NavigationController.DismissViewController(true, null));
					};
					ipc.FinishedPickingMedia += (sender, e) => {
						Decode(e.EditedImage ?? e.OriginalImage);
						this.InvokeOnMainThread(() => this.NavigationController.DismissViewController(true, null));
					};
					ipc.Canceled += (sender, e) => {
						this.InvokeOnMainThread(() => this.NavigationController.DismissViewController(true, null));
					};


					this.InvokeOnMainThread(() => this.NavigationController.PresentViewController(ipc, true, null));
				}),
				new StyledStringElement("Take new Photo", () => {

				})
			});

			this.Root.Add (new Section() {
				new ImageDisplayElement(null, 160f),
				new StyledStringElement("Decoded Results will appear here")
			});
		}

		void Decode(UIImage img)
		{
			var reader = new BarcodeReader();
			reader.PossibleFormats = new List<BarcodeFormat>() { this.Format };

			this.Root[1].Clear();
			this.Root[1].Add(new ImageDisplayElement(img, 160f));

			try
			{
				var result = reader.Decode(img);

				this.Root[1].Add(new StyledStringElement("Result:", result.Text));
			}
			catch (Exception ex)
			{
				this.Root[1].Add(new StyledMultilineElement(null, ex.ToString()));
			}
		}
	}
}

