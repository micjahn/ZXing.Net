@ECHO OFF

SET VERSION=0.16.0.0

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
IF NOT EXIST "%BINARY_DIR%\interop\zxing.interop.dll" GOTO BINARY_INTEROP_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net2.0\zxing.dll" GOTO BINARY_NET20_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net3.5\zxing.dll" GOTO BINARY_NET35_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.0\zxing.dll" GOTO BINARY_NET40_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.0\zxing.presentation.dll" GOTO BINARY_NET40_PRESENTATION_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.5\zxing.dll" GOTO BINARY_NET45_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.5\zxing.presentation.dll" GOTO BINARY_NET45_PRESENTATION_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.6\zxing.dll" GOTO BINARY_NET46_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.6\zxing.presentation.dll" GOTO BINARY_NET46_PRESENTATION_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.7\zxing.dll" GOTO BINARY_NET47_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\net4.7\zxing.presentation.dll" GOTO BINARY_NET47_PRESENTATION_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\portable\zxing.portable.dll" GOTO BINARY_PORTABLE_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\sl4\zxing.sl4.dll" GOTO BINARY_SL40_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\sl5\zxing.sl5.dll" GOTO BINARY_SL50_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\unity\zxing.unity.dll" GOTO BINARY_UNITY_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\uwp\zxing.dll" GOTO BINARY_UWP_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\winrt\zxing.winrt.dll" GOTO BINARY_WINRT_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp7.0\zxing.wp7.0.dll" GOTO BINARY_WP70_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp7.1\zxing.wp7.1.dll" GOTO BINARY_WP71_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\wp8.0\zxing.wp8.0.dll" GOTO BINARY_WP80_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\monodroid\zxing.monoandroid.dll" GOTO BINARY_MONODROID_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\winmd\zxing.winmd" GOTO BINARY_WINRTCOMPONENTS_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.0\zxing.dll" GOTO BINARY_NETSTANDARD10_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.1\zxing.dll" GOTO BINARY_NETSTANDARD11_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.3\zxing.dll" GOTO BINARY_NETSTANDARD13_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\Bindings\ZXing.CoreCompat.System.Drawing\bin\Release\netstandard1.3\zxing.corecompat.system.drawing.dll" GOTO BINARY_CORECOMPAT_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\Bindings\ZXing.ImageSharp\bin\Release\netstandard1.1\zxing.imagesharp.dll" GOTO BINARY_IMAGESHARP_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\Bindings\kinect\V1\zxing.kinect.dll" GOTO BINARY_KINECT_V1_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%\Bindings\kinect\V2\zxing.kinect.dll" GOTO BINARY_KINECT_V2_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\Bindings\ZXing.Magick\bin\Release\netstandard1.3\zxing.magick.dll" GOTO BINARY_MAGICK_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\Bindings\ZXing.OpenCV\bin\Release\netstandard1.6\zxing.opencv.dll" GOTO BINARY_OPENCV_NOT_FOUND
IF NOT EXIST "%CURRENT_DIR%\Source\Bindings\ZXing.SkiaSharp\bin\Release\netstandard1.3\zxing.skiasharp.dll" GOTO BINARY_SKIASHARP_NOT_FOUND

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
REM preparing binaries
REM ***************************************************************************************
DEL /S "%BINARY_DIR%"\Clients\*.xml
DEL /S "%BINARY_DIR%"\Clients\*.pdb

MKDIR "%BINARY_DIR%\netstandard" >NUL: 2>&1
MKDIR "%BINARY_DIR%\netstandard\1.0" >NUL: 2>&1
MKDIR "%BINARY_DIR%\netstandard\1.1" >NUL: 2>&1
MKDIR "%BINARY_DIR%\netstandard\1.3" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.0\zxing.dll" "%BINARY_DIR%\netstandard\1.0\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.0\zxing.pdb" "%BINARY_DIR%\netstandard\1.0\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.0\zxing.xml" "%BINARY_DIR%\netstandard\1.0\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.1\zxing.dll" "%BINARY_DIR%\netstandard\1.1\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.1\zxing.pdb" "%BINARY_DIR%\netstandard\1.1\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.1\zxing.xml" "%BINARY_DIR%\netstandard\1.1\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.3\zxing.dll" "%BINARY_DIR%\netstandard\1.3\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.3\zxing.pdb" "%BINARY_DIR%\netstandard\1.3\"
COPY "%CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.3\zxing.xml" "%BINARY_DIR%\netstandard\1.3\"

MKDIR "%BINARY_DIR%\Bindings" >NUL: 2>&1

