@ECHO OFF

ECHO.
ECHO Script needs to be executed with administrator rights
ECHO.

SET scriptPath=%0
SET xsdPath=%scriptPath:install_nuspec_intellisense.cmd=nuspec.xsd%

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Xml\Schemas" copy %xsdPath% "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Xml\Schemas\"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Xml\Schemas" copy %xsdPath% "%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Xml\Schemas\"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Xml\Schemas" copy %xsdPath% "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Xml\Schemas\"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Xml\Schemas" copy %xsdPath% "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Xml\Schemas\"

pause