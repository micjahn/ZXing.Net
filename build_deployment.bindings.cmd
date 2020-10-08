@ECHO OFF

SETLOCAL EnableDelayedExpansion EnableExtensions

SET VERSION=0.16.6.0

SET CURRENT_DIR=%CD%
SET BUILD_DIR=%CD%\Build
SET LOGFILE=%CD%\build_deployment.bindings.log
SET DEPLOYMENT_DIR=%BUILD_DIR%\Deployment
SET BINARY_DIR=%BUILD_DIR%\Release
SET FILENAME_BINARY=%DEPLOYMENT_DIR%\ZXing.Net.Bindings.%VERSION%.zip
SET ZIP_TOOL=%CD%\3rdparty\zip\7za.exe
SET SOURCE_EXPORT_DIR=%DEPLOYMENT_DIR%\Source
SET SVN_TOOL=%CD%\3rdparty\Subversion\svn.exe
SET GET_PUBLICKEYTOKEN_TOOL=%CD%\3rdparty\GetPublicKeyToken\GetPublicKeyToken.exe
SET HAS_VALIDATION_ERROR=0

echo. > %LOGFILE%

echo.
echo Check for missing files...
echo.

FOR /F %%b IN (build_deployment_files.bindings.txt) DO (
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
SET VALIDATION_WAS_CALLED=0

FOR /F %%b IN (build_deployment_strong_named_files.bindings.txt) DO (
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
 for /F "tokens=1 delims=" %%t in ('"%GET_PUBLICKEYTOKEN_TOOL% !f!"') DO (
  SET VALIDATION_WAS_CALLED=1
  IF NOT "%%t" == "4e88037ac681fe60" (
   echo The assembly !f! is not signed with the correct key. required: 4e88037ac681fe60, found: %%t, re-signing...
   sn -q -Ra !f! Key\private.snk
   
   for /F "tokens=2 delims=:" %%t in ('"sn -q -T !f!"') DO (
    IF NOT "%%t" == "4e88037ac681fe60" (
     echo The assembly !f! is not signed with the correct key. required: 4e88037ac681fe60, found: %%t
     SET HAS_VALIDATION_ERROR=1
    )
   )
  )
 )
)

IF NOT "!VALIDATION_WAS_CALLED!" == "1" (
 ECHO.
 ECHO The file validation procedure was not executed. Please check the deployment script.
 GOTO END
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

ECHO Copy binaries
ECHO.

FOR /F "tokens=1,2 delims= " %%b IN (build_deployment_copy_operations.bindings.txt) DO (
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

"%ZIP_TOOL%" a -tzip -mx9 -r "%FILENAME_BINARY%" Bindings ..\..\THANKS ..\..\COPYING -xr^^!Documentation >> %LOGFILE% 2>&1
if ERRORLEVEL 1 GOTO ERROR_OPERATION

CD "%CURRENT_DIR%"


REM
REM build nuget archive
REM ***************************************************************************************

echo Build nuget packages...
echo.

CALL nuget-pack.bindings.cmd >> %LOGFILE% 2>&1


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
