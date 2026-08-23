@echo off
setlocal
echo Compiling Magnifier Desktop Application for Windows...

set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo Error: C# compiler not found at %CSC_PATH%
    pause
    exit /b 1
)

"%CSC_PATH%" /nologo /target:winexe /optimize+ /win32icon:app.ico /reference:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.Core.dll /out:Magnifier.exe src\Localization.cs src\AppSettings.cs src\NativeMethods.cs src\MagnifierLens.cs src\HotkeyRecorderDialog.cs src\AboutDialog.cs src\SettingsForm.cs src\PdfViewerForm.cs src\Program.cs

if %ERRORLEVEL% equ 0 (
    echo.
    echo ===================================================
    echo  Build Successful! Magnifier.exe created.
    echo  Double-click Magnifier.exe to run.
    echo ===================================================
) else (
    echo.
    echo Build failed with error code %ERRORLEVEL%.
)

endlocal
