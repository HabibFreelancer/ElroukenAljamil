@echo off
title START ALL SERVICES
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║         DÉMARRAGE DE TOUS LES            ║
echo  ║              MICROSERVICES               ║
echo  ╚══════════════════════════════════════════╝
echo.

start "IDENTITY API  — port 5002"    cmd /k "%~dp0start-identity.bat"
timeout /t 2 /nobreak >nul

start "LISTINGS API  — port 5001"    cmd /k "%~dp0start-listings.bat"
timeout /t 2 /nobreak >nul

start "MESSAGING API — port 5003"    cmd /k "%~dp0start-messaging.bat"
timeout /t 2 /nobreak >nul

start "MEDIA API     — port 5004"    cmd /k "%~dp0start-media.bat"
timeout /t 2 /nobreak >nul

start "SEARCH API    — port 5005"    cmd /k "%~dp0start-search.bat"
timeout /t 2 /nobreak >nul

start "NOTIFICATION  — port 5006"    cmd /k "%~dp0start-notification.bat"
timeout /t 2 /nobreak >nul

start "API GATEWAY   — port 5000"    cmd /k "%~dp0start-gateway.bat"

echo.
echo  Tous les services ont été lancés dans des fenêtres séparées.
echo.
pause
