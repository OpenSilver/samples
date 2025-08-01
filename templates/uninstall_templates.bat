@echo off
setlocal enableDelayedExpansion
chcp 65001 >nul
echo ========================================
echo Uninstalling OpenSilver Templates
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

REM Check for Visual Studio installations and find templates
set "VS_TEMPLATE_BASE="
set "VS_VERSION="
set "TEMPLATES_FOUND=0"

REM Check VS 2022 first
if exist "!DOCUMENTS_PATH!\Visual Studio 2022\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2022\Templates"
    set "VS_VERSION=2022"
    set "TEMPLATES_FOUND=1"
    goto :FoundVS
)

REM Check VS 2019
if exist "!DOCUMENTS_PATH!\Visual Studio 2019\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2019\Templates"
    set "VS_VERSION=2019"
    set "TEMPLATES_FOUND=1"
    goto :FoundVS
)

REM Check VS 2017
if exist "!DOCUMENTS_PATH!\Visual Studio 2017\Templates" (
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2017\Templates"
    set "VS_VERSION=2017"
    set "TEMPLATES_FOUND=1"
    goto :FoundVS
)

:FoundVS
if !TEMPLATES_FOUND! == 0 (
    echo WARNING: No Visual Studio template directories found.
    echo This could mean:
    echo - Visual Studio is not installed
    echo - Templates were installed in a different location
    echo - Templates have already been removed
    echo.
    echo Checking common locations anyway...
    set "VS_TEMPLATE_BASE=!DOCUMENTS_PATH!\Visual Studio 2022\Templates"
    set "VS_VERSION=2022"
)

set "PROJECT_TEMPLATE_PATH=!VS_TEMPLATE_BASE!\ProjectTemplates"
set "ITEM_TEMPLATE_PATH=!VS_TEMPLATE_BASE!\ItemTemplates"

echo Using Visual Studio !VS_VERSION! template paths
echo.
echo Template paths:
echo Project Templates: !PROJECT_TEMPLATE_PATH!
echo Item Templates: !ITEM_TEMPLATE_PATH!
echo.

REM Check if any OpenSilver templates exist before proceeding
set "ANY_TEMPLATES_EXIST=0"
if exist "!PROJECT_TEMPLATE_PATH!\Showcase Template (OpenSilver).zip" set "ANY_TEMPLATES_EXIST=1"
if exist "!ITEM_TEMPLATE_PATH!\Showcase Content (OpenSilver).zip" set "ANY_TEMPLATES_EXIST=1"
if exist "!ITEM_TEMPLATE_PATH!\Showcase Item (OpenSilver).zip" set "ANY_TEMPLATES_EXIST=1"

if !ANY_TEMPLATES_EXIST! == 0 (
    echo ========================================
    echo No OpenSilver Templates Found
    echo ========================================
    echo.
    echo No OpenSilver templates were found in the expected locations.
    echo This could mean:
    echo - Templates are not installed
    echo - Templates were installed in a different location  
    echo - Templates have already been removed
    echo.
    echo Template locations checked:
    echo   !PROJECT_TEMPLATE_PATH!
    echo   !ITEM_TEMPLATE_PATH!
    echo.
    pause
    exit /b 0
)

echo ========================================
echo Removing template files...
echo ========================================

REM Define template files array for easier maintenance
set "templates[0].name=Showcase Template (OpenSilver).zip"
set "templates[0].type=project"
set "templates[1].name=Showcase Content (OpenSilver).zip"
set "templates[1].type=item"
set "templates[2].name=Showcase Item (OpenSilver).zip"
set "templates[2].type=item"

set /a success_count=0
set /a found_count=0
set /a total_count=3

for /L %%i in (0,1,2) do (
    set /a current=%%i+1
    echo [!current!/!total_count!] Removing template: !templates[%%i].name!
    
    if "!templates[%%i].type!"=="project" (
        set "template_path=!PROJECT_TEMPLATE_PATH!\!templates[%%i].name!"
    ) else (
        set "template_path=!ITEM_TEMPLATE_PATH!\!templates[%%i].name!"
    )
    
    if exist "!template_path!" (
        set /a found_count+=1
        del "!template_path!" >nul 2>&1
        
        if !ERRORLEVEL! == 0 (
            echo      SUCCESS - Template removed
            set /a success_count+=1
        ) else (
            echo      FAILED - Could not remove template
            echo      Path: !template_path!
        )
    ) else (
        echo      NOT FOUND - Template not installed
    )
)

echo.
echo ========================================
echo Uninstallation Summary
echo ========================================
echo.
echo Templates found: !found_count!/!total_count!
echo Successfully removed: !success_count!/!found_count!
echo Visual Studio Version: !VS_VERSION!
echo Documents Location: !DOCUMENTS_PATH!
echo.

if !success_count! == !found_count! (
    if !found_count! == 0 (
        echo No OpenSilver templates were found to remove.
    ) else (
        echo All found templates were successfully removed!
        echo.
        echo Removed templates from:
        echo.
        echo Project Templates:
        echo   !PROJECT_TEMPLATE_PATH!
        echo   - Showcase Template (OpenSilver).zip [REMOVED]
        echo.
        echo Item Templates:
        echo   !ITEM_TEMPLATE_PATH!
        echo   - Showcase Content (OpenSilver).zip [REMOVED]
        echo   - Showcase Item (OpenSilver).zip [REMOVED]
        echo.
        echo Please restart Visual Studio to complete the uninstallation.
    )
) else (
    echo WARNING: Some templates could not be removed.
    echo This might be due to:
    echo - File permissions issues
    echo - Files being in use by Visual Studio
    echo - Files being read-only
    echo.
    echo Please close Visual Studio and try again, or manually delete the remaining files.
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