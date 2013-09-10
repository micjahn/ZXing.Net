@ECHO OFF

ECHO.
ECHO Script needs to be executed with administrator rights
ECHO.

SET scriptPath=%0
SET xsdPath=%scriptPath:install_nuspec_intellisense.cmd=nuspec.xsd%

copy %xsdPath% "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Xml\Schemas\"

pause