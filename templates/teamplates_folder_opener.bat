@echo off
setlocal enableDelayedExpansion
chcp 65001 >nul
echo ========================================
echo Visual Studio Template Folder Opener
echo ========================================
echo.

REM Function to get actual Documents folder path
call :GetDocumentsPath DOCUMENTS_PATH
if "!DOCUMENTS_PATH!"=="" (
    echo ERROR: Could not determine Documents folder location
    pause
    exit /b 1
)

echo Detected Documents folder: !DOCUMENTS_PATH!
echo.

REM Check for Visual Studio installations and open folders
echo Scanning for Visual Studio installations...
set "FOUND_ANY=0"

REM Check VS 2022
if exist "!DOCUMENTS_PATH!\Visual Studio 2022\Templates" (
    echo [✓] Visual Studio 2022 - Found
    set "VS2022_PATH=!DOCUMENTS_PATH!\Visual Studio 2022\Templates"
    set "FOUND_ANY=1"
) else (
    echo [ ] Visual Studio 2022 - Not found
)

REM Check VS 2019
if exist "!DOCUMENTS_PATH!\Visual Studio 2019\Templates" (
    echo [✓] Visual Studio 2019 - Found
    set "VS2019_PATH=!DOCUMENTS_PATH!\Visual Studio 2019\Templates"
    set "FOUND_ANY=1"
) else (
    echo [ ] Visual Studio 2019 - Not found
)

REM Check VS 2017
if exist "!DOCUMENTS_PATH!\Visual Studio 2017\Templates" (
    echo [✓] Visual Studio 2017 - Found  
    set "VS2017_PATH=!DOCUMENTS_PATH!\Visual Studio 2017\Templates"
    set "FOUND_ANY=1"
) else (
    echo [ ] Visual Studio 2017 - Not found
)

echo.

if !FOUND_ANY! == 0 (
    echo No Visual Studio template directories found.
    echo.
    pause
    exit /b 0
)

echo ========================================
echo Opening Template Folders...
echo ========================================
echo.

REM Open VS 2022 templates
if defined VS2022_PATH (
    echo Opening Visual Studio 2022 templates...
    if exist "!VS2022_PATH!\ProjectTemplates" (
        echo   - Project Templates: !VS2022_PATH!\ProjectTemplates
        start "" explorer "!VS2022_PATH!\ProjectTemplates"
        timeout /t 1 >nul
    )
    if exist "!VS2022_PATH!\ItemTemplates" (
        echo   - Item Templates: !VS2022_PATH!\ItemTemplates  
        start "" explorer "!VS2022_PATH!\ItemTemplates"
        timeout /t 1 >nul
    )
    echo.
)

REM Open VS 2019 templates
if defined VS2019_PATH (
    echo Opening Visual Studio 2019 templates...
    if exist "!VS2019_PATH!\ProjectTemplates" (
        echo   - Project Templates: !VS2019_PATH!\ProjectTemplates
        start "" explorer "!VS2019_PATH!\ProjectTemplates"
        timeout /t 1 >nul
    )
    if exist "!VS2019_PATH!\ItemTemplates" (
        echo   - Item Templates: !VS2019_PATH!\ItemTemplates
        start "" explorer "!VS2019_PATH!\ItemTemplates"
        timeout /t 1 >nul
    )
    echo.
)

REM Open VS 2017 templates
if defined VS2017_PATH (
    echo Opening Visual Studio 2017 templates...
    if exist "!VS2017_PATH!\ProjectTemplates" (
        echo   - Project Templates: !VS2017_PATH!\ProjectTemplates
        start "" explorer "!VS2017_PATH!\ProjectTemplates"
        timeout /t 1 >nul
    )
    if exist "!VS2017_PATH!\ItemTemplates" (
        echo   - Item Templates: !VS2017_PATH!\ItemTemplates
        start "" explorer "!VS2017_PATH!\ItemTemplates"
        timeout /t 1 >nul
    )
    echo.
)

echo All template folders opened!
echo.
pause
exit /b 0

REM ========================================
REM Function to get actual Documents folder path
REM ========================================
:GetDocumentsPath
set "result_var=%1"

REM Method 1: Try PowerShell to get actual Documents path
for /f "delims=" %%i in ('powershell -NoProfile -Command "[Environment]::GetFolderPath('MyDocuments')" 2^>nul') do (
    set "DOCS_PATH=%%i"
    if not "!DOCS_PATH!"=="" (
        set "%result_var%=!DOCS_PATH!"
        goto :eof
    )
)

REM Method 2: Check registry for Shell Folders
for /f "tokens=2*" %%i in ('reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" /v "Personal" 2^>nul ^| find "Personal"') do (
    set "DOCS_PATH=%%j"
    if not "!DOCS_PATH!"=="" (
        set "%result_var%=!DOCS_PATH!"
        goto :eof
    )
)

REM Method 3: Check registry for User Shell Folders (redirected locations)
for /f "tokens=2*" %%i in ('reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v "Personal" 2^>nul ^| find "Personal"') do (
    set "DOCS_PATH=%%j"
    REM Expand environment variables if present
    call set "DOCS_PATH=!DOCS_PATH!"
    if not "!DOCS_PATH!"=="" (
        set "%result_var%=!DOCS_PATH!"
        goto :eof
    )
)

REM Method 4: Fallback to default location
set "DOCS_PATH=%USERPROFILE%\Documents"
if exist "!DOCS_PATH!" (
    set "%result_var%=!DOCS_PATH!"
    goto :eof
)

REM Method 5: Try OneDrive Documents
set "DOCS_PATH=%USERPROFILE%\OneDrive\Documents"  
if exist "!DOCS_PATH!" (
    set "%result_var%=!DOCS_PATH!"
    goto :eof
)

REM Failed to find Documents folder
set "%result_var%="
goto :eof