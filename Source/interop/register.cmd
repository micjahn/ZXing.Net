@ECHO OFF

reg Query "HKLM\Hardware\Description\System\CentralProcessor\0" | find /i "x86" > NUL && set OS=32BIT || set OS=64BIT

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /unregister %~dp0zxing.interop.dll
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /codebase %~dp0zxing.interop.dll /tlb %~dp0zxing.interop.tlb

if %OS%==64BIT "%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /unregister %~dp0zxing.interop.dll
if %OS%==64BIT "%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /codebase %~dp0zxing.interop.dll /tlb %~dp0zxing.interop.tlb

pause