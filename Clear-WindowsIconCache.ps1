<#
    Clear-WindowsIconCache.ps1
    Vide uniquement le cache d'ic?nes Windows (sans recompiler)
    ? ex?cuter en tant qu'administrateur pour de meilleurs r?sultats
#>

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Nettoyage Cache Ic?nes Windows" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "ATTENTION: Votre bureau va dispara?tre momentan?ment!" -ForegroundColor Yellow
Write-Host "(Il va red?marrer automatiquement)" -ForegroundColor Yellow
Write-Host ""

$continue = Read-Host "Continuer? (O/N)"
if ($continue -ne 'O' -and $continue -ne 'o') {
    Write-Host "Annul?." -ForegroundColor Yellow
    exit
}

try {
    # Localiser le cache d'ic?nes
    $iconcachePaths = @(
        "$env:LOCALAPPDATA\Microsoft\Windows\Explorer\iconcache*.db",
        "$env:LOCALAPPDATA\IconCache.db",
        "$env:LOCALAPPDATA\Microsoft\Windows\Explorer\thumbcache*.db"
    )
    
    # Arr?ter explorer.exe
    Write-Host "`nArr?t de explorer.exe..." -ForegroundColor Yellow
    Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    # Supprimer les fichiers de cache
    $filesDeleted = 0
    foreach ($pattern in $iconcachePaths) {
        Get-Item $pattern -ErrorAction SilentlyContinue | ForEach-Object {
            try {
                Remove-Item $_.FullName -Force -ErrorAction Stop
                Write-Host "? Supprim?: $($_.Name)" -ForegroundColor Green
                $filesDeleted++
            } catch {
                Write-Host "? Impossible de supprimer: $($_.Name)" -ForegroundColor Yellow
            }
        }
    }
    
    if ($filesDeleted -eq 0) {
        Write-Host "Aucun fichier de cache trouv? (peut-?tre d?j? nettoy?)." -ForegroundColor Yellow
    } else {
        Write-Host "`n? $filesDeleted fichier(s) de cache supprim?(s)" -ForegroundColor Green
    }
    
    # Red?marrer explorer.exe
    Write-Host "`nRed?marrage de explorer.exe..." -ForegroundColor Yellow
    Start-Process explorer.exe
    Start-Sleep -Seconds 2
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ? Nettoyage termin?" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Le cache d'ic?nes a ?t? vid?." -ForegroundColor White
    Write-Host "Red?marre ton PC pour que les changements prennent effet." -ForegroundColor Cyan
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "ERREUR: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Essaye de:" -ForegroundColor Yellow
    Write-Host "1. Fermer tous les programmes" -ForegroundColor White
    Write-Host "2. Ex?cuter ce script en tant qu'administrateur" -ForegroundColor White
    Write-Host "3. Red?marrer le PC si le probl?me persiste" -ForegroundColor White
    
    # Red?marrer explorer au cas o?
    Start-Process explorer.exe -ErrorAction SilentlyContinue
}
