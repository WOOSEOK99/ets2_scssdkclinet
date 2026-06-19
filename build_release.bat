@echo off
echo ===================================================
echo ETS2 Telemetry Server [Release Build]
echo ===================================================
echo.

echo [1/3] Clean release folder...
if exist "release" rmdir /s /q "release"
mkdir "release"

echo.
echo [2/3] Build C# project (Release Mode)...
dotnet build "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\SCSSdkClient.Demo.csproj" -c Release -o "release"

echo.
echo [3/3] Copy HTML dashboard to release folder...
if not exist "release\overlay" mkdir "release\overlay"
xcopy /Y "overlay\dashboard.html" "release\overlay\"

echo.
echo ===================================================
echo Build Completed!
echo Output Directory: %CD%\release\
echo Executable: ETS2_Telemetry_Server.exe
echo ===================================================
pause