MKDIR "%BINARY_DIR%\Bindings\CoreCompat.System.Drawing" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.CoreCompat.System.Drawing\bin\Release\netstandard1.3\zxing.corecompat.system.drawing.dll" "%BINARY_DIR%\Bindings\CoreCompat.System.Drawing\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.CoreCompat.System.Drawing\bin\Release\netstandard1.3\zxing.corecompat.system.drawing.pdb" "%BINARY_DIR%\Bindings\CoreCompat.System.Drawing\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.CoreCompat.System.Drawing\bin\Release\netstandard1.3\zxing.corecompat.system.drawing.xml" "%BINARY_DIR%\Bindings\CoreCompat.System.Drawing\"

MKDIR "%BINARY_DIR%\Bindings\ImageSharp" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.ImageSharp\bin\Release\netstandard1.1\zxing.imagesharp.dll" "%BINARY_DIR%\Bindings\ImageSharp\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.ImageSharp\bin\Release\netstandard1.1\zxing.imagesharp.pdb" "%BINARY_DIR%\Bindings\ImageSharp\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.ImageSharp\bin\Release\netstandard1.1\zxing.imagesharp.xml" "%BINARY_DIR%\Bindings\ImageSharp\"

MKDIR "%BINARY_DIR%\Bindings\Magick" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.Magick\bin\Release\netstandard1.3\zxing.magick.dll" "%BINARY_DIR%\Bindings\Magick\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.Magick\bin\Release\netstandard1.3\zxing.magick.pdb" "%BINARY_DIR%\Bindings\Magick\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.Magick\bin\Release\netstandard1.3\zxing.magick.xml" "%BINARY_DIR%\Bindings\Magick\"

MKDIR "%BINARY_DIR%\Bindings\OpenCV" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.OpenCV\bin\Release\netstandard1.6\zxing.opencv.dll" "%BINARY_DIR%\Bindings\OpenCV\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.OpenCV\bin\Release\netstandard1.6\zxing.opencv.pdb" "%BINARY_DIR%\Bindings\OpenCV\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.OpenCV\bin\Release\netstandard1.6\zxing.opencv.xml" "%BINARY_DIR%\Bindings\OpenCV\"

MKDIR "%BINARY_DIR%\Bindings\SkiaSharp" >NUL: 2>&1
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.SkiaSharp\bin\Release\netstandard1.3\zxing.skiasharp.dll" "%BINARY_DIR%\Bindings\SkiaSharp\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.SkiaSharp\bin\Release\netstandard1.3\zxing.skiasharp.pdb" "%BINARY_DIR%\Bindings\SkiaSharp\"
COPY "%CURRENT_DIR%\Source\Bindings\ZXing.SkiaSharp\bin\Release\netstandard1.3\zxing.skiasharp.xml" "%BINARY_DIR%\Bindings\SkiaSharp\"

REM
REM building archives for binaries
REM ***************************************************************************************

CD "%BINARY_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_BINARY%" ce2.0 ce3.5 net2.0 net3.5 net4.0 net4.5 net4.6 net4.7 winrt uwp netstandard unity sl4 sl5 wp7.0 wp7.1 wp8.0 monodroid winmd portable interop Bindings ..\..\THANKS ..\..\COPYING -xr!Documentation
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
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\OpenCV" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\log4net" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.NET" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.Silverlight" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\Unity" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\Base\3rdparty\Kinect" >NUL: 2>&1

MKDIR "%SVN_EXPORT_DIR%\WinMD" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Source" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Source\lib" >NUL: 2>&1
MKDIR "%SVN_EXPORT_DIR%\WinMD\Clients" >NUL: 2>&1

"%SVN_TOOL%" export --force "%SVN_URL%/Source/lib" "%SVN_EXPORT_DIR%\Base\Source\lib"
"%SVN_TOOL%" export --force "%SVN_URL%/Source/Bindings" "%SVN_EXPORT_DIR%\Base\Source\Bindings"
"%SVN_TOOL%" export --force "%SVN_URL%/Source/interop" "%SVN_EXPORT_DIR%\Base\Source\interop"
"%SVN_TOOL%" export --force "%SVN_URL%/Source/test/src" "%SVN_EXPORT_DIR%\Base\Source\test\src"
"%SVN_TOOL%" export --force "%SVN_URL%/Clients" "%SVN_EXPORT_DIR%\Base\Clients"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/AForge" "%SVN_EXPORT_DIR%\Base\3rdparty\AForge"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/EmguCV" "%SVN_EXPORT_DIR%\Base\3rdparty\EmguCV"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/OpenCV" "%SVN_EXPORT_DIR%\Base\3rdparty\OpenCV"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/NUnit.NET" "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.NET"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/NUnit.Silverlight" "%SVN_EXPORT_DIR%\Base\3rdparty\NUnit.Silverlight"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/Unity" "%SVN_EXPORT_DIR%\Base\3rdparty\Unity"
"%SVN_TOOL%" export --force "%SVN_URL%/3rdparty/Kinect" "%SVN_EXPORT_DIR%\Base\3rdparty\Kinect"
"%SVN_TOOL%" export --force "%SVN_URL%/Key" "%SVN_EXPORT_DIR%\Base\Key"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.ce.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.vs2012.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.vs2015.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.monoandroid.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.monotouch.sln" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/zxing.nunit" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/THANKS" "%SVN_EXPORT_DIR%\Base"
"%SVN_TOOL%" export --force "%SVN_URL%/COPYING" "%SVN_EXPORT_DIR%\Base"

