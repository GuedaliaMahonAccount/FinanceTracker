<#  Publish-App.ps1 — WPF .NET Framework 4.7.2 (non SDK-style)
    - Restaure via MSBuild (pas besoin de nuget.exe)
    - Force Platform=AnyCPU
    - Copie vers .\Publish et exclut PDB/XML
#>

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Publication de Finance Manager" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Configuration ---
$projectName = "Finance"                # Nom du .csproj (sans l'extension)
$projPath    = Join-Path $PSScriptRoot "$projectName.csproj"
$releaseDir  = Join-Path $PSScriptRoot "bin\Release"
$publishDir  = Join-Path $PSScriptRoot "Publish"

if (-not (Test-Path $projPath)) { throw "Projet introuvable: $projPath" }

# --- Localisation de MSBuild (VS2022 Community par défaut) ---
$msbuildCandidates = @(
  "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)
$msbuild = $msbuildCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $msbuild) {
  Write-Host "MSBuild non trouvé. Ouvre 'Developer PowerShell for VS 2022' et relance ce script." -ForegroundColor Yellow
  throw "MSBuild introuvable."
}

# --- 1) Nettoyage des anciennes compilations ---
Write-Host "1. Nettoyage des anciennes compilations..." -ForegroundColor Yellow

# Nettoyage du dossier Publish
if (Test-Path $publishDir) { Remove-Item -Path $publishDir -Recurse -Force }
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

# (Optionnel) Nettoyage bin/obj pour repartir propre (fix: pas de -Filter multiplié)
Get-ChildItem -Recurse -Force -Directory |
  Where-Object { $_.Name -in @('bin','obj') } |
  ForEach-Object {
    try { Remove-Item $_.FullName -Recurse -Force -ErrorAction Stop } catch {}
  }

# --- 2) Restauration des packages via MSBuild ---
Write-Host "`n2. Restauration des packages (MSBuild /t:Restore)..." -ForegroundColor Yellow
& $msbuild $projPath /t:Restore /p:Configuration=Release /p:Platform=AnyCPU /v:m
if ($LASTEXITCODE -ne 0) { throw "Echec du restore." }

# --- 3) Compilation en mode Release ---
Write-Host "`n3. Compilation en mode Release..." -ForegroundColor Yellow
& $msbuild $projPath /t:Clean,Build /p:Configuration=Release /p:Platform=AnyCPU /v:m
if ($LASTEXITCODE -ne 0) { throw "Echec de la compilation." }

if (-not (Test-Path $releaseDir)) { throw "Dossier de sortie introuvable: $releaseDir" }

# --- 4) Copie vers Publish et exclusions ---
Write-Host "`n4. Copie des fichiers vers le dossier de publication..." -ForegroundColor Yellow
Copy-Item -Path (Join-Path $releaseDir "*") -Destination $publishDir -Recurse -Force

# --- 5) Copie des fichiers de traduction (Localization) ---
Write-Host "`n5. Copie des fichiers de traduction..." -ForegroundColor Yellow
$localizationSource = Join-Path $PSScriptRoot "Localization\Resources"
$localizationDest = Join-Path $publishDir "Localization\Resources"

if (Test-Path $localizationSource) {
    if (-not (Test-Path $localizationDest)) {
        New-Item -ItemType Directory -Path $localizationDest -Force | Out-Null
    }
    Copy-Item -Path (Join-Path $localizationSource "*.json") -Destination $localizationDest -Force
    Write-Host "   ✓ Fichiers JSON de traduction copiés" -ForegroundColor Green
} else {
    Write-Host "   ⚠ Dossier Localization\Resources introuvable" -ForegroundColor Yellow
}

# --- 6) Copie de l'icône de l'application ---
Write-Host "`n6. Copie de l'icône de l'application..." -ForegroundColor Yellow
$iconSource = Join-Path $PSScriptRoot "Resources\app.ico"
if (Test-Path $iconSource) {
    $iconDestDir = Join-Path $publishDir "Resources"
    if (-not (Test-Path $iconDestDir)) {
        New-Item -ItemType Directory -Path $iconDestDir -Force | Out-Null
    }
    Copy-Item -Path $iconSource -Destination $iconDestDir -Force
    Write-Host "   ✓ app.ico copié vers le dossier de publication" -ForegroundColor Green
} else {
    Write-Host "   ⚠ Resources\app.ico introuvable" -ForegroundColor Yellow
}

# Exclure fichiers dev non nécessaires
$excludePatterns = @("*.pdb", "*.xml", "*.vshost.*")
foreach ($pattern in $excludePatterns) {
  Get-ChildItem -Path $publishDir -Filter $pattern -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
    try { Remove-Item $_.FullName -Force -ErrorAction Stop } catch {}
  }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Publication terminée avec succès !" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Les fichiers sont disponibles dans: $publishDir" -ForegroundColor White
Write-Host ""
Write-Host "Contenu publié:" -ForegroundColor Cyan
Write-Host "  ✓ Application compilée (Release)" -ForegroundColor White
Write-Host "  ✓ Dépendances (DLL)" -ForegroundColor White
Write-Host "  ✓ Fichiers de traduction (FR, EN, HE)" -ForegroundColor White
Write-Host "  ✓ Données (ExchangeRates.xlsx)" -ForegroundColor White
Write-Host ""
Write-Host "Prochaines étapes :" -ForegroundColor Cyan
Write-Host "1. Teste l'application depuis '$publishDir' (double-clique Finance.exe)" -ForegroundColor White
Write-Host "2. Vérifie que les traductions fonctionnent (change la langue dans Paramètres)" -ForegroundColor White
Write-Host "3. Compile l'installateur Inno Setup (Setup-Installer.iss)" -ForegroundColor White
