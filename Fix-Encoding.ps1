# Script pour corriger l'encodage des fichiers de traduction
# Assure que tous les fichiers sont en UTF-8 avec BOM

$ErrorActionPreference = "Stop"

Write-Host "=== Correction de l'encodage UTF-8 ===" -ForegroundColor Cyan

# Liste des fichiers ? v?rifier
$files = @(
    "Localization\Resources\en.json",
    "Localization\Resources\fr.json", 
    "Localization\Resources\he.json"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "`nTraitement de $file..." -ForegroundColor Yellow
        
        # Lire le contenu en UTF-8
        $content = Get-Content $file -Encoding UTF8 -Raw
        
        # Sauvegarder avec UTF-8 BOM
        $utf8 = New-Object System.Text.UTF8Encoding $true
        [System.IO.File]::WriteAllText((Resolve-Path $file).Path, $content, $utf8)
        
        Write-Host "  ? Encodage corrig? en UTF-8 avec BOM" -ForegroundColor Green
        
        # V?rifier la taille
        $size = (Get-Item $file).Length
        Write-Host "  ? Taille: $size bytes" -ForegroundColor Green
    } else {
        Write-Host "  ? Fichier non trouv?: $file" -ForegroundColor Red
    }
}

Write-Host "`n=== Copie vers bin\Debug ===" -ForegroundColor Cyan

# Cr?er les dossiers de destination
$debugPath = "bin\Debug\Localization\Resources"
$releasePath = "bin\Release\Localization\Resources"

New-Item -Path $debugPath -ItemType Directory -Force | Out-Null
New-Item -Path $releasePath -ItemType Directory -Force | Out-Null

# Copier les fichiers
foreach ($file in $files) {
    if (Test-Path $file) {
        $fileName = Split-Path $file -Leaf
        
        # Copier avec pr?servation de l'encodage
        Copy-Item $file -Destination (Join-Path $debugPath $fileName) -Force
        Copy-Item $file -Destination (Join-Path $releasePath $fileName) -Force
        
        Write-Host "  ? $fileName copi?" -ForegroundColor Green
    }
}

Write-Host "`n=== Test de lecture ===" -ForegroundColor Cyan

# Tester la lecture d'un fichier
$testFile = Join-Path $debugPath "en.json"
if (Test-Path $testFile) {
    try {
        $json = Get-Content $testFile -Encoding UTF8 -Raw | ConvertFrom-Json
        Write-Host "  ? Fichier JSON valide" -ForegroundColor Green
        Write-Host "  ? Menu.Home = '$($json.'Menu.Home')'" -ForegroundColor Green
        Write-Host "  ? Settings.Currency.Shekel = '$($json.'Settings.Currency.Shekel')'" -ForegroundColor Green
        
        # V?rifier que les emojis sont pr?sents
        if ($json.'Menu.Home' -match '??') {
            Write-Host "  ? Emojis d?tect?s correctement!" -ForegroundColor Green
        } else {
            Write-Host "  ? Emojis manquants ou corrompus" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ? Erreur: $_" -ForegroundColor Red
    }
}

Write-Host "`n=== Termin? ===" -ForegroundColor Green
Write-Host "Vous pouvez maintenant lancer l'application (F5)" -ForegroundColor White
Write-Host "`nAppuyez sur une touche pour continuer..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
