@echo off
title MEDIA API — port 5004
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║        MEDIA API  —  port 5004           ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5004...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5004 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5004 libre.
echo.
echo  [2/2] Demarrage Media.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Media.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
