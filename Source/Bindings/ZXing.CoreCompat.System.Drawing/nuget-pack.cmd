@ECHO OFF

SET OUTDIR=..\..\..\Build\Deployment
mkdir %OUTDIR%

..\..\..\3rdParty\nuget\nuget pack project.nuspec -outputdirectory %OUTDIR%