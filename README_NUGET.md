## About
A library which supports decoding and generating of barcodes (like QR Code, PDF 417, EAN, UPC, Aztec, Data Matrix, Codabar) within images.

## How to Use
The source code repository includes small examples for Windows Forms, Silverlight, Windows Phone and other project types.

The following example works with the classic .Net framework until version 4.8.1:

```csharp
// create a barcode reader instance
IBarcodeReader reader = new BarcodeReader();
// load a bitmap
var barcodeBitmap = (Bitmap)Image.FromFile("C:\\sample-barcode-image.png");
// detect and decode the barcode inside the bitmap
var result = reader.Decode(barcodeBitmap);
// do something with the result
if (result != null)
{
   txtDecoderType.Text = result.BarcodeFormat.ToString();
   txtDecoderContent.Text = result.Text;
}
```
If you want to try the sample code above within a project which target .Net Standard or .Net 5.0 or higher then you have to add one of the
additional nuget package for a specific image library: https://www.nuget.org/packages?q=ZXing.Bindings
The main package of ZXing.Net for such platforms only contains the core classes which are not dependent on a specific assembly for image formats.

```csharp
// example shows a simple decoding snippet as a .Net 8.0 console appliation which uses the ZXing.Windows.Compatibility package
using System.Drawing;
using ZXing.Windows.Compatibility;

// create a barcode reader instance
var reader = new BarcodeReader();
// load a bitmap
var barcodeBitmap = (Bitmap)Image.FromFile("C:\\sample-barcode-image.png");
// detect and decode the barcode inside the bitmap
var result = reader.Decode(barcodeBitmap);
// do something with the result
if (result != null)
{
    Console.WriteLine(result.BarcodeFormat.ToString());
    Console.WriteLine(result.Text);
}
else
{
    Console.WriteLine("No barcode found");
}
```

## Related Packages
There are several packages which can be used with different image libraries in combination with ZXing.Net.
https://www.nuget.org/packages?q=ZXing.Bindings

## Feedback
Bug reports and contributions are welcome at [the GitHub repository](https://github.com/micjahn/ZXing.Net).