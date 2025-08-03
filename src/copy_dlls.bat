@echo off
setlocal enabledelayedexpansion

set DEST_DIR=DLLs

if not exist "%DEST_DIR%" (
    mkdir "%DEST_DIR%"
)

for /d %%D in (OpenSilverShowcase.*) do (
    set "DLLNAME=%%D\bin\Debug\netstandard2.0\%%D.dll"
    if exist "!DLLNAME!" (
        echo Copying %%D.dll...
        copy /Y "!DLLNAME!" "%DEST_DIR%\"
    ) else (
        echo [MISS] !DLLNAME! NOT FOUND
    )
)

echo All DLLs copied!
pause
