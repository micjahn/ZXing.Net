@ECHO OFF

SETLOCAL EnableDelayedExpansion EnableExtensions

SET VERSION=0.16.2.0

SET CURRENT_DIR=%CD%
SET BUILD_DIR=%CD%\Build
SET LOGFILE=%CD%\build_deployment.log
SET DEPLOYMENT_DIR=%BUILD_DIR%\Deployment
SET BINARY_DIR=%BUILD_DIR%\Release
SET FILENAME_BINARY=%DEPLOYMENT_DIR%\ZXing.Net.%VERSION%.zip
SET FILENAME_DEMO_BINARY=%DEPLOYMENT_DIR%\ZXing.Net.DemoClients.%VERSION%.zip
SET FILENAME_SOURCE=%DEPLOYMENT_DIR%\ZXing.Net.Source.%VERSION%.zip
SET FILENAME_DOCUMENTATION=%DEPLOYMENT_DIR%\ZXing.Net.Documentation.%VERSION%.zip
SET ZIP_TOOL=%CD%\3rdparty\zip\7za.exe
SET SOURCE_EXPORT_DIR=%DEPLOYMENT_DIR%\Source
SET SVN_TOOL=%CD%\3rdparty\Subversion\svn.exe
SET HAS_VALIDATION_ERROR=0

echo. > %LOGFILE%

echo.
echo Check for missing files...
echo.

FOR /F %%b IN (build_deployment_files.txt) DO (
 SET f=%%b
 SET f=!f:%%BINARY_DIR%%=%BINARY_DIR%!
 SET f=!f:%%CURRENT_DIR%%=%CURRENT_DIR%!
 IF NOT EXIST !f! (
  ECHO The file !f! were not found
  SET HAS_VALIDATION_ERROR=1
 )
)

echo Check strong name of the assemblies...
echo (script has to be called in a Visual Studio command prompt, sn.exe has to be in search paths)
echo.

FOR /F %%b IN (build_deployment_strong_named_files.txt) DO (
 SET f=%%b
 SET f=!f:%%BINARY_DIR%%=%BINARY_DIR%!
 SET f=!f:%%CURRENT_DIR%%=%CURRENT_DIR%!
 
 REM validation of the strong name
 sn -q -vf !f!
 if ERRORLEVEL 1 (
  ECHO Re-signing the assembly !f!...
  sn -q -Ra !f! Key\private.snk
  sn -q -vf !f!
  if ERRORLEVEL 1 (
   echo Validation failed for !f!
   SET HAS_VALIDATION_ERROR=1
  )
 )
 
 REM validation of the correct signing key
 for /F "tokens=2 delims=:" %%t in ('"sn -q -T !f!"') DO (
  IF NOT "%%t" == " 4e88037ac681fe60" (
   echo The assembly !f! is not signed with the correct key. required: 4e88037ac681fe60, found: %%t, re-signing...
   sn -q -Ra !f! Key\private.snk
   
   for /F "tokens=2 delims=:" %%t in ('"sn -q -T !f!"') DO (
    IF NOT "%%t" == " 4e88037ac681fe60" (
     echo The assembly !f! is not signed with the correct key. required: 4e88037ac681fe60, found: %%t
     SET HAS_VALIDATION_ERROR=1
	)
   )
  )
 )
)

IF NOT "!HAS_VALIDATION_ERROR!" == "0" (
 ECHO.
 ECHO The file validation procedure was not successful.
 GOTO END
)

ECHO.
ECHO Build deployment files in directory
ECHO %DEPLOYMENT_DIR%...
ECHO.

REM
REM prepare deployment directory
REM ***************************************************************************************

IF NOT EXIST "%BUILD_DIR%" GOTO BUILD_DIR_NOT_FOUND
IF NOT EXIST "%BINARY_DIR%" GOTO BINARY_DIR_NOT_FOUND

MKDIR "%DEPLOYMENT_DIR%" >NUL: 2>&1
DEL /F "%DEPLOYMENT_DIR%\%FILENAME_BINARY%" >NUL: 2>&1

REM
REM prepare binaries
REM ***************************************************************************************
ECHO Cleanup demo client builds
ECHO.

DEL /S "%BINARY_DIR%"\Clients\*.xml >> %LOGFILE% 2>&1
DEL /S "%BINARY_DIR%"\Clients\*.pdb >> %LOGFILE% 2>&1

