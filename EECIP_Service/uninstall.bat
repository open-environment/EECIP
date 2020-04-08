@ECHO OFF
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Uninstalling EECIP Service...
echo ---------------------------------------------------
InstallUtil /u C:\EECIP_ServiceRunFolder\EECIP_Service.exe
echo ---------------------------------------------------
echo Done.
PAUSE