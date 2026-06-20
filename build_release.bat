@echo off
chcp 65001 > nul
echo ===================================================
echo ETS2 Telemetry Server [Release Build]
echo ===================================================
echo.

echo [1/4] Clean and prepare release folder...
if exist "release" rmdir /s /q "release"
md "release"
md "release\overlay"
md "release\plugins"
echo Done.

echo.
echo [2/4] Build C# project (Release Mode)...
dotnet build "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\SCSSdkClient.Demo.csproj" -c Release -o "release"
if errorlevel 1 ( echo [ERROR] Build failed! & pause & exit /b 1 )

echo.
echo [3/4] Copy overlay files...
copy /Y "overlay\dashboard.html" "release\overlay\" > nul
echo dashboard.html copied.
if exist "overlay\map" (
    xcopy /E /I /Y "overlay\map" "release\overlay\map" > nul
    echo Map tiles copied.
) else (
    echo [WARNING] overlay\map not found. Map will not work!
)

echo.
echo [4/4] Copy game plugin DLL...
if exist "scs-telemetry\vs2012\Release\scs-telemetry.dll" (
    copy /Y "scs-telemetry\vs2012\Release\scs-telemetry.dll" "release\plugins\" > nul
    echo scs-telemetry.dll copied.
) else (
    echo [WARNING] scs-telemetry.dll not found!
)

echo.
echo ===================================================
echo  Build Completed!
echo  Output: %CD%\release\
echo.
echo  [Deployment]
echo  1. ETS2_Telemetry_Server.exe - run on PC
echo  2. overlay\  - served automatically by server
echo  3. plugin\scs-telemetry.dll - copy to game plugins folder:
echo     Documents\Euro Truck Simulator 2\plugins\
echo ===================================================
pause
