@echo off
setlocal enableDelayedExpansion
chcp 65001 >nul
echo ========================================
echo Installing OpenSilver Templates
echo ========================================
echo.

REM Function to get actual Documents folder path
call :GetDocumentsPath DOCUMENTS_PATH
if "!DOCUMENTS_PATH!"=="" (
    echo ERROR: Could not determine Documents folder location
    echo Please check your system configuration.
    pause
    exit /b 1
)

echo Detected Documents folder: !DOCUMENTS_PATH!
echo.

REM Check for Visual Studio installations in order of preference
set "VS_TEMPLATE_BASE="
set "VS_VERSION="

REM Check VS 2022 first
if exist "!DOCUMENTS_PATH!\Visual Studio 2022\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2022\Templates"
    set "VS_VERSION=2022"
    goto :FoundVS
)

REM Check VS 2019
if exist "!DOCUMENTS_PATH!\Visual Studio 2019\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2019\Templates"
    set "VS_VERSION=2019"
    goto :FoundVS
)

REM Check VS 2017
if exist "!DOCUMENTS_PATH!\Visual Studio 2017\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2017\Templates"
    set "VS_VERSION=2017"
    goto :FoundVS
)

REM No existing VS templates found, default to VS 2022
set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2022\Templates"
set "VS_VERSION=2022"
echo No existing Visual Studio templates found, defaulting to Visual Studio 2022

:FoundVS
set "PROJECT_TEMPLATE_PATH=!VS_TEMPLATE_BASE!\ProjectTemplates"
set "ITEM_TEMPLATE_PATH=!VS_TEMPLATE_BASE!\ItemTemplates"

echo Using Visual Studio !VS_VERSION! template paths
echo.

REM Current script directory
set "CURRENT_DIR=%~dp0"
set "SOURCE_DIR=%CURRENT_DIR%vs-templates"

echo Template paths:
echo Project Templates: !PROJECT_TEMPLATE_PATH!
echo Item Templates: !ITEM_TEMPLATE_PATH!
echo Source Directory: !SOURCE_DIR!
echo.

REM Validate source directory exists
if not exist "!SOURCE_DIR!" (
    echo ERROR: Source directory not found: !SOURCE_DIR!
    echo Please ensure the vs-templates folder exists in the same directory as this script.
    echo.
    pause
    exit /b 1
)

REM Create template directories if they don't exist
if not exist "!PROJECT_TEMPLATE_PATH!" (
    echo Creating project template directory...
    mkdir "!PROJECT_TEMPLATE_PATH!" 2>nul
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create project template directory
        echo Please check permissions for: !PROJECT_TEMPLATE_PATH!
        pause
        exit /b 1
    )
)

if not exist "!ITEM_TEMPLATE_PATH!" (
    echo Creating item template directory...
    mkdir "!ITEM_TEMPLATE_PATH!" 2>nul
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create item template directory
        echo Please check permissions for: !ITEM_TEMPLATE_PATH!
        pause
        exit /b 1
    )
)

echo.
echo ========================================
echo Copying template files...
echo ========================================

REM Define template files array for easier maintenance
set "templates[0].name=Showcase Template (OpenSilver).zip"
set "templates[0].type=project"
set "templates[1].name=Showcase Content (OpenSilver).zip"
set "templates[1].type=item"
set "templates[2].name=Showcase Item (OpenSilver).zip"
set "templates[2].type=item"

set /a success_count=0
set /a total_count=3

for /L %%i in (0,1,2) do (
    set /a current=%%i+1
    echo [!current!/!total_count!] Copying template: !templates[%%i].name!
    
    if exist "!SOURCE_DIR!\!templates[%%i].name!" (
        if "!templates[%%i].type!"=="project" (
            copy "!SOURCE_DIR!\!templates[%%i].name!" "!PROJECT_TEMPLATE_PATH!\" >nul 2>&1
        ) else (
            copy "!SOURCE_DIR!\!templates[%%i].name!" "!ITEM_TEMPLATE_PATH!\" >nul 2>&1
        )
        
        if !ERRORLEVEL! == 0 (
            echo      SUCCESS
            set /a success_count+=1
        ) else (
            echo      FAILED - Copy operation failed
        )
    ) else (
        echo      FILE NOT FOUND - !templates[%%i].name!
    )
)

echo.
echo ========================================
echo Installation Summary
echo ========================================
echo.
echo Successfully installed: !success_count!/!total_count! templates
echo Visual Studio Version: !VS_VERSION!
echo Documents Location: !DOCUMENTS_PATH!
echo.

if !success_count! == !total_count! (
    echo All templates installed successfully!
    echo.
    echo Templates have been installed to:
    echo.
    echo Project Templates:
    echo   !PROJECT_TEMPLATE_PATH!
    echo   - Showcase Template (OpenSilver).zip
    echo.
    echo Item Templates:
    echo   !ITEM_TEMPLATE_PATH!
    echo   - Showcase Content (OpenSilver).zip
    echo   - Showcase Item (OpenSilver).zip
    echo.
    echo Please restart Visual Studio to use the new templates.
) else (
    echo WARNING: Some templates failed to install.
    echo Please check the error messages above and try again.
)

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