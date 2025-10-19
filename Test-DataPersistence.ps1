# Test-DataPersistence.ps1
# Script pour tester la persistance des donn?es apr?s la correction

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Test de Persistance des Donn?es" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$appDataPath = Join-Path $env:LOCALAPPDATA "Finance Manager"

Write-Host "1. V?rification du dossier de donn?es..." -ForegroundColor Yellow
Write-Host "   Chemin: $appDataPath" -ForegroundColor White

if (Test-Path $appDataPath) {
    Write-Host "   ? Dossier existe" -ForegroundColor Green
    
    # Afficher la structure
    Write-Host "`n2. Structure des dossiers:" -ForegroundColor Yellow
    
    $dataFolder = Join-Path $appDataPath "Data"
    if (Test-Path $dataFolder) {
        Write-Host "   ? Data\" -ForegroundColor Green
        
        $projectsFile = Join-Path $dataFolder "Projects.json"
        if (Test-Path $projectsFile) {
            $size = (Get-Item $projectsFile).Length
            Write-Host "     ? Projects.json ($size bytes)" -ForegroundColor Green
        } else {
            Write-Host "     - Projects.json (pas encore cr??)" -ForegroundColor Gray
        }
        
        $settingsFile = Join-Path $dataFolder "Settings.json"
        if (Test-Path $settingsFile) {
            $size = (Get-Item $settingsFile).Length
            Write-Host "     ? Settings.json ($size bytes)" -ForegroundColor Green
        } else {
            Write-Host "     - Settings.json (pas encore cr??)" -ForegroundColor Gray
        }
    } else {
        Write-Host "   - Data\ (pas encore cr??)" -ForegroundColor Gray
    }
    
    $uploadFolder = Join-Path $appDataPath "UploadedFiles"
    if (Test-Path $uploadFolder) {
        Write-Host "   ? UploadedFiles\" -ForegroundColor Green
        $fileCount = (Get-ChildItem $uploadFolder -File).Count
        if ($fileCount -gt 0) {
            Write-Host "     ? $fileCount fichier(s) upload?(s)" -ForegroundColor Green
        } else {
            Write-Host "     - Aucun fichier (normal si pas encore test?)" -ForegroundColor Gray
        }
    } else {
        Write-Host "   - UploadedFiles\ (pas encore cr??)" -ForegroundColor Gray
    }
    
    Write-Host "`n3. Contenu des fichiers de donn?es:" -ForegroundColor Yellow
    
    if (Test-Path $projectsFile) {
        Write-Host "`n   Projects.json:" -ForegroundColor Cyan
        $projects = Get-Content $projectsFile -Raw | ConvertFrom-Json
        if ($projects -and $projects.Count -gt 0) {
            Write-Host "   ? $($projects.Count) projet(s) sauvegard?(s)" -ForegroundColor Green
            foreach ($project in $projects) {
                $transCount = if ($project.Transactions) { $project.Transactions.Count } else { 0 }
                Write-Host "     - $($project.Name): $transCount transaction(s)" -ForegroundColor White
            }
        } else {
            Write-Host "   - Aucun projet (liste vide)" -ForegroundColor Gray
        }
    }
    
    if (Test-Path $settingsFile) {
        Write-Host "`n   Settings.json:" -ForegroundColor Cyan
        $settings = Get-Content $settingsFile -Raw | ConvertFrom-Json
        Write-Host "   ? Langue: $($settings.PreferredLanguage)" -ForegroundColor Green
        Write-Host "   ? Devise: $($settings.PreferredCurrency)" -ForegroundColor Green
    }
    
} else {
    Write-Host "   ? Dossier n'existe pas encore" -ForegroundColor Yellow
    Write-Host "   ? C'est normal si l'application n'a jamais ?t? lanc?e" -ForegroundColor Gray
}

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "  Instructions de Test" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Pour tester la correction:" -ForegroundColor White
Write-Host ""
Write-Host "1. Compilez l'application:" -ForegroundColor Yellow
Write-Host "   .\Publish-App.ps1" -ForegroundColor White
Write-Host ""
Write-Host "2. Lancez l'application depuis le dossier Publish:" -ForegroundColor Yellow
Write-Host "   .\Publish\Finance.exe" -ForegroundColor White
Write-Host ""
Write-Host "3. Dans l'application:" -ForegroundColor Yellow
Write-Host "   - Cr?ez un nouveau projet" -ForegroundColor White
Write-Host "   - Ajoutez quelques transactions" -ForegroundColor White
Write-Host "   - Uploadez un fichier (facture)" -ForegroundColor White
Write-Host "   - Changez la langue dans les param?tres" -ForegroundColor White
Write-Host ""
Write-Host "4. Fermez l'application compl?tement" -ForegroundColor Yellow
Write-Host ""
Write-Host "5. Relancez ce script pour v?rifier les donn?es:" -ForegroundColor Yellow
Write-Host "   .\Test-DataPersistence.ps1" -ForegroundColor White
Write-Host ""
Write-Host "6. Relancez l'application et v?rifiez que:" -ForegroundColor Yellow
Write-Host "   ? Vos projets sont toujours l?" -ForegroundColor Green
Write-Host "   ? Vos transactions sont sauvegard?es" -ForegroundColor Green
Write-Host "   ? Vos fichiers upload?s sont accessibles" -ForegroundColor Green
Write-Host "   ? Votre langue pr?f?r?e est conserv?e" -ForegroundColor Green
Write-Host ""

# Proposer d'ouvrir le dossier
Write-Host "Voulez-vous ouvrir le dossier de donn?es ? (O/N)" -ForegroundColor Cyan
$response = Read-Host

if ($response -eq "O" -or $response -eq "o") {
    if (Test-Path $appDataPath) {
        explorer.exe $appDataPath
    } else {
        # Cr?er le dossier et l'ouvrir
        New-Item -ItemType Directory -Path $appDataPath -Force | Out-Null
        explorer.exe $appDataPath
    }
}

Write-Host ""
Write-Host "Test termin? !" -ForegroundColor Green
Write-Host ""
