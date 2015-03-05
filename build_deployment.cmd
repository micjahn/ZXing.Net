@ECHO OFF

SET VERSION=0.14.1.0

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
SET SVN_URL_WINMD=https://zxingnet.svn.codeplex.com/svn/branches/WinMD
SET SVN_TOOL=%CD%\3rdparty\Subversion\svn.exe

IF NOT EXIST "%BINARY_DIR%\ce2.0\zxing.ce2.0.dll" GOTO BINARY_CE20_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\ce3.5\zxing.ce3.5.dll" GOTO BINARY_CE35_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net2.0\zxing.dll" GOTO BINARY_NET20_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net3.5\zxing.dll" GOTO BINARY_NET35_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.0\zxing.dll" GOTO BINARY_NET40_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.0\zxing.presentation.dll" GOTO BINARY_NET40_PRESENTATION_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.5\zxing.dll" GOTO BINARY_NET45_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\sl4\zxing.sl4.dll" GOTO BINARY_SL40_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\sl5\zxing.sl5.dll" GOTO BINARY_SL50_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\unity\zxing.unity.dll" GOTO BINARY_UNITY_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp7.0\zxing.wp7.0.dll" GOTO BINARY_WP70_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp7.1\zxing.wp7.1.dll" GOTO BINARY_WP71_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp8.0\zxing.wp8.0.dll" GOTO BINARY_WP80_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\winrt\zxing.winrt.dll" GOTO BINARY_WINRT_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\monodroid\zxing.monoandroid.dll" GOTO BINARY_MONODROID_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\winmd\zxing.winmd" GOTO BINARY_WINRTCOMPONENTS_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\portable\zxing.portable.dll" GOTO BINARY_PORTABLE_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\kinect\zxing.kinect.dll" GOTO BINARY_KINECT_NOT_FOUND

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
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_BINARY%" ce2.0 ce3.5 net2.0 net3.5 net4.0 net4.5 winrt unity sl4 sl5 wp7.0 wp7.1 wp8.0 monodroid winmd portable kinect ..\..\THANKS ..\..\COPYING -xr!Documentation
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
MKDIR "%SVN_EXPORT_DIR%\Base" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\Source" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\Source\lib" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\Source\test" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\Source\test\src" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\Clients" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\AForge" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\EmguCV" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.NET" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.Silverlight" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\Unity" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\Kinect" >NUL: 2>&1

MKDIR "%SVN_EXPORT_DIR%\WinMD" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Source" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Source\lib" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Clients" >NUL: 2>&1

"%SVN_TOOL%" export --force "%SVN_URL%/Source/lib" "%SVN_EXPORT_DIR%\Base\Source\lib"
"%SVN_TOOL%" export --force "%SVN_URL%/Source/test/src" "%SVN_EXPORT_DIR%\Base\Source\test\src"
"%SVN_TOOL%" export --force "%SVN_URL%/Clients" "%SVN_EXPORT_DIR%\Base\Clients"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/AForge" "%SVN_EXPORT_DIR%\Base\3rdparty\AForge"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/EmguCV" "%SVN_EXPORT_DIR%\Base\3rdparty\EmguCV"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/NUnit.NET" "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.NET"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/NUnit.Silverlight" "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.Silverlight"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/Unity" "%SVN_EXPORT_DIR%\Base\3rdparty\Unity"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/Kinect" "%SVN_EXPORT_DIR%\Base\3rdparty\Kinect"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.ce.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.vs2012.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.monoandroid.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.monotouch.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.nunit" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/THANKS" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/COPYING" "%SVN_EXPORT_DIR%\Base"

"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/Source/lib" "%SVN_EXPORT_DIR%\WinMD\Source\lib"
"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/Clients" "%SVN_EXPORT_DIR%\WinMD\Clients"
"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/zxing.vs2012.sln" "%SVN_EXPORT_DIR%\WinMD"

CD "%SVN_EXPORT_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_SOURCE%" Base\Source\lib\*.* Base\Source\test\src\*.* Base\Clients\*.* Base\3rdparty\*.* Base\zxing.sln Base\zxing.ce.sln Base\zxing.vs2012.sln Base\zxing.monoandroid.sln Base\zxing.monotouch.sln Base\zxing.nunit Base\THANKS WinMD\Source\lib\*.* WinMD\Clients\*.* WinMD\zxing.vs2012.sln
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

:BINARY_CE20_NOT_FOUND
ECHO The Windows CE 2.0 binaries 
ECHO %BINARY_DIR%\ce2.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_CE35_NOT_FOUND
ECHO The Windows CE 3.5 binaries 
ECHO %BINARY_DIR%\ce3.5\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET20_NOT_FOUND
ECHO The .Net 2.0 binaries 
ECHO %BINARY_DIR%\net2.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET35_NOT_FOUND
ECHO The .Net 3.5 binaries 
ECHO %BINARY_DIR%\net3.5\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET40_NOT_FOUND
ECHO The .Net 4.0 binaries 
ECHO %BINARY_DIR%\net4.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET40_PRESENTATION_NOT_FOUND
ECHO The .Net 4.0 Presentation binaries 
ECHO %BINARY_DIR%\net4.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET45_NOT_FOUND
ECHO The .Net 4.5 binaries 
ECHO %BINARY_DIR%\net4.5\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_SL40_NOT_FOUND
ECHO The Silverlight 4.0 binaries 
ECHO %BINARY_DIR%\sl4.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_SL50_NOT_FOUND
ECHO The Silverlight 5.0 binaries 
ECHO %BINARY_DIR%\sl5.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_UNITY_NOT_FOUND
ECHO The .Net 2.0 Unity binaries 
ECHO %BINARY_DIR%\unity\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_WP70_NOT_FOUND
ECHO The Windows Phone 7.0 binaries 
ECHO %BINARY_DIR%\wp7.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_WP71_NOT_FOUND
ECHO The Windows Phone 7.1 binaries 
ECHO %BINARY_DIR%\wp7.1\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_WP80_NOT_FOUND
ECHO The Windows Phone 8.0 binaries 
ECHO %BINARY_DIR%\wp8.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_WINRT_NOT_FOUND
ECHO The Windows RT binaries 
ECHO %BINARY_DIR%\winrt\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_MONODROID_NOT_FOUND
ECHO The MonoDroid binaries 
ECHO %BINARY_DIR%\monodroid\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_WINRTCOMPONENTS_NOT_FOUND
ECHO The WinRT Runtime Components binaries 
ECHO %BINARY_DIR%\winmd\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_PORTABLE_NOT_FOUND
ECHO The Portable binaries 
ECHO %BINARY_DIR%\portable\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_KINECT_NOT_FOUND
ECHO The Kinect binaries 
ECHO %BINARY_DIR%\kinect\...
ECHO weren't found.
ECHO.
GOTO END

:END

