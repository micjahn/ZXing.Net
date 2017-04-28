@ECHO OFF

SET OUTDIR=bin\Deployment
mkdir %OUTDIR%

..\..\..\3rdParty\nuget\nuget pack project.nuspec -outputdirectory %OUTDIR%