@echo off
setlocal enabledelayedexpansion

echo Scanning DLLs folder for OpenSilverShowcase DLL files...
echo.
echo Generated RegisterSingleton code:
echo ================================

REM 출력 파일 초기화
set OUTPUT_FILE=RegisterSingleton_Generated.cs
echo // Generated RegisterSingleton code > %OUTPUT_FILE%
echo // Generated on %date% at %time% >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

REM DLLs 폴더가 있는지 확인
if not exist "DLLs" (
    echo Error: DLLs folder not found!
    pause
    exit /b 1
)

REM DLLs 폴더의 모든 OpenSilverShowcase.*.dll 파일 처리
for %%f in (DLLs\OpenSilverShowcase.*.dll) do (
    set "filename=%%~nf"
    
    REM Support.dll은 제외
    echo !filename! | findstr /i "Support" >nul
    if errorlevel 1 (
        REM OpenSilverShowcase. 접두사 제거
        set "basename=!filename:OpenSilverShowcase.=!"
        
        REM Content 접미사 추가
        set "contentname=!basename!Content"
        
        REM 결과 출력 (화면과 파일 모두)
        echo container.RegisterSingleton^<IView, !contentname!^>^(nameof^(!contentname!^)^);
        echo container.RegisterSingleton^<IView, !contentname!^>^(nameof^(!contentname!^)^); >> %OUTPUT_FILE%
    ) else (
        echo Skipping: %%f (Support.dll excluded)
    )
)

echo.
echo ================================
echo Code has been saved to: %OUTPUT_FILE%
echo.