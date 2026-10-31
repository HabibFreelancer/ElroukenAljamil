@echo off
title API GATEWAY — port 5000
echo.
echo  ╔══════════════════════════════════════════╗
echo  ║       API GATEWAY  —  port 5000          ║
echo  ╚══════════════════════════════════════════╝
echo.
echo  [1/2] Verification port 5000...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5000 " ^| findstr "LISTENING"') do (
    echo  Instance detectee PID %%p - arret en cours...
    taskkill /PID %%p /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)
echo  [1/2] Port 5000 libre.
echo.
echo  [2/2] Demarrage ApiGateway...
echo  ─────────────────────────────────────────────
cd /d "%~dp0..\ElroukenAljamil.ApiGateway"
dotnet run
echo.
echo  Service arrete. Appuyez sur une touche pour fermer.
pause >nul
