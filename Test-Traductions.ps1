# Test-Traductions.ps1
# Script de v?rification pour les traductions

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  V?rification des Traductions" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$publishDir = Join-Path $PSScriptRoot "Publish"
$localizationDir = Join-Path $publishDir "Localization\Resources"

# Test 1: Dossier Publish existe
Write-Host "1. V?rification du dossier Publish..." -ForegroundColor Yellow
if (Test-Path $publishDir) {
    Write-Host "   ? Dossier Publish existe" -ForegroundColor Green
} else {
    Write-Host "   ? Dossier Publish introuvable!" -ForegroundColor Red
    Write-Host "   ? Ex?cutez d'abord: .\Publish-App.ps1" -ForegroundColor Yellow
    exit 1
}

# Test 2: Fichiers de traduction
Write-Host "`n2. V?rification des fichiers de traduction..." -ForegroundColor Yellow
$requiredFiles = @("fr.json", "en.json", "he.json")
$allFilesExist = $true

foreach ($file in $requiredFiles) {
    $filePath = Join-Path $localizationDir $file
    if (Test-Path $filePath) {
        $size = (Get-Item $filePath).Length
        Write-Host "   ? $file existe ($size octets)" -ForegroundColor Green
    } else {
        Write-Host "   ? $file manquant!" -ForegroundColor Red
        $allFilesExist = $false
    }
}

if (-not $allFilesExist) {
    Write-Host "`n   ? PROBL?ME: Fichiers de traduction manquants" -ForegroundColor Red
    Write-Host "   ? Solution: Modifiez Finance.csproj (voir FIX-TRADUCTIONS.md)" -ForegroundColor Yellow
    exit 1
}

# Test 3: Contenu des fichiers JSON
Write-Host "`n3. V?rification du contenu des fichiers JSON..." -ForegroundColor Yellow
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $localizationDir $file
    try {
        $content = Get-Content $filePath -Raw -Encoding UTF8
        $json = $content | ConvertFrom-Json
        
        # V?rifier quelques cl?s essentielles
        $requiredKeys = @("App.Title", "MainWindow.Title", "Settings.Title")
        $missingKeys = @()
        
        foreach ($key in $requiredKeys) {
            if (-not $json.PSObject.Properties[$key]) {
                $missingKeys += $key
            }
        }
        
        if ($missingKeys.Count -eq 0) {
            Write-Host "   ? $file: JSON valide avec toutes les cl?s" -ForegroundColor Green
        } else {
            Write-Host "   ? $file: Cl?s manquantes: $($missingKeys -join ', ')" -ForegroundColor Yellow
        }
        
    } catch {
        Write-Host "   ? $file: Erreur de lecture JSON!" -ForegroundColor Red
        Write-Host "     $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 4: Application ex?cutable
Write-Host "`n4. V?rification de l'ex?cutable..." -ForegroundColor Yellow
$exePath = Join-Path $publishDir "Finance.exe"
if (Test-Path $exePath) {
    $size = (Get-Item $exePath).Length / 1KB
    Write-Host "   ? Finance.exe existe ($([math]::Round($size, 2)) KB)" -ForegroundColor Green
} else {
    Write-Host "   ? Finance.exe introuvable!" -ForegroundColor Red
    exit 1
}

# Test 5: DLLs n?cessaires
Write-Host "`n5. V?rification des d?pendances..." -ForegroundColor Yellow
$requiredDlls = @("Newtonsoft.Json.dll", "EPPlus.dll")
foreach ($dll in $requiredDlls) {
    $dllPath = Join-Path $publishDir $dll
    if (Test-Path $dllPath) {
        Write-Host "   ? $dll pr?sent" -ForegroundColor Green
    } else {
        Write-Host "   ? $dll manquant" -ForegroundColor Yellow
    }
}

# Test 6: Fichiers de donn?es
Write-Host "`n6. V?rification des fichiers de donn?es..." -ForegroundColor Yellow
$dataPath = Join-Path $publishDir "Data\ExchangeRates.xlsx"
if (Test-Path $dataPath) {
    Write-Host "   ? ExchangeRates.xlsx pr?sent" -ForegroundColor Green
} else {
    Write-Host "   ? ExchangeRates.xlsx manquant" -ForegroundColor Yellow
}

# R?sum? final
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  V?rification Termin?e" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

if ($allFilesExist) {
    Write-Host "? Tous les fichiers de traduction sont pr?sents!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Vous pouvez maintenant:" -ForegroundColor Cyan
    Write-Host "1. Tester l'application: .\Publish\Finance.exe" -ForegroundColor White
    Write-Host "2. Compiler l'installateur: Setup-Installer.iss" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "? Action requise: Consultez FIX-TRADUCTIONS.md" -ForegroundColor Yellow
    Write-Host ""
}
