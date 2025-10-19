@echo off
REM Post-build script to copy localization resources

echo Copying localization resources...

if not exist "%1Localization\Resources\" mkdir "%1Localization\Resources\"

xcopy /Y /I "%2Localization\Resources\*.json" "%1Localization\Resources\"

echo Localization resources copied successfully!
