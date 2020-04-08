@ECHO OFF
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Installing EECIP Service...
echo ---------------------------------------------------
InstallUtil C:\EECIP_ServiceRunFolder\EECIP_Service.exe
echo ---------------------------------------------------
PAUSE
echo Done.
