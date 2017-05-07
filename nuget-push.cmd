@ECHO OFF

SET Id=ZXing.Net
SET VERSION=0.16.0

echo Next step - uploading all nuget packages to nuget.org...
pause

3rdParty\nuget\nuget push Build\Deployment\%ID%.%VERSION%.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.CoreCompat.System.Drawing.%VERSION%-beta.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.ImageSharp.%VERSION%-beta.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.Kinect.V1.%VERSION%.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.Kinect.V2.%VERSION%.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.Magick.%VERSION%.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.OpenCV.%VERSION%.nupkg
3rdParty\nuget\nuget push Build\Deployment\%ID%.Bindings.SkiaSharp.%VERSION%.nupkg