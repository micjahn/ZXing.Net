@ECHO OFF

SET OUTDIR=..\..\..\Build\Deployment
mkdir %OUTDIR%

..\..\..\3rdParty\nuget\nuget pack project.V1.nuspec -outputdirectory %OUTDIR%
..\..\..\3rdParty\nuget\nuget pack project.V2.nuspec -outputdirectory %OUTDIR%