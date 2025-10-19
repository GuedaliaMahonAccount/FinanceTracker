<#
    Fix-IconCache.ps1
    Nettoie le cache d'ic?nes Windows et recompile l'application avec le nouveau logo
#>

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Fix Logo - Nettoyage Cache Windows" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- 1) V?rifier que app.ico existe ---
$iconPath = Join-Path $PSScriptRoot "Resources\app.ico"
if (-not (Test-Path $iconPath)) {
    Write-Host "ERREUR: Resources\app.ico introuvable!" -ForegroundColor Red
    exit 1
}
Write-Host "? app.ico trouv?: $iconPath" -ForegroundColor Green

# --- 2) Nettoyer bin/obj ---
Write-Host "`n1. Nettoyage des dossiers bin et obj..." -ForegroundColor Yellow
Get-ChildItem -Recurse -Force -Directory |
  Where-Object { $_.Name -in @('bin','obj') } |
  ForEach-Object {
    try { 
        Remove-Item $_.FullName -Recurse -Force -ErrorAction Stop
        Write-Host "   Supprim?: $($_.FullName)" -ForegroundColor Gray
    } catch {}
  }

# --- 3) Nettoyer le dossier Publish ---
$publishDir = Join-Path $PSScriptRoot "Publish"
if (Test-Path $publishDir) {
    Write-Host "`n2. Nettoyage du dossier Publish..." -ForegroundColor Yellow
    Remove-Item -Path $publishDir -Recurse -Force
    Write-Host "   ? Dossier Publish nettoy?" -ForegroundColor Green
}

# --- 4) Tuer explorer.exe pour vider le cache d'ic?nes ---
Write-Host "`n3. Vidage du cache d'ic?nes Windows..." -ForegroundColor Yellow
Write-Host "   ATTENTION: Votre bureau va dispara?tre momentan?ment!" -ForegroundColor Yellow
Write-Host "   (Il va red?marrer automatiquement)" -ForegroundColor Yellow
Start-Sleep -Seconds 2

try {
    # Localiser le cache d'ic?nes
    $iconcachePaths = @(
        "$env:LOCALAPPDATA\Microsoft\Windows\Explorer\iconcache*.db",
        "$env:LOCALAPPDATA\IconCache.db"
    )
    
    # Arr?ter explorer.exe
    Write-Host "   Arr?t de explorer.exe..." -ForegroundColor Gray
    Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    # Supprimer les fichiers de cache
    foreach ($pattern in $iconcachePaths) {
        Get-Item $pattern -ErrorAction SilentlyContinue | ForEach-Object {
            try {
                Remove-Item $_.FullName -Force -ErrorAction Stop
                Write-Host "   Supprim?: $($_.Name)" -ForegroundColor Gray
            } catch {
                Write-Host "   Impossible de supprimer: $($_.Name)" -ForegroundColor Yellow
            }
        }
    }
    
    # Red?marrer explorer.exe
    Write-Host "   Red?marrage de explorer.exe..." -ForegroundColor Gray
    Start-Process explorer.exe
    Start-Sleep -Seconds 2
    Write-Host "   ? Cache d'ic?nes vid?" -ForegroundColor Green
    
} catch {
    Write-Host "   ? Erreur lors du nettoyage du cache: $_" -ForegroundColor Yellow
    Write-Host "   L'application sera quand m?me recompil?e." -ForegroundColor Yellow
}

# --- 5) Recompiler l'application ---
Write-Host "`n4. Recompilation de l'application..." -ForegroundColor Yellow

# Localisation de MSBuild
$msbuildCandidates = @(
  "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
)
$msbuild = $msbuildCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $msbuild) {
    Write-Host "   ERREUR: MSBuild introuvable." -ForegroundColor Red
    Write-Host "   Ouvre 'Developer PowerShell for VS 2022' et relance ce script." -ForegroundColor Yellow
    exit 1
}

$projPath = Join-Path $PSScriptRoot "Finance.csproj"

# Restore
Write-Host "   Restauration des packages..." -ForegroundColor Gray
& $msbuild $projPath /t:Restore /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw "Echec du restore." }

# Build
Write-Host "   Compilation..." -ForegroundColor Gray
& $msbuild $projPath /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw "Echec de la compilation." }

Write-Host "   ? Compilation termin?e" -ForegroundColor Green

# --- 6) Lancer Publish-App.ps1 ---
Write-Host "`n5. Cr?ation du package de publication..." -ForegroundColor Yellow
$publishScript = Join-Path $PSScriptRoot "Publish-App.ps1"
if (Test-Path $publishScript) {
    & $publishScript
} else {
    Write-Host "   ? Publish-App.ps1 introuvable, publication manuelle n?cessaire" -ForegroundColor Yellow
}

# --- R?sultat ---
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ? FIX TERMINE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "PROCHAINES ETAPES:" -ForegroundColor Cyan
Write-Host "1. Compile l'installateur (Setup-Installer.iss avec Inno Setup)" -ForegroundColor White
Write-Host "2. Installe la nouvelle version" -ForegroundColor White
Write-Host "3. Si l'ic?ne ne change toujours pas:" -ForegroundColor White
Write-Host "   - Supprime l'ancien raccourci bureau" -ForegroundColor Gray
Write-Host "   - D?sinstalle l'ancienne version" -ForegroundColor Gray
Write-Host "   - Red?marre ton PC" -ForegroundColor Gray
Write-Host "   - R?installe la nouvelle version" -ForegroundColor Gray
Write-Host ""
