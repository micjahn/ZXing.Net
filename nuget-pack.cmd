@ECHO OFF

SET OUTDIR=Build\nuget
mkdir %OUTDIR%

3rdParty\nuget\nuget pack zxing.nuspec -outputdirectory %OUTDIR%