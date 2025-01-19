@ECHO OFF

echo Next step - uploading all nuget packages to nuget.org...
pause

FOR /f "tokens=*" %%f IN ('dir /b Build\Deployment\*.nupkg') DO (
   echo %%f
   3rdParty\nuget\nuget push Build\Deployment\%%f -Source https://api.nuget.org/v3/index.json
)
