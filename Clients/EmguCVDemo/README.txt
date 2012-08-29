That demo shows the way of decoding barcodes from webcam image with the use of EmguCV.

There are two ways to do that:
* Use the default BarcodeReader from ZXing.Net which accepts a System.Drawing.Bitmap as the input (simplest way)
* Write your own luminance source class which accepts the Image class from EmguCV (simple way)

The demo shows the second one which needs a custom luminance source.
The following class is needed
* ImageLuminanceSource: calculating the luminance values based upon a Image<Emgu.CV.Structure.Bgr, byte>

The next two classes are nice to have but only needed to have clear class structure:
* IBarcodeReaderImage: interface for the customized barcode reader
* BarcodeReaderImage: barcode reader class which is derived from BarcodeReaderGeneric<T>.
  It is needed for the ImageLuminanceSource instance which the BarcodeReaderGeneric consumes.