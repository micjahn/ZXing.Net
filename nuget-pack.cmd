@ECHO OFF

SET OUTDIR=Build\Deployment
mkdir %OUTDIR%

3rdParty\nuget\nuget pack zxing.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.CoreCompat.System.Drawing\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.ImageSharp\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.Magick\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.OpenCV\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.SkiaSharp\project.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.Kinect\project.V1.nuspec -outputdirectory %OUTDIR%
3rdParty\nuget\nuget pack Source\adapters\ZXing.Kinect\project.V2.nuspec -outputdirectory %OUTDIR%