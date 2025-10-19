# Script de d?ploiement des ressources de traduction
# ? ex?cuter depuis le dossier du projet Finance

Write-Host "=== D?ploiement des ressources de traduction ===" -ForegroundColor Cyan

# 1. Cr?er les dossiers n?cessaires
$debugPath = "bin\Debug\Localization\Resources"
$releasePath = "bin\Release\Localization\Resources"

Write-Host "`nCr?ation des dossiers..." -ForegroundColor Yellow
New-Item -Path $debugPath -ItemType Directory -Force | Out-Null
New-Item -Path $releasePath -ItemType Directory -Force | Out-Null
Write-Host "? Dossiers cr??s" -ForegroundColor Green

# 2. Copier les fichiers JSON
Write-Host "`nCopie des fichiers de traduction..." -ForegroundColor Yellow
$sourceFiles = Get-ChildItem "Localization\Resources\*.json"
foreach ($file in $sourceFiles) {
    Copy-Item $file.FullName -Destination $debugPath -Force
    Copy-Item $file.FullName -Destination $releasePath -Force
    Write-Host "  ? $($file.Name) copi?" -ForegroundColor Green
}

# 3. V?rifier que les fichiers ont ?t? copi?s
Write-Host "`nV?rification des fichiers copi?s..." -ForegroundColor Yellow
$requiredFiles = @("en.json", "fr.json", "he.json")
$allFilesPresent = $true

foreach ($fileName in $requiredFiles) {
    $filePath = Join-Path $debugPath $fileName
    if (Test-Path $filePath) {
        $fileSize = (Get-Item $filePath).Length
        Write-Host "  ? $fileName ($fileSize bytes)" -ForegroundColor Green
    } else {
        Write-Host "  ? $fileName MANQUANT!" -ForegroundColor Red
        $allFilesPresent = $false
    }
}

# 4. Tester le contenu d'un fichier
Write-Host "`nTest du contenu de en.json..." -ForegroundColor Yellow
$enJsonPath = Join-Path $debugPath "en.json"
if (Test-Path $enJsonPath) {
    try {
        $content = Get-Content $enJsonPath -Encoding UTF8 | ConvertFrom-Json
        Write-Host "  ? Fichier JSON valide" -ForegroundColor Green
        Write-Host "  ? Nombre de cl?s: $($content.PSObject.Properties.Count)" -ForegroundColor Green
        Write-Host "  ? App.Title = '$($content.'App.Title')'" -ForegroundColor Green
        Write-Host "  ? MainWindow.Title = '$($content.'MainWindow.Title')'" -ForegroundColor Green
    } catch {
        Write-Host "  ? Erreur de parsing JSON: $_" -ForegroundColor Red
        $allFilesPresent = $false
    }
} else {
    Write-Host "  ? Fichier en.json introuvable!" -ForegroundColor Red
    $allFilesPresent = $false
}

# 5. R?sum?
Write-Host "`n=== R?sum? ===" -ForegroundColor Cyan
if ($allFilesPresent) {
    Write-Host "? Tous les fichiers sont pr?ts!" -ForegroundColor Green
    Write-Host "`nVous pouvez maintenant:" -ForegroundColor White
    Write-Host "  1. Lancer l'application (F5 dans Visual Studio)" -ForegroundColor White
    Write-Host "  2. V?rifier la fen?tre Output pour les logs de debug" -ForegroundColor White
    Write-Host "  3. Les traductions devraient fonctionner!" -ForegroundColor White
} else {
    Write-Host "? Des probl?mes ont ?t? d?tect?s" -ForegroundColor Red
    Write-Host "V?rifiez que les fichiers JSON existent dans Localization\Resources\" -ForegroundColor Yellow
}

Write-Host "`nAppuyez sur une touche pour continuer..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
