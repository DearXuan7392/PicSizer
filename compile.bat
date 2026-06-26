@echo off

echo ========================================
echo  Start compiling PicSizer
echo ========================================

set "NO_PAUSE=0"
if "%~1"=="-nopause" set "NO_PAUSE=1"
if "%~1"=="nopause" set "NO_PAUSE=1"

if not exist "target" mkdir target

set CGO_ENABLED=0

set GUI_PATH=./cmd/gui
set CLI_PATH=./cmd/cli

echo [1/2] Compiling GUI version (%GUI_PATH%)...
go build -trimpath -ldflags="-s -w -H windowsgui" -o target/PicSizer.exe %GUI_PATH%
if %errorlevel% neq 0 (
    echo [ERROR] GUI build failed! Please check paths or code.
    goto ERROR_EXIT
)
echo [OK] GUI version compiled successfully.

echo [2/2] Compiling CLI version (%CLI_PATH%)...
go build -trimpath -ldflags="-s -w" -o target/PicSizer-cli.exe %CLI_PATH%
if %errorlevel% neq 0 (
    echo [ERROR] CLI build failed! Please check paths or code.
    goto ERROR_EXIT
)
echo [OK] CLI version compiled successfully.

echo ========================================
echo  Build completed successfully!
echo  GUI: target\PicSizer.exe
echo  CLI: target\PicSizer-cli.exe
echo ========================================

if "%NO_PAUSE%"=="1" (
    exit /b 0
) else (
    echo.
    echo Press any key to exit...
    pause > nul
    exit /b 0
)

:ERROR_EXIT
if "%NO_PAUSE%"=="1" (
    exit /b 1
) else (
    echo.
    echo compile error
    pause > nul
    exit /b 1
)