"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/Source/lib" "%SVN_EXPORT_DIR%\WinMD\Source\lib"
"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/Clients" "%SVN_EXPORT_DIR%\WinMD\Clients"
"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/Key" "%SVN_EXPORT_DIR%\WinMD\Key"
"%SVN_TOOL%" export --force "%SVN_URL_WINMD%/zxing.vs2012.sln" "%SVN_EXPORT_DIR%\WinMD"

CD "%SVN_EXPORT_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_SOURCE%" Base\Source\lib\*.* Base\Source\Bindings\*.* Base\Source\interop\*.* Base\Source\test\src\*.* Base\Clients\*.* Base\3rdparty\*.* Base\Key\*.* Base\zxing.sln Base\zxing.ce.sln Base\zxing.vs2012.sln Base\zxing.vs2015.sln Base\zxing.monoandroid.sln Base\zxing.monotouch.sln Base\zxing.nunit Base\THANKS Base\COPYING WinMD\Source\lib\*.* WinMD\Clients\*.* WinMD\Key\*.* WinMD\zxing.vs2012.sln
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

:BINARY_INTEROP_NOT_FOUND
ECHO The Interop binaries 
ECHO %BINARY_DIR%\interop\...
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

:BINARY_NET45_PRESENTATION_NOT_FOUND
ECHO The .Net 4.5 Presentation binaries 
ECHO %BINARY_DIR%\net4.5\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET46_NOT_FOUND
ECHO The .Net 4.6 binaries 
ECHO %BINARY_DIR%\net4.6\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET46_PRESENTATION_NOT_FOUND
ECHO The .Net 4.6 Presentation binaries 
ECHO %BINARY_DIR%\net4.6\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET47_NOT_FOUND
ECHO The .Net 4.7 binaries 
ECHO %BINARY_DIR%\net4.7\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NET47_PRESENTATION_NOT_FOUND
ECHO The .Net 4.7 Presentation binaries 
ECHO %BINARY_DIR%\net4.7\...
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

:BINARY_KINECT_V1_NOT_FOUND
ECHO The Kinect V1 binaries 
ECHO %BINARY_DIR%\Bindings\Kinect\V1\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_KINECT_V2_NOT_FOUND
ECHO The Kinect V2 binaries 
ECHO %BINARY_DIR%\Bindings\Kinect\V2\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_UWP_NOT_FOUND
ECHO The UWP binaries 
ECHO %BINARY_DIR%\uwp\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NETSTANDARD10_NOT_FOUND
ECHO The .Net Standard 1.0 binaries 
ECHO %CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.0\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NETSTANDARD11_NOT_FOUND
ECHO The .Net Standard 1.1 binaries 
ECHO %CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.1\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_NETSTANDARD13_NOT_FOUND
ECHO The .Net Standard 1.3 binaries 
ECHO %CURRENT_DIR%\Source\lib\ZXing.Net\bin\Release\netstandard1.3\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_CORECOMPAT_NOT_FOUND
ECHO The CoreCompat.System.Drawing bindings binaries 
ECHO %CURRENT_DIR%\Source\Bindings\ZXing.CoreCompat.System.Drawing\bin\Release\netstandard1.3\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_IMAGESHARP_NOT_FOUND
ECHO The ImageSharp bindings binaries 
ECHO %CURRENT_DIR%\Source\Bindings\ZXing.ImageSharp\bin\Release\netstandard1.1\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_MAGICK_NOT_FOUND
ECHO The Magick bindings binaries 
ECHO %CURRENT_DIR%\Source\Bindings\ZXing.Magick\bin\Release\netstandard1.3\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_OPENCV_NOT_FOUND
ECHO The OpenCV bindings binaries 
ECHO %CURRENT_DIR%\Source\Bindings\ZXing.OpenCV\bin\Release\netstandard1.6\...
ECHO weren't found.
ECHO.
GOTO END

:BINARY_SKIASHARP_NOT_FOUND
ECHO The OpenCV bindings binaries 
ECHO %CURRENT_DIR%\Source\Bindings\ZXing.SkiaSharp\bin\Release\netstandard1.3\...
ECHO weren't found.
ECHO.
GOTO END

:END

