<#
    BUILD-NEW-INSTALLER.ps1
    Script tout-en-un pour cr?er un nouvel installateur avec le nouveau logo
#>

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "??????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   BUILD NOUVEL INSTALLATEUR - NOUVEAU LOGO ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# --- V?rifications pr?alables ---
Write-Host "[1/5] V?rifications pr?alables..." -ForegroundColor Yellow

# V?rifier app.ico
$iconPath = Join-Path $PSScriptRoot "Resources\app.ico"
if (-not (Test-Path $iconPath)) {
    Write-Host "? ERREUR: Resources\app.ico introuvable!" -ForegroundColor Red
    exit 1
}
Write-Host "      ? app.ico trouv?" -ForegroundColor Green

# V?rifier Inno Setup
$innoSetupPaths = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 5\ISCC.exe"
)
$iscc = $innoSetupPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $iscc) {
    Write-Host "      ? Inno Setup non trouv? (optionnel)" -ForegroundColor Yellow
    Write-Host "        Tu devras compiler manuellement Setup-Installer.iss" -ForegroundColor Gray
} else {
    Write-Host "      ? Inno Setup trouv?" -ForegroundColor Green
}

# --- Nettoyage et compilation ---
Write-Host ""
Write-Host "[2/5] Nettoyage et compilation..." -ForegroundColor Yellow
$fixScript = Join-Path $PSScriptRoot "Fix-IconCache.ps1"
if (Test-Path $fixScript) {
    & $fixScript
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Erreur lors de la compilation" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "? Fix-IconCache.ps1 introuvable!" -ForegroundColor Red
    exit 1
}

# --- V?rifier que Publish contient Finance.exe ---
Write-Host ""
Write-Host "[3/5] V?rification du package de publication..." -ForegroundColor Yellow
$publishExe = Join-Path $PSScriptRoot "Publish\Finance.exe"
if (-not (Test-Path $publishExe)) {
    Write-Host "? Finance.exe introuvable dans Publish\" -ForegroundColor Red
    exit 1
}
Write-Host "      ? Finance.exe pr?sent dans Publish\" -ForegroundColor Green

# V?rifier l'ic?ne dans Publish
$publishIcon = Join-Path $PSScriptRoot "Publish\Resources\app.ico"
if (Test-Path $publishIcon) {
    Write-Host "      ? app.ico pr?sent dans Publish\Resources\" -ForegroundColor Green
} else {
    Write-Host "      ? app.ico absent de Publish\Resources\" -ForegroundColor Yellow
}

# --- Compilation de l'installateur ---
Write-Host ""
Write-Host "[4/5] Compilation de l'installateur..." -ForegroundColor Yellow
$issFile = Join-Path $PSScriptRoot "Setup-Installer.iss"

if (-not (Test-Path $issFile)) {
    Write-Host "? Setup-Installer.iss introuvable!" -ForegroundColor Red
    exit 1
}

if ($iscc) {
    Write-Host "      Compilation en cours..." -ForegroundColor Gray
    & $iscc $issFile /Q
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "      ? Installateur compil? avec succ?s" -ForegroundColor Green
        
        # Trouver le fichier de setup cr??
        $installerDir = Join-Path $PSScriptRoot "Installer"
        $setupFile = Get-ChildItem -Path $installerDir -Filter "FinanceManager-Setup-*.exe" -ErrorAction SilentlyContinue | 
                     Sort-Object LastWriteTime -Descending | 
                     Select-Object -First 1
        
        if ($setupFile) {
            Write-Host "      ?? Installateur: $($setupFile.Name)" -ForegroundColor Cyan
        }
    } else {
        Write-Host "      ? Erreur lors de la compilation de l'installateur" -ForegroundColor Red
        Write-Host "        Ouvre Setup-Installer.iss avec Inno Setup pour voir les d?tails" -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "      ? Compilation manuelle n?cessaire" -ForegroundColor Yellow
    Write-Host "        1. Ouvre Setup-Installer.iss avec Inno Setup" -ForegroundColor Gray
    Write-Host "        2. Clique sur Build ? Compile" -ForegroundColor Gray
}

# --- Instructions finales ---
Write-Host ""
Write-Host "[5/5] Instructions d'installation..." -ForegroundColor Yellow
Write-Host ""
Write-Host "??????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?          BUILD TERMINE AVEC SUCCES         ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
Write-Host "PROCHAINES ETAPES POUR VOIR LE NOUVEAU LOGO:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1??  DESINSTALLER L'ANCIENNE VERSION" -ForegroundColor Yellow
Write-Host "    ? Param?tres ? Applications ? Finance Manager ? D?sinstaller" -ForegroundColor White
Write-Host "    ? Supprimer manuellement le raccourci bureau s'il existe" -ForegroundColor White
Write-Host ""
Write-Host "2??  REDEMARRER LE PC" -ForegroundColor Yellow
Write-Host "    ? IMPORTANT: Pour vider le cache d'ic?nes Windows" -ForegroundColor White
Write-Host ""
Write-Host "3??  INSTALLER LA NOUVELLE VERSION" -ForegroundColor Yellow
if ($setupFile) {
    Write-Host "    ? Lance: .\Installer\$($setupFile.Name)" -ForegroundColor White
} else {
    Write-Host "    ? Lance le setup dans le dossier .\Installer\" -ForegroundColor White
}
Write-Host ""
Write-Host "4??  VERIFIER LE LOGO" -ForegroundColor Yellow
Write-Host "    ? Raccourci bureau" -ForegroundColor Gray
Write-Host "    ? Menu D?marrer" -ForegroundColor Gray
Write-Host "    ? Barre des t?ches (app ouverte)" -ForegroundColor Gray
Write-Host "    ? Param?tres ? Applications" -ForegroundColor Gray
Write-Host ""
Write-Host "????????????????????????????????????????????" -ForegroundColor DarkGray
Write-Host "?? Si le logo ne change toujours pas:" -ForegroundColor Cyan
Write-Host "   ? Ex?cute Clear-WindowsIconCache.ps1 en tant qu'admin" -ForegroundColor White
Write-Host "   ? Red?marre ? nouveau le PC" -ForegroundColor White
Write-Host "   ? Consulte FIX-LOGO-GUIDE.md pour plus d'aide" -ForegroundColor White
Write-Host ""
