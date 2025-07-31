@echo off
setlocal enableDelayedExpansion

echo ========================================
echo Installing OpenSilver Templates
echo ========================================
echo.

REM Set Visual Studio template paths
set "PROJECT_TEMPLATE_PATH=%USERPROFILE%\Documents\Visual Studio 2022\Templates\ProjectTemplates"
set "ITEM_TEMPLATE_PATH=%USERPROFILE%\Documents\Visual Studio 2022\Templates\ItemTemplates"

REM Current script directory
set "CURRENT_DIR=%~dp0"
set "SOURCE_DIR=%CURRENT_DIR%vs-templates"

echo Template paths:
echo Project Templates: %PROJECT_TEMPLATE_PATH%
echo Item Templates: %ITEM_TEMPLATE_PATH%
echo Source Directory: %SOURCE_DIR%
echo.

REM Create template directories if they don't exist
if not exist "%PROJECT_TEMPLATE_PATH%" (
    echo Creating project template directory...
    mkdir "%PROJECT_TEMPLATE_PATH%"
)

if not exist "%ITEM_TEMPLATE_PATH%" (
    echo Creating item template directory...
    mkdir "%ITEM_TEMPLATE_PATH%"
)

echo.
echo ========================================
echo Copying template files...
echo ========================================

REM Copy Project Template (Showcase Template)
echo [1/3] Copying project template: Showcase Template (OpenSilver).zip
if exist "%SOURCE_DIR%\Showcase Template (OpenSilver).zip" (
    copy "%SOURCE_DIR%\Showcase Template (OpenSilver).zip" "%PROJECT_TEMPLATE_PATH%\" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS
    ) else (
        echo      FAILED
    )
) else (
    echo      FILE NOT FOUND
)

REM Copy Item Template 1 (Showcase Content)
echo [2/3] Copying item template: Showcase Content (OpenSilver).zip
if exist "%SOURCE_DIR%\Showcase Content (OpenSilver).zip" (
    copy "%SOURCE_DIR%\Showcase Content (OpenSilver).zip" "%ITEM_TEMPLATE_PATH%\" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS
    ) else (
        echo      FAILED
    )
) else (
    echo      FILE NOT FOUND
)

REM Copy Item Template 2 (Showcase Item)
echo [3/3] Copying item template: Showcase Item (OpenSilver).zip
if exist "%SOURCE_DIR%\Showcase Item (OpenSilver).zip" (
    copy "%SOURCE_DIR%\Showcase Item (OpenSilver).zip" "%ITEM_TEMPLATE_PATH%\" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS
    ) else (
        echo      FAILED
    )
) else (
    echo      FILE NOT FOUND
)

echo.
echo ========================================
echo Installation Complete!
echo ========================================
echo.
echo Templates have been installed to:
echo.
echo Project Templates:
echo   %PROJECT_TEMPLATE_PATH%
echo   - Showcase Template (OpenSilver).zip
echo.
echo Item Templates:
echo   %ITEM_TEMPLATE_PATH%
echo   - Showcase Content (OpenSilver).zip
echo   - Showcase Item (OpenSilver).zip
echo.
echo Please restart Visual Studio to use the new templates.
echo.
pause