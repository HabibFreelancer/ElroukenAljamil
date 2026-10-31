@echo off
title LISTINGS API — port 5001
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║       LISTINGS API  —  port 5001         ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5001...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5001 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5001 libre.
echo.
echo  [2/2] Demarrage Listings.API...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.Listings.API"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
