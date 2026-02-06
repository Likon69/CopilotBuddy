@echo off
REM Profile Generator - Quick Start Script

set ZYGOR_FILE=C:\Users\Texy\Desktop\ZygorGuidesViewerClassicTBC\Guides-MOP\Leveling\ZygorLevelingHordeWOTLK.lua
set QUESTIE_PATH=C:\Users\Texy\Desktop\Questie-335
set OUTPUT_DIR=%~dp0output

echo ========================================
echo  Zygor to CopilotBuddy Profile Generator
echo ========================================
echo.
echo Zygor Source: %ZYGOR_FILE%
echo Questie Path: %QUESTIE_PATH%
echo Output Dir:   %OUTPUT_DIR%
echo.

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

echo [1] List all guides
echo [2] Generate ALL profiles
echo [3] Generate Orc profiles only
echo [4] Generate Troll profiles only
echo [5] Generate Blood Elf profiles only
echo [6] Exit
echo.

set /p choice="Select option: "

if "%choice%"=="1" (
    python "%~dp0zygor_parser.py" "%ZYGOR_FILE%" --list
    pause
)
if "%choice%"=="2" (
    python "%~dp0zygor_parser.py" "%ZYGOR_FILE%" -o "%OUTPUT_DIR%" -q "%QUESTIE_PATH%"
    pause
)
if "%choice%"=="3" (
    python "%~dp0zygor_parser.py" "%ZYGOR_FILE%" -o "%OUTPUT_DIR%" -q "%QUESTIE_PATH%" -r Orc
    pause
)
if "%choice%"=="4" (
    python "%~dp0zygor_parser.py" "%ZYGOR_FILE%" -o "%OUTPUT_DIR%" -q "%QUESTIE_PATH%" -r Troll
    pause
)
if "%choice%"=="5" (
    python "%~dp0zygor_parser.py" "%ZYGOR_FILE%" -o "%OUTPUT_DIR%" -q "%QUESTIE_PATH%" -r BloodElf
    pause
)
if "%choice%"=="6" exit
