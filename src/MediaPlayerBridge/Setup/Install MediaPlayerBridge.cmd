@echo off
setlocal EnableExtensions
title MediaPlayerBridge Setup
cd /d "%~dp0"

:menu
cls
echo ==================================================
echo                MediaPlayerBridge Setup
echo ==================================================
echo.
echo [1] Set up everything  (piXel + startup + start)
echo [2] Install or repair piXel VISUALIZER support
echo [3] Enable startup
echo [4] Start the bridge now
echo [5] Disable startup
echo [6] Uninstall  (remove piXel support and startup)
echo [Q] Quit
echo.
choice /C 123456Q /N /M "Choose an option"

if errorlevel 7 goto end
if errorlevel 6 goto uninstall
if errorlevel 5 goto removeStartup
if errorlevel 4 goto start
if errorlevel 3 goto startup
if errorlevel 2 goto pixel
if errorlevel 1 goto all

:all
echo.
echo Installing piXel adapter...
"%~dp0MediaPlayerBridge.exe" --install-pixel
if errorlevel 1 goto failed
echo Enabling startup...
"%~dp0MediaPlayerBridge.exe" --install-startup
if errorlevel 1 goto failed
goto start

:pixel
echo.
echo Installing or repairing piXel adapter...
"%~dp0MediaPlayerBridge.exe" --install-pixel
if errorlevel 1 goto failed
echo.
echo Done. Refresh the piXel skin in Rainmeter.
pause
goto menu

:startup
echo.
echo Enabling startup...
"%~dp0MediaPlayerBridge.exe" --install-startup
if errorlevel 1 goto failed
echo.
echo Done.
pause
goto menu

:start
echo.
echo Starting the bridge in the background...
start "" /B "%~dp0MediaPlayerBridge.exe"
echo Done. You can close this setup window.
pause
goto end

:removeStartup
echo.
echo Removing startup entry...
"%~dp0MediaPlayerBridge.exe" --remove-startup
if errorlevel 1 goto failed
echo.
echo Done. The bridge will no longer start when you sign in.
pause
goto menu

:uninstall
echo.
echo Stopping the bridge...
taskkill /IM MediaPlayerBridge.exe /F >nul 2>&1
echo Removing piXel integration, startup entry and local data...
"%~dp0MediaPlayerBridge.exe" --uninstall
if errorlevel 1 goto failed
echo.
echo Uninstalled. You can now delete the extracted program folder.
pause
goto end

:failed
echo.
echo Setup failed. Make sure Rainmeter and piXel VISUALIZER are installed.
pause
goto menu

:end
endlocal
