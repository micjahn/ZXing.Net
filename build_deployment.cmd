@ECHO OFF

SET VERSION=0.8.0.0

SET CURRENT_DIR=%CD%
SET BUILD_DIR=%CD%\Build
SET DEPLOYMENT_DIR=%BUILD_DIR%\Deployment
SET BINARY_DIR=%BUILD_DIR%\Release
SET FILENAME_BINARY=%DEPLOYMENT_DIR%\ZXing.Net.%VERSION%.zip
SET FILENAME_DEMO_BINARY=%DEPLOYMENT_DIR%\ZXing.Net.DemoClients.%VERSION%.zip
SET FILENAME_SOURCE=%DEPLOYMENT_DIR%\ZXing.Net.Source.%VERSION%.zip
SET FILENAME_DOCUMENTATION=%DEPLOYMENT_DIR%\ZXing.Net.Documentation.%VERSION%.zip
SET ZIP_TOOL=%CD%\3rdparty\zip\7za.exe
SET SVN_EXPORT_DIR=%DEPLOYMENT_DIR%\Source
SET SVN_URL=https://zxingnet.svn.codeplex.com/svn/trunk
SET SVN_TOOL=%CD%\3rdparty\Subversion\svn.exe

ECHO.
ECHO Build deployment files in directory
ECHO %DEPLOYMENT_DIR%...
ECHO.

REM
REM preparing deployment directory
REM ***************************************************************************************

IF NOT EXIST "%BUILD_DIR%" GOTO BUILD_DIR_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%" GOTO BINARY_DIR_NOT_FOUND

MKDIR "%DEPLOYMENT_DIR%" >NUL: 2>&1
DEL /F "%DEPLOYMENT_DIR%\%FILENAME_BINARY%" >NUL: 2>&1


REM
REM building archives for binaries
REM ***************************************************************************************

CD "%BINARY_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_BINARY%" net2.0 net4.0 unity sl4 sl5 wp7.0 wp7.1
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_DEMO_BINARY%" Clients
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_DOCUMENTATION%" Documentation
CD "%CURRENT_DIR%"

ECHO.


REM
REM building nuget archive
REM ***************************************************************************************

CALL nuget-pack.cmd

ECHO.


REM
REM building source archive
REM ***************************************************************************************

RMDIR /S /Q "%SVN_EXPORT_DIR%" >NUL: 2>&1

MKDIR "%SVN_EXPORT_DIR%" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Source" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Source\lib" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Source\test" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Source\test\src" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Clients" >NUL: 2>&1

"%SVN_TOOL%" export --force "%SVN_URL%/Source/lib" "%SVN_EXPORT_DIR%\Source\lib"
"%SVN_TOOL%" export --force "%SVN_URL%/Source/test/src" "%SVN_EXPORT_DIR%\Source\test\src"
"%SVN_TOOL%" export --force "%SVN_URL%/Clients" "%SVN_EXPORT_DIR%\Clients"
"%SVN_TOOL%" export --force "%SVN_URL%/Documentation" "%SVN_EXPORT_DIR%\Documentation"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.sln" "%SVN_EXPORT_DIR%"

CD "%SVN_EXPORT_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_SOURCE%" Source\lib\*.* Source\test\src\*.* Clients\*.* zxing.sln
CD "%CURRENT_DIR%"

RMDIR /S /Q "%SVN_EXPORT_DIR%" >NUL: 2>&1


GOTO END

:BUILD_DIR_NOT_FOUND
ECHO The directory 
ECHO %BUILD_DIR%
ECHO doesn't exist.
ECHO.
GOTO END

:BINARY_DIR_NOT_FOUND
ECHO The directory 
ECHO %BINARY_DIR%
ECHO doesn't exist.
ECHO.
GOTO END

:END