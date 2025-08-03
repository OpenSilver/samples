@echo off
setlocal

REM Copy all files from Dlls folder to target folder relative to the location of this bat file
set "SRC=%~dp0Dlls"
set "DST=%~dp0..\..\opensilver.enterpriseapp\src\DLLs"

if not exist "%DST%" (
    mkdir "%DST%"
)

copy "%SRC%\*" "%DST%\" /Y

echo Copy completed.
pause
