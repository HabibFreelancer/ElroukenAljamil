@echo off
title IDENTITY API — port 5002
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║       IDENTITY API  —  port 5002         ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5002...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5002 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5002 libre.
echo.
echo  [2/2] Demarrage Identity.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Identity.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
