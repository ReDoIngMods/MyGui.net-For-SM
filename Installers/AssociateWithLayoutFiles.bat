@echo off
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo File associations require administrative privileges. Please run this script as Administrator.
    pause
    exit /b
)

echo Press enter to associate .layout files with MyGui.net.exe (the exe must be in the same folder as this script!)
pause

set "programPath=%~dp0MyGui.net.exe"

cls
if exist "%programPath%" (
	:: Setting
	assoc .layout=MyGuiDotNetLayoutFileType
	ftype MyGuiDotNetLayoutFileType="%programPath%" "%1"

	cls
	echo File association created for .layout files with MyGui.net.exe! (Please remember that this link is absolute, if you move the program you will have to re-run this script!)
	
) else ( echo Error: MyGui.net.exe not found in the current directory! )

pause