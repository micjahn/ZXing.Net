@ECHO OFF

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /unregister %~dp0zxing.interop.dll

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /codebase %~dp0zxing.interop.dll /tlb %~dp0zxing.interop.tlb

pause