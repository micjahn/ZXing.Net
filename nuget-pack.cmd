@ECHO OFF

SET OUTDIR=Build\Deployment
mkdir %OUTDIR%

3rdParty\nuget\nuget pack zxing.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.CoreCompat.System.Drawing\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.ImageSharp\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.Magick\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.OpenCV\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.SkiaSharp\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.Kinect\project.V1.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\Bindings\ZXing.Kinect\project.V2.nuspec -outputdirectory %OUTDIR%