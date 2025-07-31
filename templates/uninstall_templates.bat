@echo off
setlocal enableDelayedExpansion
echo ========================================
echo Uninstalling OpenSilver Templates
echo ========================================
echo.

REM Set Visual Studio template paths
set "PROJECT_TEMPLATE_PATH=%USERPROFILE%\Documents\Visual Studio 2022\Templates\ProjectTemplates"
set "ITEM_TEMPLATE_PATH=%USERPROFILE%\Documents\Visual Studio 2022\Templates\ItemTemplates"

echo Template paths:
echo Project Templates: %PROJECT_TEMPLATE_PATH%
echo Item Templates: %ITEM_TEMPLATE_PATH%
echo.

echo ========================================
echo Removing template files...
echo ========================================

REM Remove Project Template (Showcase Template)
echo [1/3] Removing project template: Showcase Template (OpenSilver).zip
if exist "%PROJECT_TEMPLATE_PATH%\Showcase Template (OpenSilver).zip" (
    del "%PROJECT_TEMPLATE_PATH%\Showcase Template (OpenSilver).zip" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS - Template removed
    ) else (
        echo      FAILED - Could not remove template
    )
) else (
    echo      NOT FOUND - Template not installed
)

REM Remove Item Template 1 (Showcase Content)
echo [2/3] Removing item template: Showcase Content (OpenSilver).zip
if exist "%ITEM_TEMPLATE_PATH%\Showcase Content (OpenSilver).zip" (
    del "%ITEM_TEMPLATE_PATH%\Showcase Content (OpenSilver).zip" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS - Template removed
    ) else (
        echo      FAILED - Could not remove template
    )
) else (
    echo      NOT FOUND - Template not installed
)

REM Remove Item Template 2 (Showcase Item)
echo [3/3] Removing item template: Showcase Item (OpenSilver).zip
if exist "%ITEM_TEMPLATE_PATH%\Showcase Item (OpenSilver).zip" (
    del "%ITEM_TEMPLATE_PATH%\Showcase Item (OpenSilver).zip" > nul 2>&1
    if !ERRORLEVEL! == 0 (
        echo      SUCCESS - Template removed
    ) else (
        echo      FAILED - Could not remove template
    )
) else (
    echo      NOT FOUND - Template not installed
)

echo.
echo ========================================
echo Uninstallation Complete!
echo ========================================
echo.

echo OpenSilver templates have been removed from:
echo.
echo Project Templates:
echo   %PROJECT_TEMPLATE_PATH%
echo   - Showcase Template (OpenSilver).zip [REMOVED]
echo.
echo Item Templates:
echo   %ITEM_TEMPLATE_PATH%
echo   - Showcase Content (OpenSilver).zip [REMOVED]
echo   - Showcase Item (OpenSilver).zip [REMOVED]
echo.
echo Please restart Visual Studio to complete the uninstallation.
echo.
pause