@ECHO OFF

SET OUTDIR=Build\Deployment
mkdir %OUTDIR%

3rdParty\nuget\nuget pack zxing.nuspec -outputdirectory %OUTDIR%