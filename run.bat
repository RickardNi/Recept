@echo off
netstat -ano | findstr :5218 > nul
if %errorlevel% equ 0 (
    echo Port 5218 in use, killing process...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5218') do taskkill /F /PID %%a 2>nul
)

dotnet watch run --non-interactive
