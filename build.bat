@echo off
chcp 65001 >nul
echo ===================================================
echo   ETS2 Telemetry Dashboard 빌드 및 배포 스크립트
echo ===================================================
echo.

echo [1/2] C# 서버 앱(SCSSdkClient.Demo)을 빌드합니다...
dotnet build "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\SCSSdkClient.Demo.csproj"

echo.
echo [2/2] HTML 대시보드 파일을 실행 폴더(bin\Debug\overlay)로 복사합니다...
if not exist "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\bin\Debug\overlay" mkdir "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\bin\Debug\overlay"
copy /Y "overlay\dashboard.html" "scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\bin\Debug\overlay\dashboard.html"

echo.
echo ===================================================
echo   모든 작업이 완료되었습니다!
echo   [실행 파일 위치]
echo   scs-client\C#\SCSSdkClient.Demo\SCSSdkClient.Demo\bin\Debug\SCSSdkClient.Demo.exe
echo ===================================================
pause
