@echo off
setlocal
echo Packaging Firefox Extension v1.3.0 for Debugging and Release...

python -c "import zipfile, os; files=['manifest.json','background.js','content.js','content.css','locales.js']; dirs=['icons','popup','pdf-viewer']; packages=['magnifier-1.3.0.xpi','magnifier-1.3.0.zip','magnifier-debug.xpi','magnifier-debug.zip']; [(lambda z: [[z.write(f, f) for f in files if os.path.exists(f)], [z.write(os.path.join(r, fn), os.path.relpath(os.path.join(r, fn), '.')) for d in dirs for r, _, fl in os.walk(d) for fn in fl], z.close()])(zipfile.ZipFile(pkg, 'w', zipfile.ZIP_DEFLATED)) for pkg in packages]"

if %ERRORLEVEL% equ 0 (
    echo.
    echo =========================================================
    echo  Successfully created:
    echo    - magnifier-1.3.0.xpi  (Signed / Release package)
    echo    - magnifier-1.3.0.zip  (GitHub Release archive)
    echo    - magnifier-debug.xpi  (Temporary debug package)
    echo    - magnifier-debug.zip  (Temporary debug package)
    echo.
    echo  To debug in Firefox:
    echo  1. Open Firefox and go to: about:debugging#/runtime/this-firefox
    echo  2. Click "Load Temporary Add-on..."
    echo  3. Select "manifest.json" (or "magnifier-1.3.0.zip")
    echo =========================================================
    goto end
)

echo.
echo Packaging failed with error %ERRORLEVEL%.

:end
endlocal
