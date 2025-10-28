
<div align="right">
  <details>
    <summary >🌐 Language</summary>
    <div>
      <div align="center">
        <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=en">English</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=zh-CN">简体中文</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=zh-TW">繁體中文</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=ja">日本語</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=ko">한국어</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=hi">हिन्दी</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=th">ไทย</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=fr">Français</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=de">Deutsch</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=es">Español</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=it">Italiano</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=ru">Русский</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=pt">Português</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=nl">Nederlands</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=pl">Polski</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=ar">العربية</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=fa">فارسی</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=tr">Türkçe</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=vi">Tiếng Việt</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=id">Bahasa Indonesia</a>
        | <a href="https://openaitx.github.io/view.html?user=micjahn&project=ZXing.Net&lang=as">অসমীয়া</
      </div>
    </div>
  </details>
</div>

# ZXing.Net 

[![N|NuGet](https://img.shields.io/nuget/v/ZXing.Net.svg)](https://www.nuget.org/packages/ZXing.Net/)
[![Build status](https://ci.appveyor.com/api/projects/status/49uvsxpw3ak9jtmm?svg=true)](https://ci.appveyor.com/project/MichaelJahn/zxing-net)
[![Donate](https://img.shields.io/badge/%F0%9F%92%99-Donate%20%2F%20Support%20Us-blue.svg)](#donate)

![ZXing.Net.Mobile Logo](https://raw.githubusercontent.com/micjahn/ZXing.Net/master/Icons/logo.jpg)

## Project Description
A library which supports decoding and generating of barcodes (like QR Code, PDF 417, EAN, UPC, Aztec, Data Matrix, Codabar) within images.

The project is a port of the java based barcode reader and generator library ZXing.  
https://github.com/zxing/zxing  
It has been ported by hand with a lot of optimizations and improvements.

The following barcodes are supported by the decoder:
UPC-A, UPC-E, EAN-8, EAN-13, Code 39, Code 93, Code 128, ITF, Codabar, MSI, RSS-14 (all variants), QR Code, Data Matrix, Aztec and PDF-417.
The encoder supports the following formats:
UPC-A, EAN-8, EAN-13, Code 39, Code 128, ITF, Codabar, Plessey, MSI, QR Code, PDF-417, Aztec, Data Matrix

#### Assemblies are available for the following platforms:

* .Net 2.0, 3.5, 4.x, 5.0, 6.0, 7.0
* Windows RT Class Library and Runtime Components (winmd)
* .NET Standard / .NET Core / UWP
* Portable Class Library
* Unity3D (.Net 2.0 built without System.Drawing reference)
* Xamarin.Android (formerly Mono for Android)
* bindings to Windows.Compatibility, CoreCompat.System.Drawing, ImageSharp, SkiaSharp, OpenCVSharp, Magick, Kinect V1 and V2, EmguCV, Eto.Forms, ZKWeb.System.Drawing
* support COM interop, can be used with VBA

#### obsolete Assemblies are available for the following platforms up to release 0.16:
* Windows Phone 7.0, 7.1 and 8.0
* Windows CE
* Silverlight 4 and 5

The library is available in the [release section](https://github.com/micjahn/ZXing.Net/releases) and as [NuGet package](https://www.nuget.org/packages/ZXing.Net/), too.

#### Additional platform support without pre-built binaries
The library can be built for Xamarin.iOS (formerly MonoTouch). The project file and solution are available in the source code repository.

A special version for the [.Net Micro Framework](http://www.microsoft.com/netmf/) can be found in a separate branch in the source code repository.

#### The following demo clients are available:

* decoder for the command line
* encoder for the command line
* Windows Forms demo (demonstrates decoding and encoding of static images and from a camera)
* Windows Service demo (demonstrates decoding of static images)
* Windows Presentation Framework demo (demonstrates decoding of static images)
* Windows RT demo (demonstrates decoding of static images)
* Windows Store App with HTML5/JS (demonstrates decoding of static images)
* Unity3D and Vuforia demo (demonstrates encoding of barcodes and decoding of images from a camera with [Unity3D](http://unity3d.com/))
* EmguCV demo (demonstrates decoding of images from a camera and uses the [EmguCV framework](http://www.emgu.com/))
* OpenCV demo (demonstrates decoding of images from a camera and uses the [OpenCVSharp framework](https://github.com/shimat/opencvsharp/))
* AForge demo (demonstrates decoding of images from a camera and uses the [AForge framework](http://www.aforgenet.com/))

## Thanks
Many thanks to the team of the [zxing project](https://github.com/zxing/zxing) for their great work. ZXing.Net would not be possible without your work!
## Usage examples
The source code repository includes small examples for Windows Forms, Windows Phone and other project types.
Obsolete examples are available for the following platforms in separate branches:
0.16: 
* Silverlight 4 and 5,
* Windows CE demo (demonstrates decoding of static images)
* Windows Phone demo (demonstrates decoding of static images and from a camera)

#### small example decoding a barcode inside a bitmap (.Net 2.0/3.5/4.x)
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
#### important notice for .Net Standard and .Net 5.0 and above target platforms
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
## Help wanted
All help is welcome!
## Feedback
You use the library?
We would be happy if you give us a short note on the use of the library.

You found a bug?
Please create a new issue here or start a discussion about it if you are not sure.

You use the library and you are not happy with it?
Write us an email please or start a discussion about your problems with it. We will try to help you.

And you can find me on [Twitter](http://twitter.com/micjahn).
[![N|http://twitter.com/micjahn](https://img.shields.io/twitter/follow/espadrine.svg?style=social&label=Follow)](http://twitter.com/micjahn)
## Support it
If you find the project useful and you wish to support the future development feel free to support it with a donation.

## Donate

[![N|Donate](https://www.paypal.com/en_US/i/btn/btn_donateCC_LG_global.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BYHN42UHPA86E)

Beside a donation patches, bug reports, feedback and other useful help are always welcome!

## Sponsors

Support us with a monthly donation and help us continue our activities. 
Become a sponsor and get your logo on our README on Github with a link to your site. 
* [[Become a Github Sponsor](https://github.com/sponsors/micjahn)]
* [[Become a Opencollective backer/sponsor](https://opencollective.com/zxingnet)]

## Donation WITHOUT money
It would be really, really great if you could support one of my social projects. You can support it WITHOUT paying money.
You only have to go to the following url before you buy anything from a supported online shop (like Amazon or eBay):  
http://www.bildungsspender.de/kitadorfhain  
Select you prefered online shop and go shopping like everytime. The online shop will pay a provision to our Kindergarten for your purchase. No extra costs for you. There are 85 thankful kids.
(and one thankful developer of ZXing.Net ;) )
