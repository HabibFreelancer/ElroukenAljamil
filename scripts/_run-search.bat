@echo off
title SEARCH API — port 5005
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║       SEARCH API  —  port 5005           ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5005...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5005 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5005 libre.
echo.
echo  [2/2] Demarrage Search.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Search.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
