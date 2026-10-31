@echo off
title NOTIFICATION API — port 5006
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║    NOTIFICATION API  —  port 5006        ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5006...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5006 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5006 libre.
echo.
echo  [2/2] Demarrage Notification.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Notification.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
