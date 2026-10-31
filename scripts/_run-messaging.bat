@echo off
title MESSAGING API — port 5003
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║      MESSAGING API  —  port 5003         ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5003...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5003 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5003 libre.
echo.
echo  [2/2] Demarrage Messaging.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Messaging.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
