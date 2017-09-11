@ECHO OFF

echo Next step - uploading all nuget packages to nuget.org...
pause

FOR /f "tokens=*" %%f IN ('dir /b Build\Deployment\*.nupkg') DO (
   echo %%f
   3rdParty\nuget\nuget push Build\Deployment\%%f -Source https://www.nuget.org/api/v2/package
)