ECHO Copy binaries
ECHO.

FOR /F "tokens=1,2 delims= " %%b IN (build_deployment_copy_operations.txt) DO (
 SET f=%%b
 SET f=!f:%%BINARY_DIR%%=%BINARY_DIR%!
 SET f=!f:%%CURRENT_DIR%%=%CURRENT_DIR%!

 SET d=%%c
 SET d=!d:%%BINARY_DIR%%=%BINARY_DIR%!
 SET d=!d:%%CURRENT_DIR%%=%CURRENT_DIR%!
 
 ECHO Copy !f! to !d! >> %LOGFILE% 2>&1
 
 MKDIR "!d!" >> %LOGFILE% 2>&1
 
 COPY "!f!" "!d!" >> %LOGFILE% 2>&1
 if ERRORLEVEL 1 GOTO ERROR_OPERATION
)

REM
REM build archives for binaries
REM ***************************************************************************************

CD "%BINARY_DIR%"

echo Build assembly archive...
echo.

"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_BINARY%" ce2.0 ce3.5 net2.0 net3.5 net4.0 net4.5 net4.6 net4.7 winrt uwp netstandard unity sl4 sl5 wp7.0 wp7.1 wp8.0 monodroid winmd portable interop Bindings ..\..\THANKS ..\..\COPYING -xr^^!Documentation >> %LOGFILE% 2>&1
if ERRORLEVEL 1 GOTO ERROR_OPERATION

echo Build assembly archive - demo clients...
echo.

"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_DEMO_BINARY%" Clients >> %LOGFILE% 2>&1
if ERRORLEVEL 1 GOTO ERROR_OPERATION

echo Build documentation archive...
echo.

"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_DOCUMENTATION%" Documentation >> %LOGFILE% 2>&1
if ERRORLEVEL 1 GOTO ERROR_OPERATION

CD "%CURRENT_DIR%"


REM
REM build nuget archive
REM ***************************************************************************************

echo Build nuget packages...
echo.

CALL nuget-pack.cmd >> %LOGFILE% 2>&1


REM
REM build source archive
REM ***************************************************************************************

echo Build source code archive...
echo.

RMDIR /S /Q "%SOURCE_EXPORT_DIR%" >NUL: 2>&1

FOR /F "tokens=1,2 delims= " %%b IN (build_deployment_source_export.txt) DO (
 SET f=%%b

 SET d=%%c
 SET d=!d:%%BINARY_DIR%%=%BINARY_DIR%!
 SET d=!d:%%CURRENT_DIR%%=%CURRENT_DIR%!
 
 ECHO Export !f! to !d! >> %LOGFILE% 2>&1
 
 MKDIR "%SOURCE_EXPORT_DIR%\!d!" >> %LOGFILE% 2>&1
  
 echo "%SVN_TOOL%" export --force "!f!" "%SOURCE_EXPORT_DIR%\!d!" >> %LOGFILE% 2>&1
 "%SVN_TOOL%" export --force "!f!" "%SOURCE_EXPORT_DIR%\!d!" >> %LOGFILE% 2>&1
 if ERRORLEVEL 1 GOTO ERROR_OPERATION
)

CD "%SOURCE_EXPORT_DIR%"
"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_SOURCE%" Base\Source\lib\*.* Base\Source\Bindings\*.* Base\Source\interop\*.* Base\Source\test\src\*.* Base\Clients\*.* Base\3rdparty\*.* Base\Key\*.* Base\zxing.sln Base\zxing.ce.sln Base\zxing.vs2012.sln Base\zxing.vs2015.sln Base\zxing.monoandroid.sln Base\zxing.monotouch.sln Base\zxing.nunit Base\THANKS Base\COPYING WinMD\Source\lib\*.* WinMD\Clients\*.* WinMD\Key\*.* WinMD\zxing.vs2012.sln -xr^^!..svnbridge >> %LOGFILE% 2>&1
CD "%CURRENT_DIR%"

RMDIR /S /Q "%SOURCE_EXPORT_DIR%" >NUL: 2>&1


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

:ERROR_OPERATION
ECHO An error occurred, please check the logfile %LOGFILE%

:END

ENDLOCAL
